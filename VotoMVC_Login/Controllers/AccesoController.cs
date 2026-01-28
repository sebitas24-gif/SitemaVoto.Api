using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VotoMVC_Login.Service;

namespace VotoMVC_Login.Controllers
{
    public class AccesoController : Controller
    {
        private readonly ApiService _api;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccesoController(ApiService api, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _api = api;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Index() => View();

        // =========================
        // VOTANTE (solo PAD, NO OTP)
        // =========================
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

            // TODO: aquí deberías validar PAD contra la API si ya tienes endpoint.
            await LoginIdentityAsync(cedula.Trim(), "Votante");

            return Redirect("http://localhost:5004/Votante/Papeleta");
        }

        // =========================
        // JEFE (OTP real por correo)
        // =========================
        [HttpGet]
        public IActionResult JefeCedula() => View();

        [HttpPost]
        public async Task<IActionResult> JefeCedula(string cedula, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(cedula) || cedula.Trim().Length != 10)
            {
                TempData["Error"] = "Ingresa una cédula válida (10 dígitos).";
                return RedirectToAction(nameof(JefeCedula));
            }

            cedula = cedula.Trim();

            try
            {
                var r = await _api.SolicitarOtpCorreoAsync(cedula, ct);

                if (r?.Ok != true)
                {
                    TempData["Error"] = r?.Error ?? "No se pudo enviar OTP.";
                    return RedirectToAction(nameof(JefeCedula));
                }

                // ✅ guardamos para el siguiente paso
                HttpContext.Session.SetString("cedula_jefe", cedula);
                TempData["Msg"] = $"✅ OTP enviado a: {r.DestinoMasked ?? r.Destino}";

                return RedirectToAction(nameof(JefeOtp));
            }
            catch (TaskCanceledException)
            {
                TempData["Error"] = "⏳ La API tardó demasiado (Render puede estar despertando). Intenta otra vez.";
                return RedirectToAction(nameof(JefeCedula));
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = "❌ No se pudo conectar con la API. Revisa Api:BaseUrl. " + ex.Message;
                return RedirectToAction(nameof(JefeCedula));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "❌ Error inesperado: " + ex.Message;
                return RedirectToAction(nameof(JefeCedula));
            }
        }

        [HttpGet]
        public IActionResult JefeOtp()
        {
            var cedula = HttpContext.Session.GetString("cedula_jefe");
            if (string.IsNullOrWhiteSpace(cedula))
                return RedirectToAction(nameof(JefeCedula));

            ViewBag.Msg = TempData["Msg"];
            ViewBag.Error = TempData["Error"];
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> JefeOtp(string codigo, CancellationToken ct)
        {
            var cedula = HttpContext.Session.GetString("cedula_jefe");
            if (string.IsNullOrWhiteSpace(cedula))
                return RedirectToAction(nameof(JefeCedula));

            if (string.IsNullOrWhiteSpace(codigo))
            {
                TempData["Error"] = "Ingresa el OTP.";
                return RedirectToAction(nameof(JefeOtp));
            }

            var r = await _api.VerificarOtpAsync(cedula, codigo.Trim(), ct);
            if (r?.Ok != true)
            {
                TempData["Error"] = r?.Error ?? "OTP inválido.";
                return RedirectToAction(nameof(JefeOtp));
            }

            if (r.Rol != 2)
            {
                TempData["Error"] = "Esta cédula no corresponde a Jefe de Junta.";
                return RedirectToAction(nameof(JefeOtp));
            }

            await LoginIdentityAsync(cedula, "JefeJunta");
            HttpContext.Session.Remove("cedula_jefe");

            return Redirect("http://localhost:5004/JefeJunta/Panel");
        }

        // =========================
        // ADMIN (OTP real por correo)
        // =========================
        [HttpGet]
        public IActionResult AdminCedula() => View();

        [HttpPost]
        public async Task<IActionResult> AdminCedula(string cedula, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(cedula) || cedula.Trim().Length != 10)
            {
                TempData["Error"] = "Ingresa una cédula válida (10 dígitos).";
                return RedirectToAction(nameof(AdminCedula));
            }

            cedula = cedula.Trim();

            var r = await _api.SolicitarOtpCorreoAsync(cedula, ct);
            if (r?.Ok != true)
            {
                TempData["Error"] = r?.Error ?? "No se pudo enviar OTP.";
                return RedirectToAction(nameof(AdminCedula));
            }

            HttpContext.Session.SetString("cedula_admin", cedula);
            TempData["Msg"] = $"✅ OTP enviado a: {r.DestinoMasked ?? r.Destino}";
            return RedirectToAction(nameof(AdminOtp));
        }

        [HttpGet]
        public IActionResult AdminOtp()
        {
            var cedula = HttpContext.Session.GetString("cedula_admin");
            if (string.IsNullOrWhiteSpace(cedula))
                return RedirectToAction(nameof(AdminCedula));

            ViewBag.Msg = TempData["Msg"];
            ViewBag.Error = TempData["Error"];
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AdminOtp(string codigo, CancellationToken ct)
        {
            var cedula = HttpContext.Session.GetString("cedula_admin");
            if (string.IsNullOrWhiteSpace(cedula))
                return RedirectToAction(nameof(AdminCedula));

            if (string.IsNullOrWhiteSpace(codigo))
            {
                TempData["Error"] = "Ingresa el OTP.";
                return RedirectToAction(nameof(AdminOtp));
            }

            var r = await _api.VerificarOtpAsync(cedula, codigo.Trim(), ct);
            if (r?.Ok != true)
            {
                TempData["Error"] = r?.Error ?? "OTP inválido.";
                return RedirectToAction(nameof(AdminOtp));
            }

            if (r.Rol != 1)
            {
                TempData["Error"] = "Esta cédula no corresponde a Administrador.";
                return RedirectToAction(nameof(AdminOtp));
            }

            await LoginIdentityAsync(cedula, "Admin");
            HttpContext.Session.Remove("cedula_admin");

            return Redirect("http://localhost:5004/Admin/Index");
        }

        // Logout
        public async Task<IActionResult> Salir()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Index));
        }

        // ✅ Menú: botones
        [HttpGet] public IActionResult Resultados() => Redirect("http://localhost:5004/Resultados?tab=live");
        [HttpGet] public IActionResult VotanteLogin() => RedirectToAction(nameof(Votante));
        [HttpGet] public IActionResult JefeLogin() => RedirectToAction(nameof(JefeCedula));
        [HttpGet] public IActionResult AdminLogin() => RedirectToAction(nameof(AdminCedula));

        // Helper Identity
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
