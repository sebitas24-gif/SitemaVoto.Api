using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VotoMVC_Login.Models;
using VotoMVC_Login.Service;

namespace VotoMVC_Login.Controllers
{
    [Authorize(Roles = "Jefe")]
    public class JefeController : Controller
    {
        private readonly ApiService _api;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

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

        // ✅ Vista 1
        [HttpGet]
        public IActionResult Login()
        {
            return View(); // Views/Jefe/Login.cshtml
        }

        // ✅ Valida cédula y si es Jefe -> pasa al Panel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string cedula, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(cedula) || cedula.Trim().Length != 10)
            {
                TempData["Error"] = "Ingresa una cédula válida (10 dígitos).";
                return RedirectToAction(nameof(Login));
            }

            cedula = cedula.Trim();

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(60));

            var r = await _api.GetRolPorCedulaAsync(cedula, cts.Token);

            if (!r.Ok)
            {
                TempData["Error"] = r.Error ?? "No se pudo validar la cédula.";
                return RedirectToAction(nameof(Login));
            }

            // ✅ Rol 2 = JefeJunta (según tu enum)
            if (r.Rol != 2)
            {
                TempData["Error"] = "Esta cédula no corresponde a un Jefe de Junta.";
                return RedirectToAction(nameof(Login));
            }

            // ✅ Loguear en Identity para usar [Authorize]
            await LoginIdentityAsync(cedula, "JefeJunta");

            // ✅ Pasa a la otra vista
            return RedirectToAction(nameof(Panel));
        }

        // ✅ Vista 2 (Panel del Jefe)
        [Authorize(Roles = "JefeJunta")]
        [HttpGet]
        public IActionResult Panel()
        {
            return View(new VotoMVC_Login.Models.JefePanelVm());
        }

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


    }
}
