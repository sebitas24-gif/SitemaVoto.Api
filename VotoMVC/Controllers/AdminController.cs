using Microsoft.AspNetCore.Mvc;
using VotoMVC.Services;

namespace VotoMVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly AdminApiService _adminApi;

        public AdminController(AdminApiService adminApi)
        {
            _adminApi = adminApi;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AsignarRoles()
        {
            return View();
        }

        //POST: hacer admin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HacerAdmin(int idVotante)
        {
            var token = HttpContext.Session.GetString("token");
            var ok = await _adminApi.HacerAdminAsync(idVotante, token);

            TempData["msg"] = ok ? "✅ Rol ADMIN asignado." : "❌ No se pudo asignar ADMIN.";
            return RedirectToAction(nameof(AsignarRoles));
        }

        // POST: quitar admin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuitarAdmin(int idVotante)
        {
            var token = HttpContext.Session.GetString("token");
            var ok = await _adminApi.QuitarAdminAsync(idVotante, token);

            TempData["msg"] = ok ? "✅ Rol ADMIN removido." : $"❌ No se pudo quitar ADMIN";
            return RedirectToAction(nameof(AsignarRoles));
        }

        //POST: hacer candidato
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HacerCandidato(int idVotante, string partido, string eslogan)
        {
            var token = HttpContext.Session.GetString("token");
            var ok = await _adminApi.HacerCandidatoAsync(idVotante, partido ?? "", eslogan ?? "", token);

            TempData["msg"] = ok ? "✅ Rol CANDIDATO asignado." : $"❌ No se pudo asignar CANDIDATO";
            return RedirectToAction(nameof(AsignarRoles));
        }

        //POST: quitar candidato
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuitarCandidato(int idVotante)
        {
            var token = HttpContext.Session.GetString("token");
            var ok = await _adminApi.QuitarCandidatoAsync(idVotante, token);

            TempData["msg"] = ok ? "✅ Rol CANDIDATO removido." : $"❌ No se pudo quitar CANDIDATO";
            return RedirectToAction(nameof(AsignarRoles));
        }
    }
}
