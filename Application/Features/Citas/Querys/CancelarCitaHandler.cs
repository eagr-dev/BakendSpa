using BackendSpa.Application.Common.Responsive;
using BackendSpa.Application.Features.Notificaciones.DTO;
using BackendSpa.Application.Features.Notificaciones.Querys;
using BackendSpa.Application.Interfaces;
using BackendSpa.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackendSpa.Application.Features.Citas.Querys
{
    public class CancelarCitaHandler : IRequestHandler<CancelarCitaCommand, Responsive<bool>>
    {
        private readonly IAppDbContext _db;
        private readonly IConfiguration _config;
        private readonly IMediator _mediator;
        private readonly string numeroTelefonicoSpa = string.Empty;

        public CancelarCitaHandler(IAppDbContext db, IPlataformaPago plataforma, IConfiguration config, IMediator mediator)
        {
            _db = db;
            _config = config;
            numeroTelefonicoSpa = _config["NumberPhone:PhoneNumber"]!;
            _mediator = mediator;
        }
        public async Task<Responsive<bool>> Handle(CancelarCitaCommand request, CancellationToken cancellationToken)
        {
            var cita = await _db.Citas
                .Include(c => c.Cliente)
                .FirstOrDefaultAsync(c => c.IdCita == request.IdCita, cancellationToken) ??
                throw new ArgumentException($"la cita con el id {request.IdCita} no existe");

            if (cita.Estado == Domain.EstadoCita.Cancelada) return new Responsive<bool>(false,
                $"La cita con el id {request.IdCita} ya fue cancelada", false);

            cita.Estado = EstadoCita.Cancelada;
            string msg = $"el usuario {cita.Cliente.Nombre} cancelo su cita de las {cita.HoraInicio}";
            string Mensaje = $"Su cita fue cancelada con exito";

            //Enviar de que se cancelacion al spa
            NotificacionDto dtoSpa = new()
            {
                IdNotificacion = 0,
                IdCita = cita.IdCita,
                Destinatario = numeroTelefonicoSpa,
                Tipo = TipoNotificacion.Duena.ToString(),
                Mensaje = msg,
                EnviadoEn = DateTime.UtcNow
            };

            await _mediator.Send(new AddNotificacion(dtoSpa), cancellationToken);

            NotificacionDto dtoCliente = new()
            {
                IdNotificacion = 0,
                IdCita = cita.IdCita,
                Destinatario = cita.Cliente.Telefono,
                Tipo = TipoNotificacion.Cliente.ToString(),
                Mensaje = Mensaje,
                EnviadoEn = DateTime.UtcNow
            };

            await _mediator.Send(new AddNotificacion(dtoCliente), cancellationToken);

            await _db.SaveChangesAsync(cancellationToken);

            return new Responsive<bool>(true, "", true);
        }
    }
}
