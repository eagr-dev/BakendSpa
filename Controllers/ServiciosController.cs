using BackendSpa.Application.Features.Servicios.Querys;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace BackendSpa.Controllers
{

    [ApiController]
    [Route("api/servicios")]
    [EnableRateLimiting("servicios-policy")]
    public class ServiciosController : ControllerBase
    {
        private readonly ISender _mediator;

        public ServiciosController(ISender mediator)
        {
            _mediator = mediator;
        }

        // GET api/servicios
        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _mediator.Send(new GetServiciosQuery()));

        // GET api/servicios/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var resultado = await _mediator.Send(new GetServicioByIdQuery(id));

            if (resultado is null)
                return NotFound($"Servicio con id {id} no encontrado");

            return Ok(resultado);
        }
    }
}
