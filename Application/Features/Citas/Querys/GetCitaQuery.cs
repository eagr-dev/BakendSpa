using BackendSpa.Application.Common.Responsive;
using BackendSpa.Application.Features.Citas.DTO;
using BackendSpa.Domain.Interface;
using MediatR;

namespace BackendSpa.Application.Features.Citas.Querys
{
    public record GetCreateCita(CrearCitaDto cita) : IRequest<Responsive<CitaDto>>;
}
