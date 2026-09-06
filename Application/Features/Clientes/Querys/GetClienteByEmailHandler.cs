using BackendSpa.Application.Common.Responsive;
using BackendSpa.Application.Features.Clientes.DTO;
using BackendSpa.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackendSpa.Application.Features.Clientes.Querys
{
    public class GetClienteByEmailHandler : IRequestHandler<GetClienteByEmail, Responsive<ClienteDto>>
    {
        private readonly IAppDbContext _db;

        public GetClienteByEmailHandler(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<Responsive<ClienteDto>> Handle(GetClienteByEmail request, CancellationToken cancellationToken)
        {
            string email = request.Email;
            var cliente = await _db.Clientes.FirstOrDefaultAsync(c => c.Email == email, cancellationToken);

            if (cliente is null) return new Responsive<ClienteDto>(false, $"el email {email} no existe", null);

            ClienteDto dto = new(cliente!.IdCliente, cliente.Nombre, cliente.Email, cliente.Telefono);

            return new Responsive<ClienteDto>(true, "", dto);
        }
    }
}
