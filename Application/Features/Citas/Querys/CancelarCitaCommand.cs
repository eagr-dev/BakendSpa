using BackendSpa.Application.Common.Responsive;
using MediatR;

namespace BackendSpa.Application.Features.Citas.Querys
{
    public record CancelarCitaCommand(int IdCita) : IRequest<Responsive<bool>>;
}
