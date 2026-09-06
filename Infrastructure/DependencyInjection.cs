using BackendSpa.Application.Interfaces;
using BackendSpa.Domain.Interface;
using BackendSpa.Infrastructure.BackgroundServices;
using BackendSpa.Infrastructure.Persistance;
using BackendSpa.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace BackendSpa.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(
                    configuration.GetConnectionString("DefaultConnection"),
                    ServerVersion.AutoDetect(configuration.GetConnectionString("DefaultConnection"))
                ));

            services.AddScoped<IAppDbContext>(provider =>
                provider.GetRequiredService<AppDbContext>());

            services.AddScoped<IPlataformaPago, MercadoPagoService>();
            services.AddScoped<INotificacion, TwilioService>();

            services.AddScoped<ICalculoAnticipo, CalculoAnticipoService>();

            services.AddHostedService<CitasPendientesJob>();
            services.AddHostedService<NotificacionesFallidasJob>();

            //Hacer 3 intentos antes de decir que ya se fallo completamente
            services.AddHttpClient<MercadoPagoService>()
            .AddTransientHttpErrorPolicy(policy =>
                policy.WaitAndRetryAsync(3, retryAttempt =>
                TimeSpan.FromSeconds(retryAttempt)  
            )
            );

            return services;
        }
    }
}
