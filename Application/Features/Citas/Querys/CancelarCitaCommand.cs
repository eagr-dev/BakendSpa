using BackendSpa.Application.Common.Responsive;
using MediatR;

namespace BackendSpa.Application.Features.Citas.Querys
{
    public record CancelarCitaCommand(int IdCita, string NumeroTelefonico, string Email) : IRequest<Responsive<bool>>;
}
