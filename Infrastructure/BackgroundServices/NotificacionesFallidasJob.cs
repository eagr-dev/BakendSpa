using BackendSpa.Application.Features.Notificaciones.DTO;
using BackendSpa.Application.Features.Notificaciones.Querys;
using BackendSpa.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackendSpa.Infrastructure.BackgroundServices
{
    public class NotificacionesFallidasJob : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<NotificacionesFallidasJob> _logger;
        private readonly TimeSpan _intervalo = TimeSpan.FromHours(1);
        private readonly double diferenciaHorarioUtcAGdl = -6;
        private readonly int MaxIntentos = 3;

        public NotificacionesFallidasJob(IServiceScopeFactory scopeFactory, ILogger<NotificacionesFallidasJob> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ProcesarNotificacionesFallidas(stoppingToken);
                await Task.Delay(_intervalo, stoppingToken);
            }
        }

        private async Task ProcesarNotificacionesFallidas(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
            var notificacion = scope.ServiceProvider.GetRequiredService<INotificacion>();

            var notificaciones = await db.Notificaciones
            .Join(db.Citas,
                n => n.IdCita,
                c => c.IdCita,
                (n, c) => new { Notificacion = n, Cita = c })
            .Where(x => x.Notificacion.Status == "fallido" &&
                        x.Notificacion.Intentos < MaxIntentos &&
                        x.Cita.Fecha > DateTime.UtcNow.AddHours(diferenciaHorarioUtcAGdl))
            .Select(x => x.Notificacion)
            .AsTracking()
            .ToListAsync(cancellationToken);

            _logger.LogInformation("Notificaciones fallidas a reintentar: {Count}", notificaciones.Count);

            foreach (var notif in notificaciones)
            {
                notif.Intentos++;

                var resultado = await notificacion.EnviarMensajeAsync(notif.Destinatario, notif.Mensaje);

                notif.Status = resultado.Success ? "enviado" : "fallido";
                notif.ErrorDetalle = resultado.Mensaje;
                notif.EnviadoEn = resultado.Success ? DateTime.UtcNow : notif.EnviadoEn;

                _logger.LogInformation("Reintento notificacion {Id}: {Status} {Error}",
                    notif.IdNotificacion,
                    resultado.Success ? "exitoso" : "fallido",
                    resultado.Mensaje ?? "");
            }

            await db.SaveChangesAsync(cancellationToken); 
        }
    }
}
