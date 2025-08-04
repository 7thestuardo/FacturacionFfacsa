using System;
using System.Collections.Generic;

namespace FacturacionFfacsa.Shared.Models
{
    public class Factura
    {
        public int Id { get; set; }
        public string Numero { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public decimal Total { get; set; }

        // Relación maestro-detalle
        public List<FacturaDetalle> Detalles { get; set; } = new();
    }
}
