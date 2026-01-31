using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VotoMVC_Login.Models.DTOs;
using VotoMVC_Login.Models.ViewModels;
using VotoMVC_Login.Models.ViewModels.Admin;
using VotoMVC_Login.Service;

namespace VotoMVC_Login.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApiService _api;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signIn;
        private readonly RoleManager<IdentityRole> _roleManager;

        private const string SessCedula = "admin_cedula";

        public AdminController(
            ApiService api,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signIn,
            RoleManager<IdentityRole> roleManager)
        {
            _api = api;
            _userManager = userManager;
            _signIn = signIn;
            _roleManager = roleManager;
        }

        // =========================
        // LOGIN (CEDULA -> OTP)
        // =========================
        [HttpGet]
        public IActionResult Login() => View(new AdminLoginVm());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AdminLoginVm vm, CancellationToken ct)
        {
            vm.Cedula = (vm.Cedula ?? "").Trim();
            if (vm.Cedula.Length != 10)
            {
                vm.Error = "Ingresa una cédula válida (10 dígitos).";
                return View(vm);
            }

            // Solicita OTP
            var r = await _api.SolicitarOtpCorreoAsync(vm.Cedula, ct);
            if (!r.Ok)
            {
                vm.Error = r.Error ?? "No se pudo solicitar OTP.";
                return View(vm);
            }

            HttpContext.Session.SetString(SessCedula, vm.Cedula);
            return RedirectToAction(nameof(Otp));
        }

        [HttpGet]
        public IActionResult Otp()
        {
            var cedula = HttpContext.Session.GetString(SessCedula);
            if (string.IsNullOrWhiteSpace(cedula)) return RedirectToAction(nameof(Login));
            return View(new AdminOtpVm { Cedula = cedula });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Otp(AdminOtpVm vm, CancellationToken ct)
        {
            var cedula = HttpContext.Session.GetString(SessCedula);
            if (string.IsNullOrWhiteSpace(cedula)) return RedirectToAction(nameof(Login));

            vm.Cedula = cedula;
            vm.Codigo = (vm.Codigo ?? "").Trim();

            if (vm.Codigo.Length < 4)
            {
                vm.Error = "Código OTP inválido.";
                return View(vm);
            }

            var r = await _api.VerificarOtpAsync(cedula, vm.Codigo, ct);
            if (!r.Ok)
            {
                vm.Error = r.Error ?? "OTP incorrecto.";
                return View(vm);
            }

            // Rol 1 = Admin (según tu enum)
            if (r.Rol != 1)
            {
                vm.Error = "Tu usuario no tiene rol Administrador.";
                return View(vm);
            }

            await LoginIdentityAsync(cedula, "Admin");
            HttpContext.Session.Remove(SessCedula);

            return RedirectToAction(nameof(Panel));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signIn.SignOutAsync();
            return RedirectToAction(nameof(Login));
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

            await _signIn.SignInAsync(user, isPersistent: false);
        }

        // =========================
        // PANEL
        // =========================
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Panel(CancellationToken ct)
        {
            var vm = new AdminPanelVm();

            var proc = await _api.GetProcesoActivoAsync(ct);
            if (proc?.ok == true && proc.data != null)
            {
                vm.Proceso.Nombre = proc.data.nombre;
                vm.Proceso.Tipo = proc.data.estado.ToString(); // si quieres, cámbialo a texto
                vm.Proceso.Inicio = proc.data.inicioUtc?.ToString("dd/MM/yyyy HH:mm") ?? "—";
                vm.Proceso.Cierre = proc.data.finUtc?.ToString("dd/MM/yyyy HH:mm") ?? "—";
                vm.Proceso.Estado = proc.data.estado.ToString();
            }
            else
            {
                vm.Error = proc?.error ?? "No hay proceso activo.";
            }

            return View(vm);
        }

        // =========================
        // PROCESOS
        // =========================
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Procesos(CancellationToken ct)
        {
            var vm = new AdminProcesosVm();

            var proc = await _api.GetProcesoActivoAsync(ct);
            if (proc?.ok == true && proc.data != null)
            {
                vm.Activo = new ProcesoCardVm
                {
                    Nombre = proc.data.nombre,
                    Tipo = proc.data.estado.ToString(),
                    Inicio = proc.data.inicioUtc?.ToString("dd/MM/yyyy HH:mm") ?? "—",
                    Cierre = proc.data.finUtc?.ToString("dd/MM/yyyy HH:mm") ?? "—",
                    Estado = proc.data.estado.ToString()
                };
                vm.Nuevo.Estado = 2;
            }

            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Procesos(AdminProcesosVm vm, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                vm.Error = "Revisa los campos del formulario.";
                return View(vm);
            }

            var dto = new ApiService.ProcesoCreateDto
            {
                Nombre = vm.Nuevo.Nombre,
                Tipo = vm.Nuevo.Tipo,
                Estado = vm.Nuevo.Estado,
                InicioLocal = vm.Nuevo.InicioLocal,
                FinLocal = vm.Nuevo.FinLocal
            };

            var r = await _api.CrearProcesoAsync(dto, ct);
            if (r?.ok != true)
            {
                vm.Error = r?.error ?? "No se pudo crear el proceso.";
                return View(vm);
            }

            vm.Ok = $"Proceso creado. Id={r.data}";
            return RedirectToAction(nameof(Procesos));
        }

        // =========================
        // CANDIDATOS
        // =========================
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Candidatos(CancellationToken ct)
        {
            var vm = new AdminCandidatosVm();

            var proc = await _api.GetProcesoActivoAsync(ct);
            vm.ProcesoElectoralId = proc?.data?.id ?? 0;
            vm.Nuevo.ProcesoElectoralId = vm.ProcesoElectoralId;

            var list = await _api.GetCandidatosAdminAsync(ct) ?? new();
            vm.Lista = list.Select(x => new CandidatoRowVm
            {
                Id = x.Id,
                ProcesoElectoralId = x.ProcesoElectoralId,
                NombreCompleto = x.NombreCompleto,
                Partido = x.Partido,
                Binomio = x.Binomio,
                NumeroLista = x.NumeroLista,
                Activo = x.Activo
            }).ToList();

            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Candidatos(AdminCandidatosVm vm, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                vm.Error = "Revisa los campos del candidato.";
                vm.Lista = (await _api.GetCandidatosAdminAsync(ct) ?? new())
                    .Select(x => new CandidatoRowVm
                    {
                        Id = x.Id,
                        ProcesoElectoralId = x.ProcesoElectoralId,
                        NombreCompleto = x.NombreCompleto,
                        Partido = x.Partido,
                        Binomio = x.Binomio,
                        NumeroLista = x.NumeroLista,
                        Activo = x.Activo
                    }).ToList();
                return View(vm);
            }

            var dto = new ApiService.CandidatoCreateDto
            {
                ProcesoElectoralId = vm.Nuevo.ProcesoElectoralId,
                NombreCompleto = vm.Nuevo.NombreCompleto,
                Partido = vm.Nuevo.Partido,
                Binomio = vm.Nuevo.Binomio,
                NumeroLista = vm.Nuevo.NumeroLista,
                Activo = vm.Nuevo.Activo
            };

            var r = await _api.CrearCandidatoAsync(dto, ct);
            if (!r.Ok)
            {
                TempData["Error"] = "No se pudo crear: " + r.Error;
                return RedirectToAction(nameof(Candidatos));
            }

            TempData["Ok"] = "Candidato creado.";
            return RedirectToAction(nameof(Candidatos));
        }

        // =========================
        // PADRON
        // =========================
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Padron(CancellationToken ct)
        {
            var vm = new AdminPadronVm();

            var list = await _api.GetPadronAsync(ct);
            vm.Lista = list.Select(x => new PadronRowVm
            {
                Cedula = x.Cedula,
                NombreCompleto = x.NombreCompleto,
                Provincia = x.Provincia,
                CodigoPad = x.CodigoPad,
                Estado = x.Estado
            }).ToList();

            vm.Ok = TempData["Ok"] as string;
            vm.Error = TempData["Error"] as string;

            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerarPadDemo(CancellationToken ct)
        {
            var r = await _api.GenerarPadDemoAsync(ct);
            if (!r.Ok) TempData["Error"] = "Error: " + r.Error;
            else TempData["Ok"] = "Códigos generados (demo). " + r.Error;

            return RedirectToAction(nameof(Padron));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> ExportarPadronCsv(CancellationToken ct)
        {
            var list = await _api.GetPadronAsync(ct);

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Cedula,NombreCompleto,Provincia,CodigoPad,Estado");

            foreach (var x in list)
            {
                string esc(string s) => "\"" + (s ?? "").Replace("\"", "\"\"") + "\"";
                sb.AppendLine($"{esc(x.Cedula)},{esc(x.NombreCompleto)},{esc(x.Provincia)},{esc(x.CodigoPad)},{esc(x.Estado)}");
            }

            return File(System.Text.Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "padron.csv");
        }

        // =========================
        // RESULTADOS
        // =========================
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Resultados(CancellationToken ct)
        {
            var data = await _api.GetResultadosNacionalAsync(ct);
            return View(data ?? new ResultadosNacionalResponse());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnviarResultadosCorreo(CancellationToken ct)
        {
            var correo = HttpContext.Session.GetString("correo");

            if (string.IsNullOrWhiteSpace(correo))
            {
                TempData["Error"] = "No se encontró el correo del usuario.";
                return RedirectToAction("Resultados");
            }

            var ok = await _api.EnviarResultadosAlCorreoAsync(correo, ct);

            TempData[ok ? "Ok" : "Error"] = ok
                ? $"Resultados enviados a {correo}"
                : "No se pudo enviar el correo.";

            return RedirectToAction("Resultados");
        }

        [HttpGet]
        public IActionResult LoginCredenciales()
        {
            return View(new AdminLoginPasswordVm());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginCredenciales(AdminLoginPasswordVm vm)
        {
            vm.Email = (vm.Email ?? "").Trim();

            if (!ModelState.IsValid)
                return View(vm);

            var user = await _userManager.FindByEmailAsync(vm.Email);
            if (user == null)
            {
                vm.Error = "Correo no encontrado.";
                return View(vm);
            }

            // SignIn por UserName (Identity trabaja con username)
            var result = await _signIn.PasswordSignInAsync(user.UserName!, vm.Password, isPersistent: false, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                vm.Error = "Contraseña incorrecta.";
                return View(vm);
            }

            // ✅ Seguridad: Solo Admin entra al panel
            if (!await _userManager.IsInRoleAsync(user, "Admin"))
            {
                await _signIn.SignOutAsync();
                vm.Error = "No tienes permisos de Administrador.";
                return View(vm);
            }

            // ✅ Guarda correo para "Enviar resultados al correo"
            HttpContext.Session.SetString("correo", user.Email ?? "");
            HttpContext.Session.SetString("cedula", user.UserName ?? "");

            return RedirectToAction(nameof(Panel));
        }


    }
}
