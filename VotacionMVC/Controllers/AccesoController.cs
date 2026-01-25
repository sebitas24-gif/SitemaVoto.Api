using Microsoft.AspNetCore.Mvc;
using VotacionMVC.Models.DTOs;
using VotacionMVC.Models.ViewModels;
using VotacionMVC.Service;
using Microsoft.AspNetCore.Http;

namespace VotacionMVC.Controllers
{
    public class AccesoController : Controller
    {
        private readonly ApiService _api;
        public AccesoController(ApiService api)
        {
            _api = api;
        }
         [HttpGet]
        public IActionResult Index()
        {
            // Menú con 4 botones (como tu simulación)
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Votante(string cedula, string codigoPad, CancellationToken ct)
        {
            var req = new PadronValidarRequest
            {
                cedula = cedula.Trim(),
                codigoPad = codigoPad.Trim()
            };

            var resp = await _api.PostAsync<PadronValidarRequest, PadronValidarResponse>(
                "api/Padron/validar", req, ct);

            if (resp == null || resp.ok == false)
            {
                ViewBag.Msg = resp?.error ?? "No se pudo validar.";
                return View();
            }

            HttpContext.Session.SetString("cedula", req.cedula);
            HttpContext.Session.SetString("codigoPad", req.codigoPad);

            return RedirectToAction("Papeleta", "Votante");
        }

        [HttpGet]
        public async Task<IActionResult> Admin(CancellationToken ct)
        {
            var procesoResp = await _api.GetProcesoActivoAsync(ct);

            var vm = new AdminDashboardVm();

            if (procesoResp == null)
            {
                vm.Error = "No se pudo obtener el proceso activo.";
                return View(vm);
            }

            // ✅ Mapea SIN asumir nombres: usa ToString() / null-coalescing
            // PERO aquí necesitamos leer las propiedades reales.
            // Como aún no las tenemos, lo hacemos con lo seguro:
            vm.Proceso= new ProcesoPanelVm
            {
                Nombre = GetString(procesoResp, "Nombre", "nombre", "nombreProceso", "proceso", "titulo"),
                Tipo = GetString(procesoResp, "Tipo", "tipo", "tipoEleccion"),
                Inicio = GetDate(procesoResp, "Inicio", "inicio", "inicioLocal", "fechaInicio"),
                Cierre = GetDate(procesoResp, "Cierre", "cierre", "fin", "finLocal", "fechaFin"),


                Estado = GetString(procesoResp, "Estado", "estado", "EstadoProceso")
            };

            return View(vm);
        }

        // Helpers para leer propiedades aunque no sepas el nombre exacto (REFLEXIÓN)
        private static string GetString(object obj, params string[] props)
        {
            var t = obj.GetType();
            foreach (var p in props)
            {
                var pi = t.GetProperty(p);
                if (pi == null) continue;
                var val = pi.GetValue(obj)?.ToString();
                if (!string.IsNullOrWhiteSpace(val)) return val;
            }
            return "—";
        }

        private static DateTime? GetDate(object obj, params string[] props)
        {
            var t = obj.GetType();
            foreach (var p in props)
            {
                var pi = t.GetProperty(p);
                if (pi == null) continue;

                var raw = pi.GetValue(obj);
                if (raw is DateTime dt) return dt;

                if (raw != null && DateTime.TryParse(raw.ToString(), out var parsed))
                    return parsed;
            }
            return null;
        }



        [HttpGet]
        public IActionResult Jefe() => View();


        
    }
}
