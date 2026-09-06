using BackendSpa.Application.Common.Responsive;
using BackendSpa.Application.Features.Notificaciones.DTO;
using BackendSpa.Domain;
using MediatR;

namespace BackendSpa.Application.Features.Notificaciones.Querys
{
    public record AddNotificacion(NotificacionDto Dto) : IRequest<Responsive<bool>>;
}
