using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VotoMVC_Login.Models;
using VotoMVC_Login.Service;

namespace VotoMVC_Login.Controllers
{
    public class JefeController : Controller
    {
        private readonly ApiService _api;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private const string SessCedula = "jefe_cedula";

        public JefeController(
            ApiService api,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _api = api;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        // =========================
        // LOGIN JEFE (CEDULA -> OTP)
        // =========================
        [HttpGet]
        public IActionResult Login()
        {
            return View(); // Views/Jefe/Login.cshtml
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string cedula, CancellationToken ct)
        {
            cedula = (cedula ?? "").Trim();

            if (cedula.Length != 10)
            {
                TempData["Error"] = "Ingresa una cédula válida (10 dígitos).";
                return RedirectToAction(nameof(Login));
            }

            // 1) Solicitar OTP al correo (igual que Admin)
            var r = await _api.SolicitarOtpCorreoAsync(cedula, ct);

            if (!r.Ok)
            {
                TempData["Error"] = r.Error ?? "No se pudo solicitar OTP.";
                return RedirectToAction(nameof(Login));
            }

            // 2) Guardar cédula en sesión y mandar a vista OTP
            HttpContext.Session.SetString(SessCedula, cedula);

            return RedirectToAction(nameof(Otp));
        }

        // =========================
        // OTP JEFE
        // =========================
       [HttpGet]
public IActionResult Otp()
{
    var cedula = HttpContext.Session.GetString("jefe_cedula");
    if (string.IsNullOrWhiteSpace(cedula))
        return RedirectToAction(nameof(Login));

    ViewBag.Cedula = cedula;
    return View(); // => Views/Jefe/Otp.cshtml
}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Otp(string codigo, CancellationToken ct)
        {
            var cedula = HttpContext.Session.GetString(SessCedula);
            if (string.IsNullOrWhiteSpace(cedula))
                return RedirectToAction(nameof(Login));

            codigo = (codigo ?? "").Trim();

            if (codigo.Length < 4)
            {
                TempData["ErrorOtp"] = "Código OTP inválido.";
                return RedirectToAction(nameof(Otp));
            }

            // 3) Verificar OTP
            var r = await _api.VerificarOtpAsync(cedula, codigo, ct);

            if (!r.Ok)
            {
                TempData["ErrorOtp"] = r.Error ?? "OTP incorrecto.";
                return RedirectToAction(nameof(Otp));
            }

            // 4) Validar que sea Jefe (Rol 2)
            if (r.Rol != 2)
            {
                TempData["ErrorOtp"] = "Esta cédula no corresponde a un Jefe de Junta.";
                return RedirectToAction(nameof(Login));
            }

            // 5) Loguear Identity con rol JefeJunta
            await LoginIdentityAsync(cedula, "JefeJunta");

            // 6) Limpiar la sesión OTP
            HttpContext.Session.Remove(SessCedula);

            return RedirectToAction(nameof(Panel));
        }

        // =========================
        // PANEL (solo Jefe)
        // =========================
        [Authorize(Roles = "JefeJunta")]
        [HttpGet]
        public IActionResult Panel()
        {
            return View(new VotoMVC_Login.Models.JefePanelVm());
        }

        [Authorize(Roles = "JefeJunta")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Panel(string cedulaBuscada, CancellationToken ct)
        {
            var vm = new JefePanelVm { CedulaBuscada = cedulaBuscada };

            if (string.IsNullOrWhiteSpace(cedulaBuscada) || cedulaBuscada.Length != 10)
            {
                vm.Error = "Cédula inválida.";
                return View(vm);
            }

            try
            {
                var data = await _api.GetPadronPorCedulaAsync(cedulaBuscada, ct);

                if (data == null)
                {
                    vm.Error = "No existe en padrón.";
                    return View(vm);
                }

                vm.Ciudadano = data;
                vm.Msg = "Ciudadano encontrado.";
                return View(vm);
            }
            catch (Exception ex)
            {
                vm.Error = "Error consultando API: " + ex.Message;
                return View(vm);
            }
        }

        // =========================
        // LOGOUT JEFE
        // =========================
        [Authorize(Roles = "JefeJunta")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }

        // =========================
        // HELPER Identity
        // =========================
        private async Task LoginIdentityAsync(string cedula, string rol)
        {
            if (!await _roleManager.RoleExistsAsync(rol))
                await _roleManager.CreateAsync(new IdentityRole(rol));

            var user = await _userManager.FindByNameAsync(cedula);

            if (user == null)
            {
                user = new IdentityUser { UserName = cedula };
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
