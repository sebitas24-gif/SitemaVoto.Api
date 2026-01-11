using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SitemaVoto.Api.DTOs;
using SitemaVoto.Api.Services;
using System.Security.Cryptography;
using System.Text;
using VotoModelos;

namespace SitemaVoto.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VotacionController : ControllerBase
    {
        private readonly SitemaVotoApiContext _db;
        private readonly VotacionService _svc;

        public VotacionController(SitemaVotoApiContext db, VotacionService svc)
        {
            _db = db;
            _svc = svc;
        }

        // GET: api/votacion/proceso-activo
        [HttpGet("proceso-activo")]
        public async Task<IActionResult> ProcesoActivo()
        {
            var proceso = await _db.ProcesoElectorales
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Estado == true);

            if (proceso == null) return NotFound("No hay proceso activo.");

            return Ok(new
            {
                proceso.Id,
                proceso.Nombre,
                proceso.TipoEleccion,
                proceso.FechaInicio,
                proceso.FechaFin,
                proceso.Estado
            });
        }

        // GET: api/votacion/opciones-activo
        [HttpGet("opciones-activo")]
        public async Task<IActionResult> OpcionesActivo()
        {
            var proceso = await _db.ProcesoElectorales
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Estado ==true);

            if (proceso == null) return NotFound("No hay proceso activo.");

            var opciones = await _db.OpcionElectorales
                .AsNoTracking()
                .Where(o => o.Id == proceso.Id && o.Activo == true)
                .Select(o => new
                {
                    o.Id,
                    o.NombreOpcion,
                    o.Tipo,
                    o.Cargo
                })
                .ToListAsync();

            return Ok(new { procesoId = proceso.Id, opciones });
        }

        // POST: api/votacion/emitir
        [HttpPost("emitir")]
        public async Task<IActionResult> Emitir([FromBody] EmitirVotoDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Cedula) || dto.IdOpcion <= 0)
                return BadRequest("Cédula e IdOpcion son requeridos.");

            var (ok, error, data) = await _svc.EmitirVotoAsync(dto.Cedula, dto.IdOpcion);
            if (!ok) return BadRequest(error);

            return Ok(data);
        }
    }
}
