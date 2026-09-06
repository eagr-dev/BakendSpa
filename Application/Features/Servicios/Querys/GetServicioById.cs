using BackendSpa.Application.Features.Servicios.DTO;
using BackendSpa.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackendSpa.Application.Features.Servicios.Querys
{
    public class GetServicioById : IRequestHandler<GetServicioByIdQuery, ServicioDto?>
    {
        private readonly IAppDbContext _db;

        public GetServicioById(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<ServicioDto?> Handle(GetServicioByIdQuery request, CancellationToken cancellationToken)
        {
            return await _db.Servicios
                .Where(s => s.IdServicio == request.IdServicio && s.Activo)
                .Include(s => s.Categoria)
                .Select(s => new ServicioDto(
                    s.IdServicio,
                    s.Nombre,
                    s.Descripcion,
                    s.DuracionMin,
                    s.Precio,
                    s.Categoria.Nombre
                ))
                .FirstOrDefaultAsync(cancellationToken);
        }

    }
}
