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
    public class ParticipacionVotantesController : ControllerBase
    {
        private readonly SitemaVotoApiContext _context;

        public ParticipacionVotantesController(SitemaVotoApiContext context)
        {
            _context = context;
        }

        // GET: api/ParticipacionVotantes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ParticipacionVotante>>> GetParticipacionVotante()
        {
            return await _context.ParticipacionVotantes.ToListAsync();
        }

        // GET: api/ParticipacionVotantes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ParticipacionVotante>> GetParticipacionVotante(int id)
        {
            var participacionVotante = await _context.ParticipacionVotantes.FindAsync(id);

            if (participacionVotante == null)
            {
                return NotFound();
            }

            return participacionVotante;
        }

        // PUT: api/ParticipacionVotantes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutParticipacionVotante(int id, ParticipacionVotante participacionVotante)
        {
            if (id != participacionVotante.Id)
            {
                return BadRequest();
            }

            _context.Entry(participacionVotante).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ParticipacionVotanteExists(id))
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

        // POST: api/ParticipacionVotantes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ParticipacionVotante>> PostParticipacionVotante(ParticipacionVotante participacionVotante)
        {
            _context.ParticipacionVotantes.Add(participacionVotante);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetParticipacionVotante", new { id = participacionVotante.Id }, participacionVotante);
        }

        // DELETE: api/ParticipacionVotantes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParticipacionVotante(int id)
        {
            var participacionVotante = await _context.ParticipacionVotantes.FindAsync(id);
            if (participacionVotante == null)
            {
                return NotFound();
            }

            _context.ParticipacionVotantes.Remove(participacionVotante);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ParticipacionVotanteExists(int id)
        {
            return _context.ParticipacionVotantes.Any(e => e.Id == id);
        }
    }
}
