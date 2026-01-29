using System.Text;
using System.Text.Json;

namespace VotoMVC_Login.Service
{

    public class ApiService
    {
        private readonly IHttpClientFactory _http;
        private readonly IConfiguration _cfg;

        private readonly JsonSerializerOptions _jsonOpts = new() { PropertyNameCaseInsensitive = true };

        public ApiService(IHttpClientFactory http, IConfiguration cfg)
        {
            _http = http;
            _cfg = cfg;
        }

        private HttpClient Client() => _http.CreateClient("Api");

        private string SolicitarOtpPath => _cfg["Api:SolicitarOtpPath"] ?? "api/acceso/solicitar-otp";
        private string VerificarOtpPath => _cfg["Api:VerificarOtpPath"] ?? "api/acceso/verificar-otp";

        public class SolicitarOtpRequest
        {
            public string Cedula { get; set; } = "";
            public int Metodo { get; set; } = 1;
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

        public async Task<SolicitarOtpResponse> SolicitarOtpCorreoAsync(string cedula, CancellationToken ct)
        {
            var req = new SolicitarOtpRequest { Cedula = cedula, Metodo = 1 };
            var json = JsonSerializer.Serialize(req);

            for (int intento = 1; intento <= 3; intento++)
            {
                try
                {
                    // ✅ IMPORTANTÍSIMO: crear el request dentro del intento
                    using var message = new HttpRequestMessage(HttpMethod.Post, SolicitarOtpPath)
                    {
                        Content = new StringContent(json, Encoding.UTF8, "application/json")
                    };

                    using var resp = await Client().SendAsync(message, ct);
                    var raw = await resp.Content.ReadAsStringAsync(ct);

                    try
                    {
                        var data = JsonSerializer.Deserialize<SolicitarOtpResponse>(raw, _jsonOpts);
                        if (data == null)
                            return new SolicitarOtpResponse { Ok = false, Error = $"HTTP {(int)resp.StatusCode} - {raw}" };

                        if (!resp.IsSuccessStatusCode && string.IsNullOrWhiteSpace(data.Error))
                            data.Error = $"HTTP {(int)resp.StatusCode} - {raw}";

                        return data;
                    }
                    catch
                    {
                        return new SolicitarOtpResponse { Ok = false, Error = $"HTTP {(int)resp.StatusCode} - {raw}" };
                    }
                }
                catch (TaskCanceledException) when (intento < 3)
                {
                    // ✅ Render cold start / timeout: esperar y reintentar
                    await Task.Delay(1500 * intento, CancellationToken.None);
                    continue;
                }
                catch (HttpRequestException) when (intento < 3)
                {
                    await Task.Delay(1500 * intento, CancellationToken.None);
                    continue;
                }
            }

            return new SolicitarOtpResponse
            {
                Ok = false,
                Error = "⏳ La API está lenta (Render cold start) o no respondió. Intenta de nuevo."
            };
        }


        public async Task<VerificarOtpResponse> VerificarOtpAsync(string cedula, string codigo, CancellationToken ct)
        {
            var req = new VerificarOtpRequest { Cedula = cedula, Codigo = codigo };

            var json = JsonSerializer.Serialize(req);
            var message = new HttpRequestMessage(HttpMethod.Post, VerificarOtpPath)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            using var resp = await Client().SendAsync(message, ct);
            var raw = await resp.Content.ReadAsStringAsync(ct);

            try
            {
                var data = JsonSerializer.Deserialize<VerificarOtpResponse>(raw, _jsonOpts);
                if (data == null)
                    return new VerificarOtpResponse { Ok = false, Error = $"HTTP {(int)resp.StatusCode} - {raw}" };

                if (!resp.IsSuccessStatusCode && string.IsNullOrWhiteSpace(data.Error))
                    data.Error = $"HTTP {(int)resp.StatusCode} - {raw}";

                return data;
            }
            catch
            {
                return new VerificarOtpResponse { Ok = false, Error = $"HTTP {(int)resp.StatusCode} - {raw}" };
            }
        }



    }
    }
