using MediatR;
using BackendSpa.Application.Common.Responsive;
using BackendSpa.Application.Features.Citas.DTO;

namespace BackendSpa.Application.Features.Citas.Querys
{
    public record GetEstaDisponibleQuery(DisponibilidadDTO cita) : IRequest<Responsive<bool>>;
}
