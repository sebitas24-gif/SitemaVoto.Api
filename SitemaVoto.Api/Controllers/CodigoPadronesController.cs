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
    public class CodigoPadronesController : ControllerBase
    {
        private readonly SitemaVotoApiContext _context;

        public CodigoPadronesController(SitemaVotoApiContext context)
        {
            _context = context;
        }

        // GET: api/CodigoPadrons
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CodigoPadron>>> GetCodigoPadron()
        {
            return await _context.CodigoPadrones.ToListAsync();
        }

        // GET: api/CodigoPadrons/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CodigoPadron>> GetCodigoPadron(int id)
        {
            var codigoPadron = await _context.CodigoPadrones.FindAsync(id);

            if (codigoPadron == null)
            {
                return NotFound();
            }

            return codigoPadron;
        }

        // PUT: api/CodigoPadrons/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCodigoPadron(int id, CodigoPadron codigoPadron)
        {
            if (id != codigoPadron.Id)
            {
                return BadRequest();
            }

            _context.Entry(codigoPadron).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CodigoPadronExists(id))
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

        // POST: api/CodigoPadrons
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CodigoPadron>> PostCodigoPadron(CodigoPadron codigoPadron)
        {
            _context.CodigoPadrones.Add(codigoPadron);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCodigoPadron", new { id = codigoPadron.Id }, codigoPadron);
        }

        // DELETE: api/CodigoPadrons/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCodigoPadron(int id)
        {
            var codigoPadron = await _context.CodigoPadrones.FindAsync(id);
            if (codigoPadron == null)
            {
                return NotFound();
            }

            _context.CodigoPadrones.Remove(codigoPadron);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CodigoPadronExists(int id)
        {
            return _context.CodigoPadrones.Any(e => e.Id == id);
        }
    }
}
