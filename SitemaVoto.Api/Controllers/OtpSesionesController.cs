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
    public class OtpSesionesController : ControllerBase
    {
        private readonly SitemaVotoApiContext _context;

        public OtpSesionesController(SitemaVotoApiContext context)
        {
            _context = context;
        }

        // GET: api/OtpSesions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OtpSesion>>> GetOtpSesion()
        {
            return await _context.OtpSesiones.ToListAsync();
        }

        // GET: api/OtpSesions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OtpSesion>> GetOtpSesion(Guid id)
        {
            var otpSesion = await _context.OtpSesiones.FindAsync(id);

            if (otpSesion == null)
            {
                return NotFound();
            }

            return otpSesion;
        }

        // PUT: api/OtpSesions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOtpSesion(Guid id, OtpSesion otpSesion)
        {
            if (id != otpSesion.Id)
            {
                return BadRequest();
            }

            _context.Entry(otpSesion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OtpSesionExists(id))
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

        // POST: api/OtpSesions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OtpSesion>> PostOtpSesion(OtpSesion otpSesion)
        {
            _context.OtpSesiones.Add(otpSesion);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOtpSesion", new { id = otpSesion.Id }, otpSesion);
        }

        // DELETE: api/OtpSesions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOtpSesion(Guid id)
        {
            var otpSesion = await _context.OtpSesiones.FindAsync(id);
            if (otpSesion == null)
            {
                return NotFound();
            }

            _context.OtpSesiones.Remove(otpSesion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OtpSesionExists(Guid id)
        {
            return _context.OtpSesiones.Any(e => e.Id == id);
        }
    }
}
