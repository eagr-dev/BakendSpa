namespace BackendSpa.Application.Features.Citas.DTO
{
    public record CitaDto(
        int IdCita,
        int IdCliente,
        //objeto cliente con el nombre para mayor seguridad
        string NombreCliente, 
        DateTime Fecha,
        TimeSpan HoraInicio,
        TimeSpan? HoraFin,
        //De enum  a string para mejor lectura
        string Estado,        
        decimal PrecioTotal,
        decimal Anticipo
    );

    public record CrearCitaDto(
        string NombreCliente,
        string Email,
        string Telefono,
        DateTime Fecha,
        TimeSpan HoraInicio,
        List<int> IdServicios
    );
}
