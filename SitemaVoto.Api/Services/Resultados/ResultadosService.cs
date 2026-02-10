using SitemaVoto.Api.Services.Procesos;
using SitemaVoto.Api.Services.Resultados.Models;
using Microsoft.EntityFrameworkCore;

namespace SitemaVoto.Api.Services.Resultados
{
    public class ResultadosService : IResultadosService
    {
        private readonly SitemaVotoApiContext _db;
        private readonly IProcesoService _proceso;

        public ResultadosService(SitemaVotoApiContext db, IProcesoService proceso)
        {
            _db = db;
            _proceso = proceso;
        }


        public async Task<ResultadosResponse> GetNacionalAsync(CancellationToken ct)
        {
            var proc = await _proceso.GetProcesoActivoAsync(ct)
                      ?? await _db.ProcesoElectorales.AsNoTracking()
                            .OrderByDescending(x => x.Id)
                            .FirstOrDefaultAsync(ct);

            if (proc == null)
                return new("CONFIGURACION", Array.Empty<ResultadoItem>(), Array.Empty<LiderProvincia>());

            // ✅ Mapa: Id -> Nombre
            var mapNombre = await _db.Candidatos.AsNoTracking()
                .Where(c => c.ProcesoElectoralId == proc.Id)
                .ToDictionaryAsync(c => c.Id, c => c.NombreCompleto, ct);

            // ✅ Mapa: Id -> ImagenUrl
            var mapFoto = await _db.Candidatos.AsNoTracking()
                .Where(c => c.ProcesoElectoralId == proc.Id)
                .ToDictionaryAsync(c => c.Id, c => c.ImagenUrl, ct);

            // Nacional
            var porCandidato = await _db.Votos.AsNoTracking()
                .Where(v => v.ProcesoElectoralId == proc.Id)
                .GroupBy(v => v.CandidatoId)
                .Select(g => new { CandidatoId = g.Key, Total = g.LongCount() })
                .ToListAsync(ct);

            var listaNacional = porCandidato
                .Select(x =>
                {
                    if (x.CandidatoId == null)
                        return new ResultadoItem("Voto en Blanco", x.Total, null);

                    var id = x.CandidatoId.Value;

                    var nombre = mapNombre.TryGetValue(id, out var n) ? n : "Candidato";
                    var foto = mapFoto.TryGetValue(id, out var u) ? u : null;

                    return new ResultadoItem(nombre, x.Total, foto);
                })
                .OrderByDescending(x => x.Votos)
                .ToList();

            // Líder por provincia (solo votos)
            var grupoProv = await _db.Votos.AsNoTracking()
                .Where(v => v.ProcesoElectoralId == proc.Id)
                .GroupBy(v => new { v.Provincia, v.CandidatoId })
                .Select(g => new { g.Key.Provincia, g.Key.CandidatoId, Total = g.LongCount() })
                .ToListAsync(ct);

            var lideres = grupoProv
                .GroupBy(x => x.Provincia)
                .Select(g =>
                {
                    var top = g.OrderByDescending(x => x.Total).First();

                    if (top.CandidatoId == null)
                        return new LiderProvincia(g.Key, "Voto en Blanco", top.Total, null);

                    var id = top.CandidatoId.Value;

                    var nombre = mapNombre.TryGetValue(id, out var n) ? n : "Candidato";
                    var foto = mapFoto.TryGetValue(id, out var u) ? u : null;

                    return new LiderProvincia(g.Key, nombre, top.Total, foto);
                })
                .OrderBy(x => x.Provincia)
                .ToList();

            var estado = (await _proceso.GetEstadoActualAsync(ct))
                .ToString()
                .ToUpperInvariant();

            return new ResultadosResponse(estado, listaNacional, lideres);
        }

        public async Task<IReadOnlyList<ResultadoItem>> GetPorProvinciaAsync(string provincia, CancellationToken ct)
        {
            var proc = await _proceso.GetProcesoActivoAsync(ct)
                      ?? await _db.ProcesoElectorales.AsNoTracking()
                            .OrderByDescending(x => x.Id)
                            .FirstOrDefaultAsync(ct);

            if (proc == null) return Array.Empty<ResultadoItem>();

            // ✅ Mapa: Id -> Nombre
            var mapNombre = await _db.Candidatos.AsNoTracking()
                .Where(c => c.ProcesoElectoralId == proc.Id)
                .ToDictionaryAsync(c => c.Id, c => c.NombreCompleto, ct);

            // ✅ Mapa: Id -> ImagenUrl
            var mapFoto = await _db.Candidatos.AsNoTracking()
                .Where(c => c.ProcesoElectoralId == proc.Id)
                .ToDictionaryAsync(c => c.Id, c => c.ImagenUrl, ct);

            var porProv = await _db.Votos.AsNoTracking()
                .Where(v => v.ProcesoElectoralId == proc.Id && v.Provincia == provincia)
                .GroupBy(v => v.CandidatoId)
                .Select(g => new { CandidatoId = g.Key, Total = g.LongCount() })
                .ToListAsync(ct);

            return porProv
                .Select(x =>
                {
                    if (x.CandidatoId == null)
                        return new ResultadoItem("Voto en Blanco", x.Total, null);

                    var id = x.CandidatoId.Value;

                    var nombre = mapNombre.TryGetValue(id, out var n) ? n : "Candidato";
                    var foto = mapFoto.TryGetValue(id, out var u) ? u : null;

                    return new ResultadoItem(nombre, x.Total, foto);
                })
                .OrderByDescending(x => x.Votos)
                .ToList();
        }
    }
    }
