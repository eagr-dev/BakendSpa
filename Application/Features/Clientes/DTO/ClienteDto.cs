namespace BackendSpa.Application.Features.Clientes.DTO
{
    public record ClienteDto(
        int IdCliente,
        string Nombre,
        string Email,
        string Telefono
    );
}
