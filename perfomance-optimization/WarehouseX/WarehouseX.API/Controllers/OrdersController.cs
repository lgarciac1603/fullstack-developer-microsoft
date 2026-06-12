using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseX.Core.Models;
using WarehouseX.Data;

namespace WarehouseX.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _context;
    public OrdersController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> GetAllOrders() => Ok(await _context.Orders.ToListAsync());

    [HttpGet("details")]
    public async Task<IActionResult> GetOrdersWithDetails()
    {
        var orders = await _context.Orders.ToListAsync();
        foreach (var order in orders)
            order.OrderItems = await _context.OrderItems.Where(oi => oi.OrderId == order.Id).ToListAsync();
        return Ok(orders);
    }

    [HttpGet("recent")]
    public async Task<IActionResult> GetRecentOrders()
    {
        var orders = await _context.Orders.ToListAsync();
        var recent = orders.Where(o => o.OrderDate > DateTime.Now.AddDays(-30));
        return Ok(recent);
    }
}
