using Microsoft.EntityFrameworkCore;
using ProductApi.Domain.Entities;
namespace ProductApi.infrastructure.Data
{
    public class ProductDBContext(DbContextOptions<ProductDBContext> options) : DbContext(options)
    {
        public DbSet<Product> Products { get; set; }
    }
}
