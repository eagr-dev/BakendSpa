using BackendSpa.Application.Common.Responsive;
using MediatR;

namespace BackendSpa.Application.Common.Behavior
{
    //Para que el pendejo de MediatR no capture la excepcion
    public class ExceptionHandlingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> _logger;

        public ExceptionHandlingBehavior(ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            try
            {
                return await next();
            }
            catch (Exception ex)
            {
                _logger.LogError("MediatR capturó: {Tipo} - {Message}", ex.GetType().Name, ex.Message);
                var responseType = typeof(TResponse);
                if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Responsive<>))
                {
                    var innerType = responseType.GetGenericArguments()[0];
                    var responsive = Activator.CreateInstance(responseType, false, ex.Message, null);
                    return (TResponse)responsive!;
                }

                throw;
            }
        }
    }
}
