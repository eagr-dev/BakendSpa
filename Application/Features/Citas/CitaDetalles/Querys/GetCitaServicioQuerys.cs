using BackendSpa.Application.Common.Responsive;
using BackendSpa.Application.Features.Citas.CitaDetalles.DTO;
using BackendSpa.Domain;
using MediatR;

namespace BackendSpa.Application.Features.Citas.CitaDetalles.Querys
{

    public record GetCitaServicioCreate(List<CitaServicioDto> CitaServicioDtos) : IRequest<Responsive<bool>>;
}
