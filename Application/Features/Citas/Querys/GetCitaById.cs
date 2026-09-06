using BackendSpa.Application.Common.Responsive;
using BackendSpa.Application.Features.Citas.DTO;
using BackendSpa.Domain;
using MediatR;

namespace BackendSpa.Application.Features.Citas.Querys
{
    public record GetCitaById(int id) : IRequest<Responsive<CitaDto>>;
}
