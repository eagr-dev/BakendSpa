using BackendSpa.Application.Common.Responsive;
using BackendSpa.Application.Features.Citas.CitaDetalles.DTO;
using BackendSpa.Application.Features.Citas.CitaDetalles.Querys;
using BackendSpa.Application.Features.Citas.DTO;
using BackendSpa.Application.Features.Clientes.DTO;
using BackendSpa.Application.Features.Clientes.Querys;
using BackendSpa.Application.Features.Pagos.Querys;
using BackendSpa.Application.Features.Servicios.DTO;
using BackendSpa.Application.Features.Servicios.Querys;
using BackendSpa.Application.Interfaces;
using BackendSpa.Domain;
using BackendSpa.Domain.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackendSpa.Application.Features.Citas.Querys
{
    public class GetCrearCita : IRequestHandler<GetCreateCitaQuery, Responsive<CitaDto>>
    {
        private readonly IAppDbContext _db;
        private readonly ISender _mediator;
        readonly int horasDepilacion = 120;
        private readonly ICalculoAnticipo _anticipo;


        public GetCrearCita(IAppDbContext db, ISender mediator, ICalculoAnticipo anticipo)
        {
            _db = db;
            _mediator = mediator;
            _anticipo = anticipo;
        }

        public async Task<Responsive<CitaDto>> Handle(GetCreateCitaQuery request, CancellationToken cancellationToken)
        {
            var citaReq = request.cita;

            // 1. Obtener servicios y calcular tiempos (Lógica rápida en memoria)
            var servicios = await _db.Servicios
                .Where(s => citaReq.IdServicios.Contains(s.IdServicio))
                .ToListAsync(cancellationToken);

            if (servicios.Count != citaReq.IdServicios.Count)
                return new Responsive<CitaDto>(false, "Uno o más servicios especificados no existen.", null);

            int duracionTotalMinutos = servicios.Sum(s => s.DuracionMin ?? horasDepilacion);
            TimeSpan horaFin = citaReq.HoraInicio.Add(TimeSpan.FromMinutes(duracionTotalMinutos));
            decimal precioTotal = servicios.Sum(s => s.Precio);
            decimal anticipo = _anticipo.Calcular(precioTotal);

            // 2. INICIAR TRANSACCIÓN (100% C#)
            using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                // A) Bloquear las citas de la fecha especificada en MySQL durante esta petición
                bool estaOcupado = await _db.Citas
                    .FromSqlRaw("SELECT * FROM citas WHERE fecha = {0} FOR UPDATE", citaReq.Fecha)
                    .AnyAsync(c =>
                        c.Fecha == citaReq.Fecha &&
                        c.HoraInicio < horaFin &&
                        c.HoraFin > citaReq.HoraInicio &&
                        c.Estado != EstadoCita.Cancelada, cancellationToken);

                if (estaOcupado)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return new Responsive<CitaDto>(false, "El horario seleccionado ya no se encuentra disponible.", null);
                }

                // B) Obtener o Crear Cliente
                var clienteResponseEmail = await _mediator.Send(new GetClienteByEmail(citaReq.Email), cancellationToken);
                var clienteResponseNumero = await _mediator.Send(new GetClienteByPhone(citaReq.Telefono), cancellationToken);

                ClienteDto clienteDto;

                if (clienteResponseEmail.Success && clienteResponseEmail.Data is not null)
                {
                    // El cliente ya existía en la DB
                    clienteDto = clienteResponseEmail.Data;
                }
                else if(clienteResponseNumero.Success && clienteResponseNumero.Data is not null)
                {
                    // El cliente ya existía en la DB
                    clienteDto = clienteResponseNumero.Data;
                }
                else
                {
                    // El cliente no existe: crearlo usando tu handler validado (aplica Regex automáticamente)
                    var nuevoClienteDto = new ClienteDto(0, citaReq.NombreCliente, citaReq.Email, citaReq.Telefono);
                    var crearClienteResponse = await _mediator.Send(new GetCreateCliente(nuevoClienteDto), cancellationToken);

                    if (!crearClienteResponse.Success)
                    {
                        // Si falló alguna Regex de nombre, email o teléfono
                        await transaction.RollbackAsync(cancellationToken);
                        return new Responsive<CitaDto>(false, crearClienteResponse.Mensaje, null);
                    }

                    clienteDto = crearClienteResponse.Data!;
                }


                // C) Crear Cita
                var nuevaCita = new Cita
                {
                    IdCliente = clienteDto.IdCliente,
                    Fecha = citaReq.Fecha,
                    HoraInicio = citaReq.HoraInicio,
                    HoraFin = horaFin,
                    Estado = EstadoCita.Pendiente,
                    PrecioTotal = precioTotal,
                    Anticipo = anticipo,
                    CreadoEn = DateTime.UtcNow
                };

                _db.Citas.Add(nuevaCita);
                await _db.SaveChangesAsync(cancellationToken);

                // D) Crear CitaServicios
                var citaServicios = servicios.Select(s => new CitaServicio
                {
                    IdCita = nuevaCita.IdCita,
                    IdServicio = s.IdServicio,
                    PrecioUnitario = s.Precio
                }).ToList();

                _db.CitaServicios.AddRange(citaServicios);
                await _db.SaveChangesAsync(cancellationToken);

                // E) Pasarela de Pago
                var pago = await _mediator.Send(new CrearPreferenciaCommand(nuevaCita.IdCita, anticipo), cancellationToken);

                if (!pago.Success)
                {
                    // Si el pago falla, deshalcemos TODO en la base de datos de golpe
                    await transaction.RollbackAsync(cancellationToken);
                    return new Responsive<CitaDto>(false, pago.Mensaje, null);
                }

                // F) Confirmar Transacción
                await transaction.CommitAsync(cancellationToken);

                var dto = new CitaDto(
                    nuevaCita.IdCita,
                    clienteDto.IdCliente,
                    clienteDto.Nombre,
                    nuevaCita.Fecha,
                    nuevaCita.HoraInicio,
                    nuevaCita.HoraFin,
                    "Pendiente",
                    precioTotal,
                    anticipo
                );

                return new Responsive<CitaDto>(true, pago.Data!, dto);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                var mensajeDetallado = ex.InnerException != null ? $"{ex.Message} | Detalle: {ex.InnerException.Message}" : ex.Message;
                return new Responsive<CitaDto>(false, $"Error al procesar la cita: {mensajeDetallado}", null);
            }
        }
    }
}
