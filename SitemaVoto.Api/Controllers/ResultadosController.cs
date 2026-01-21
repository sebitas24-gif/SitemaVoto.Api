using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SitemaVoto.Api.Services.Resultados.Models;
using SitemaVoto.Api.Services.Resultados;

namespace SitemaVoto.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultadosController : ControllerBase
    {
        private readonly IResultadosService _resultados;

        public ResultadosController(IResultadosService resultados)
        {
            _resultados = resultados;
        }

        // GET: api/Resultados/nacional
        [HttpGet("nacional")]
        public async Task<ActionResult<ResultadosResponse>> Nacional(CancellationToken ct)
        {
            var r = await _resultados.GetNacionalAsync(ct);
            return Ok(r);
        }

        // GET: api/Resultados/provincia/Pichincha
        [HttpGet("provincia/{provincia}")]
        public async Task<ActionResult<IReadOnlyList<ResultadoItem>>> PorProvincia(string provincia, CancellationToken ct)
        {
            var r = await _resultados.GetPorProvinciaAsync(provincia, ct);
            return Ok(r);
        }
    }
}
