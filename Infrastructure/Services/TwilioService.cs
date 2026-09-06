using BackendSpa.Application.Common.Responsive;
using BackendSpa.Application.Interfaces;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace BackendSpa.Infrastructure.Services
{
    public class TwilioService : INotificacion
    {
        private readonly string _fromNumber;

        public TwilioService(IConfiguration config)
        {
            var accountSid = config["Twilio:AccountSid"]!;
            var authToken = config["Twilio:AuthToken"]!;
            _fromNumber = config["Twilio:PhoneNumber"]!;

            TwilioClient.Init(accountSid, authToken);
        }

        public async Task<Responsive<bool>> EnviarMensajeAsync(string destinatario, string mensaje)
        {
            try
            {
                var message = await MessageResource.CreateAsync(
                to: new Twilio.Types.PhoneNumber($"whatsapp:{destinatario}"),
                from: new Twilio.Types.PhoneNumber($"whatsapp:{_fromNumber}"),
                body: mensaje
                );
                bool exitoso = message.Status != MessageResource.StatusEnum.Failed
            && message.Status != MessageResource.StatusEnum.Undelivered;

                return new Responsive<bool>(exitoso,
                    exitoso ? "" : $"Twilio status: {message.Status} --message: {message.ErrorMessage}",
                    exitoso);
            }
            catch (Exception ex)
            {
                return new Responsive<bool>(false, $"Error Twilio: {ex.Message}", false);
            }
        }
    }
}
