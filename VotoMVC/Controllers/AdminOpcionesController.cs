using Microsoft.AspNetCore.Mvc;
using VotoModelos;
using VotoMVC.Services;

namespace VotoMVC.Controllers
{
    public class AdminOpcionesController : Controller
    {
        private readonly ProcesoApiService _procesos;
        private readonly OpcionApiService _opciones;

        public AdminOpcionesController(ProcesoApiService procesos, OpcionApiService opciones)
        {
            _procesos = procesos;
            _opciones = opciones;
        }

        private string? Token() => HttpContext.Session.GetString("token");

        // Pantalla para escoger proceso
        public async Task<IActionResult> Procesos()
        {
            var lista = await _procesos.GetAllAsync();
            return View(lista);
        }

        // Listar opciones de un proceso
        public async Task<IActionResult> Index(int idProceso)
        {
            ViewBag.IdProceso = idProceso;
            var lista = await _opciones.GetByProcesoAsync(idProceso);
            return View(lista);
        }

        [HttpGet]
        public IActionResult Create(int idProceso)
        {
            return View(new OpcionElectoral
            {
                Id = idProceso,
                Activo = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OpcionElectoral model)
        {
            if (!ModelState.IsValid) return View(model);

            // Si es BLANCO, IdCandidato debe ser null
            if ((model.Tipo ?? "").ToUpper() == "BLANCO")
                model.IdCandidato = null;

            var ok = await _opciones.CreateAsync(model, Token());
            if (!ok)
            {
                ModelState.AddModelError("", "No se pudo crear la opción (API rechazó la solicitud).");
                return View(model);
            }

            return RedirectToAction(nameof(Index), new { idProceso = model.Id });
        }
    }
}
