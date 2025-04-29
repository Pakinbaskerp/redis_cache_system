using Domain;
using Microsoft.EntityFrameworkCore;

namespace RedisProductAPI.Infrastructure.Persistence;
public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}

