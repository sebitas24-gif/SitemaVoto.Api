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
    public class HistorialResultadosController : ControllerBase
    {
        private readonly SitemaVotoApiContext _context;

        public HistorialResultadosController(SitemaVotoApiContext context)
        {
            _context = context;
        }

        // GET: api/HistorialResultados
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HistorialResultados>>> GetHistorialResultados()
        {
            return await _context.HistorialResultados.ToListAsync();
        }

        // GET: api/HistorialResultados/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HistorialResultados>> GetHistorialResultados(int id)
        {
            var historialResultados = await _context.HistorialResultados.FindAsync(id);

            if (historialResultados == null)
            {
                return NotFound();
            }

            return historialResultados;
        }

        // PUT: api/HistorialResultados/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHistorialResultados(int id, HistorialResultados historialResultados)
        {
            if (id != historialResultados.Id)
            {
                return BadRequest();
            }

            _context.Entry(historialResultados).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HistorialResultadosExists(id))
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

        // POST: api/HistorialResultados
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<HistorialResultados>> PostHistorialResultados(HistorialResultados historialResultados)
        {
            _context.HistorialResultados.Add(historialResultados);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetHistorialResultados", new { id = historialResultados.Id }, historialResultados);
        }

        // DELETE: api/HistorialResultados/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHistorialResultados(int id)
        {
            var historialResultados = await _context.HistorialResultados.FindAsync(id);
            if (historialResultados == null)
            {
                return NotFound();
            }

            _context.HistorialResultados.Remove(historialResultados);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HistorialResultadosExists(int id)
        {
            return _context.HistorialResultados.Any(e => e.Id == id);
        }
    }
}
