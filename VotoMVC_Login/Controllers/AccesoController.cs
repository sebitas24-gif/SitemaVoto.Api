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
        private readonly IConfiguration _cfg;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccesoController(
     ApiService api,
     UserManager<IdentityUser> userManager,
     SignInManager<IdentityUser> signInManager,
     RoleManager<IdentityRole> roleManager,
     IConfiguration cfg)
        {
            _api = api;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _cfg = cfg;
        }


        [HttpGet]
        public IActionResult Index() => View();

        // Para evitar 404 si tu menú llama /Acceso/JefeLogin
        [HttpGet]
        public IActionResult JefeLogin() => RedirectToAction(nameof(JefeCedula));

        [HttpGet]
        public IActionResult AdminLogin() => RedirectToAction(nameof(AdminCedula));

        // =========================
        // JEFE
        // =========================
        [HttpGet]
        public IActionResult JefeCedula()
        {
            ViewBag.Error = TempData["Error"];
            ViewBag.Msg = TempData["Msg"];
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JefeCedula(string cedula, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(cedula) || cedula.Trim().Length != 10)
            {
                TempData["Error"] = "Ingresa una cédula válida (10 dígitos).";
                return RedirectToAction(nameof(JefeCedula));
            }

            cedula = cedula.Trim();

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(60)); // solo validar rol, no debe tardar

            try
            {
                var r = await _api.GetRolPorCedulaAsync(cedula, cts.Token);

                if (r.Ok != true)
                {
                    TempData["Error"] = r.Error ?? "No se pudo validar la cédula.";
                    return RedirectToAction(nameof(JefeCedula));
                }

                // ✅ Solo entra si es JefeJunta = 2
                if (r.Rol != 2)
                {
                    TempData["Error"] = "Esta cédula no corresponde a un Jefe de Junta.";
                    return RedirectToAction(nameof(JefeCedula));
                }

                // ✅ Login Identity directo
                await LoginIdentityAsync(cedula, "JefeJunta");

                // ✅ Redirigir al panel (según tu config)
                var url = _cfg["Redirects:JefeUrl"] ?? "http://localhost:5004/JefeJunta/Panel";
                return Redirect(url);
            }
            catch (TaskCanceledException)
            {
                TempData["Error"] = "⏳ La API no respondió a tiempo (timeout).";
                return RedirectToAction(nameof(JefeCedula));
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = "❌ Error conectando con la API: " + ex.Message;
                return RedirectToAction(nameof(JefeCedula));
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminCedula(string cedula, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(cedula) || cedula.Trim().Length != 10)
            {
                TempData["Error"] = "Ingresa una cédula válida (10 dígitos).";
                return RedirectToAction(nameof(AdminCedula));
            }

            cedula = cedula.Trim();

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(180));

            try
            {
                var r = await _api.SolicitarOtpCorreoAsync(cedula, cts.Token);

                if (r.Ok != true)
                {
                    TempData["Error"] = r.Error ?? "No se pudo enviar OTP.";
                    return RedirectToAction(nameof(AdminCedula));
                }

                HttpContext.Session.SetString("cedula_admin", cedula);
                TempData["Msg"] = $"✅ OTP enviado a: {r.DestinoMasked ?? r.Destino}";
                return RedirectToAction(nameof(AdminOtp));
            }
            catch (TaskCanceledException)
            {
                TempData["Error"] = "⏳ La API no respondió a tiempo (timeout). Abre Swagger una vez y reintenta.";
                return RedirectToAction(nameof(AdminCedula));
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = "❌ Error conectando con la API: " + ex.Message;
                return RedirectToAction(nameof(AdminCedula));
            }
        }


        [HttpGet]
        public IActionResult JefeOtp()
        {
            var cedula = HttpContext.Session.GetString("cedula_jefe");
            if (string.IsNullOrWhiteSpace(cedula))
                return RedirectToAction(nameof(JefeCedula));

            ViewBag.Error = TempData["Error"];
            ViewBag.Msg = TempData["Msg"];
            ViewBag.Cedula = cedula;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
            if (r.Ok != true)
            {
                TempData["Error"] = r.Error ?? "OTP inválido.";
                return RedirectToAction(nameof(JefeOtp));
            }

            if (r.Rol != 2)
            {
                TempData["Error"] = "Esta cédula no corresponde a Jefe de Junta.";
                return RedirectToAction(nameof(JefeOtp));
            }

            await LoginIdentityAsync(cedula, "JefeJunta");
            HttpContext.Session.Remove("cedula_jefe");

            var url = _cfg["Redirects:JefeUrl"] ?? "http://localhost:5004/JefeJunta/Panel";
            return Redirect(url);
        }

        // =========================
        // ADMIN
        // =========================
        [HttpGet]
        public IActionResult AdminCedula()
        {
            ViewBag.Error = TempData["Error"];
            ViewBag.Msg = TempData["Msg"];
            return View();
        }

    

        [HttpGet]
        public IActionResult AdminOtp()
        {
            var cedula = HttpContext.Session.GetString("cedula_admin");
            if (string.IsNullOrWhiteSpace(cedula))
                return RedirectToAction(nameof(AdminCedula));

            ViewBag.Error = TempData["Error"];
            ViewBag.Msg = TempData["Msg"];
            ViewBag.Cedula = cedula;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
            if (r.Ok != true)
            {
                TempData["Error"] = r.Error ?? "OTP inválido.";
                return RedirectToAction(nameof(AdminOtp));
            }

            if (r.Rol != 1)
            {
                TempData["Error"] = "Esta cédula no corresponde a Administrador.";
                return RedirectToAction(nameof(AdminOtp));
            }

            await LoginIdentityAsync(cedula, "Admin");
            HttpContext.Session.Remove("cedula_admin");

            var url = _cfg["Redirects:AdminUrl"] ?? "http://localhost:5004/Admin/Index";
            return Redirect(url);
        }

        // =========================
        // Logout
        // =========================
        public async Task<IActionResult> Salir()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // Helper Identity
        // =========================
        private async Task LoginIdentityAsync(string cedula, string rol)
        {
            // crear rol si no existe
            if (!await _roleManager.RoleExistsAsync(rol))
                await _roleManager.CreateAsync(new IdentityRole(rol));

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
            {
                var addRole = await _userManager.AddToRoleAsync(user, rol);
                if (!addRole.Succeeded)
                    throw new Exception(string.Join(" | ", addRole.Errors.Select(e => e.Description)));
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
        }

        [HttpGet]
        public async Task<IActionResult> Ping(CancellationToken ct)
        {
            try
            {
                var client = HttpContext.RequestServices
                    .GetRequiredService<IHttpClientFactory>()
                    .CreateClient("Api");

                using var resp = await client.GetAsync("swagger/index.html", ct);
                var txt = await resp.Content.ReadAsStringAsync(ct);

                return Content($"Status={(int)resp.StatusCode} {resp.ReasonPhrase}\nLen={txt.Length}");
            }
            catch (Exception ex)
            {
                return Content("Ping ERROR: " + ex.Message);
            }
        }

     

        [HttpGet]
        public IActionResult VotanteCedula() => View();
    }
}
