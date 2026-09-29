using BackendSpa.Application.Common.Responsive;
using BackendSpa.Application.Features.Clientes.DTO;
using MediatR;

namespace BackendSpa.Application.Features.Clientes.Querys
{
    public record GetClienteById(int id) : IRequest<Responsive<ClienteDto>>;
    public record GetClienteByPhone(string PhoneNumber) : IRequest<Responsive<ClienteDto>>;
    public record GetClienteByEmail(string Email) : IRequest<Responsive<ClienteDto>>;
    public record GetCreateCliente(ClienteDto Client) : IRequest<Responsive<ClienteDto>>;
}
