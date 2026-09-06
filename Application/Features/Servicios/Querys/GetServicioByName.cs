using BackendSpa.Application.Features.Servicios.DTO;
using BackendSpa.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackendSpa.Application.Features.Servicios.Querys
{
    public class GetServicioByName : IRequestHandler<GetServicioByNameQuery, ServicioDto?>
    {
        private readonly IAppDbContext _db;

        public GetServicioByName(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<ServicioDto?> Handle(GetServicioByNameQuery request, CancellationToken cancellationToken)
        {
            return await _db.Servicios
                .Where(s => s.Nombre == request.NameServicio && s.Activo)
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
