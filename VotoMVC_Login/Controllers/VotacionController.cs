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

        // ✅ Keys de sesión (NO rutas)
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

            var r = await _api.ValidarPadConGetAsync(cedula, codigoPad, ct);
            if (!r.Ok)
            {
                ViewBag.Error = r.Error ?? "No se pudo validar el PAD.";
                return View();
            }

            HttpContext.Session.SetString(SessionKeys.Cedula, cedula);
            HttpContext.Session.SetString(SessionKeys.CodigoUnico, (r.Data?.codigoPad ?? "").Trim());

            return RedirectToAction(nameof(Papeleta));
        }

        [HttpGet]
        public async Task<IActionResult> Papeleta(CancellationToken ct)
        {
            var cedula = HttpContext.Session.GetString(SessionKeys.Cedula);
            var pad = HttpContext.Session.GetString(SessionKeys.CodigoUnico);

            if (string.IsNullOrWhiteSpace(cedula) || string.IsNullOrWhiteSpace(pad))
                return RedirectToAction(nameof(Index));

            var proc = await _api.GetProcesoActivoAsync(ct);
            var procesoId = proc?.data?.id ?? 0;

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

            if (procesoId <= 0)
            {
                vm.Error = proc?.error ?? "No hay proceso electoral activo.";
                return View(vm);
            }

            var lista = await _api.GetCandidatosAsync(ct) ?? new List<ApiService.CandidatoDto>();
            vm.Candidatos = lista.Where(x => x.activo).ToList();

            if (vm.Candidatos.Count == 0)
                vm.Error = "No hay candidatos activos.";

            return View(vm); // Views/Votacion/Papeleta.cshtml
        }

        // POST: /Votacion/Confirmar (guarda selección y va a pantalla confirmar)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Confirmar(int procesoId, int candidatoId)
        {
            HttpContext.Session.SetInt32(VOTO_PROCESO, procesoId);
            HttpContext.Session.SetInt32(VOTO_CANDIDATO, candidatoId);

            return RedirectToAction(nameof(Confirmar));
        }

        // GET: /Votacion/Confirmar
        [HttpGet]
        public async Task<IActionResult> Confirmar(CancellationToken ct)
        {
            var cedula = HttpContext.Session.GetString(SessionKeys.Cedula) ?? "";
            var pad = HttpContext.Session.GetString(SessionKeys.CodigoUnico) ?? "";
            if (string.IsNullOrWhiteSpace(cedula) || string.IsNullOrWhiteSpace(pad))
                return RedirectToAction(nameof(Index));

            var procesoId = HttpContext.Session.GetInt32(VOTO_PROCESO) ?? 0;
            var candidatoId = HttpContext.Session.GetInt32(VOTO_CANDIDATO) ?? 0;

            var proc = await _api.GetProcesoActivoAsync(ct);
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

            return View(vm); // Views/Votacion/Confirmar.cshtml
        }

        // POST: /Votacion/Emitir
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EmitirVoto(CancellationToken ct)
        {
            var cedula = HttpContext.Session.GetString(SessionKeys.Cedula);
            var pad = HttpContext.Session.GetString(SessionKeys.CodigoUnico);
            if (string.IsNullOrWhiteSpace(cedula) || string.IsNullOrWhiteSpace(pad))
                return RedirectToAction(nameof(Index));
            var candidatoId = HttpContext.Session.GetInt32(VOTO_CANDIDATO) ?? 0;

            var dto = new ApiService.EmitirVotoDto
            {
                Cedula = cedula!,
                CodigoPad = pad!,
                // 0 = blanco => null
                CandidatoId = (candidatoId == 0 ? (int?)null : candidatoId)
            };



            var resp = await _api.EmitirVotoAsync(dto, ct);



            if (resp == null || !resp.Ok)
            {
                TempData["Error"] = resp?.Error ?? "No se pudo emitir el voto.";
                return RedirectToAction(nameof(Papeleta));
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
