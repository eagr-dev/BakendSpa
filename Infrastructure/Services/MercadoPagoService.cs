using BackendSpa.Application.Common.Responsive;
using BackendSpa.Application.Features.Pagos.DTO;
using BackendSpa.Application.Interfaces;
using MediatR;
using System.Buffers.Text;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Twilio.Http;
using static System.Net.WebRequestMethods;

namespace BackendSpa.Infrastructure.Services
{
    public class MercadoPagoService(System.Net.Http.HttpClient httpClient, IConfiguration configuration, ILogger<MercadoPagoService> logger) : IPlataformaPago
    {
        private readonly System.Net.Http.HttpClient _http = httpClient;
        private readonly string _accessToken = configuration["MercadoPago:AccessToken"]!;
        private readonly IConfiguration _config = configuration;
        private const string BaseUrl = "https://api.mercadopago.com";
        private readonly ILogger<MercadoPagoService> _logger = logger;

        public async Task<Responsive<string>> CrearPreferenciaAsync(int idCita, decimal anticipo, string descripcion)
        {
            try
            {
                var request = new PreferenciaRequest
                {
                    ExternalReference = idCita.ToString(),
                    NotificationUrl = _config["MercadoPago:WebhookUrl"]!,
                    BackUrls = new BackUrls
                    {
                        Success = _config["MercadoPago:UrlBase"] + "/pago/exito",
                        Failure = _config["MercadoPago:UrlBase"] + "/pago/error",
                        Pending = _config["MercadoPago:UrlBase"] + "/pago/pendiente"
                    },
                    Items = new List<ItemMP>
            {
                new ItemMP
                {
                    Title = descripcion,
                    Quantity = 1,
                    UnitPrice = anticipo
                }
            }
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var httpRequest = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, $"{BaseUrl}/checkout/preferences")
                {
                    Content = content
                };

                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

                var response = await _http.SendAsync(httpRequest);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return new Responsive<string>(false, $"MP Error: {responseBody}", null);

                var preferencia = JsonSerializer.Deserialize<PreferenciaResponse>(responseBody);
                return new Responsive<string>(true, "", preferencia!.SandboxInitPoint); // 👈 aquí
            }
            catch (HttpRequestException ex)
            {
                return new Responsive<string>(false, $"Error de conexión: {ex.Message}", null);
            }
            catch (JsonException ex)
            {
                return new Responsive<string>(false, $"Error al leer respuesta de MP: {ex.Message}", null);
            }
        }

        public async Task<JsonDocument?> ObtenerPagoAsync(string paymentId)
        {
            var httpRequest = new HttpRequestMessage(System.Net.Http.HttpMethod.Get, $"{BaseUrl}/v1/payments/{paymentId}");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

            var response = await _http.SendAsync(httpRequest);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return null;

            return JsonDocument.Parse(responseBody);
        }

        public async Task<bool> ExpirarPreferenciaAsync(string preferenceId)
        {
            var body = new
            {
                expires = true,
                expiration_date_to = DateTime.UtcNow.AddSeconds(-1) // ya expirada
                    .ToString("yyyy-MM-ddTHH:mm:ss.fffzzz")
            };

            var response = await _http.PatchAsJsonAsync(
                $"https://api.mercadopago.com/checkout/preferences/{preferenceId}",
                body
            );

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ReembolsarPagoAsync(string paymentId)
        {
            var httpRequest = new HttpRequestMessage(
                System.Net.Http.HttpMethod.Post,
                $"{BaseUrl}/v1/payments/{paymentId}/refunds"
            );
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            httpRequest.Content = new StringContent("{}", Encoding.UTF8, "application/json");

            var response = await _http.SendAsync(httpRequest);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                _logger.LogWarning("[Reembolso] Falló | PaymentId: {PaymentId} | Status: {Status} | Body: {Body}",
                    paymentId, response.StatusCode, body);
            else
                _logger.LogInformation("[Reembolso] Exitoso | PaymentId: {PaymentId}", paymentId);

            return response.IsSuccessStatusCode;
        }

    }
}
