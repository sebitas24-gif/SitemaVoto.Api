using Microsoft.AspNetCore.Mvc;
using VotoMVC_Login.Service;
using VotoMVC_Login.Services;
using VotoMVC_Login.Models.ViewModels;
using VotoMVC_Login.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
namespace VotoMVC_Login.Controllers
{

    public class VotacionController : Controller
    {
        private readonly ApiService _api;

        private const string VOTO_PROCESO = "VOTO_PROCESO";
        private const string VOTO_CANDIDATO = "VOTO_CANDIDATO";

        public VotacionController(ApiService api) => _api = api;

        [HttpGet]
        public IActionResult Index() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string cedula, string codigoPad, CancellationToken ct)
        {
            cedula = (cedula ?? "").Trim();
            codigoPad = (codigoPad ?? "").Trim();

            if (cedula.Length != 10 || string.IsNullOrWhiteSpace(codigoPad))
            {
                ViewBag.Error = "Cédula (10 dígitos) y código PAD son obligatorios.";
                return View();
            }

            // 1) Validar que exista proceso ACTIVO antes de validar PAD
            var proc = await _api.GetProcesoActivoAsync(ct);

            if (proc == null || !proc.ok || proc.data == null)
            {
                // limpiar sesión por seguridad
                HttpContext.Session.Clear();

                ViewBag.Error = "No hay un proceso electoral ACTIVO. No se puede votar.";
                return View(); // o RedirectToAction(nameof(Index))
            }
            var procesoId = proc.data.id;


            // 2) Validar PAD (aquí tu API debe devolver si ya está usado)
            var r = await _api.ValidarPadConGetAsync(cedula, codigoPad, ct);
            if (!r.Ok)
            {
                ViewBag.Error = r.Error ?? "No se pudo validar el PAD.";
                return View();
            }

            // ✅ Si tu API devuelve algo tipo: r.Data.usado / r.Data.puedeVotar
            // adapta el nombre. Ejemplo recomendado:
            if (r.Data != null && r.Data.usado == true)
            {
                ViewBag.Error = "Este código PAD ya fue utilizado. No se puede volver a votar.";
                return View();
            }

            // Guardar sesión
            HttpContext.Session.SetString(SessionKeys.Cedula, cedula);
            HttpContext.Session.SetString(SessionKeys.CodigoUnico, (r.Data?.codigoPad ?? "").Trim());

            // Guardar el proceso (para que no te cambien en medio)
            HttpContext.Session.SetInt32(VOTO_PROCESO, procesoId);

            return RedirectToAction(nameof(Papeleta));
        }

        [HttpGet]
        public async Task<IActionResult> Papeleta(CancellationToken ct)
        {
            var cedula = HttpContext.Session.GetString(SessionKeys.Cedula);
            var pad = HttpContext.Session.GetString(SessionKeys.CodigoUnico);

            if (string.IsNullOrWhiteSpace(cedula) || string.IsNullOrWhiteSpace(pad))
                return RedirectToAction(nameof(Index));

            // 1) Verificar proceso activo
            var proc = await _api.GetProcesoActivoAsync(ct);

            if (proc == null || !proc.ok || proc.data == null)
            {
                // limpiar sesión por seguridad
                HttpContext.Session.Clear();

                ViewBag.Error = "No hay un proceso electoral ACTIVO. No se puede votar.";
                return View(); // o RedirectToAction(nameof(Index))
            }
            var procesoId = proc.data.id;

            // 2) Revalidar que PAD no esté usado (por si intentan reingresar)
            var padCheck = await _api.ValidarPadConGetAsync(cedula!, pad!, ct);
            if (!padCheck.Ok)
            {
                TempData["Error"] = padCheck.Error ?? "No se pudo validar el PAD.";
                return RedirectToAction(nameof(Index));
            }
            if (padCheck.Data != null && padCheck.Data.usado == true)
            {
                TempData["Error"] = "Este código PAD ya fue utilizado. No se puede volver a votar.";
                return RedirectToAction(nameof(Index));
            }

            var vm = new VotacionPapeletaVm
            {
                Cedula = cedula!,
                CodigoPad = pad!,
                Proceso = proc,
                ProcesoId = procesoId,
                ProcesoNombre = proc?.data?.nombre ?? "Proceso Activo",
                Tipo = "Plancha (Binomio)",
                Normas = "Seleccione 1 opción y confirme."
            };

            var lista = await _api.GetCandidatosAsync(ct) ?? new List<ApiService.CandidatoDto>();
            vm.Candidatos = lista.Where(x => x.activo).ToList();

            if (vm.Candidatos.Count == 0)
                vm.Error = "No hay candidatos activos.";

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Confirmar(int procesoId, int candidatoId)
        {
            HttpContext.Session.SetInt32(VOTO_PROCESO, procesoId);
            HttpContext.Session.SetInt32(VOTO_CANDIDATO, candidatoId);
            return RedirectToAction(nameof(Confirmar));
        }

        [HttpGet]
        public async Task<IActionResult> Confirmar(CancellationToken ct)
        {
            var cedula = HttpContext.Session.GetString(SessionKeys.Cedula) ?? "";
            var pad = HttpContext.Session.GetString(SessionKeys.CodigoUnico) ?? "";
            if (string.IsNullOrWhiteSpace(cedula) || string.IsNullOrWhiteSpace(pad))
                return RedirectToAction(nameof(Index));

            // Verificar proceso activo
            var proc = await _api.GetProcesoActivoAsync(ct);

            if (proc == null || !proc.ok || proc.data == null)
            {
                TempData["Error"] = "No hay proceso ACTIVO. No se puede votar.";
                return RedirectToAction(nameof(Index));
            }

            var procesoActivoId = proc.data.id;

            var procesoId = HttpContext.Session.GetInt32(VOTO_PROCESO) ?? 0;
            var candidatoId = HttpContext.Session.GetInt32(VOTO_CANDIDATO) ?? 0;

            // Si cambió el proceso mientras estaba en pantalla, lo boto
            if (procesoId != procesoActivoId)
            {
                TempData["Error"] = "El proceso activo cambió. Vuelve a ingresar.";
                return RedirectToAction(nameof(Index));
            }

            // Revalidar PAD
            var padCheck = await _api.ValidarPadConGetAsync(cedula, pad, ct);
            if (!padCheck.Ok || (padCheck.Data != null && padCheck.Data.usado == true))
            {
                TempData["Error"] = "El código PAD ya fue utilizado o no es válido.";
                return RedirectToAction(nameof(Index));
            }

            var lista = await _api.GetCandidatosAsync(ct) ?? new List<ApiService.CandidatoDto>();

            var vm = new VotacionPapeletaVm
            {
                Cedula = cedula,
                CodigoPad = pad,
                Proceso = proc,
                ProcesoId = procesoId,
                ProcesoNombre = proc?.data?.nombre ?? "Proceso Activo",
                Tipo = "Plancha (Binomio)",
                Normas = "Seleccione 1 opción y confirme.",
                Candidatos = lista.Where(x => x.activo).ToList(),
                CandidatoId = candidatoId
            };

            if (procesoId <= 0) vm.Error = "Proceso inválido. Vuelve a la papeleta.";
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EmitirVoto(CancellationToken ct)
        {
            var cedula = HttpContext.Session.GetString(SessionKeys.Cedula);
            var pad = HttpContext.Session.GetString(SessionKeys.CodigoUnico);
            if (string.IsNullOrWhiteSpace(cedula) || string.IsNullOrWhiteSpace(pad))
                return RedirectToAction(nameof(Index));

            // Verificar proceso activo antes de emitir voto
            var proc = await _api.GetProcesoActivoAsync(ct);

            if (proc == null || !proc.ok || proc.data == null)
            {
                TempData["Error"] = "No hay proceso ACTIVO. No se puede votar.";
                return RedirectToAction(nameof(Index));
            }

            var procesoActivoId = proc.data.id;


            // Revalidar PAD antes de emitir
            var padCheck = await _api.ValidarPadConGetAsync(cedula!, pad!, ct);
            if (!padCheck.Ok)
            {
                TempData["Error"] = padCheck.Error ?? "No se pudo validar el PAD.";
                return RedirectToAction(nameof(Index));
            }
            if (padCheck.Data != null && padCheck.Data.usado == true)
            {
                TempData["Error"] = "Este código PAD ya fue utilizado. No se puede volver a votar.";
                return RedirectToAction(nameof(Index));
            }

            var candidatoId = HttpContext.Session.GetInt32(VOTO_CANDIDATO) ?? 0;

            var dto = new ApiService.EmitirVotoDto
            {
                Cedula = cedula!,
                CodigoPad = pad!,
                CandidatoId = (candidatoId == 0 ? (int?)null : candidatoId)
            };

            var resp = await _api.EmitirVotoAsync(dto, ct);

            if (resp == null || !resp.Ok)
            {
                TempData["Error"] = resp?.Error ?? "No se pudo emitir el voto.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Comprobante"] = resp.Comprobante ?? "";
            return RedirectToAction(nameof(Exito));
        }

        [HttpGet]
        public IActionResult Exito()
        {
            ViewBag.Comprobante = TempData["Comprobante"] ?? "";
            ViewBag.Error = TempData["Error"] ?? "";
            return View();
        }

    }
}
