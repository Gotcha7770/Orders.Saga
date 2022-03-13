using Microsoft.EntityFrameworkCore;
using Orders.Saga.Models;

namespace Orders.Saga;

public class ApplicationDbContext : DbContext
{
    public DbSet<Order> Orders { get; set; }
}