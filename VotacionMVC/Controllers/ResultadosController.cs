using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text.Json;
using VotacionMVC.Models.DTOs;
using VotacionMVC.Models.ViewModels;
using VotacionMVC.Service;

namespace VotacionMVC.Controllers
{
    public class ResultadosController : Controller
    {
        private readonly ApiService _api;
        public ResultadosController(ApiService api) => _api = api;

        [HttpGet]
        public async Task<IActionResult> Index(string tab = "envivo", CancellationToken ct = default)
        {
            var path = tab == "finales"
                ? "api/Resultados/finales"
                : "api/Resultados/en-vivo";

            var data = await _api.TryGetResultadosAsync(path, ct);

            ViewBag.Tab = tab;
            ViewBag.Debug = _api.LastError;
            return View(data ?? new ResultadosNacionalDto());
        }

    }
}
