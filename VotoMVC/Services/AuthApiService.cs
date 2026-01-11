using System.Text.Json;
using System.Text;
using VotoMVC.ViewModelos;

namespace VotoMVC.Services
{
    public class AuthApiService
    {
        private readonly HttpClient _http;
        private readonly string _baseUrl;

        public AuthApiService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _baseUrl = config["ApiSettings:BaseUrl"]!;
        }

        public async Task<bool> SolicitarCodigoAsync(string cedula)
        {
            var res = await _http.PostAsJsonAsync($"{_baseUrl}/api/auth/solicitar-codigo",
                new { Cedula = cedula });

            return res.IsSuccessStatusCode;
        }

        public async Task<(bool ok, List<string> roles, string? token)> VerificarCodigoAsync(string cedula, string codigo)
        {
            var res = await _http.PostAsJsonAsync($"{_baseUrl}/api/auth/verificar-codigo",
                new { Cedula = cedula, Codigo = codigo });

            if (!res.IsSuccessStatusCode)
                return (false, new List<string>(), null);

            // tu API devuelve { roles = [...] }
            var json = await res.Content.ReadFromJsonAsync<dynamic>();
            var roles = new List<string>();

            if (json?.roles != null)
            {
                foreach (var r in json.roles)
                    roles.Add((string)r);
            }

            // si luego metes JWT, aquí lo recibes. Por ahora null.
            return (true, roles, null);
        }

        // ✅ NUEVO: traer perfil por cédula
        public async Task<PerfilVM?> ObtenerPerfilAsync(string cedula)
        {
            var res = await _http.GetAsync($"{_baseUrl}/api/auth/perfil/{cedula}");
            if (!res.IsSuccessStatusCode) return null;

            return await res.Content.ReadFromJsonAsync<PerfilVM>();
        }

        // ✅ NUEVO: actualizar correo
        public async Task<bool> ActualizarCorreoAsync(string cedula, string correo)
        {
            var res = await _http.PutAsJsonAsync($"{_baseUrl}/api/auth/correo",
                new { Cedula = cedula, Correo = correo });

            return res.IsSuccessStatusCode;
        }
    }
}
