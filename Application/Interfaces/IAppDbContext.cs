using BackendSpa.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BackendSpa.Application.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<Categoria> Categorias { get; }
        DbSet<Servicio> Servicios { get; }
        DbSet<Cliente> Clientes { get; }
        DbSet<Cita> Citas { get; }
        DbSet<CitaServicio> CitaServicios { get; }
        DbSet<Pago> Pagos { get; }
        DbSet<Notificacion> Notificaciones { get; }

        DatabaseFacade Database { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
