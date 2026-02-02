using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VotoMVC_Login.Models.DTOs;
using VotoMVC_Login.Service;

namespace VotoMVC_Login.Controllers
{
    [AllowAnonymous]
    public class ResultadosPublicosController : Controller
    {
        private readonly ApiService _api;
        public ResultadosPublicosController(
           ApiService api
           ) { _api = api; }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var data = await _api.GetResultadosNacionalAsync(ct);
            ViewBag.Modo = "EN_VIVO";
            return View(data ?? new ResultadosNacionalResponse());
        }

        // FINALES (usa otro endpoint si tienes; si no, usa el mismo por ahora)
        [HttpGet]
        public async Task<IActionResult> Finales(CancellationToken ct)
        {
            // Si tu API tiene /api/Resultados/final úsalo aquí
            var data = await _api.GetResultadosFinalesAsync(ct);


            ViewBag.Modo = "FINALES";
            return View("Index", data ?? new ResultadosNacionalResponse()); // 👈 reutiliza Index.cshtml
        }

    }
}
