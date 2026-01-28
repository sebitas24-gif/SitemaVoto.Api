using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace VotoMVC_Login.Controllers
{
    public class AccesoController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccesoController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Menú
        [HttpGet]
        public IActionResult Index() => View();

        // ----------------------------
        // VOTANTE (1 paso): cédula + PAD
        // ----------------------------
        [HttpGet]
        public IActionResult Votante() => View();

        [HttpPost]
        public async Task<IActionResult> Votante(string cedula, string pad)
        {
            if (string.IsNullOrWhiteSpace(cedula) || string.IsNullOrWhiteSpace(pad))
            {
                ViewBag.Error = "Ingresa cédula y código PAD.";
                return View();
            }

            // TODO: aquí llamas tu API real:
            // POST api/Padron/validar { cedula, codigoPad }
            var ok = true; // <- reemplazar por resultado real
            if (!ok)
            {
                ViewBag.Error = "PAD inválido.";
                return View();
            }

            await LoginIdentityAsync(cedula, "Votante");
            return Redirect("http://localhost:5004/Acceso/Votante");
        }

        // =========================================================
        // JEFE (2 pasos): 1) cédula  2) OTP
        // =========================================================

        [HttpGet]
        public IActionResult JefeCedula() => View();

        [HttpPost]
        public IActionResult JefeCedula(string cedula)
        {
            if (string.IsNullOrWhiteSpace(cedula))
            {
                ViewBag.Error = "Ingresa la cédula.";
                return View();
            }

            // TODO: Validar que esa cédula sea JEFE en tu API
            // y enviar OTP al correo/SMS (endpoint enviar-otp).
            // Aquí dejamos demo:
            var existe = true; // <- reemplazar
            if (!existe)
            {
                ViewBag.Error = "Cédula no encontrada o no es Jefe de Junta.";
                return View();
            }

            // Guardamos para el paso 2
            TempData["cedula_jefe"] = cedula;

            // (Demo) “enviar OTP”
            TempData["msg"] = "Se envió el código OTP (demo: 123456).";

            return RedirectToAction(nameof(JefeOtp));
        }

        [HttpGet]
        public IActionResult JefeOtp()
        {
            if (TempData["cedula_jefe"] == null && TempData.Peek("cedula_jefe") == null)
                return RedirectToAction(nameof(JefeCedula));

            ViewBag.Msg = TempData["msg"];
            TempData.Keep("cedula_jefe");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> JefeOtp(string otp)
        {
            var cedula = TempData.Peek("cedula_jefe")?.ToString();
            if (string.IsNullOrWhiteSpace(cedula))
                return RedirectToAction(nameof(JefeCedula));

            if (string.IsNullOrWhiteSpace(otp))
            {
                ViewBag.Error = "Ingresa el OTP.";
                TempData.Keep("cedula_jefe");
                return View();
            }

            // TODO: Validar OTP real en tu API
            // POST api/otp/validar { cedula, otp }
            var ok = (otp == "123456"); // demo
            if (!ok)
            {
                ViewBag.Error = "OTP inválido.";
                TempData.Keep("cedula_jefe");
                return View();
            }

            // OTP válido → Login + rol
            await LoginIdentityAsync(cedula, "JefeJunta");

            TempData.Remove("cedula_jefe");
            return Redirect("http://localhost:5004/Acceso/Jefe");
        }

        // =========================================================
        // ADMIN (2 pasos): 1) cédula  2) OTP
        // =========================================================

        [HttpGet]
        public IActionResult AdminCedula() => View();

        [HttpPost]
        public IActionResult AdminCedula(string cedula)
        {
            if (string.IsNullOrWhiteSpace(cedula))
            {
                ViewBag.Error = "Ingresa la cédula.";
                return View();
            }

            // TODO: Validar que esa cédula sea ADMIN en tu API
            // y enviar OTP al correo/SMS.
            var existe = true; // <- reemplazar
            if (!existe)
            {
                ViewBag.Error = "Cédula no encontrada o no es Administrador.";
                return View();
            }

            TempData["cedula_admin"] = cedula;
            TempData["msg"] = "Se envió el código OTP (demo: 123456).";

            return RedirectToAction(nameof(AdminOtp));
        }

        [HttpGet]
        public IActionResult AdminOtp()
        {
            if (TempData["cedula_admin"] == null && TempData.Peek("cedula_admin") == null)
                return RedirectToAction(nameof(AdminCedula));

            ViewBag.Msg = TempData["msg"];
            TempData.Keep("cedula_admin");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AdminOtp(string otp)
        {
            var cedula = TempData.Peek("cedula_admin")?.ToString();
            if (string.IsNullOrWhiteSpace(cedula))
                return RedirectToAction(nameof(AdminCedula));

            if (string.IsNullOrWhiteSpace(otp))
            {
                ViewBag.Error = "Ingresa el OTP.";
                TempData.Keep("cedula_admin");
                return View();
            }

            // TODO: Validar OTP real en tu API
            var ok = (otp == "123456"); // demo
            if (!ok)
            {
                ViewBag.Error = "OTP inválido.";
                TempData.Keep("cedula_admin");
                return View();
            }

            await LoginIdentityAsync(cedula, "Admin");

            TempData.Remove("cedula_admin");
            return Redirect("http://localhost:5004/Acceso/Admin");
        }

        // Logout
        public async Task<IActionResult> Salir()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Index));
        }

        // Helper: crea usuario Identity y lo loguea sin contraseña
        private async Task LoginIdentityAsync(string cedula, string rol)
        {
            var user = await _userManager.FindByNameAsync(cedula);

            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = cedula,
                    Email = $"{cedula}@demo.local"
                };

                var tmpPass = "Tmp@" + Guid.NewGuid().ToString("N") + "1a";
                var create = await _userManager.CreateAsync(user, tmpPass);

                if (!create.Succeeded)
                    throw new Exception(string.Join(" | ", create.Errors.Select(e => e.Description)));
            }

            if (!await _userManager.IsInRoleAsync(user, rol))
                await _userManager.AddToRoleAsync(user, rol);

            await _signInManager.SignInAsync(user, isPersistent: false);
        }
    }
}
