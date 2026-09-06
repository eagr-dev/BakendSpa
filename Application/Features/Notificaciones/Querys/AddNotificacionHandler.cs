using BackendSpa.Application.Common.Responsive;
using BackendSpa.Application.Features.Notificaciones.DTO;
using BackendSpa.Application.Interfaces;
using BackendSpa.Domain;
using MediatR;

namespace BackendSpa.Application.Features.Notificaciones.Querys
{
    public class AddNotificacionHandler(IAppDbContext db, INotificacion notificacion) : IRequestHandler<AddNotificacion, Responsive<bool>>
    {
        private readonly IAppDbContext _db = db;
        private readonly INotificacion _notificacion = notificacion;
        public async Task<Responsive<bool>> Handle(AddNotificacion request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;
            if (!Enum.TryParse<TipoNotificacion>(dto.Tipo, true, out var tipoNotificacion))
            {
                throw new ArgumentException($"El tipo '{dto.Tipo}' no es válido.");
            }

            var resultadoSms = await _notificacion.EnviarMensajeAsync(dto.Destinatario, dto.Mensaje);

            Notificacion notificacion = new()
            {
                IdCita = dto.IdCita,
                Destinatario = dto.Destinatario,
                Tipo = tipoNotificacion,
                Mensaje = dto.Mensaje,
                Status = resultadoSms.Success ? "enviado" : "fallido",
                EnviadoEn = resultadoSms.Success ? DateTime.UtcNow : null,
                Intentos = 0,
                ErrorDetalle = resultadoSms.Mensaje,
            };

            await _db.Notificaciones.AddAsync(notificacion, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);

            return new Responsive<bool>(true, "", true);
        }
    }
}
