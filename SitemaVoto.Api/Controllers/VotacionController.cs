using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SitemaVoto.Api.DTOs;
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
        private readonly IConfiguration _config;

        public VotacionController(SitemaVotoApiContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        // ✅ LECTURA: proceso activo (Estado=true y fecha dentro de rango)
        [HttpGet("proceso-activo")]
        public async Task<ActionResult<ProcesoActivoDto>> GetProcesoActivo()
        {
            var now = DateTime.UtcNow;

            var p = await _db.ProcesoElectorales
                .Where(x => x.Estado == true && x.FechaInicio <= now && x.FechaFin >= now)
                .OrderByDescending(x => x.FechaInicio)
                .FirstOrDefaultAsync();

            if (p == null) return NotFound("No hay proceso activo.");

            return new ProcesoActivoDto
            {
                IdProceso = p.Id,
                Nombre = p.Nombre,
                TipoEleccion = p.TipoEleccion,
                FechaInicio = p.FechaInicio,
                FechaFin = p.FechaFin
            };
        }

        // ✅ LECTURA: opciones activas del proceso
        [HttpGet("opciones/{idProceso:int}")]
        public async Task<ActionResult<List<OpcionDto>>> GetOpciones(int idProceso)
        {
            var opciones = await _db.OpcionElectorales
                .Where(o => o.Id == idProceso && o.Activo == true)
                .OrderBy(o => o.Tipo) // BLANCO / CANDIDATO
                .ThenBy(o => o.NombreOpcion)
                .Select(o => new OpcionDto
                {
                    IdOpcion = o.Id,
                    NombreOpcion = o.NombreOpcion,
                    Tipo = o.Tipo,
                    Cargo = o.Cargo
                })
                .ToListAsync();

            return opciones;
        }

        // ✅ ESCRITURA: emitir voto (voto único por proceso + papeleta)
        [HttpPost("emitir")]
        public async Task<ActionResult<EmitirVotoResponse>> Emitir([FromBody] EmitirVotoRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Cedula))
                return BadRequest("Cédula requerida.");

            // 1) validar votante
            var votante = await _db.Votantes.FirstOrDefaultAsync(v => v.Cedula == req.Cedula);
            if (votante == null) return NotFound("Votante no existe.");

            // 2) validar proceso
            var proceso = await _db.ProcesoElectorales.FirstOrDefaultAsync(p => p.Id == req.IdProceso);
            if (proceso == null) return NotFound("Proceso no existe.");

            var now = DateTime.UtcNow;
            if (!proceso.Estado || proceso.FechaInicio > now || proceso.FechaFin < now)
                return BadRequest("Proceso no está activo.");

            // 3) validar opción pertenece al proceso y está activa
            var opcion = await _db.OpcionElectorales
                .FirstOrDefaultAsync(o => o.Id == req.IdOpcion && o.Id == req.IdProceso && o.Activo == true);

            if (opcion == null) return BadRequest("Opción inválida.");

            // 4) voto único: si ya existe papeleta para (votante, proceso) => bloquear
            var yaVoto = await _db.Papeletas.AnyAsync(p => p.IdVotante == votante.Id && p.Id == proceso.Id);
            if (yaVoto) return Conflict("Este votante ya emitió su voto en este proceso.");

            // 5) crear “voto_encriptado” (no guardar opción en claro)
            var votoEncriptado = BuildProtectedVote(req.IdProceso, req.IdOpcion);

            // 6) crear papeleta (constancia sin revelar opción)
            var codigo = GenerateCode(10);

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                _db.Votos.Add(new Voto
                {
                    Id = proceso.Id,
                    VotoEncriptado = votoEncriptado,
                    FechaVoto = DateTime.UtcNow
                });

                _db.Papeletas.Add(new Papeleta
                {
                    IdVotante = votante.Id,
                    Id = proceso.Id,
                    CodigoConfirmacion = codigo,
                    FechaEmision = DateTime.UtcNow
                });

                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                return new EmitirVotoResponse
                {
                    Ok = true,
                    Message = "Voto registrado correctamente.",
                    CodigoVerificacion = codigo,
                    FechaEmision = DateTime.UtcNow
                };
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        // -------- Helpers --------

        private string BuildProtectedVote(int idProceso, int idOpcion)
        {
            // payload: "proceso:opcion:timestamp"
            var payload = $"{idProceso}:{idOpcion}:{DateTime.UtcNow:O}";

            // HMAC-SHA256(payload + pepper) => base64(payload|hmac)
            var pepper = _config["VotingCrypto:Pepper"] ?? "DEV-PEPPER";
            var bytes = Encoding.UTF8.GetBytes(payload);
            var key = Encoding.UTF8.GetBytes(pepper);

            using var hmac = new HMACSHA256(key);
            var sig = hmac.ComputeHash(bytes);

            var packed = $"{payload}|{Convert.ToBase64String(sig)}";
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(packed));
        }

        private static string GenerateCode(int len)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var data = new byte[len];
            RandomNumberGenerator.Fill(data);
            var sb = new StringBuilder(len);
            foreach (var b in data) sb.Append(chars[b % chars.Length]);
            return sb.ToString();
        }
    }
}
