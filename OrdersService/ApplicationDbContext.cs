using Microsoft.EntityFrameworkCore;
using Orders.Saga.Models;

namespace Orders.Saga;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    { }
    
    public DbSet<Order> Orders { get; set; }
}