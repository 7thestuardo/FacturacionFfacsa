using System.ComponentModel.DataAnnotations;

public class FacturaDTO
{
    public int Id { get; set; } // para actualizar, si es 0 es creación nueva
    [Required(ErrorMessage = "El número es obligatorio")]
    public string Numero { get; set; } = string.Empty;
    [Required(ErrorMessage = "La fecha es obligatoria")]
    public DateTime Fecha { get; set; }
    [Required(ErrorMessage = "El nombre del cliente es obligatorio")]
    public string Cliente { get; set; } = string.Empty;
    public List<FacturaDetalleDTO> Detalles { get; set; } = new();
}

