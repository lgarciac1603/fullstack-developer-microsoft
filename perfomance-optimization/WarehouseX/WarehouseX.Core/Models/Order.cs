using WarehouseX.Core.Models;

namespace WarehouseX.Core.Models;

public class Order
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime? ShippedDate { get; set; }
    public List<OrderItem> OrderItems { get; set; } = new();
}