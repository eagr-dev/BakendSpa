using BackendSpa.Application.Common.Responsive;
using System.Text.Json;

namespace BackendSpa.Application.Interfaces
{
    public interface IPlataformaPago
    {
        Task<Responsive<string>> CrearPreferenciaAsync(int idCita, decimal anticipo, string descripcion);
        Task<JsonDocument?> ObtenerPagoAsync(string paymentId);

        Task<bool> ExpirarPreferenciaAsync(string preferenceId);

        Task<bool> ReembolsarPagoAsync(string paymentId);
    }
}
