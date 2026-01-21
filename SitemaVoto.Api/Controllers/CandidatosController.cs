using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VotoModelos.Entidades;


using SitemaVoto.Api.DTOs.Candidatos;

namespace SitemaVoto.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidatosController : ControllerBase
    {
        private readonly SitemaVotoApiContext _context;

        public CandidatosController(SitemaVotoApiContext context)
        {
            _context = context;
        }

        // GET: api/Candidatoes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CandidatoResponseDto>>> Get(CancellationToken ct)
        {
            var list = await _context.Candidatos
                .AsNoTracking()
                .Select(c => new CandidatoResponseDto
                {
                    Id = c.Id,
                    ProcesoElectoralId = c.ProcesoElectoralId,
                    NombreCompleto = c.NombreCompleto,
                    Partido = c.Partido,
                    Binomio = c.Binomio,
                    NumeroLista = c.NumeroLista,
                    Activo = c.Activo
                })
                .ToListAsync(ct);

            return Ok(list);
        }

        // GET: api/Candidatoes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Candidato>> GetCandidato(int id, CancellationToken ct)
        {
            var candidato = await _context.Candidatos.FindAsync(new object[] { id }, ct);
            if (candidato == null) return NotFound();
            return Ok(candidato);
        }

        // PUT: api/Candidatoes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCandidato(int id, Candidato candidato)
        {
            if (id != candidato.Id)
            {
                return BadRequest();
            }

            _context.Entry(candidato).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CandidatoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Candidatoes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Candidato>> PostCandidato(CandidatoCreateDto dto, CancellationToken ct)
        {
            // validar que exista el proceso
            var existeProceso = await _context.ProcesoElectorales
                .AnyAsync(p => p.Id == dto.ProcesoElectoralId, ct);

            if (!existeProceso)
                return BadRequest($"ProcesoElectoralId {dto.ProcesoElectoralId} no existe.");

            var candidato = new Candidato
            {
                ProcesoElectoralId = dto.ProcesoElectoralId,
                NombreCompleto = dto.NombreCompleto,
                Partido = dto.Partido,
                Binomio = dto.Binomio,
                NumeroLista = dto.NumeroLista,
                Activo = dto.Activo
            };

            _context.Candidatos.Add(candidato);
            await _context.SaveChangesAsync(ct);

            return CreatedAtAction(nameof(GetCandidato), new { id = candidato.Id }, candidato);
        }

        // DELETE: api/Candidatoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCandidato(int id)
        {
            var candidato = await _context.Candidatos.FindAsync(id);
            if (candidato == null)
            {
                return NotFound();
            }

            _context.Candidatos.Remove(candidato);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CandidatoExists(int id)
        {
            return _context.Candidatos.Any(e => e.Id == id);
        }
    }
}
