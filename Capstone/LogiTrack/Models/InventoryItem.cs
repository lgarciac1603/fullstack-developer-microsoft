using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LogiTrack.Models;

/// <summary>
/// Entidad que representa un item en el inventario
/// </summary>
public class InventoryItem
{
    [Key]
    public int ItemId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }

    [Required]
    [MaxLength(50)]
    public string Location { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Category { get; set; }

    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    // Clave foránea para relación con Order
    public int? OrderId { get; set; }

    [JsonIgnore]
    public Order? Order { get; set; }

    /// <summary>
    /// Método que formatea la información del item para mostrar
    /// </summary>
    public string DisplayInfo()
    {
        return $"Item: {Name} | Quantity: {Quantity} | Location: {Location} | Category: {Category ?? "N/A"}";
    }

    /// <summary>
    /// Actualiza la cantidad y registra la fecha de modificación
    /// </summary>
    public void UpdateQuantity(int newQuantity)
    {
        Quantity = newQuantity;
        LastUpdated = DateTime.UtcNow;
    }
}