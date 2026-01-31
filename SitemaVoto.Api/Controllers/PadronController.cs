using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SitemaVoto.Api.DTOs.Padron;
using SitemaVoto.Api.Services.Padron;
using VotoModelos.Enums;

namespace SitemaVoto.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PadronController : ControllerBase
    {
        private readonly IPadronService _padron;
        private readonly SitemaVotoApiContext _db;
        private readonly ILogger<PadronController> _log;
        public PadronController(IPadronService padron, SitemaVotoApiContext db, ILogger<PadronController> log)
        {
            _padron = padron;
            _db = db;
            _log = log;
        }

        /// <summary>
        /// Devuelve el proceso activo; si no hay, el último.
        /// Y si ese proceso NO tiene códigos PAD generados, entonces usa el último proceso que SÍ tenga códigos.
        /// (Esto es ideal para que el Admin/Padrón muestre códigos cuando ya existen en BD)
        /// </summary>
        private async Task<int> GetProcesoIdParaPadronAsync(CancellationToken ct)
        {
            // 1) Proceso activo (o último)
            var procesoId = await _db.ProcesoElectorales
                .AsNoTracking()
                .Where(p => p.Estado == EstadoProceso.Activo)
                .OrderByDescending(p => p.Id)
                .Select(p => p.Id)
                .FirstOrDefaultAsync(ct);

            if (procesoId == 0)
            {
                procesoId = await _db.ProcesoElectorales
                    .AsNoTracking()
                    .OrderByDescending(p => p.Id)
                    .Select(p => p.Id)
                    .FirstOrDefaultAsync(ct);
            }

            if (procesoId == 0) return 0;

            // 2) Si el proceso elegido no tiene códigos, usar el último proceso que sí tenga códigos
            var tieneCodigos = await _db.CodigoPadrones
                .AsNoTracking()
                .AnyAsync(x => x.ProcesoElectoralId == procesoId, ct);

            if (!tieneCodigos)
            {
                var procesoConCodigos = await _db.CodigoPadrones
                    .AsNoTracking()
                    .OrderByDescending(x => x.Id)
                    .Select(x => x.ProcesoElectoralId)
                    .FirstOrDefaultAsync(ct);

                if (procesoConCodigos != 0)
                    procesoId = procesoConCodigos;
            }

            return procesoId;
        }

        // ✅ GET: api/Padron  (lo usa Admin/Padrón en el MVC)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PadronItemDto>>> GetPadron(CancellationToken ct)
        {
            var procesoId = await GetProcesoIdParaPadronAsync(ct);
            if (procesoId == 0) return Ok(new List<PadronItemDto>());

            var list = await (
                from u in _db.Usuarios.AsNoTracking()
                where u.Rol == RolUsuario.Votante
                join cp in _db.CodigoPadrones.AsNoTracking().Where(x => x.ProcesoElectoralId == procesoId)
                    on u.Id equals cp.UsuarioId into g
                from cp in g.DefaultIfEmpty()
                orderby u.Provincia, u.Apellidos, u.Nombres
                select new PadronItemDto
                {
                    Cedula = u.Cedula,
                    NombreCompleto = (u.Nombres + " " + u.Apellidos).Trim(),
                    Provincia = u.Provincia,
                    CodigoPad = cp != null ? cp.Codigo : "",
                    Estado = cp == null ? "No generado" : (cp.Usado ? "Usado" : "Generado")
                }
            ).ToListAsync(ct);

            return Ok(list);
        }

        // ✅ GET: api/Padron/cedula(lo usa Jefe de Junta)
        [HttpGet("cedula/{cedula}")]
        public async Task<ActionResult<JefeVerificacionDto>> GetPorCedula(string cedula, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(cedula))
                return BadRequest("Cédula requerida.");

            var procesoId = await GetProcesoIdParaPadronAsync(ct);
            if (procesoId == 0) return BadRequest("No existe proceso electoral.");

            var user = await _db.Usuarios
                .AsNoTracking()
                .Include(u => u.Junta)
                .FirstOrDefaultAsync(u => u.Cedula == cedula, ct);

            if (user == null) return NotFound("No existe en padrón.");
            _log.LogInformation("CEDULA={Cedula} USERID={UserId} PROCESO={ProcesoId}", cedula, user.Id, procesoId);

            var pad = await _db.CodigoPadrones
     .AsNoTracking()
     .Where(x => x.UsuarioId == user.Id && x.ProcesoElectoralId == procesoId)
     .OrderByDescending(x => x.EmitidoEn)
     .Select(x => x.Codigo)
     .FirstOrDefaultAsync(ct);
            _log.LogInformation("PADS_FOR_USER: {Json}", System.Text.Json.JsonSerializer.Serialize(pad));


            var dto = new JefeVerificacionDto
            {
                Cedula = user.Cedula,
                Nombres = user.Nombres,
                Apellidos = user.Apellidos,
                Correo = user.Correo ?? "",
                Telefono = user.Telefono ?? "",
                Provincia = user.Junta?.Provincia ?? user.Provincia,
                Canton = user.Junta?.Canton ?? user.Canton,
                
                Mesa = user.Junta?.CodigoMesa ?? "",
                CodigoPad = pad ?? "",
                Estado = "PADRÓN"
            };

            return Ok(dto);
        }

        // ✅ POST: api/Padron/validar
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

        // ✅ POST: api/Padron/generar-demo
        [HttpPost("generar-demo")]
        public async Task<IActionResult> GenerarCodigosDemo(CancellationToken ct)
        {
            // Proceso activo (o último)
            var procesoId = await _db.ProcesoElectorales
                .AsNoTracking()
                .Where(p => p.Estado == EstadoProceso.Activo)
                .OrderByDescending(p => p.Id)
                .Select(p => p.Id)
                .FirstOrDefaultAsync(ct);

            if (procesoId == 0)
            {
                procesoId = await _db.ProcesoElectorales
                    .AsNoTracking()
                    .OrderByDescending(p => p.Id)
                    .Select(p => p.Id)
                    .FirstOrDefaultAsync(ct);
            }

            if (procesoId == 0) return BadRequest("No existe proceso electoral.");

            var votantes = await _db.Usuarios
                .Where(u => u.Rol == RolUsuario.Votante)
                .Select(u => u.Id)
                .ToListAsync(ct);

            var yaTienen = await _db.CodigoPadrones
                .Where(x => x.ProcesoElectoralId == procesoId)
                .Select(x => x.UsuarioId)
                .ToListAsync(ct);

            var nuevos = 0;
            var rnd = new Random();

            foreach (var userId in votantes)
            {
                if (yaTienen.Contains(userId)) continue;

                _db.CodigoPadrones.Add(new VotoModelos.Entidades.CodigoPadron
                {
                    ProcesoElectoralId = procesoId,
                    UsuarioId = userId,
                    EmitidoPorUsuarioId = null,
                    Codigo = $"PAD-{rnd.Next(100000, 999999)}",
                    EmitidoEn = DateTime.UtcNow,
                    Usado = false
                });

                nuevos++;
            }

            await _db.SaveChangesAsync(ct);
            return Ok(new { ok = true, procesoId, generados = nuevos });
        }


    }
}
