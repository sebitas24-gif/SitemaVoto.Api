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

        [HttpGet]
        public async Task<IActionResult> Papeleta(CancellationToken ct)
        {
            // Debe venir de AccesoController.Votante (sesión)
            var cedula = HttpContext.Session.GetString("cedula");
            var codigoPad = HttpContext.Session.GetString("codigoPad");

            if (string.IsNullOrWhiteSpace(cedula) || string.IsNullOrWhiteSpace(codigoPad))
                return RedirectToAction("Index", "Acceso");

            var vm = new VotantePapeletaVm
            {
                Proceso = await _api.GetProcesoActivoAsync(ct),
                Candidatos = (await _api.GetCandidatosAsync(ct)) ?? new List<CandidatoDto>()
            };

            if (vm.Proceso == null) vm.Error = "No se pudo obtener el proceso activo.";
            if (vm.Candidatos.Count == 0) vm.Error = (vm.Error ?? "") + " No hay candidatos.";

            return View(vm);
        }

        [HttpPost]
        public IActionResult Confirmar(string? candidatoId, bool blanco)
        {
            // Guardamos selección para el POST final
            HttpContext.Session.SetString("voto_blanco", blanco ? "1" : "0");
            HttpContext.Session.SetString("voto_candidatoId", candidatoId ?? "");

            return RedirectToAction("Confirmar");
        }

        [HttpGet]
        public async Task<IActionResult> Confirmar(CancellationToken ct)
        {
            var cedula = HttpContext.Session.GetString("cedula");
            var codigoPad = HttpContext.Session.GetString("codigoPad");
            if (string.IsNullOrWhiteSpace(cedula) || string.IsNullOrWhiteSpace(codigoPad))
                return RedirectToAction("Index", "Acceso");

            var blanco = HttpContext.Session.GetString("voto_blanco") == "1";
            var candidatoId = HttpContext.Session.GetString("voto_candidatoId");

            var vm = new VotantePapeletaVm
            {
                Proceso = await _api.GetProcesoActivoAsync(ct),
                Candidatos = (await _api.GetCandidatosAsync(ct)) ?? new List<CandidatoDto>(),
                Blanco = blanco,
                CandidatoId = candidatoId
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Emitir(CancellationToken ct)
        {
            var cedula = HttpContext.Session.GetString("cedula");
            var codigoPad = HttpContext.Session.GetString("codigoPad");
            if (string.IsNullOrWhiteSpace(cedula) || string.IsNullOrWhiteSpace(codigoPad))
                return RedirectToAction("Index", "Acceso");

            var candidatoIdRaw = (HttpContext.Session.GetString("voto_candidatoId") ?? "").Trim();

            // ✅ Si no eligió nada, volver
            
        if (string.IsNullOrWhiteSpace(candidatoIdRaw))
            {
                TempData["Msg"] = "❌ Debe seleccionar un candidato o Voto en Blanco.";
                return RedirectToAction("Papeleta");
            }

            // ✅ Convertir a int (0 = blanco)
            if (!int.TryParse(candidatoIdRaw, out var candidatoId))
            {
                TempData["Msg"] = "❌ Selección inválida. Vuelva a elegir.";
                return RedirectToAction("Papeleta");
            }

            // ✅ Request real según TU DTO
            var req = new VotacionEmitirRequest
            {
                cedula = cedula,
                codigoPad = codigoPad,
                candidatoId = candidatoId
            };

            var resp = await _api.EmitirVotoAsync(req, ct);

            if (resp == null)
            {
                TempData["Msg"] = "❌ No se pudo emitir el voto (API no respondió o falló).";
                return RedirectToAction("Papeleta");
            }

            // Limpieza para que no vuelva a emitir por refresh
            HttpContext.Session.Remove("voto_candidatoId");

            TempData["Msg"] = "✅ Voto registrado correctamente.";
            return RedirectToAction("Comprobante");
        }

        [HttpGet]
        public IActionResult Comprobante()
        {
            ViewBag.Msg = TempData["Msg"] ?? "✅ Voto procesado.";
            return View();
        }
    }
    }

