using BackendSpa.Domain;
using BackendSpa.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackendSpa.Infrastructure.Persistance
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Cita> Citas { get; set; }
        public DbSet<CitaServicio> CitaServicios { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cita>()
                .Property(c => c.Estado)
                .HasColumnType("enum('pendiente','confirmada','cancelada')")
                .HasConversion(
                    v => v.ToString().ToLower(),
                    v => Enum.Parse<EstadoCita>(v, ignoreCase: true)
                );

            modelBuilder.Entity<Notificacion>()
                .Property(n => n.Tipo)
                .HasColumnType("enum('cliente','duena')")
                .HasConversion(
                    v => v.ToString().ToLower(),
                    v => Enum.Parse<TipoNotificacion>(v, ignoreCase: true)
                );
        }
    }
}
