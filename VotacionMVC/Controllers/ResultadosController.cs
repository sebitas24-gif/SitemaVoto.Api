using Microsoft.AspNetCore.Mvc;
using VotacionMVC.Models.DTOs;
using VotacionMVC.Service;

namespace VotacionMVC.Controllers
{
    public class ResultadosController : Controller
    {
        private readonly ApiService _api;
        public ResultadosController(ApiService api) => _api = api;

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var data = await _api.GetResultadosNacionalAsync() ?? new ResultadosNacionalResponse();
            return View(data);
        }
    }
}
