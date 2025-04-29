using Microsoft.AspNetCore.Mvc;
using RedisProductAPI.Domain.Dto;
using RedisProductAPI.Infrastructure.Contract;

namespace RedisProductAPI.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase {
    private readonly IRedisCacheService _redisCacheService;

    public AuthController(IRedisCacheService redisCacheService){
        _redisCacheService = redisCacheService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto){
        string sessionId = Guid.NewGuid().ToString();
        await _redisCacheService.SetDataAsync(sessionId, loginDto, TimeSpan.FromMinutes(2));
        return Ok(sessionId);
    }
}