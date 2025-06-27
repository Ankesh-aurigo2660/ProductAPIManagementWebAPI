using Microsoft.EntityFrameworkCore;
using ProductManagementAPI.Models.Entities;

namespace ProductManagementAPI.Data
{
    public class ProductDBContext : DbContext
    {
        public ProductDBContext(DbContextOptions<ProductDBContext> options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; } = null!;
    }
}
