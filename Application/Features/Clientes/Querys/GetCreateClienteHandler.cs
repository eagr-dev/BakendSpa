using BackendSpa.Application.Common.Responsive;
using BackendSpa.Application.Features.Clientes.DTO;
using BackendSpa.Application.Interfaces;
using BackendSpa.Domain;
using MediatR;
using System.Text.RegularExpressions;

namespace BackendSpa.Application.Features.Clientes.Querys
{
    public class GetCreateClienteHandler : IRequestHandler<GetCreateCliente, Responsive<ClienteDto>>
    {
        private readonly IAppDbContext _db;

        public GetCreateClienteHandler(IAppDbContext db)
        {
            _db = db;
        }
        public async Task<Responsive<ClienteDto>> Handle(GetCreateCliente request, CancellationToken cancellationToken)
        {
            var cliente = request.cliente;
            const string validatorName = @"[A-Za-z]";
            const string validatorEmail = @"[a-z0-9.]+@[a-z0-9]+\.[a-z]+";
            const string validatorNumber = @"^\+\d{10,15}$";


            bool isNameValid = Regex.IsMatch(cliente.Nombre, validatorName);
            if (!isNameValid)
                return new Responsive<ClienteDto>(false,
                    $"el nombre {cliente.Nombre} no es valido"
                    , null);

            bool isEmailValid = Regex.IsMatch(cliente.Email, validatorEmail);
            if (!isEmailValid || cliente.Email is null)
                return new Responsive<ClienteDto>(false,
                    $"el email {cliente.Email} no es valido"
                    , null);

            bool isNumberValid = Regex.IsMatch(cliente.Telefono, validatorNumber);
            if (!isNumberValid)
                return new Responsive<ClienteDto>(false,
                    $"el numero del cliente {cliente.Telefono} no es valido"
                    , null);

            Cliente entidad = new()
            {
                Nombre = cliente.Nombre,
                Email = cliente.Email,
                Telefono = cliente.Telefono,
                CreadoEn = DateTime.UtcNow
            };

            await _db.Clientes.AddAsync(entidad, cancellationToken);

            await _db.SaveChangesAsync(cancellationToken);

            ClienteDto responsive = new(entidad.IdCliente, cliente.Nombre, cliente.Email, cliente.Telefono);

            return new Responsive<ClienteDto>(true, "", responsive);
        }
    }
}
