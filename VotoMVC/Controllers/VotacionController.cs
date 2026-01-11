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

        private string? Cedula() => HttpContext.Session.GetString("cedula");
        private string? Token() => HttpContext.Session.GetString("token");

        // ✅ LECTURA
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var proc = await _api.GetProcesoActivoAsync();
            if (proc == null) return View("SinProceso");

            int idProceso = (int)proc.idProceso;

            var opcionesRaw = await _api.GetOpcionesAsync(idProceso) ?? new List<dynamic>();

            var vm = new VotacionIndexVM
            {
                IdProceso = idProceso,
                NombreProceso = (string?)proc.nombre,
                TipoEleccion = (string?)proc.tipoEleccion,
                Opciones = opcionesRaw.Select(o => new OpcionVM
                {
                    IdOpcion = (int)o.idOpcion,
                    NombreOpcion = (string?)o.nombreOpcion,
                    Tipo = (string?)o.tipo,
                    Cargo = (string?)o.cargo
                }).ToList()
            };

            return View(vm);
        }

        // ✅ POST (solo navegación, no guarda nada aún)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Confirmar(VotacionIndexVM vm)
        {
            if (vm.OpcionSeleccionadaId == null)
            {
                TempData["msg"] = "Selecciona una opción antes de continuar.";
                return RedirectToAction(nameof(Index));
            }

            // guardamos selección en TempData para no exponer en URL
            TempData["idProceso"] = vm.IdProceso;
            TempData["idOpcion"] = vm.OpcionSeleccionadaId.Value;

            return RedirectToAction(nameof(ConfirmarVista));
        }

        // ✅ LECTURA: pantalla confirmación
        [HttpGet]
        public async Task<IActionResult> ConfirmarVista()
        {
            if (TempData["idProceso"] == null || TempData["idOpcion"] == null)
                return RedirectToAction(nameof(Index));

            int idProceso = (int)TempData["idProceso"]!;
            int idOpcion = (int)TempData["idOpcion"]!;

            // recargar opciones para mostrar nombre de la opción elegida
            var opcionesRaw = await _api.GetOpcionesAsync(idProceso) ?? new List<dynamic>();
            var elegido = opcionesRaw.FirstOrDefault(o => (int)o.idOpcion == idOpcion);

            var vm = new ConfirmarVotoVM
            {
                IdProceso = idProceso,
                IdOpcion = idOpcion,
                NombreOpcion = elegido != null ? (string?)elegido.nombreOpcion : "Opción",
                Cargo = elegido != null ? (string?)elegido.cargo : ""
            };

            return View("Confirmar", vm);
        }

        // ✅ ESCRITURA: emitir voto real
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Emitir(ConfirmarVotoVM vm)
        {
            var cedula = Cedula();
            if (string.IsNullOrWhiteSpace(cedula))
                return RedirectToAction("Login", "Auth");

            var resp = await _api.EmitirAsync(cedula, vm.IdProceso, vm.IdOpcion, Token());
            if (resp == null)
            {
                TempData["msg"] = "Error al emitir voto.";
                return RedirectToAction(nameof(Index));
            }

            var final = new FinalVM
            {
                Message = (string?)resp.message,
                CodigoVerificacion = (string?)resp.codigoVerificacion,
                FechaEmision = DateTime.UtcNow
            };

            // cerrar sesión por seguridad
            HttpContext.Session.Clear();

            return View("Final", final);
        }
    }
}
