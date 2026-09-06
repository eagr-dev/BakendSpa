using BackendSpa.Application.Common.Responsive;
using BackendSpa.Application.Features.Clientes.DTO;
using BackendSpa.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackendSpa.Application.Features.Clientes.Querys
{
    public class GetClienteByNameHandler : IRequestHandler<GetClienteByName, Responsive<ClienteDto>>
    {
        private readonly IAppDbContext _db;

        public GetClienteByNameHandler(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<Responsive<ClienteDto>> Handle(GetClienteByName request, CancellationToken cancellationToken)
        {
            string name = request.Name;
            var cliente = await _db.Clientes.FirstOrDefaultAsync(c => c.Nombre == name, cancellationToken);

            if (cliente is null) return new Responsive<ClienteDto>(false, $"el nombre {name} no existe", null);

            ClienteDto dto = new(cliente!.IdCliente, cliente.Nombre, cliente.Email, cliente.Telefono);

            return new Responsive<ClienteDto>(true, "", dto);
        }
    }
}
