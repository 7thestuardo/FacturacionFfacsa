using System.Text.Json;
using System.Text.Json.Serialization;
using FacturacionFfacsa.Server.Data;
using FacturacionFfacsa.Server.Services;
using FacturacionFfacsa.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FacturacionFfacsa.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacturaController : ControllerBase
    {
        private readonly IFacturaService _facturaService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FacturaController> _logger;

        public FacturaController(
            IFacturaService facturaService,
            ApplicationDbContext context,
            ILogger<FacturaController> logger)
        {
            _facturaService = facturaService;
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAll()
        {
            try
            {
                var facturas = await _context.Facturas
                    .Select(f => new
                    {
                        f.Id,
                        f.Numero,
                        f.Fecha,
                        f.Cliente,
                        f.Total,
                        Detalles = f.Detalles.Select(d => new
                        {
                            d.Id,
                            d.ProductoId,
                            ProductoNombre = d.Producto != null ? d.Producto.Nombre : "N/A",
                            d.Cantidad,
                            d.Precio,
                            Subtotal = d.Cantidad * d.Precio
                        })
                    })
                    .ToListAsync();

                return Ok(facturas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener facturas");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetFactura(int id)
        {
            try
            {
                var factura = await _context.Facturas
                    .Where(f => f.Id == id)
                    .Select(f => new
                    {
                        f.Id,
                        f.Numero,
                        f.Fecha,
                        f.Cliente,
                        f.Total,
                        Detalles = f.Detalles.Select(d => new
                        {
                            d.Id,
                            d.ProductoId,
                            ProductoNombre = d.Producto != null ? d.Producto.Nombre : "N/A",
                            d.Cantidad,
                            d.Precio,
                            Subtotal = d.Cantidad * d.Precio
                        })
                    })
                    .FirstOrDefaultAsync();

                if (factura == null)
                    return NotFound();

                return Ok(factura);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener factura con ID {id}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Factura>> Crear([FromBody] Factura factura)
        {
            try
            {
                // Validar productos
                var productoIds = factura.Detalles.Select(d => d.ProductoId).Distinct();
                var productosExistentes = await _context.Productos
                    .Where(p => productoIds.Contains(p.Id))
                    .Select(p => p.Id)
                    .ToListAsync();

                var productosNoExistentes = productoIds.Except(productosExistentes).ToList();
                if (productosNoExistentes.Any())
                {
                    return BadRequest($"Los siguientes productos no existen: {string.Join(", ", productosNoExistentes)}");
                }

                // Calcular total
                factura.Total = factura.Detalles.Sum(d => d.Cantidad * d.Precio);

                _context.Facturas.Add(factura);
                await _context.SaveChangesAsync();

                // Retornar factura sin referencias circulares
                var jsonOptions = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                };
                var facturaSerializada = JsonSerializer.Serialize(factura, jsonOptions);
                var facturaResult = JsonSerializer.Deserialize<Factura>(facturaSerializada, jsonOptions);

                return CreatedAtAction(nameof(GetFactura), new { id = factura.Id }, facturaResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear factura");
                return StatusCode(500, "Error interno al crear factura");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Factura factura)
        {
            try
            {
                if (id != factura.Id)
                    return BadRequest("ID de factura no coincide");

                // Validar productos
                var productoIds = factura.Detalles.Select(d => d.ProductoId).Distinct();
                var productosExistentes = await _context.Productos
                    .Where(p => productoIds.Contains(p.Id))
                    .Select(p => p.Id)
                    .ToListAsync();

                var productosNoExistentes = productoIds.Except(productosExistentes).ToList();
                if (productosNoExistentes.Any())
                {
                    return BadRequest($"Los siguientes productos no existen: {string.Join(", ", productosNoExistentes)}");
                }

                // Calcular total
                factura.Total = factura.Detalles.Sum(d => d.Cantidad * d.Precio);

                _context.Entry(factura).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FacturaExists(id))
                        return NotFound();
                    else
                        throw;
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar factura con ID {id}");
                return StatusCode(500, "Error interno al actualizar factura");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var factura = await _context.Facturas.FindAsync(id);
                if (factura == null)
                    return NotFound();

                _context.Facturas.Remove(factura);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar factura con ID {id}");
                return StatusCode(500, "Error interno al eliminar factura");
            }
        }

        private bool FacturaExists(int id)
        {
            return _context.Facturas.Any(e => e.Id == id);
        }
    }
}