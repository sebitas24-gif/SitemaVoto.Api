using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SitemaVoto.Api.Services.Resultados.Models;
using SitemaVoto.Api.Services.Resultados;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Diagnostics;
using System.Text;
using SitemaVoto.Api.Services.Email;
using SitemaVoto.Api.Services.Procesos;
using Serilog;
using SitemaVoto.Api.DTOs.Resultados;

namespace SitemaVoto.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultadosController : ControllerBase
    {
        private readonly IProcesoService _proceso;
        private readonly IEmailSenderApp _email;
        private readonly IResultadosService _resultados;

        public ResultadosController(IResultadosService resultados, IProcesoService proceso, IEmailSenderApp email)
        {
            _resultados = resultados;
            _proceso = proceso;
            _email = email;
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
        [HttpGet("final")]
        public async Task<IActionResult> Final(CancellationToken ct)
        {
            // Por ahora devuelve lo mismo que nacional
            var data = await _resultados.GetNacionalAsync(ct);
            return Ok(data);
        }


        [HttpPost("enviar-correo")]
        public async Task<IActionResult> EnviarCorreo([FromBody] EnviarResultadosCorreoRequest req, CancellationToken ct)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Correo))
                return BadRequest(new { ok = false, error = "Correo requerido." });

            // Traer resultados nacionales reales
            var res = await _resultados.GetNacionalAsync(ct);

            var sb = new StringBuilder();
            sb.AppendLine("REPORTE DE RESULTADOS - Sistema de Voto Electrónico");
            sb.AppendLine($"Fecha UTC: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine(new string('-', 55));
            sb.AppendLine($"Estado del proceso: {res.EstadoProceso}");
            sb.AppendLine(new string('-', 55));

            sb.AppendLine("VOTOS POR CANDIDATO / OPCIÓN:");
            if (res.PorCandidato != null && res.PorCandidato.Any())
            {
                foreach (var item in res.PorCandidato.OrderByDescending(x => x.Votos))
                    sb.AppendLine($"- {item.Nombre}: {item.Votos}");
            }
            else
            {
                sb.AppendLine("- (sin datos)");
            }

            sb.AppendLine(new string('-', 55));
            sb.AppendLine("LÍDERES POR PROVINCIA:");
            if (res.LideresPorProvincia != null && res.LideresPorProvincia.Any())
            {
                foreach (var l in res.LideresPorProvincia.OrderBy(x => x.Provincia))
                    sb.AppendLine($"- {l.Provincia}: {l.Lider} ({l.VotosLider} votos)");
            }
            else
            {
                sb.AppendLine("- (sin datos)");
            }

            await _email.SendAsync(req.Correo, "Reporte de resultados - Voto Electrónico", sb.ToString(), ct);

            return Ok(new { ok = true });
        }

    }
}
