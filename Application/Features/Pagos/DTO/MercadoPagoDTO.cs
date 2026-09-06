using System.Text.Json.Serialization;

namespace BackendSpa.Application.Features.Pagos.DTO
{

    // Lo que enviamos a MP para crear una preferencia
    public class PreferenciaRequest
    {
        [JsonPropertyName("items")]
        public List<ItemMP> Items { get; set; } = new();

        [JsonPropertyName("back_urls")]
        public BackUrls BackUrls { get; set; } = new();

        [JsonPropertyName("external_reference")]
        public string ExternalReference { get; set; } = string.Empty;

        [JsonPropertyName("notification_url")]
        public string NotificationUrl { get; set; } = string.Empty;
    }

    public class ItemMP
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("unit_price")]
        public decimal UnitPrice { get; set; }

        [JsonPropertyName("currency_id")]
        public string CurrencyId { get; set; } = "MXN";
    }

    public class BackUrls
    {
        [JsonPropertyName("success")]
        public string Success { get; set; } = string.Empty;

        [JsonPropertyName("failure")]
        public string Failure { get; set; } = string.Empty;

        [JsonPropertyName("pending")]
        public string Pending { get; set; } = string.Empty;
    }

    // Lo que MP nos responde
    public class PreferenciaResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("init_point")]
        public string InitPoint { get; set; } = string.Empty;

        [JsonPropertyName("sandbox_init_point")]
        public string SandboxInitPoint { get; set; } = string.Empty;
    }

    // Notificación que llega al webhook
    public class WebhookNotification
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("data")]
        public WebhookData? Data { get; set; }
    }

    public class WebhookData
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
    }
}
