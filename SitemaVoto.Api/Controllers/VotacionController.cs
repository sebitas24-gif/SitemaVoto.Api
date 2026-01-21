using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SitemaVoto.Api.DTOs.Votacion;
using SitemaVoto.Api.Services.Votacion;

namespace SitemaVoto.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VotacionController : ControllerBase
    {
        private readonly IVotacionService _votacion;

        public VotacionController(IVotacionService votacion) => _votacion = votacion;

        [HttpGet("candidatos")]
        public async Task<ActionResult<List<CandidatoDto>>> Candidatos(CancellationToken ct)
        {
            var list = await _votacion.GetCandidatosAsync(ct);
            return Ok(list.Select(x => new CandidatoDto
            {
                Id = x.Id,
                Nombre = x.Nombre,
                Partido = x.Partido
            }).ToList());
        }

        [HttpPost("emitir")]
        public async Task<ActionResult<EmitirVotoResultDto>> Emitir([FromBody] EmitirVotoDto dto, CancellationToken ct)
        {
            var r = await _votacion.EmitirVotoAsync(dto.Cedula, dto.CodigoPad, dto.CandidatoId, ct);
            return Ok(new EmitirVotoResultDto { Ok = r.Ok, Error = r.Error,Comprobante = r.CodigoComprobante });
        }
    }

}
