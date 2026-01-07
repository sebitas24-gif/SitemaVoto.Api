using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SitemaVoto.Api.DTOs;
using VotoModelos;

namespace SitemaVoto.Api.Controllers
{
    [ApiController]
    [Route("api/admin/roles")]
    [Authorize(Roles = "ADMIN")]
    public class AdminRolesController : ControllerBase
    {
        private readonly SitemaVotoApiContext _db;

        public AdminRolesController(SitemaVotoApiContext db)
        {
            _db = db;
        }

        // ✅ Asignar rol ADMIN a un votante
        [HttpPost("hacer-admin")]
        public async Task<IActionResult> HacerAdmin([FromBody] AsignarAdminDto dto)
        {
            if (dto == null || dto.IdVotante <= 0)
                return BadRequest("IdVotante inválido.");

            var existeVotante = await _db.Votantes.AnyAsync(v => v.Id == dto.IdVotante);
            if (!existeVotante) return NotFound("Votante no existe.");

            var yaEsAdmin = await _db.Administrador.AnyAsync(a => a.IdVotante == dto.IdVotante);
            if (yaEsAdmin) return Conflict("Este votante ya es ADMIN.");

            var admin = new Administrador { IdVotante = dto.IdVotante };
            _db.Administrador.Add(admin);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Rol ADMIN asignado." });
        }

        // ✅ Quitar rol ADMIN (opcional)
        [HttpDelete("quitar-admin/{idVotante:int}")]
        public async Task<IActionResult> QuitarAdmin(int idVotante)
        {
            var admin = await _db.Administrador.FirstOrDefaultAsync(a => a.IdVotante == idVotante);
            if (admin == null) return NotFound("No es ADMIN.");

            _db.Administrador.Remove(admin);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Rol ADMIN removido." });
        }

        // ✅ Asignar rol CANDIDATO a un votante
        [HttpPost("hacer-candidato")]
        public async Task<IActionResult> HacerCandidato([FromBody] AsignarCandidatoDto dto)
        {
            if (dto == null || dto.IdVotante <= 0)
                return BadRequest("IdVotante inválido.");

            var existeVotante = await _db.Votantes.AnyAsync(v => v.Id == dto.IdVotante);
            if (!existeVotante) return NotFound("Votante no existe.");

            var yaEsCandidato = await _db.Candidato.AnyAsync(c => c.IdVotante == dto.IdVotante);
            if (yaEsCandidato) return Conflict("Este votante ya es CANDIDATO.");

            var cand = new Candidato
            {
                IdVotante = dto.IdVotante,
                Partido = dto.Partido,
                Eslogan = dto.Eslogan
            };

            _db.Candidato.Add(cand);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Rol CANDIDATO asignado." });
        }

        // ✅ Quitar rol CANDIDATO (opcional)
        [HttpDelete("quitar-candidato/{idVotante:int}")]
        public async Task<IActionResult> QuitarCandidato(int idVotante)
        {
            var cand = await _db.Candidato.FirstOrDefaultAsync(c => c.IdVotante == idVotante);
            if (cand == null) return NotFound("No es CANDIDATO.");

            _db.Candidato.Remove(cand);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Rol CANDIDATO removido." });
        }
        [HttpPost("cambiar-rol")]
        public async Task<IActionResult> CambiarRol([FromBody] CambiarRolDto dto)
        {
            if (dto.IdVotante <= 0)
                return BadRequest("IdVotante inválido.");

            var existeVotante = await _db.Votantes.AnyAsync(v => v.Id == dto.IdVotante);
            if (!existeVotante) return NotFound("Votante no existe.");

            // Quitar roles actuales
            var admin = await _db.Administrador.FirstOrDefaultAsync(a => a.IdVotante == dto.IdVotante);
            if (admin != null) _db.Administrador.Remove(admin);

            var candidato = await _db.Candidato.FirstOrDefaultAsync(c => c.IdVotante == dto.IdVotante);
            if (candidato != null) _db.Candidato.Remove(candidato);

            // Asignar nuevo rol
            if (dto.NuevoRol == "ADMIN")
            {
                _db.Administrador.Add(new Administrador { IdVotante = dto.IdVotante });
            }
            else if (dto.NuevoRol == "CANDIDATO")
            {
                _db.Candidato.Add(new Candidato { IdVotante = dto.IdVotante });
            }
            // SOLO_VOTANTE → no se agrega nada

            await _db.SaveChangesAsync();

            return Ok(new { message = $"Rol cambiado a {dto.NuevoRol}" });
        }

    }
}
