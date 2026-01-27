using Microsoft.AspNetCore.Mvc;
using VotacionMVC.Service;
using VotacionMVC.Models.DTOs;
using VotacionMVC.Models.ViewModels;

namespace VotacionMVC.Controllers
{
    public class VotanteController : Controller
    {
        private readonly ApiService _api;
        public VotanteController(ApiService api) => _api = api;

        // GET: /Votante/Papeleta
        [HttpGet]
        public async Task<IActionResult> Papeleta()
        {
            var cedula = HttpContext.Session.GetString("cedula");
            var codigoPad = HttpContext.Session.GetString("codigoPad");

            if (string.IsNullOrWhiteSpace(cedula) || string.IsNullOrWhiteSpace(codigoPad))
                return RedirectToAction("Index", "Acceso");

            var proceso = await _api.GetProcesoActivoAsync();
            var candidatos = await _api.GetCandidatosAsync() ?? new List<CandidatoDto>();

            var vm = new VotantePapeletaVm
            {
                Cedula = cedula,
                CodigoPad = codigoPad,
                Proceso = proceso,
                Candidatos = candidatos
            };

            if (proceso == null)
                vm.Error = "No se pudo obtener el proceso activo.";

            if (candidatos.Count == 0)
                vm.Error = (vm.Error == null) ? "No hay candidatos." : (vm.Error + " No hay candidatos.");

            return View(vm);
        }

        // POST: /Votante/Confirmar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Confirmar(int CandidatoId)
        {
            // Guardamos selección (0 = blanco)
            HttpContext.Session.SetString("voto_candidatoId", CandidatoId.ToString());
            return RedirectToAction(nameof(Confirmar));
        }

        // GET: /Votante/Confirmar
        [HttpGet]
        public async Task<IActionResult> Confirmar()
        {
            var cedula = HttpContext.Session.GetString("cedula");
            var codigoPad = HttpContext.Session.GetString("codigoPad");
            if (string.IsNullOrWhiteSpace(cedula) || string.IsNullOrWhiteSpace(codigoPad))
                return RedirectToAction("Index", "Acceso");

            var candidatoIdRaw = HttpContext.Session.GetString("voto_candidatoId") ?? "0";
            int.TryParse(candidatoIdRaw, out var candidatoId);

            var proceso = await _api.GetProcesoActivoAsync();
            var candidatos = await _api.GetCandidatosAsync() ?? new List<CandidatoDto>();

            var vm = new VotantePapeletaVm
            {
                Cedula = cedula,
                CodigoPad = codigoPad,
                Proceso = proceso,
                Candidatos = candidatos,
                CandidatoId = candidatoId
            };

            return View(vm); // necesitas Views/Votante/Confirmar.cshtml
        }

        // POST: /Votante/Emitir
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Emitir()
        {
            var cedula = HttpContext.Session.GetString("cedula");
            var codigoPad = HttpContext.Session.GetString("codigoPad");
            if (string.IsNullOrWhiteSpace(cedula) || string.IsNullOrWhiteSpace(codigoPad))
                return RedirectToAction("Index", "Acceso");

            var candidatoIdRaw = HttpContext.Session.GetString("voto_candidatoId") ?? "0";
            int.TryParse(candidatoIdRaw, out var candidatoId);

            var req = new VotacionEmitirRequest
            {
                cedula = cedula,
                codigoPad = codigoPad,
                candidatoId = candidatoId // 0 = blanco
            };

            var resp = await _api.EmitirVotoAsync(req);

            if (resp == null || resp.ok == false)
            {
                TempData["Msg"] = "❌ No se pudo emitir el voto: " + (resp?.error ?? "Error desconocido");
                return RedirectToAction(nameof(Papeleta));
            }

            // limpiamos selección
            HttpContext.Session.Remove("voto_candidatoId");

            TempData["Msg"] = "✅ Voto registrado correctamente.";
            TempData["Comprobante"] = resp.comprobante ?? "";
            return RedirectToAction(nameof(Comprobante));
        }

        // GET: /Votante/Comprobante
        [HttpGet]
        public IActionResult Comprobante()
        {
            ViewBag.Msg = TempData["Msg"] ?? "✅ Voto procesado.";
            ViewBag.Comprobante = TempData["Comprobante"] ?? "";
            return View();
        }
    }
    }

