using Microsoft.EntityFrameworkCore;
using StockService.Models;

namespace StockService;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    { }
    
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasData(
                new Product
                {
                    Id = 1,
                    State = ProductState.Free
                },
                new Product
                {
                    Id = 2,
                    State = ProductState.Free
                },
                new Product
                {
                    Id = 3,
                    State = ProductState.Free
                });
        });
            
        base.OnModelCreating(modelBuilder);
    }
}