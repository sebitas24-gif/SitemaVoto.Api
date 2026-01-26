using Microsoft.AspNetCore.Mvc;
using VotacionMVC.Models.DTOs;
using VotacionMVC.Service;

namespace VotacionMVC.Controllers
{
    public class VotacionController : Controller
    {
        private readonly ApiService _api;
        public VotacionController(ApiService api) => _api = api;

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var candidatos = await _api.GetCandidatosAsync() ?? new List<CandidatoDto>();

            var vm = new VotarViewModel
            {
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(VotarViewModel vm)
        {
            var candidatos = await _api.GetCandidatosAsync() ?? new List<CandidatoDto>();

            if (string.IsNullOrWhiteSpace(vm.cedula) || vm.cedula.Length != 10)
            {
                vm.ok = false;
                vm.error = "Cédula inválida (10 dígitos).";
                return View(vm);
            }
            if (string.IsNullOrWhiteSpace(vm.codigoPad))
            {
                vm.ok = false;
                vm.error = "Ingrese el código PAD.";
                return View(vm);
            }

            var resp = await _api.EmitirVotoAsync(new VotacionEmitirRequest
            {
                cedula = vm.cedula.Trim(),
                codigoPad = vm.codigoPad.Trim(),
                candidatoId = vm.CandidatoId
            });

            vm.ok = resp?.ok ?? false;
            vm.error = resp?.error;
            vm.comprobante = resp?.comprobante;

            return View(vm);
        }
    }
}
