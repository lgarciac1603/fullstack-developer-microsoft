using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LogiTrack.Models;

namespace LogiTrack.Data;

/// <summary>
/// Contexto de base de datos con Identity y relaciones personalizadas
/// </summary>
public class LogiTrackContext : IdentityDbContext<ApplicationUser>
{
    public LogiTrackContext(DbContextOptions<LogiTrackContext> options)
        : base(options)
    {
    }

    public DbSet<InventoryItem> InventoryItems { get; set; }
    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurar relación uno a muchos entre Order e InventoryItem
        modelBuilder.Entity<InventoryItem>()
            .HasOne(i => i.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.SetNull); // Si se elimina una orden, los items no se eliminan

        // Índices para mejorar rendimiento
        modelBuilder.Entity<InventoryItem>()
            .HasIndex(i => i.Name)
            .HasDatabaseName("IX_InventoryItem_Name");

        modelBuilder.Entity<InventoryItem>()
            .HasIndex(i => i.Location)
            .HasDatabaseName("IX_InventoryItem_Location");

        modelBuilder.Entity<Order>()
            .HasIndex(o => o.CustomerName)
            .HasDatabaseName("IX_Order_CustomerName");

        modelBuilder.Entity<Order>()
            .HasIndex(o => o.Status)
            .HasDatabaseName("IX_Order_Status");
    }
}