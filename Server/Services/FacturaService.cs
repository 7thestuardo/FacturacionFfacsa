using FacturacionFfacsa.Server.Data;
using FacturacionFfacsa.Server.Services;
using FacturacionFfacsa.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class FacturaService : IFacturaService
{
    private readonly ApplicationDbContext _context;

    public FacturaService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Factura>> GetFacturasAsync()
    {
        return await _context.Facturas.ToListAsync();
    }

    public async Task<Factura> GetFacturaCompleta(int id)
    {
        return await _context.Facturas
            .Include(f => f.Detalles)
            .ThenInclude(d => d.Producto)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<Factura> CrearFacturaAsync(FacturaDTO facturaDto)
    {
        try
        {
            if (facturaDto == null)
                throw new ArgumentNullException(nameof(facturaDto));

            if (facturaDto.Detalles == null || !facturaDto.Detalles.Any())
                throw new InvalidOperationException("La factura debe tener al menos un detalle.");

            var factura = new Factura
            {
                Numero = facturaDto.Numero,
                Fecha = facturaDto.Fecha,
                Cliente = facturaDto.Cliente,
                Detalles = new List<FacturaDetalle>()
            };

            foreach (var detalleDto in facturaDto.Detalles)
            {
                var productoExiste = await _context.Productos.AnyAsync(p => p.Id == detalleDto.ProductoId);
                if (!productoExiste)
                    throw new InvalidOperationException($"Producto con ID {detalleDto.ProductoId} no existe");

                factura.Detalles.Add(new FacturaDetalle
                {
                    ProductoId = detalleDto.ProductoId,
                    Cantidad = (int)detalleDto.Cantidad,
                    Precio = detalleDto.Precio
                });
            }

            // ✅ Calcular total correctamente
            factura.Total = factura.Detalles.Sum(d => d.Cantidad * d.Precio);

            _context.Facturas.Add(factura);
            await _context.SaveChangesAsync();

            return await _context.Facturas
                .Include(f => f.Detalles)
                .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(f => f.Id == factura.Id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al crear factura: {ex.Message}");
            throw;
        }
    }


    public async Task<bool> ActualizarFacturaAsync(int id, FacturaDTO facturaDto)
    {
        var factura = await _context.Facturas
            .Include(f => f.Detalles)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (factura == null)
            return false;

        factura.Numero = facturaDto.Numero;
        factura.Fecha = facturaDto.Fecha;
        factura.Cliente = facturaDto.Cliente;

        // Actualizar detalles: estrategia sencilla -> borrar todos y agregar nuevos
        _context.FacturaDetalles.RemoveRange(factura.Detalles);

        factura.Detalles = facturaDto.Detalles.Select(d => new FacturaDetalle
        {
            ProductoId = d.ProductoId,
            Cantidad = (int)d.Cantidad,
            Precio = d.Precio
        }).ToList();

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EliminarFacturaAsync(int id)
    {
        var factura = await _context.Facturas.FindAsync(id);
        if (factura == null)
            return false;

        _context.Facturas.Remove(factura);
        await _context.SaveChangesAsync();
        return true;
    }
}
