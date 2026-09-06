namespace BackendSpa.Application.Features.Servicios.DTO
{
    public record ServicioDto(
    int IdServicio,
    string Nombre,
    string? Descripcion,
    int? DuracionMin,
    decimal Precio,
    string Categoria
);
}
