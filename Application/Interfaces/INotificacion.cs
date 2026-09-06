using BackendSpa.Application.Common.Responsive;

namespace BackendSpa.Application.Interfaces
{
    public interface INotificacion
    {
        Task<Responsive<bool>> EnviarMensajeAsync(string destinatario, string mensaje);
    }
}
