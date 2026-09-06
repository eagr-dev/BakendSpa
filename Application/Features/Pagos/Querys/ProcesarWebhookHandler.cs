using BackendSpa.Application.Common.Responsive;
using BackendSpa.Application.Features.Notificaciones.DTO;
using BackendSpa.Application.Features.Notificaciones.Querys;
using BackendSpa.Application.Interfaces;
using BackendSpa.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Twilio.TwiML.Messaging;

namespace BackendSpa.Application.Features.Pagos.Querys
{
    public class ProcesarWebhookHandler : IRequestHandler<ProcesarWebhookCommand, Responsive<bool>>
    {
        private readonly IAppDbContext _db;
        private readonly IPlataformaPago _plataforma;
        private readonly IConfiguration _config;
        private readonly IMediator _mediator;
        private readonly string numeroTelefonicoSpa = string.Empty;

        public ProcesarWebhookHandler(IAppDbContext db, IPlataformaPago plataforma, IConfiguration config, IMediator mediator)
        {
            _db = db;
            _plataforma = plataforma;
            _config = config;
            numeroTelefonicoSpa = _config["NumberPhone:PhoneNumber"]!;
            _mediator = mediator;
        }

        public async Task<Responsive<bool>> Handle(ProcesarWebhookCommand request, CancellationToken cancellationToken)
        {
            var notif = request.Notificacion;

            if (notif.Type != "payment" || notif.Data?.Id is null)
                return new Responsive<bool>(true, "Notificacion ignorada", false);

            // 1. Idempotencia: verificar si ya fue procesado
            var pagoExistente = await _db.Pagos
                .AnyAsync(p => p.PaymentId == notif.Data.Id, cancellationToken);

            if (pagoExistente)
                return new Responsive<bool>(true, $"PaymentId {notif.Data.Id} ya procesado", false);

            // 2. Consultar el pago en MP
            var pagoMp = await _plataforma.ObtenerPagoAsync(notif.Data.Id);
            if (pagoMp is null)
                return new Responsive<bool>(false, "No se pudo obtener el pago de MP", false);

            var root = pagoMp.RootElement;

            var extRef = root.TryGetProperty("external_reference", out var er) ? er.GetString() : null;
            var prefId = root.TryGetProperty("preference_id", out var pref) ? pref.GetString() : null;
            var status = root.TryGetProperty("status", out var st) ? st.GetString() : null;

            // 3. Buscar el pago en BD
            var pago = await _db.Pagos
                .FirstOrDefaultAsync(p =>
                    (extRef != null && p.ExternalReference == extRef) ||
                    (prefId != null && p.PreferenceId == prefId),
                    cancellationToken);

            if (pago is null)
                return new Responsive<bool>(false, "Pago no encontrado en BD", false);

            

            // 4. Actualizar estado de la cita y enviar notificaciones
            var cita = await _db.Citas
                .Include(c => c.Cliente)
                .FirstOrDefaultAsync(c => c.IdCita == pago.IdCita, cancellationToken);

            if (cita is not null && cita.Estado == EstadoCita.Confirmada)
            {
                // Reembolsar el pago duplicado automáticamente
                await _plataforma.ReembolsarPagoAsync(notif.Data.Id);
                return new Responsive<bool>(true, "Cita ya confirmada, pago duplicado reembolsado", false);
            }

            // 5. Actualizar datos del pago
            pago.PaymentId = notif.Data.Id;  // <-- Esto cierra la idempotencia en futuros webhooks
            pago.Status = status ?? pago.Status;
            pago.CollectionStatus = root.TryGetProperty("status_detail", out var sd) ? sd.GetString() : null;
            pago.PaymentType = root.TryGetProperty("payment_type_id", out var pt) ? pt.GetString() : null;
            pago.ProcessingMode = root.TryGetProperty("processing_mode", out var pm) ? pm.GetString() : null;

            await _db.SaveChangesAsync(cancellationToken);

            if (cita is not null)
            {
                Cliente cliente = cita.Cliente;
                string Mensaje = string.Empty;

                if (status == "approved")
                {

                    cita.Estado = EstadoCita.Confirmada;

                    if (!string.IsNullOrEmpty(pago.PreferenceId))
                        await _plataforma.ExpirarPreferenciaAsync(pago.PreferenceId);

                    string msg = $"El cliente {cliente.Nombre} con numero {cliente.Telefono} aseguro una cita " +
                        $"el dia {cita.Fecha:dd/MM/yyyy} a las {cita.HoraInicio}";

                    await _mediator.Send(new AddNotificacion(new NotificacionDto
                    {
                        IdNotificacion = 0,
                        IdCita = cita.IdCita,
                        Destinatario = numeroTelefonicoSpa,
                        Tipo = TipoNotificacion.Duena.ToString(),
                        Mensaje = msg,
                        EnviadoEn = DateTime.UtcNow
                    }), cancellationToken);

                    Mensaje = $"Tu cita del {cita.Fecha:dd/MM/yyyy} a las {cita.HoraInicio}" +
                        $" ha sido confirmada. ¡Te esperamos!.\n" +
                        $"En caso de desear cancelar solo de click aqui " +
                        $"{_config["MercadoPago:UrlBase"]}/api/citas/cancelarcita/{cita.IdCita}";
                }
                else if (status == "rejected" || status == "cancelled")
                {
                    cita.Estado = EstadoCita.Cancelada;
                    Mensaje = "Su cita fue cancelada";
                }

                if (!string.IsNullOrEmpty(Mensaje))
                {
                    await _mediator.Send(new AddNotificacion(new NotificacionDto
                    {
                        IdNotificacion = 0,
                        IdCita = cita.IdCita,
                        Destinatario = cita.Cliente.Telefono,
                        Tipo = TipoNotificacion.Cliente.ToString(),
                        Mensaje = Mensaje,
                        EnviadoEn = DateTime.UtcNow
                    }), cancellationToken);
                }

                await _db.SaveChangesAsync(cancellationToken);  // Guardar estado de la cita
            }

            return new Responsive<bool>(true, $"Pago procesado: {status}", true);
        }
    }
}
