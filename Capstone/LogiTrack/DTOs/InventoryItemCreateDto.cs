using System.ComponentModel.DataAnnotations;

namespace LogiTrack.DTOs;

/// <summary>
/// Data Transfer Object para crear items de inventario
/// </summary>
public class InventoryItemCreateDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "La cantidad es requerida")]
    [Range(0, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor o igual a 0")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "La ubicación es requerida")]
    [MaxLength(50)]
    public string Location { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Category { get; set; }
}