using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SitemaVoto.Api.DTOs.Padron;
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
        public async Task<ActionResult<IEnumerable<CodigoPadronResponseDto>>> Get(CancellationToken ct)
        {
            var list = await _context.CodigoPadrones
                .AsNoTracking()
                .OrderByDescending(x => x.Id)
                .Select(x => new CodigoPadronResponseDto
                {
                    Id = x.Id,
                    ProcesoElectoralId = x.ProcesoElectoralId,
                    UsuarioId = x.UsuarioId,
                    EmitidoPorUsuarioId = x.EmitidoPorUsuarioId,
                    Codigo = x.Codigo,
                    EmitidoEn = x.EmitidoEn,
                    Usado = x.Usado
                })
                .ToListAsync(ct);

            return Ok(list);
        }

        // GET: api/CodigoPadrons/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CodigoPadronResponseDto>> GetById(int id, CancellationToken ct)
        {
            var x = await _context.CodigoPadrones
                .AsNoTracking()
                .Where(a => a.Id == id)
                .Select(a => new CodigoPadronResponseDto
                {
                    Id = a.Id,
                    ProcesoElectoralId = a.ProcesoElectoralId,
                    UsuarioId = a.UsuarioId,
                    EmitidoPorUsuarioId = a.EmitidoPorUsuarioId,
                    Codigo = a.Codigo,
                    EmitidoEn = a.EmitidoEn,
                    Usado = a.Usado
                })
                .FirstOrDefaultAsync(ct);

            if (x == null) return NotFound();
            return Ok(x);
        }

        // PUT: api/CodigoPadrons/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> MarcarUsado(int id, CancellationToken ct)
        {
            var entity = await _context.CodigoPadrones.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (entity == null) return NotFound();

            entity.Usado = true;
            await _context.SaveChangesAsync(ct);
            return NoContent();
        }

        // POST: api/CodigoPadrons
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CodigoPadronResponseDto>> Post([FromBody] CodigoPadronCreateDto dto, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(dto.Codigo))
                return BadRequest("Código PAD requerido.");

            var procExiste = await _context.ProcesoElectorales.AnyAsync(p => p.Id == dto.ProcesoElectoralId, ct);
            if (!procExiste) return BadRequest("ProcesoElectoralId no existe.");

            var userExiste = await _context.Usuarios.AnyAsync(u => u.Id == dto.UsuarioId, ct);
            if (!userExiste) return BadRequest("UsuarioId no existe.");

            var entity = new CodigoPadron
            {
                ProcesoElectoralId = dto.ProcesoElectoralId,
                UsuarioId = dto.UsuarioId,
                EmitidoPorUsuarioId = dto.EmitidoPorUsuarioId,
                Codigo = dto.Codigo.Trim(),
                EmitidoEn = DateTime.UtcNow,
                Usado = false
            };

            _context.CodigoPadrones.Add(entity);
            await _context.SaveChangesAsync(ct);

            var resp = new CodigoPadronResponseDto
            {
                Id = entity.Id,
                ProcesoElectoralId = entity.ProcesoElectoralId,
                UsuarioId = entity.UsuarioId,
                EmitidoPorUsuarioId = entity.EmitidoPorUsuarioId,
                Codigo = entity.Codigo,
                EmitidoEn = entity.EmitidoEn,
                Usado = entity.Usado
            };

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, resp);
        }


        // DELETE: api/CodigoPadrons/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var entity = await _context.CodigoPadrones.FindAsync(new object[] { id }, ct);
            if (entity == null) return NotFound();

            _context.CodigoPadrones.Remove(entity);
            await _context.SaveChangesAsync(ct);
            return NoContent();
        }

        private bool CodigoPadronExists(int id)
        {
            return _context.CodigoPadrones.Any(e => e.Id == id);
        }
    }
}
