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

            var perfil = await _auth.ObtenerPerfilAsync(vm.Cedula.Trim());
            if (perfil == null)
            {
                ModelState.AddModelError("", "La cédula no existe en el padrón.");
                return View(vm);
            }

            // Mostrar datos para confirmar/editar correo
            return View("Perfil", perfil);
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

            HttpContext.Session.SetString("cedula", vm.Cedula.Trim());
            HttpContext.Session.SetString("token", token ?? "");
            HttpContext.Session.SetString("roles", string.Join(",", roles));

            // ✅ si tiene varios roles, elige
            if (roles.Count > 1)
                return View("ElegirRol", roles);

            return RedirigirPorRol(roles[0]);
        }

        private IActionResult RedirigirPorRol(string rol)
        {
            rol = rol.ToUpperInvariant();
            if (rol == "ADMIN") return RedirectToAction("Index", "Admin");
            if (rol == "CANDIDATO") return RedirectToAction("Index", "Candidato"); // si tienes
            return RedirectToAction("Index", "Votacion");
        }
        [HttpPost]
        public IActionResult ElegirRol(string rol)
        {
            HttpContext.Session.SetString("rolActivo", rol);
            return RedirigirPorRol(rol);
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnviarCodigo(PerfilVM vm)
        {
            if (string.IsNullOrWhiteSpace(vm.Cedula))
                return BadRequest();

            // si escribió correo, lo actualizamos
            if (!string.IsNullOrWhiteSpace(vm.Correo))
                await _auth.ActualizarCorreoAsync(vm.Cedula.Trim(), vm.Correo.Trim());

            // ahora sí: pedir OTP
            var ok = await _auth.SolicitarCodigoAsync(vm.Cedula.Trim());
            if (!ok)
            {
                ModelState.AddModelError("", "No se pudo enviar el código (verifica correo).");
                return View("Perfil", vm);
            }

            // guardamos roles disponibles para luego elegir
            TempData["rolesDisponibles"] = string.Join(",", vm.RolesDisponibles ?? new());
            return RedirectToAction(nameof(Verificar), new { cedula = vm.Cedula.Trim() });
        }

    }
}
