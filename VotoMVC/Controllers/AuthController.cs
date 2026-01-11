using Microsoft.AspNetCore.Mvc;
using VotoMVC.Services;
using VotoMVC.ViewModelos;

namespace VotoMVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthApiService _auth;

        public AuthController(AuthApiService auth)
        {
            _auth = auth;
        }

        [HttpGet]
        public IActionResult Login() => View(new LoginCedulaVM());

        [HttpPost]
        public async Task<IActionResult> Login(LoginCedulaVM vm)
        {
            if (string.IsNullOrWhiteSpace(vm.Cedula))
            {
                ModelState.AddModelError("", "Cédula requerida.");
                return View(vm);
            }

            var ok = await _auth.SolicitarCodigoAsync(vm.Cedula.Trim());
            if (!ok)
            {
                ModelState.AddModelError("", "No se pudo enviar el código. Verifica la cédula y el correo.");
                return View(vm);
            }

            return RedirectToAction(nameof(Verificar), new { cedula = vm.Cedula.Trim() });
        }

        [HttpGet]
        public IActionResult Verificar(string cedula)
        {
            return View(new VerificarCodigoVM { Cedula = cedula });
        }

        [HttpPost]
        public async Task<IActionResult> Verificar(VerificarCodigoVM vm)
        {
            if (string.IsNullOrWhiteSpace(vm.Cedula) || string.IsNullOrWhiteSpace(vm.Codigo))
            {
                ModelState.AddModelError("", "Cédula y código son requeridos.");
                return View(vm);
            }

            var (ok, roles, token) = await _auth.VerificarCodigoAsync(vm.Cedula.Trim(), vm.Codigo.Trim());
            if (!ok)
            {
                ModelState.AddModelError("", "Código inválido o expirado.");
                return View(vm);
            }

            // Guardar en sesión
            HttpContext.Session.SetString("cedula", vm.Cedula.Trim());
            HttpContext.Session.SetString("roles", string.Join(",", roles));
            if (!string.IsNullOrWhiteSpace(token))
                HttpContext.Session.SetString("token", token);

            // Redirección según rol
            if (roles.Contains("ADMIN"))
                return RedirectToAction("Index", "Admin");

            return RedirectToAction("Index", "Votacion");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }
    }
}
