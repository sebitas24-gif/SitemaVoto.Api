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

        public PadronController(IPadronService padron, SitemaVotoApiContext db)
        {
            _padron = padron;
            _db = db;
        }

        // ✅ GET: api/Padron  (lo usa Admin/Padrón en el MVC)
     
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PadronItemDto>>> GetPadron(CancellationToken ct)
        {
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


        // ✅ GET: api/Padron/cedula/1234567890 (lo usa Jefe de Junta)
        [HttpGet("cedula/{cedula}")]
        public async Task<ActionResult<JefeVerificacionDto>> GetPorCedula(string cedula, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(cedula))
                return BadRequest("Cédula requerida.");

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

            var user = await _db.Usuarios
                .AsNoTracking()
                .Include(u => u.Junta)
                .FirstOrDefaultAsync(u => u.Cedula == cedula, ct);

            if (user == null) return NotFound("No existe en padrón.");

            var pad = await _db.CodigoPadrones
                .AsNoTracking()
                .Where(x => x.UsuarioId == user.Id && x.ProcesoElectoralId == procesoId)
                .Select(x => x.Codigo)
                .FirstOrDefaultAsync(ct);

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

        // ✅ POST: api/Padron/validar (ya lo tenías)
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

            // ids que YA tienen código para ese proceso
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
