using BackendSpa.Application.Common.Responsive;
using BackendSpa.Application.Features.Citas.CitaDetalles.DTO;
using BackendSpa.Application.Interfaces;
using BackendSpa.Domain;
using MediatR;

namespace BackendSpa.Application.Features.Citas.CitaDetalles.Querys
{
    public class GetCitaServicioCreateHandler(IAppDbContext db) : IRequestHandler<GetCitaServicioCreate, Responsive<bool>>
    {
        private readonly IAppDbContext _db = db;
        public async Task<Responsive<bool>> Handle(GetCitaServicioCreate request, CancellationToken cancellationToken)
        {
            var dtos = request.CitaServicioDtos;

            foreach (var item in dtos)
            {
                CitaServicio EntidadCitaServicio = new()
                {
                    IdCita = item.IdCita,
                    IdServicio = item.IdServicio,
                    PrecioUnitario = item.PrecioUnitario
                };

                await _db.CitaServicios.AddAsync(EntidadCitaServicio, cancellationToken);
            }

            await _db.SaveChangesAsync(cancellationToken);

            return new Responsive<bool>(true, "", true);
        }
    }
}
