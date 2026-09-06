using BackendSpa.Application.Features.Servicios.DTO;
using MediatR;

namespace BackendSpa.Application.Features.Servicios.Querys
{
    public record GetServiciosQuery() : IRequest<List<ServicioDto>>;

    public record GetServicioByIdQuery(int IdServicio) : IRequest<ServicioDto?>;

    public record GetServicioByNameQuery(string NameServicio) : IRequest<ServicioDto?>;

    public record GetCreateServicioQuery(ServicioDto servicio) : IRequest<ServicioDto?>;
}
