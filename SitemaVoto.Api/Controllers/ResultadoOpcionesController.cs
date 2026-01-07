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
    public class ResultadoOpcionesController : ControllerBase
    {
        private readonly SitemaVotoApiContext _context;

        public ResultadoOpcionesController(SitemaVotoApiContext context)
        {
            _context = context;
        }

        // GET: api/ResultadoOpciones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResultadoOpcion>>> GetResultadoOpcion()
        {
            return await _context.ResultadoOpcion.ToListAsync();
        }

        // GET: api/ResultadoOpciones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ResultadoOpcion>> GetResultadoOpcion(int id)
        {
            var resultadoOpcion = await _context.ResultadoOpcion.FindAsync(id);

            if (resultadoOpcion == null)
            {
                return NotFound();
            }

            return resultadoOpcion;
        }

        // PUT: api/ResultadoOpciones/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutResultadoOpcion(int id, ResultadoOpcion resultadoOpcion)
        {
            if (id != resultadoOpcion.Id)
            {
                return BadRequest();
            }

            _context.Entry(resultadoOpcion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ResultadoOpcionExists(id))
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

        // POST: api/ResultadoOpciones
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ResultadoOpcion>> PostResultadoOpcion(ResultadoOpcion resultadoOpcion)
        {
            _context.ResultadoOpcion.Add(resultadoOpcion);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetResultadoOpcion", new { id = resultadoOpcion.Id }, resultadoOpcion);
        }

        // DELETE: api/ResultadoOpciones/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResultadoOpcion(int id)
        {
            var resultadoOpcion = await _context.ResultadoOpcion.FindAsync(id);
            if (resultadoOpcion == null)
            {
                return NotFound();
            }

            _context.ResultadoOpcion.Remove(resultadoOpcion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ResultadoOpcionExists(int id)
        {
            return _context.ResultadoOpcion.Any(e => e.Id == id);
        }
    }
}
