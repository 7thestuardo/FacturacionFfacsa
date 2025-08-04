namespace FacturacionFfacsa.Server.Services;
using FacturacionFfacsa.Shared.Models;

public interface IFacturaService
{
    Task<List<Factura>> GetFacturasAsync(); 
    Task<Factura> GetFacturaCompleta(int id);
    Task<Factura> CrearFacturaAsync(FacturaDTO facturaDto);
    Task<bool> ActualizarFacturaAsync(int id, FacturaDTO facturaDto);
    Task<bool> EliminarFacturaAsync(int id);
}
