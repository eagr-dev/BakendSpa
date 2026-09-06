namespace BackendSpa.Application.Features.Citas.CitaDetalles.DTO
{
    public record CitaServicioDto(
        int IdCitaServicio,
        int IdCita,
        int IdServicio,
        decimal PrecioUnitario
    );
}
