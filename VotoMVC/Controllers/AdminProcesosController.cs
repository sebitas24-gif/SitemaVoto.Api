using Microsoft.AspNetCore.Mvc;
using VotoModelos;
using VotoMVC.Services;

namespace VotoMVC.Controllers
{
    public class AdminProcesosController : Controller
    {
        private readonly ProcesoApiService _api;
        public AdminProcesosController(ProcesoApiService api)
        {
            _api = api;
        }

        private string? Token() => HttpContext.Session.GetString("token");

        // GET: /AdminProcesos
        public async Task<IActionResult> Index()
        {
            var lista = await _api.GetAllAsync();
            return View(lista);
        }

        // GET: /AdminProcesos/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new ProcesoElectoral
            {
                FechaInicio = DateTime.Now,
                FechaFin = DateTime.Now.AddHours(1),
                Estado = true
            });
        }

        // POST: /AdminProcesos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProcesoElectoral model)
        {
            if (!ModelState.IsValid) return View(model);

            var ok = await _api.CreateAsync(model, Token());
            if (!ok)
            {
                ModelState.AddModelError("", "No se pudo crear el proceso (API rechazó la solicitud).");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /AdminProcesos/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var proc = await _api.GetByIdAsync(id);
            if (proc == null) return NotFound();
            return View(proc);
        }

        // POST: /AdminProcesos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProcesoElectoral model)
        {
            if (id != model.Id) return BadRequest();

            if (!ModelState.IsValid) return View(model);

            var ok = await _api.UpdateAsync(id, model, Token());
            if (!ok)
            {
                ModelState.AddModelError("", "No se pudo actualizar el proceso (API rechazó la solicitud).");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /AdminProcesos/Cerrar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cerrar(int id)
        {
            var ok = await _api.CerrarAsync(id, Token());
            TempData["msg"] = ok ? "✅ Proceso cerrado (Estado=false)" : "❌ No se pudo cerrar el proceso.";
            return RedirectToAction(nameof(Index));
        }
    }
    }
