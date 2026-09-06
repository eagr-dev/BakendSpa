using BackendSpa.Application.Common.Responsive;
using BackendSpa.Application.Features.Citas.DTO;
using BackendSpa.Application.Interfaces;
using BackendSpa.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackendSpa.Application.Features.Citas.Querys
{
    public class GetCitaByIdHandler(IAppDbContext db) : IRequestHandler<GetCitaById, Responsive<CitaDto>>
    {
        private readonly IAppDbContext _db = db;

        public async Task<Responsive<CitaDto>> Handle(GetCitaById request, CancellationToken cancellationToken)
        {
            var cita = await _db.Citas
                .Include(c => c.Cliente)
                .FirstOrDefaultAsync(c => c.IdCita == request.id, cancellationToken) ??
                throw new ArgumentException($"la cita con el id {request.id} no existe");

            // Mapeo manual hacia el Record CitaDto
            var dto = new CitaDto(
                cita.IdCita,
                cita.IdCliente,
                cita.Cliente.Nombre, // Accedemos gracias al Include
                cita.Fecha,
                cita.HoraInicio,
                cita.HoraFin,
                cita.Estado.ToString(), // Convertimos el Enum a string
                cita.PrecioTotal,
                cita.Anticipo
            );

            return new Responsive<CitaDto>(true, "", dto);
        }
    }
}
