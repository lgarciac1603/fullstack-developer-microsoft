using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LogiTrack.Data;
using LogiTrack.Models;
using LogiTrack.DTOs;

namespace LogiTrack.Controllers;

/// <summary>
/// Controlador de órdenes con validación y autorización
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly LogiTrackContext _context;
    private readonly ILogger<OrderController> _logger;

    public OrderController(LogiTrackContext context, ILogger<OrderController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// GET: /api/orders - Obtiene todas las órdenes
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var orders = await _context.Orders
                .Include(o => o.Items) // Eager Loading para evitar N+1
                .AsNoTracking() // Optimización
                .OrderByDescending(o => o.DatePlaced)
                .ToListAsync();

            return Ok(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener órdenes");
            return StatusCode(500, new { message = "Error al obtener órdenes" });
        }
    }

    /// <summary>
    /// GET: /api/orders/{id} - Obtiene una orden con sus items
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
                return NotFound(new { message = $"Orden con ID {id} no encontrada" });

            return Ok(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al obtener orden {id}");
            return StatusCode(500, new { message = "Error al obtener la orden" });
        }
    }

    /// <summary>
    /// POST: /api/orders - Crea una nueva orden (solo Manager/Admin)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> Create([FromBody] OrderCreateDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });

        try
        {
            // Validar que haya items
            if (model.InventoryItemIds == null || !model.InventoryItemIds.Any())
                return BadRequest(new { message = "La orden debe incluir al menos un item" });

            var order = new Order
            {
                CustomerName = model.CustomerName,
                DatePlaced = DateTime.UtcNow,
                Status = model.Status ?? "Pending"
            };

            // Agregar items a la orden
            foreach (var itemId in model.InventoryItemIds.Distinct())
            {
                var item = await _context.InventoryItems.FindAsync(itemId);
                if (item != null)
                {
                    order.AddItem(item);
                }
                else
                {
                    _logger.LogWarning($"Item {itemId} no encontrado para la orden");
                }
            }

            // Validar que haya al menos un item válido
            if (!order.Items.Any())
                return BadRequest(new { message = "Ninguno de los items especificados existe" });

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Orden creada: #{order.OrderId} para {order.CustomerName}");

            return CreatedAtAction(nameof(GetById), new { id = order.OrderId }, order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear orden");
            return StatusCode(500, new { message = "Error al crear la orden" });
        }
    }

    /// <summary>
    /// DELETE: /api/orders/{id} - Elimina una orden (solo Manager/Admin)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
                return NotFound(new { message = $"Orden con ID {id} no encontrada" });

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Orden eliminada: #{id}");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al eliminar orden {id}");
            return StatusCode(500, new { message = "Error al eliminar la orden" });
        }
    }
}