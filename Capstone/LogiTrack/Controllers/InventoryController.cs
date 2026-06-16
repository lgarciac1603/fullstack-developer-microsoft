using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using LogiTrack.Data;
using LogiTrack.Models;
using LogiTrack.DTOs;

namespace LogiTrack.Controllers;

/// <summary>
/// Controlador de inventario con caché y autorización
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InventoryController : ControllerBase
{
    private readonly LogiTrackContext _context;
    private readonly IMemoryCache _cache;
    private readonly ILogger<InventoryController> _logger;
    private const string InventoryCacheKey = "InventoryList";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(30);

    public InventoryController(
        LogiTrackContext context,
        IMemoryCache cache,
        ILogger<InventoryController> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// GET: /api/inventory - Obtiene todos los items (con caché)
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            // Intentar obtener de caché
            if (_cache.TryGetValue(InventoryCacheKey, out List<InventoryItem>? cachedItems))
            {
                _logger.LogInformation("Inventario servido desde caché");
                return Ok(cachedItems);
            }

            // Si no está en caché, obtener de BD con optimización
            var items = await _context.InventoryItems
                .AsNoTracking() // Optimización: no trackear cambios
                .OrderBy(i => i.Name)
                .ToListAsync();

            // Guardar en caché
            _cache.Set(InventoryCacheKey, items, CacheDuration);

            _logger.LogInformation($"Inventario servido desde BD: {items.Count} items");

            return Ok(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener inventario");
            return StatusCode(500, new { message = "Error al obtener inventario" });
        }
    }

    /// <summary>
    /// GET: /api/inventory/{id} - Obtiene un item por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var item = await _context.InventoryItems
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.ItemId == id);

            if (item == null)
                return NotFound(new { message = $"Item con ID {id} no encontrado" });

            return Ok(item);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al obtener item {id}");
            return StatusCode(500, new { message = "Error al obtener el item" });
        }
    }

    /// <summary>
    /// POST: /api/inventory - Crea un nuevo item (solo Manager/Admin)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> Create([FromBody] InventoryItemCreateDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });

        try
        {
            var item = new InventoryItem
            {
                Name = model.Name,
                Quantity = model.Quantity,
                Location = model.Location,
                Category = model.Category,
                LastUpdated = DateTime.UtcNow
            };

            _context.InventoryItems.Add(item);
            await _context.SaveChangesAsync();

            // Invalidar caché
            _cache.Remove(InventoryCacheKey);

            _logger.LogInformation($"Item creado: {item.Name} (ID: {item.ItemId})");

            return CreatedAtAction(nameof(GetById), new { id = item.ItemId }, item);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear item");
            return StatusCode(500, new { message = "Error al crear el item" });
        }
    }

    /// <summary>
    /// PUT: /api/inventory/{id} - Actualiza un item (solo Manager/Admin)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] InventoryItemCreateDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });

        try
        {
            var existingItem = await _context.InventoryItems.FindAsync(id);
            if (existingItem == null)
                return NotFound(new { message = $"Item con ID {id} no encontrado" });

            existingItem.Name = model.Name;
            existingItem.Quantity = model.Quantity;
            existingItem.Location = model.Location;
            existingItem.Category = model.Category;
            existingItem.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Invalidar caché
            _cache.Remove(InventoryCacheKey);

            _logger.LogInformation($"Item actualizado: {existingItem.Name} (ID: {id})");

            return Ok(existingItem);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al actualizar item {id}");
            return StatusCode(500, new { message = "Error al actualizar el item" });
        }
    }

    /// <summary>
    /// DELETE: /api/inventory/{id} - Elimina un item (solo Manager/Admin)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var item = await _context.InventoryItems.FindAsync(id);
            if (item == null)
                return NotFound(new { message = $"Item con ID {id} no encontrado" });

            _context.InventoryItems.Remove(item);
            await _context.SaveChangesAsync();

            // Invalidar caché
            _cache.Remove(InventoryCacheKey);

            _logger.LogInformation($"Item eliminado: {item.Name} (ID: {id})");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al eliminar item {id}");
            return StatusCode(500, new { message = "Error al eliminar el item" });
        }
    }
}