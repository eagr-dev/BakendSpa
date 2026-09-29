using BackendSpa.Application.Features.Citas.DTO;
using BackendSpa.Application.Features.Citas.Querys;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace BackendSpa.Controllers
{
    [ApiController]
    [Route("api/citas")]
    [EnableRateLimiting("citas-policy")]
    public class CitasController : ControllerBase
    {
        private readonly ISender _mediator;

        public CitasController(ISender mediator)
        {
            _mediator = mediator;
        }

        // POST api/citas
        [HttpPost]
        public async Task<IActionResult> CrearCita([FromBody] CrearCitaDto cita)
        {
            var resultado = await _mediator.Send(new GetCreateCitaQuery(cita));

            if (!resultado.Success)
                return BadRequest(resultado.Mensaje);

            return Ok(resultado);
        }

        // GET api/citas/disponibilidad
        [HttpGet("disponibilidad")]
        public async Task<IActionResult> GetDisponibilidad([FromQuery] DisponibilidadDTO dto)
        {
            var resultado = await _mediator.Send(new GetEstaDisponibleQuery(dto));

            if (!resultado.Success)
                return BadRequest(resultado.Mensaje);

            return Ok(resultado);
        }

        [HttpDelete("cancelarcita/{id}")]
        public async Task<IActionResult> CancelarCita([FromRoute] int id, 
            [FromQuery] string telefono, [FromQuery] string email)
        {
            var resultado = await _mediator.Send(new CancelarCitaCommand(id, telefono, email));

            if (!resultado.Success)
                return BadRequest(resultado.Mensaje);

            return Ok(resultado);
        }
    }
}
