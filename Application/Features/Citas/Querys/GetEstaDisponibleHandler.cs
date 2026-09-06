using BackendSpa.Application.Features.Servicios.DTO;
using BackendSpa.Application.Interfaces;
using BackendSpa.Application.Common.Responsive;
using MediatR;
using Microsoft.EntityFrameworkCore;
using BackendSpa.Application.Features.Citas.DTO;

namespace BackendSpa.Application.Features.Citas.Querys
{
    public class GetEstaDisponibleHandler : IRequestHandler<GetEstaDisponibleQuery, Responsive<bool>>
    {
        private readonly IAppDbContext _db;
        private readonly TimeSpan HoraEntrada = new(7, 59, 59), HoraSalida = new(20,0,0);
        private readonly double diferenciaHorarioUtcAGdl = 6; 

        public GetEstaDisponibleHandler(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<Responsive<bool>> Handle(GetEstaDisponibleQuery request, CancellationToken cancellationToken)
        {

            if (HoraEntrada >= request.cita.HoraInicio || HoraSalida <= request.cita.HoraInicio) return new Responsive<bool>(
                false,"la hora pedida esta fuera de las horas de trabajo",false
                );

            DateTime Fecha = request.cita.Fecha;
            TimeSpan Horario = request.cita.HoraInicio;
            DateTime FechaTotalCitaGDL = new DateTime(
                Fecha.Year, 
                Fecha.Month, 
                Fecha.Day, 
                Horario.Hours, 
                Horario.Minutes,
                Horario.Seconds
                );
            DateTime FechaTotaCitaUTC = FechaTotalCitaGDL.AddHours(diferenciaHorarioUtcAGdl); 

            if(DateTime.UtcNow >= FechaTotaCitaUTC) 
            {
                return new Responsive<bool>(false, "La hora pedida ya transcurrio elija nueva fecha", false);
            }

            bool estaOcupado = await _db.Citas.AnyAsync(c =>
            c.Fecha == request.cita.Fecha &&
            c.HoraInicio < request.cita.HoraFin &&
            c.HoraFin > request.cita.HoraInicio &&
            c.Estado != Domain.EstadoCita.Cancelada,
            cancellationToken);

            bool estaDisponible = !estaOcupado;
            return new Responsive<bool>(true, estaDisponible ? "hora disponible" : "hora no disponible" , estaDisponible);
        }
    }
}
