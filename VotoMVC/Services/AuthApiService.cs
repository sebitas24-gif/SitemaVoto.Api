using System.Text.Json;
using System.Text;

namespace VotoMVC.Services
{
    public class AuthApiService
    {
        private readonly HttpClient _http;
        private readonly string _baseUrl;

        public AuthApiService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _baseUrl = config["ApiSettings:BaseUrl"]!.TrimEnd('/');
        }

        public async Task<bool> SolicitarCodigoAsync(string cedula)
        {
            var payload = JsonSerializer.Serialize(new { cedula });
            var resp = await _http.PostAsync(
                $"{_baseUrl}/api/auth/solicitar-codigo",
                new StringContent(payload, Encoding.UTF8, "application/json"));

            return resp.IsSuccessStatusCode;
        }

        public async Task<(bool ok, List<string> roles, string? token)> VerificarCodigoAsync(string cedula, string codigo)
        {
            var payload = JsonSerializer.Serialize(new { cedula, codigo });
            var resp = await _http.PostAsync(
                $"{_baseUrl}/api/auth/verificar-codigo",
                new StringContent(payload, Encoding.UTF8, "application/json"));

            if (!resp.IsSuccessStatusCode)
                return (false, new List<string>(), null);

            var json = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            // ✅ tu API ya devuelve roles (según lo que hicimos)
            var roles = new List<string>();
            if (doc.RootElement.TryGetProperty("roles", out var rolesEl) && rolesEl.ValueKind == JsonValueKind.Array)
            {
                roles = rolesEl.EnumerateArray()
                    .Select(x => x.GetString() ?? "")
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToList();
            }

            // ✅ opcional: si tu API devuelve token, lo guardamos
            string? token = null;
            if (doc.RootElement.TryGetProperty("token", out var tokenEl))
                token = tokenEl.GetString();

            return (true, roles, token);
        }
    }
}
