using BackendSpa.Application.Features.Pagos.DTO;
using BackendSpa.Application.Features.Pagos.Querys;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BackendSpa.Controllers
{
    [ApiController]
    [Route("api/pagos")]
    public class PagosController : ControllerBase
    {
        private readonly ISender _mediator;

        public PagosController(ISender mediator)
        {
            _mediator = mediator;
        }

        // POST api/pagos/webhook
        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook([FromBody] WebhookNotification notif)
        {
            await _mediator.Send(new ProcesarWebhookCommand(notif));
            return Ok();
        }
    }
}
