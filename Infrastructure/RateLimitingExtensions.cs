using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace BackendSpa.Infrastructure
{
    public static class RateLimitingExtensions
    {
        public static IServiceCollection AddRateLimitingPolicies(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter("citas-policy", opt =>
                {
                    opt.PermitLimit = 3;
                    opt.Window = TimeSpan.FromSeconds(30);
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 0;
                });

                options.AddFixedWindowLimiter("servicios-policy", opt =>
                {
                    opt.PermitLimit = 10;
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 5;
                });

                options.OnRejected = async (context, token) =>
                {
                    context.HttpContext.Response.StatusCode = 429;

                    await context.HttpContext.Response.WriteAsJsonAsync(new
                    {
                        error = "Too Many Requests"
                    }, token);
                };
            });

            return services;
        }
    }
}
