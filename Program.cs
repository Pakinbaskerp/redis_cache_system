using Microsoft.EntityFrameworkCore;
using RedisProductAPI.Infrastructure.Cache;
using RedisProductAPI.Infrastructure.Contract;
using RedisProductAPI.Infrastructure.Persistence;
using RedisProductAPI.MiddleWare;

var builder = WebApplication.CreateBuilder(args);

// ✅ 1. Add services
builder.Services.AddControllers(); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();

// ✅ 2. Build app
var app = builder.Build();

// ✅ 3. Middlewares
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<SessionManagementMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();

// ✅ 4. Map Controllers
app.MapControllers();

// ✅ 5. Run
app.Run();
