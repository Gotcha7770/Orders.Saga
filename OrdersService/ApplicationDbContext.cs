using Microsoft.EntityFrameworkCore;
using OrdersService.Models;

namespace OrdersService;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    { }
    
    public DbSet<Order> Orders { get; set; }
}