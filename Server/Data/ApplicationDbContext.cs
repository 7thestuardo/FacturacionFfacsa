using FacturacionFfacsa.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace FacturacionFfacsa.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<FacturaDetalle> FacturaDetalles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configura la relación maestro-detalle entre Factura y FacturaDetalle
            modelBuilder.Entity<FacturaDetalle>()
                .HasOne(fd => fd.Factura)
                .WithMany(f => f.Detalles)
                .HasForeignKey(fd => fd.FacturaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configura relación entre FacturaDetalle y Producto
            modelBuilder.Entity<FacturaDetalle>()
                .HasOne(fd => fd.Producto)
                .WithMany() // si Producto no tiene lista de detalles, dejamos vacío
                .HasForeignKey(fd => fd.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
