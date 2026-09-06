namespace BackendSpa.Application.Features.Citas.DTO
{
    public record DisponibilidadDTO(
        DateTime Fecha,
        TimeSpan HoraInicio,
        TimeSpan HoraFin
        );
        
}
