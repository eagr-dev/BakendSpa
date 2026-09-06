using BackendSpa.Application.Common.Responsive;
using BackendSpa.Application.Interfaces;
using BackendSpa.Domain;
using MediatR;

namespace BackendSpa.Application.Features.Pagos.Querys
{
    public class CrearPreferenciaHandler : IRequestHandler<CrearPreferenciaCommand, Responsive<string>>
    {
        private readonly IAppDbContext _db;
        private readonly IPlataformaPago _plataforma;

        public CrearPreferenciaHandler(IAppDbContext db, IPlataformaPago plataforma)
        {
            _db = db;
            _plataforma = plataforma;
        }

        public async Task<Responsive<string>> Handle(CrearPreferenciaCommand request, CancellationToken cancellationToken)
        {
            // 1. Verificar que la cita existe
            var cita = await _db.Citas.FindAsync(request.IdCita, cancellationToken)
                ?? throw new ArgumentException($"La cita con id {request.IdCita} no existe");

            // 2. Llamar a MP
            var resultado = await _plataforma.CrearPreferenciaAsync(
                request.IdCita,
                request.Anticipo,
                "Anticipo cita Spa Volta Vida"
            );

            if (!resultado.Success)
                return new Responsive<string>(false, resultado.Mensaje, null);

            // 3. Guardar el pago en BD
            Pago pago = new()
            {
                IdCita = request.IdCita,
                Monto = request.Anticipo,
                Status = "pending",
                ExternalReference = request.IdCita.ToString(),
                CreadoEn = DateTime.UtcNow
            };

            await _db.Pagos.AddAsync(pago, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);

            // 4. Devolver URL de pago
            return new Responsive<string>(true, "", resultado.Data);
        }
    }

}
