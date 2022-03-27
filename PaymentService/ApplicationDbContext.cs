using Microsoft.EntityFrameworkCore;
using PaymentService.Models;

namespace PaymentService;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    { }
    
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasData(new User
                {
                    Id = Guid.NewGuid(),
                    CanPay = true
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    CanPay = false
                });
        });
        
        base.OnModelCreating(modelBuilder);
    } }