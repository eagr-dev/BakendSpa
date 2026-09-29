using BackendSpa.Application.Common.Responsive;
using BackendSpa.Application.Features.Clientes.DTO;
using BackendSpa.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackendSpa.Application.Features.Clientes.Querys
{
    public class GetClienteByPhoneHandler : IRequestHandler<GetClienteByPhone, Responsive<ClienteDto>>
    {
        private readonly IAppDbContext _db;

        public GetClienteByPhoneHandler(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<Responsive<ClienteDto>> Handle(GetClienteByPhone request, CancellationToken cancellationToken)
        {
            string number = request.PhoneNumber;
            var cliente = await _db.Clientes.FirstOrDefaultAsync(c => c.Telefono == number, cancellationToken);

            if (cliente is null) return new Responsive<ClienteDto>(false, $"el nombre {number} no existe", null);

            ClienteDto dto = new(cliente!.IdCliente, cliente.Nombre, cliente.Email, cliente.Telefono);

            return new Responsive<ClienteDto>(true, "", dto);
        }
    }
}
