using FacturacionFfacsa.Server.Data;
using FacturacionFfacsa.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FacturacionFfacsa.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacturaDetalleController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FacturaDetalleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/FacturaDetalle
        [HttpGet]
        public async Task<ActionResult<List<FacturaDetalle>>> Get()
        {
            return await _context.FacturaDetalles
                .Include(fd => fd.Producto) //  incluir datos del producto
                .ToListAsync();
        }

        // GET: api/FacturaDetalle/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FacturaDetalle>> Get(int id)
        {
            var detalle = await _context.FacturaDetalles
                .Include(fd => fd.Producto)
                .FirstOrDefaultAsync(fd => fd.Id == id);

            if (detalle == null)
                return NotFound();

            return detalle;
        }

        // POST: api/FacturaDetalle
        [HttpPost]
        public async Task<ActionResult> Post(FacturaDetalle detalle)
        {
            _context.FacturaDetalles.Add(detalle);
            await _context.SaveChangesAsync();
            return Ok(detalle);
        }

        // PUT: api/FacturaDetalle/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, FacturaDetalle detalle)
        {
            if (id != detalle.Id)
                return BadRequest();

            var existe = await _context.FacturaDetalles.AnyAsync(fd => fd.Id == id);
            if (!existe)
                return NotFound();

            _context.Entry(detalle).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/FacturaDetalle/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var detalle = await _context.FacturaDetalles.FindAsync(id);
            if (detalle == null)
                return NotFound();

            _context.FacturaDetalles.Remove(detalle);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // obtener todos los detalles por factura
        // GET: api/FacturaDetalle/Factura/5
        [HttpGet("Factura/{facturaId}")]
        public async Task<ActionResult<List<FacturaDetalle>>> GetByFactura(int facturaId)
        {
            return await _context.FacturaDetalles
                .Where(fd => fd.FacturaId == facturaId)
                .Include(fd => fd.Producto)
                .ToListAsync();
        }
    }
}
