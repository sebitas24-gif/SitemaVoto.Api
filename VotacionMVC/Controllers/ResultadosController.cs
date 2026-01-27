using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text.Json;
using VotacionMVC.Models.DTOs;
using VotacionMVC.Models.Resultados;
using VotacionMVC.Models.ViewModels;
using VotacionMVC.Service;

namespace VotacionMVC.Controllers
{
    public class ResultadosController : Controller
    {
        private readonly ApiService _api;

        private static readonly JsonSerializerOptions _jsonOpts = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public ResultadosController(ApiService api) => _api = api;

        [HttpGet]
        public async Task<IActionResult> Index(string modo = "vivo", string provincia = "Nacional", CancellationToken ct = default)
        {
            modo = (modo ?? "vivo").Trim().ToLowerInvariant();
            provincia = string.IsNullOrWhiteSpace(provincia) ? "Nacional" : provincia.Trim();

            // 1) Traer resultados (en vivo / final)
            ResultadosNacionalResponse? data = null;

            try
            {
                var raw = await _api.GetResultadosRawAsync(modo, ct);
                if (raw.HasValue)
                {
                    data = JsonSerializer.Deserialize<ResultadosNacionalResponse>(
                        raw.Value.GetRawText(),
                        _jsonOpts
                    );
                }
            }
            catch
            {
                // no hacemos nada, usamos fallback abajo
            }

            // fallback simple (si por algo falló el raw)
            data ??= await _api.GetResultadosNacionalAsync(ct);
            data ??= new ResultadosNacionalResponse();

            // 2) Mapear líderes por provincia para la tabla
            var lideres = data.lideresPorProvincia
                .Select(x => new ProvinciaLiderVm
                {
                    Provincia = x.provincia ?? "",
                    Lider = x.lider ?? "",
                    Votos = x.votosLider
                })
                .ToList();

            // 3) Definir líder grande (Nacional o por provincia si se filtró)
            string liderNombre = "—";
            long liderVotos = 0;
            string liderAmbito = "Nacional";

            if (!provincia.Equals("Nacional", StringComparison.OrdinalIgnoreCase))
            {
                var lp = lideres.FirstOrDefault(x =>
                    x.Provincia.Equals(provincia, StringComparison.OrdinalIgnoreCase));

                if (lp != null)
                {
                    liderNombre = lp.Lider;
                    liderVotos = lp.Votos;
                    liderAmbito = lp.Provincia;
                }
                else
                {
                    provincia = "Nacional"; // si vino una provincia que no existe, volvemos a nacional
                }
            }

            if (provincia.Equals("Nacional", StringComparison.OrdinalIgnoreCase))
            {
                var max = data.porCandidato
                    .OrderByDescending(x => x.votos)
                    .FirstOrDefault();

                if (max != null)
                {
                    liderNombre = max.nombre;
                    liderVotos = max.votos;
                }
                liderAmbito = "Nacional";
            }

            // 4) Armar ViewModel para tu vista actual (sin cambiarla)
            var cultura = new CultureInfo("es-EC");
            var vm = new ResultadosVm
            {
                Modo = (modo == "final") ? "final" : "vivo",
                EstadoProceso = string.IsNullOrWhiteSpace(data.estadoProceso) ? "—" : data.estadoProceso.ToUpperInvariant(),
                UltimaActualizacion = DateTime.Now.ToString("d/M/yyyy, h:mm:ss tt", cultura),

                LiderNombre = string.IsNullOrWhiteSpace(liderNombre) ? "—" : liderNombre,
                LiderAmbito = liderAmbito,
                LiderVotos = liderVotos,

                ProvinciaSeleccionada = provincia,
                LideresPorProvincia = lideres
            };

            return View(vm);
        }

    }
}
