using Microsoft.AspNetCore.Mvc;
using VotacionMVC.Models.DTOs;
using VotacionMVC.Service;

namespace VotacionMVC.Controllers
{
    public class AccesoController : Controller
    {
        private readonly ApiService _api;
        public AccesoController(ApiService api)
        {
            _api = api;
        }
        [HttpGet]
        public IActionResult Votante() => View();
         [HttpGet]
        public IActionResult Index()
        {
            // Menú con 4 botones (como tu simulación)
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Votante(string cedula, string codigoPad, CancellationToken ct)
        {
            var req = new PadronValidarRequest
            {
                cedula = cedula.Trim(),
                codigoPad = codigoPad.Trim()
            };

            var resp = await _api.PostAsync<PadronValidarRequest, PadronValidarResponse>(
                "api/Padron/validar", req, ct);

            if (resp == null || resp.ok == false)
            {
                ViewBag.Msg = resp?.error ?? "No se pudo validar.";
                return View();
            }

            HttpContext.Session.SetString("cedula", req.cedula);
            HttpContext.Session.SetString("codigoPad", req.codigoPad);

            return RedirectToAction("Papeleta", "Votante");
        }

        [HttpGet]
        public IActionResult Jefe() => View();


        [HttpGet]
        public IActionResult Admin() => View();
    }
}
