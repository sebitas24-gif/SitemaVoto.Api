using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text.Json;
using VotacionMVC.Models.DTOs;
using VotacionMVC.Models.ViewModels;
using VotacionMVC.Service;

namespace VotacionMVC.Controllers
{
    public class ResultadosController : Controller
    {
        private readonly ApiService _api;
        public ResultadosController(ApiService api) => _api = api;

        [HttpGet]
        public async Task<IActionResult> Index(string modo = "vivo", string provincia = "Nacional", CancellationToken ct = default)
        {
            var root = await _api.GetResultadosRawAsync(modo, ct);

            var vm = root.HasValue ? ParseResultados(root.Value) : new ResultadosVm();
            vm.Modo = (modo ?? "vivo").ToLower();
            vm.ProvinciaSeleccionada = string.IsNullOrWhiteSpace(provincia) ? "Nacional" : provincia;

            // Si eliges provincia y existe tabla por provincia, cambiamos el “líder” grande
            if (vm.ProvinciaSeleccionada != "Nacional" && vm.LideresPorProvincia.Count > 0)
            {
                var match = vm.LideresPorProvincia
                    .FirstOrDefault(x => string.Equals(x.Provincia, vm.ProvinciaSeleccionada, StringComparison.OrdinalIgnoreCase));

                if (match != null)
                {
                    vm.LiderAmbito = match.Provincia;
                    vm.LiderNombre = string.IsNullOrWhiteSpace(match.Lider) ? "—" : match.Lider;
                    vm.LiderVotos = match.Votos;
                }
            }

            return View(vm);
        }

        private static ResultadosVm ParseResultados(JsonElement root)
        {
            // Muchos endpoints vienen como { ok, error, data: {...} }
            var data = TryGet(root, "data") ?? root;

            var vm = new ResultadosVm();

            // Estado proceso (string o int)
            vm.EstadoProceso = GetStringAny(data, "estadoProceso", "estado", "Estado", "procesoEstado") ?? "—";
            if (vm.EstadoProceso == "—")
            {
                var estadoInt = GetIntAny(data, "estado", "estadoProceso", "procesoEstado");
                if (estadoInt.HasValue)
                    vm.EstadoProceso = (estadoInt.Value == 1) ? "ACTIVO" : "CERRADO";
            }

            // Última actualización
            vm.UltimaActualizacion = GetDateStringAny(data, "ultimaActualizacion", "updatedAt", "lastUpdate", "ultima_actualizacion") ?? "—";

            // Líder nacional
            var liderObj = TryGet(data, "lider") ?? TryGet(data, "ganador") ?? TryGet(data, "candidatoLider");
            if (liderObj.HasValue && liderObj.Value.ValueKind == JsonValueKind.Object)
            {
                vm.LiderNombre = GetStringAny(liderObj.Value, "nombre", "name", "candidato", "candidatoNombre", "nombreCompleto") ?? "—";
                vm.LiderVotos = GetLongAny(liderObj.Value, "votos", "totalVotos", "count", "votosTotales") ?? 0;
                vm.LiderAmbito = GetStringAny(liderObj.Value, "ambito", "provincia") ?? "Nacional";
            }
            else
            {
                // alternativa por si viene plano
                vm.LiderNombre = GetStringAny(data, "liderNombre", "ganadorNombre", "candidatoLiderNombre") ?? vm.LiderNombre;
                vm.LiderVotos = GetLongAny(data, "liderVotos", "ganadorVotos") ?? vm.LiderVotos;
            }

            // Tabla líder por provincia
            var arr = TryGet(data, "liderPorProvincia")
                   ?? TryGet(data, "porProvincia")
                   ?? TryGet(data, "provincias");

            if (arr.HasValue && arr.Value.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in arr.Value.EnumerateArray())
                {
                    if (item.ValueKind != JsonValueKind.Object) continue;

                    var prov = GetStringAny(item, "provincia", "nombreProvincia", "name") ?? "";
                    var lid = GetStringAny(item, "lider", "candidato", "nombreLider", "ganador") ?? "";
                    var vts = GetLongAny(item, "votos", "totalVotos", "count") ?? 0;

                    if (!string.IsNullOrWhiteSpace(prov))
                        vm.LideresPorProvincia.Add(new ProvinciaLiderVm { Provincia = prov, Lider = lid, Votos = vts });
                }
            }

            // si no vino ambito, fuerza Nacional
            if (string.IsNullOrWhiteSpace(vm.LiderAmbito)) vm.LiderAmbito = "Nacional";

            return vm;
        }

        // ===================== HELPERS JSON =====================
        private static JsonElement? TryGet(JsonElement obj, string prop)
        {
            if (obj.ValueKind != JsonValueKind.Object) return null;
            if (obj.TryGetProperty(prop, out var v)) return v;
            return null;
        }

        private static string? GetStringAny(JsonElement obj, params string[] props)
        {
            foreach (var p in props)
            {
                var el = TryGet(obj, p);
                if (!el.HasValue) continue;

                if (el.Value.ValueKind == JsonValueKind.String)
                {
                    var s = el.Value.GetString();
                    if (!string.IsNullOrWhiteSpace(s)) return s;
                }

                if (el.Value.ValueKind == JsonValueKind.Number)
                    return el.Value.ToString();
            }
            return null;
        }

        private static int? GetIntAny(JsonElement obj, params string[] props)
        {
            foreach (var p in props)
            {
                var el = TryGet(obj, p);
                if (!el.HasValue) continue;

                if (el.Value.ValueKind == JsonValueKind.Number && el.Value.TryGetInt32(out var n))
                    return n;

                if (el.Value.ValueKind == JsonValueKind.String && int.TryParse(el.Value.GetString(), out var s))
                    return s;
            }
            return null;
        }

        private static long? GetLongAny(JsonElement obj, params string[] props)
        {
            foreach (var p in props)
            {
                var el = TryGet(obj, p);
                if (!el.HasValue) continue;

                if (el.Value.ValueKind == JsonValueKind.Number && el.Value.TryGetInt64(out var n))
                    return n;

                if (el.Value.ValueKind == JsonValueKind.String && long.TryParse(el.Value.GetString(), out var s))
                    return s;
            }
            return null;
        }

        private static string? GetDateStringAny(JsonElement obj, params string[] props)
        {
            foreach (var p in props)
            {
                var el = TryGet(obj, p);
                if (!el.HasValue) continue;

                if (el.Value.ValueKind == JsonValueKind.String)
                {
                    var raw = el.Value.GetString();
                    if (string.IsNullOrWhiteSpace(raw)) continue;

                    if (DateTime.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var dt))
                        return dt.ToString("d/M/yyyy, h:mm:ss tt", new CultureInfo("es-EC"));

                    return raw;
                }
            }
            return null;
        }
    }
}
