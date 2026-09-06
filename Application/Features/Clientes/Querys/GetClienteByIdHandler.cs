using BackendSpa.Application.Common.Responsive;
using BackendSpa.Application.Features.Clientes.DTO;
using BackendSpa.Application.Interfaces;
using MediatR;

namespace BackendSpa.Application.Features.Clientes.Querys
{
    public class GetClienteByIdHandler : IRequestHandler<GetClienteById, Responsive<ClienteDto>>
    {
        private readonly IAppDbContext _db;

        public GetClienteByIdHandler(IAppDbContext db)
        {
            _db = db;
        }
        public async Task<Responsive<ClienteDto>> Handle(GetClienteById request, CancellationToken cancellationToken)
        {
            int id_cliente = request.id;

            var cliente = await _db.Clientes.FindAsync(id_cliente, cancellationToken);

            if (cliente is null) new Responsive<ClienteDto>(false, $"el id {id_cliente} no existe", null);

            ClienteDto dto = new(cliente!.IdCliente, cliente.Nombre, cliente.Email, cliente.Telefono);

            return new Responsive<ClienteDto>(true, "", dto);
        }
    }
}
