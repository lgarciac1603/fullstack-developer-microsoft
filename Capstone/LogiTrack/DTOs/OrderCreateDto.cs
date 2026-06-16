using System.ComponentModel.DataAnnotations;

namespace LogiTrack.DTOs;

/// <summary>
/// Data Transfer Object para crear órdenes
/// </summary>
public class OrderCreateDto
{
    [Required(ErrorMessage = "El nombre del cliente es requerido")]
    [MaxLength(100)]
    public string CustomerName { get; set; } = string.Empty;

    public List<int> InventoryItemIds { get; set; } = new();

    [MaxLength(50)]
    public string Status { get; set; } = "Pending";
}