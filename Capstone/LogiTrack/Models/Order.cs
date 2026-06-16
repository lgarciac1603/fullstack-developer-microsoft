using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LogiTrack.Models;

/// <summary>
/// Entidad que representa una orden de compra
/// </summary>
public class Order
{
    [Key]
    public int OrderId { get; set; }

    [Required]
    [MaxLength(100)]
    public string CustomerName { get; set; } = string.Empty;

    [Required]
    public DateTime DatePlaced { get; set; } = DateTime.UtcNow;

    public DateTime? DateShipped { get; set; }

    [MaxLength(50)]
    public string Status { get; set; } = "Pending";

    public decimal TotalAmount { get; set; }

    // Relación uno a muchos: una orden puede tener muchos items
    public List<InventoryItem> Items { get; set; } = new();

    /// <summary>
    /// Agrega un item a la orden
    /// </summary>
    public void AddItem(InventoryItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        item.OrderId = this.OrderId;
        item.Order = this;
        Items.Add(item);
        RecalculateTotal();
    }

    /// <summary>
    /// Remueve un item de la orden por su ID
    /// </summary>
    public void RemoveItem(int itemId)
    {
        var item = Items.FirstOrDefault(i => i.ItemId == itemId);
        if (item != null)
        {
            item.OrderId = null;
            item.Order = null;
            Items.Remove(item);
            RecalculateTotal();
        }
    }

    /// <summary>
    /// Recalcula el total de la orden
    /// </summary>
    private void RecalculateTotal()
    {
        // Asumimos un precio base de $10 por item para el ejemplo
        TotalAmount = Items.Count * 10m;
    }

    /// <summary>
    /// Retorna un resumen de la orden
    /// </summary>
    public string GetOrderSummary()
    {
        return $"Order #{OrderId} for {CustomerName} | Items: {Items.Count} | " +
               $"Total: ${TotalAmount:F2} | Status: {Status} | Placed: {DatePlaced:d}";
    }

    /// <summary>
    /// Marca la orden como enviada
    /// </summary>
    public void MarkAsShipped()
    {
        Status = "Shipped";
        DateShipped = DateTime.UtcNow;
    }
}