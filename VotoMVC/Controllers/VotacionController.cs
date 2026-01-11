using Microsoft.AspNetCore.Mvc;
using VotoMVC.Services;
using VotoMVC.ViewModelos;
using VotoMVC.ViewModelos.Votacion;

namespace VotoMVC.Controllers
{
    public class VotacionController : Controller
    {
        private readonly VotacionApiService _api;

        public VotacionController(VotacionApiService api)
        {
            _api = api;
        }

        // GET: /Votacion/Index
        public async Task<IActionResult> Index()
        {
            var cedula = HttpContext.Session.GetString("cedula");
            if (string.IsNullOrWhiteSpace(cedula))
                return RedirectToAction("Login", "Auth");

            var proceso = await _api.ObtenerProcesoActivoAsync();
            var opciones = await _api.ObtenerOpcionesActivoAsync();

            if (proceso == null || opciones == null)
            {
                ViewBag.Error = "No hay proceso electoral activo o no se pudieron cargar opciones.";
                return View(new OpcionesActivoVM());
            }

            ViewBag.Proceso = proceso;
            return View(opciones);
        }

        // POST: /Votacion/Confirmar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirmar(int idOpcion)
        {
            var cedula = HttpContext.Session.GetString("cedula");
            if (string.IsNullOrWhiteSpace(cedula))
                return RedirectToAction("Login", "Auth");

            var opciones = await _api.ObtenerOpcionesActivoAsync();
            if (opciones == null)
            {
                TempData["Error"] = "No se pudieron cargar opciones.";
                return RedirectToAction("Index");
            }

            var op = opciones.Opciones.FirstOrDefault(x => x.IdOpcion == idOpcion);
            if (op == null)
            {
                TempData["Error"] = "Opción inválida.";
                return RedirectToAction("Index");
            }

            return View(op);
        }

        // POST: /Votacion/Emitir
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Emitir(int idOpcion)
        {
            var cedula = HttpContext.Session.GetString("cedula");
            if (string.IsNullOrWhiteSpace(cedula))
                return RedirectToAction("Login", "Auth");

            var conf = await _api.EmitirVotoAsync(cedula, idOpcion);
            if (conf == null)
            {
                TempData["Error"] = "No se pudo registrar el voto (puede que ya votaste).";
                return RedirectToAction("Index");
            }

            return View("Confirmacion", conf);
        }
    }
}
