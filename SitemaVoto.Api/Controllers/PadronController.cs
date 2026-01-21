using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SitemaVoto.Api.DTOs.Padron;
using SitemaVoto.Api.Services.Padron;

namespace SitemaVoto.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PadronController : ControllerBase
    {
        private readonly IPadronService _padron;

        public PadronController(IPadronService padron) => _padron = padron;

        [HttpPost("validar")]
        public async Task<ActionResult<ValidarPadResultDto>> Validar([FromBody] ValidarPadDto dto, CancellationToken ct)
        {
            var r = await _padron.ValidarCedulaPadAsync(dto.Cedula, dto.CodigoPad, ct);

            return Ok(new ValidarPadResultDto
            {
                Ok = r.Ok,
                Error = r.Error,
                ProcesoId = r.ProcesoId,
                VotanteId = r.VotanteId,
                Cedula = r.Cedula,
                Nombres = r.Nombres,
                Apellidos = r.Apellidos,
                Correo = r.Correo,
                Telefono = r.Telefono,
                Provincia = r.Provincia,
                Canton = r.Canton,
                CodigoMesa = r.CodigoMesa,
                CodigoPad = r.CodigoPad
            });
        }
    }
}
