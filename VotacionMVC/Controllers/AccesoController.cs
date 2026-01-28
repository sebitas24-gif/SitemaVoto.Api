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
        public async Task<IActionResult> Index(string cedula, int metodo, CancellationToken ct)
        {
            var r = await _api.SolicitarOtpAsync(cedula, metodo, ct);
            if (r == null || !r.Ok)
            {
                ViewBag.Error = r?.Error ?? "No se pudo solicitar OTP.";
                return View();
            }

            HttpContext.Session.SetString("otp_cedula", cedula);
            HttpContext.Session.SetInt32("otp_metodo", metodo);

            return RedirectToAction(nameof(Verificar));
        }
        [HttpGet]
        public IActionResult Verificar()
        {
            if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("otp_cedula")))
                return RedirectToAction(nameof(Index));
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Verificar(string codigo, CancellationToken ct)
        {
            var cedula = HttpContext.Session.GetString("otp_cedula");
            if (string.IsNullOrWhiteSpace(cedula))
                return RedirectToAction(nameof(Index));

            var r = await _api.VerificarOtpAsync(cedula, codigo, ct);
            if (r == null || !r.Ok)
            {
                ViewBag.Error = r?.Error ?? "OTP inválido.";
                return View();
            }

            // guardamos sesión del usuario
            HttpContext.Session.SetString("cedula", cedula);
            HttpContext.Session.SetInt32("rol", r.Rol);

            // limpiar flujo otp
            HttpContext.Session.Remove("otp_cedula");

            // redirección simple según rol (ajusta si tus roles son 1/2/3)
            if (r.Rol == 1) return RedirectToAction("Procesos", "Admin");        // Admin
            if (r.Rol == 2) return RedirectToAction("Panel", "JefeJunta");      // Jefe
            return RedirectToAction("Index", "Home");                           // Votante (o acceso PAD)
        }

        [HttpGet]
        public IActionResult Votante()
        {
            // 🔥 Temporal: valores de prueba (pon una cédula/pad que exista en tu API)
            HttpContext.Session.SetString("cedula", "0102030405");
            HttpContext.Session.SetString("codigoPad", "PAD-123456");

            return RedirectToAction("Papeleta", "Votante");
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
        public IActionResult Jefe() => View(model: null);

        [HttpPost]
        public async Task<IActionResult> Jefe(string cedula, CancellationToken ct)
        {
            cedula = (cedula ?? "").Trim();

            if (string.IsNullOrWhiteSpace(cedula))
            {
                ViewBag.Msg = "Ingrese una cédula.";
                return View(model: null);
            }

            var data = await _api.GetVotantePorCedulaAsync(cedula, ct);

            if (data == null)
            {
                ViewBag.Msg = "❌ No existe un votante con esa cédula en el padrón.";
                return View(model: null);
            }

            ViewBag.Msg = "✅ Verificación: Si coincide, puede entregar el código PAD para votar.";
            return View(data);
        }

        [HttpGet]
        public IActionResult Bridge(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return RedirectToAction("Index");

            string data;
            try
            {
                data = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(token));
            }
            catch
            {
                return RedirectToAction("Index");
            }

            // data: "cedula|rol|fecha"
            var parts = data.Split('|');
            if (parts.Length < 2) return RedirectToAction("Index");

            var cedula = parts[0];
            var rol = parts[1];

            // Guardamos en sesión para el resto del sistema
            HttpContext.Session.SetString("cedula", cedula);
            HttpContext.Session.SetString("rol", rol);

            // Redirigir según rol
            return rol switch
            {
                "Admin" => RedirectToAction("Admin", "Acceso"),
                "JefeJunta" => RedirectToAction("Index", "Jefe"),   // ajusta a tu controlador real
                _ => RedirectToAction("Papeleta", "Votante"),
            };
        }



    }
}
