using System.Text.Json;

namespace VotoMVC_Login.Service
{

    public class ApiService
    {
        private readonly IHttpClientFactory _http;
        private readonly JsonSerializerOptions _jsonOpts = new() { PropertyNameCaseInsensitive = true };

        public ApiService(IHttpClientFactory http) => _http = http;

        private HttpClient Client() => _http.CreateClient("Api");

        public class SolicitarOtpRequest
        {
            public string Cedula { get; set; } = "";
            public int Metodo { get; set; } = 1; // 1=Correo
        }

        public class SolicitarOtpResponse
        {
            public bool Ok { get; set; }
            public string? Error { get; set; }
            public string? Destino { get; set; }
            public string? DestinoMasked { get; set; }
            public string? Nota { get; set; }
        }

        public class VerificarOtpRequest
        {
            public string Cedula { get; set; } = "";
            public string Codigo { get; set; } = "";
        }

        public class VerificarOtpResponse
        {
            public bool Ok { get; set; }
            public string? Error { get; set; }
            public int Rol { get; set; }
        }

        public async Task<SolicitarOtpResponse?> SolicitarOtpCorreoAsync(string cedula, CancellationToken ct)
        {
            var req = new SolicitarOtpRequest { Cedula = cedula, Metodo = 1 };
            var resp = await Client().PostAsJsonAsync("api/acceso/solicitar-otp", req, ct);
            return await resp.Content.ReadFromJsonAsync<SolicitarOtpResponse>(_jsonOpts, ct);
        }

        public async Task<VerificarOtpResponse?> VerificarOtpAsync(string cedula, string codigo, CancellationToken ct)
        {
            var req = new VerificarOtpRequest { Cedula = cedula, Codigo = codigo };
            var resp = await Client().PostAsJsonAsync("api/acceso/verificar-otp", req, ct);
            return await resp.Content.ReadFromJsonAsync<VerificarOtpResponse>(_jsonOpts, ct);
        }
    }
    }
