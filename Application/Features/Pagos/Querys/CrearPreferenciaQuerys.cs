namespace BackendSpa.Application.Features.Pagos.Querys
{
    using BackendSpa.Application.Common.Responsive;
    using MediatR;

    public record CrearPreferenciaCommand(int IdCita, decimal Anticipo)
        : IRequest<Responsive<string>>;
}
