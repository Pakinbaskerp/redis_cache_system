using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedisProductAPI.Infrastructure.Contract;
using RedisProductAPI.Infrastructure.Persistence;

namespace RedisProductAPI.Controllers
{
    [ApiController]
    [Route("api/v1/product")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IRedisCacheService _redisCacheService;

        public ProductsController(
            AppDbContext context,
            IRedisCacheService redisCacheService)
        {
            _context = context;
            _redisCacheService = redisCacheService;
        }

        [HttpGet("{proId}")]
        public async Task<ActionResult<Product>> GetProductById(string proId)
        {
            string key = $"product-{proId}";
            Product? cachedProduct = await _redisCacheService.GetDataAsync<Product>(key);
            if(cachedProduct == null){
                cachedProduct = await _context.Products
                                        .FirstOrDefaultAsync(p => p.ProductId == proId);

                if (cachedProduct == null)
                {
                    return NotFound();
                }
                await _redisCacheService.SetDataAsync(key, cachedProduct, TimeSpan.FromMinutes(10));
            }
            

            return Ok(cachedProduct);
        }

        [HttpPost()]
        public async Task<IActionResult> CreateProduct([FromBody] Product product){
            
            _context.Products.Add(product);
            string key = $"product-{product.ProductId}";
            await _redisCacheService.SetDataAsync(key, product, TimeSpan.FromMinutes(10));
            await _context.SaveChangesAsync();
            return Ok(product.Id);
        }

        [HttpPut("{proId}")]
        public async Task<IActionResult> UpdateProduct( [FromRoute] string proId, [FromBody]Product product){
            string key = $"product-{product.ProductId}";
            Product? fetchProduct = _context.Products.FirstOrDefault(p => p.ProductId == proId);
            if(fetchProduct is null){
                return NotFound();
            }
            fetchProduct.Name = product.Name;
            fetchProduct.Price = product.Price;
            await _redisCacheService.UpdateDataAsyn(key, fetchProduct, TimeSpan.FromMinutes(10));
            _context.Update(fetchProduct);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{prodId}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] string prodId){
            string key = $"product-{prodId}";
            await _redisCacheService.RemoveDataAsync(key);
            Product? product = _context.Products.FirstOrDefault(x => x.ProductId == prodId);
            _context.Remove(product!);
           await _context.SaveChangesAsync();

           return Ok();

        }
    }
}
