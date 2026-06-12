using Microsoft.EntityFrameworkCore;
using WarehouseX.Core.Models;

namespace WarehouseX.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Product> Products { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>().HasIndex(o => o.OrderDate).HasDatabaseName("IX_Orders_OrderDate");
        modelBuilder.Entity<Order>().HasIndex(o => o.CustomerId).HasDatabaseName("IX_Orders_CustomerId");
        modelBuilder.Entity<Order>().HasIndex(o => o.Status).HasDatabaseName("IX_Orders_Status");
        modelBuilder.Entity<Product>().HasIndex(p => p.Category).HasDatabaseName("IX_Products_Category");
        modelBuilder.Entity<Product>().HasIndex(p => p.Price).HasDatabaseName("IX_Products_Price");
    }
}
