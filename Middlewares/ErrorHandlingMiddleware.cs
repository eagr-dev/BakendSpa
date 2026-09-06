using BackendSpa.Application.Common.Responsive;
using System.Text.Json;

namespace BackendSpa.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;//Pasa a los controladores
                                               //para llegar hasta las capas logicas y ver si regresa excepcion
        
        private readonly ILogger<ErrorHandlingMiddleware> _logger;//hacer de manera mas sencilla una depuracion

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                if (context.Response.HasStarted && context.Response.StatusCode == 429)
                {
                    _logger.LogInformation("alguien se paso de la raya");
                    return; 
                }

                _logger.LogInformation("Sin excepciones");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Error de validación: {Message}", ex.Message);
                await EscribirRespuesta(context, StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado");
                await EscribirRespuesta(context, StatusCodes.Status500InternalServerError,
                    "Ocurrió un error interno, intenta de nuevo más tarde");
            }
        }

        private static async Task EscribirRespuesta(HttpContext context, int statusCode, string mensaje)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var respuesta = new Responsive<object>(false, mensaje, null);
            var json = JsonSerializer.Serialize(respuesta);

            await context.Response.WriteAsync(json);
        }
    }
}
