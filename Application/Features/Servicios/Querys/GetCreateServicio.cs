using BackendSpa.Application.Features.Servicios.DTO;
using BackendSpa.Application.Interfaces;
using BackendSpa.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackendSpa.Application.Features.Servicios.Querys
{
    public class GetCreateServicio : IRequestHandler<GetCreateServicioQuery, ServicioDto?>
    {
        private readonly IAppDbContext _db;

        public GetCreateServicio(IAppDbContext db)
        {
            _db = db;
        }
        public async Task<ServicioDto?> Handle(GetCreateServicioQuery request, CancellationToken cancellationToken)
        {
            ServicioDto dto = request.servicio;
            if (dto.Precio <= 0) throw new ArgumentException("El precio no puede ser 0 o negativo");
            if (dto.DuracionMin! <= 0) throw new ArgumentException("Los servicios no pueden ser 0 o negativo");
            if(string.IsNullOrWhiteSpace(dto.Nombre)) throw new ArgumentException("El servicio debe tener un nombre");
            else if(dto.Nombre.Length <= 2) throw new ArgumentException("El servicio debe tener un nombre mayor a 2 caracteres");
            if (string.IsNullOrWhiteSpace(dto.Categoria)) throw new ArgumentException("El servicio debe tener una categoria");

            var categoria = await _db.Categorias.
                FirstOrDefaultAsync(c => c.Nombre == dto.Categoria, cancellationToken) ??
                throw new InvalidOperationException($"El nombre de la categoria {dto.Categoria} no existe");

            Servicio entidad = new();
            entidad.Nombre = dto.Nombre;
            entidad.Descripcion = dto.Descripcion;
            entidad.DuracionMin = dto.DuracionMin;
            entidad.Precio = dto.Precio;
            entidad.IdCategoria = categoria.IdCategoria;
            
            await _db.Servicios.AddAsync(entidad, cancellationToken);

            await _db.SaveChangesAsync(cancellationToken);

            return dto;
        }
    }
}
