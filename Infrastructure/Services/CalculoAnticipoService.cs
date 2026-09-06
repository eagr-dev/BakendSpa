using BackendSpa.Domain.Interface;

namespace BackendSpa.Infrastructure.Services
{
    public class CalculoAnticipoService : ICalculoAnticipo
    {
        public decimal Calcular(decimal precioTotal) => precioTotal * 0.5m;
    }
}
