using BackendSpa.Application.Features.Citas.Querys;
using BackendSpa.Application.Interfaces;
using BackendSpa.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackendSpa.Infrastructure.BackgroundServices
{
    public class CitasPendientesJob : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<CitasPendientesJob> _logger;
        private readonly TimeSpan _intervalo = TimeSpan.FromMinutes(15); // corre cada 15 min

        public CitasPendientesJob(IServiceScopeFactory scopeFactory, ILogger<CitasPendientesJob> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ProcesarCitasPendientes(stoppingToken);
                await Task.Delay(_intervalo, stoppingToken);
            }
        }

        private async Task ProcesarCitasPendientes(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
            var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

            var limite = DateTime.UtcNow.AddHours(-1); // más de 1 hora pendiente

            var citasPendientes = await db.Citas
                .Where(c => c.Estado == EstadoCita.Pendiente && c.CreadoEn < limite)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Citas pendientes a cancelar: {Count}", citasPendientes.Count);

            foreach (var cita in citasPendientes)
            {
                await mediator.Send(new CancelarCitaCommand(cita.IdCita), cancellationToken);
                _logger.LogInformation("Cita {IdCita} cancelada por timeout", cita.IdCita);
            }
        }
    }
}
