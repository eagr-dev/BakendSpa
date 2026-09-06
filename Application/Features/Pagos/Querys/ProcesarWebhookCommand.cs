using BackendSpa.Application.Common.Responsive;
using BackendSpa.Application.Features.Pagos.DTO;
using MediatR;

namespace BackendSpa.Application.Features.Pagos.Querys
{
    public record ProcesarWebhookCommand(WebhookNotification Notificacion)
    : IRequest<Responsive<bool>>;
}
