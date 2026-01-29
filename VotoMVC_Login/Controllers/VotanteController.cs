using Microsoft.AspNetCore.Mvc;
using VotoMVC_Login.Service;
using VotoMVC_Login.Models;
using VotoMVC_Login.Services;
using VotoMVC_Login.Models.ViewModels;
using VotoMVC_Login.Models.DTOs;
namespace VotoMVC_Login.Controllers
{
    [Route("votante")]
    public class VotanteController : Controller
    {
        private readonly ApiService _api;
        public VotanteController(ApiService api) => _api = api;

        // GET: /Votante/Papeleta
        [HttpGet]
        public async Task<IActionResult> Papeleta(CancellationToken ct)
        {
            var cedula = HttpContext.Session.GetString("cedula");
            var codigoPad = HttpContext.Session.GetString("codigoPad");

            if (string.IsNullOrWhiteSpace(cedula) || string.IsNullOrWhiteSpace(codigoPad))
                return RedirectToAction("Index", "Acceso");

            var proceso = await _api.GetProcesoActivoAsync(ct);
            var candidatos = await _api.GetCandidatosAsync(ct) ?? new List<ApiService.CandidatoDto>();

            var vm = new VotanteLoginVm
            {
                Cedula = cedula,
                CodigoPad = codigoPad,
             
            };

            if (proceso?.data?.id <= 0)
                vm.Error = proceso?.error ?? "No hay proceso electoral activo.";

            if (candidatos.Count == 0)
                vm.Error = (vm.Error == null) ? "No hay candidatos." : (vm.Error + " No hay candidatos.");

            return View(vm); // Views/Votante/Papeleta.cshtml
        }

        // POST: /Votante/Confirmar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Confirmar(int CandidatoId)
        {
            HttpContext.Session.SetString("voto_candidatoId", CandidatoId.ToString());
            return RedirectToAction(nameof(Confirmar));
        }

        // GET: /Votante/Confirmar
        [HttpGet]
        public async Task<IActionResult> Confirmar(CancellationToken ct)
        {
            var cedula = HttpContext.Session.GetString("cedula");
            var codigoPad = HttpContext.Session.GetString("codigoPad");
            if (string.IsNullOrWhiteSpace(cedula) || string.IsNullOrWhiteSpace(codigoPad))
                return RedirectToAction("Index", "Acceso");

            var candidatoIdRaw = HttpContext.Session.GetString("voto_candidatoId") ?? "0";
            int.TryParse(candidatoIdRaw, out var candidatoId);

            var proceso = await _api.GetProcesoActivoAsync(ct);
            var candidatos = await _api.GetCandidatosAsync(ct) ?? new List<ApiService.CandidatoDto>();

            var vm = new VotanteLoginVm
            {
                Cedula = cedula,
                CodigoPad = codigoPad,
               
            };

            return View(vm); // Views/Votante/Confirmar.cshtml
        }

        // POST: /Votante/Emitir
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Emitir(CancellationToken ct)
        {
            
            return RedirectToAction(nameof(Comprobante));
        }

        // GET: /Votante/Comprobante
        [HttpGet]
        public IActionResult Comprobante()
        {
            ViewBag.Msg = TempData["Msg"] ?? "✅ Voto procesado.";
            ViewBag.Comprobante = TempData["Comprobante"] ?? "";
            return View(); // Views/Votante/Comprobante.cshtml
        }


    }
}
