using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VotoModelos;

namespace SitemaVoto.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PapeletasController : ControllerBase
    {
        private readonly SitemaVotoApiContext _context;

        public PapeletasController(SitemaVotoApiContext context)
        {
            _context = context;
        }

        // GET: api/Papeletas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Papeleta>>> GetPapeleta()
        {
            return await _context.Papeletas.ToListAsync();
        }

        // GET: api/Papeletas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Papeleta>> GetPapeleta(int id)
        {
            var papeleta = await _context.Papeletas.FindAsync(id);

            if (papeleta == null)
            {
                return NotFound();
            }

            return papeleta;
        }

        // PUT: api/Papeletas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPapeleta(int id, Papeleta papeleta)
        {
            if (id != papeleta.Id)
            {
                return BadRequest();
            }

            _context.Entry(papeleta).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PapeletaExists(id))
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

        // POST: api/Papeletas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Papeleta>> PostPapeleta(Papeleta papeleta)
        {
            _context.Papeletas.Add(papeleta);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPapeleta", new { id = papeleta.Id }, papeleta);
        }

        // DELETE: api/Papeletas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePapeleta(int id)
        {
            var papeleta = await _context.Papeletas.FindAsync(id);
            if (papeleta == null)
            {
                return NotFound();
            }

            _context.Papeletas.Remove(papeleta);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PapeletaExists(int id)
        {
            return _context.Papeletas.Any(e => e.Id == id);
        }
    }
}
