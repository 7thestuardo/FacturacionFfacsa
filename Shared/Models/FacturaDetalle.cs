using System.Text.Json.Serialization;
using FacturacionFfacsa.Shared.Models;

public class FacturaDetalle
{
    public int Id { get; set; }
    public int FacturaId { get; set; }
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    public decimal Precio { get; set; }

    [JsonIgnore] // ← Esto evita la serialización de la propiedad
    public Factura? Factura { get; set; }

    public Producto? Producto { get; set; }
}