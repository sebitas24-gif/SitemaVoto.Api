using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace VotoMVC_Login.Controllers
{
    public class BridgeController : Controller
    {
        private readonly IConfiguration _cfg;
        public BridgeController(IConfiguration cfg) => _cfg = cfg;

        public IActionResult IrASistema()
        {
            var cedula = User.Identity?.Name ?? "";
            var rol = User.FindFirst(ClaimTypes.Role)?.Value ?? "Votante";

            var token = Convert.ToBase64String(
                System.Text.Encoding.UTF8.GetBytes($"{cedula}|{rol}|{DateTime.UtcNow:O}")
            );

            var url = $"http://localhost:5004/Acceso/Bridge?token={Uri.EscapeDataString(token)}";
            return Redirect(url);

        }
    }
}
