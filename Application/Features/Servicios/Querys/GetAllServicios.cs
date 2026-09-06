using BackendSpa.Application.Features.Servicios.DTO;
using BackendSpa.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace BackendSpa.Application.Features.Servicios.Querys
{
    public class GetAllServicios : IRequestHandler<GetServiciosQuery, List<ServicioDto>>
    {
        private readonly IAppDbContext _db;

        public GetAllServicios(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<List<ServicioDto>> Handle(GetServiciosQuery request, CancellationToken cancellationToken)
        {
            return await _db.Servicios
            .Where(s => s.Activo)
            .Include(s => s.Categoria)
            .Select(s => new ServicioDto(
            s.IdServicio,
            s.Nombre,
            s.Descripcion,
            s.DuracionMin,
            s.Precio,
            s.Categoria.Nombre
            ))
            .ToListAsync(cancellationToken);
        }

    }
}
