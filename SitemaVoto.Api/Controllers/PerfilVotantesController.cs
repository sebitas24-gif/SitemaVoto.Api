using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VotoModelos.Entidades;

namespace SitemaVoto.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerfilVotantesController : ControllerBase
    {
        private readonly SitemaVotoApiContext _context;

        public PerfilVotantesController(SitemaVotoApiContext context)
        {
            _context = context;
        }

        // GET: api/PerfilVotantes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PerfilVotante>>> GetPerfilVotante()
        {
            return await _context.PerfilVotantes.ToListAsync();
        }

        // GET: api/PerfilVotantes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PerfilVotante>> GetPerfilVotante(int id)
        {
            var perfilVotante = await _context.PerfilVotantes.FindAsync(id);

            if (perfilVotante == null)
            {
                return NotFound();
            }

            return perfilVotante;
        }

        // PUT: api/PerfilVotantes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPerfilVotante(int id, PerfilVotante perfilVotante)
        {
            if (id != perfilVotante.Id)
            {
                return BadRequest();
            }

            _context.Entry(perfilVotante).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PerfilVotanteExists(id))
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

        // POST: api/PerfilVotantes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PerfilVotante>> PostPerfilVotante(PerfilVotante perfilVotante)
        {
            _context.PerfilVotantes.Add(perfilVotante);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPerfilVotante", new { id = perfilVotante.Id }, perfilVotante);
        }

        // DELETE: api/PerfilVotantes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerfilVotante(int id)
        {
            var perfilVotante = await _context.PerfilVotantes.FindAsync(id);
            if (perfilVotante == null)
            {
                return NotFound();
            }

            _context.PerfilVotantes.Remove(perfilVotante);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PerfilVotanteExists(int id)
        {
            return _context.PerfilVotantes.Any(e => e.Id == id);
        }
    }
}
