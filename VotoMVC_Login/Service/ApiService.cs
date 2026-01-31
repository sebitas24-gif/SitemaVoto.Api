
using System.Text;
using System.Text.Json;
using VotoMVC_Login.Models;
using VotoMVC_Login.Models.DTOs;

namespace VotoMVC_Login.Service
{

    public class ApiService
    {
        private readonly IHttpClientFactory _http;
        private readonly IConfiguration _cfg;

        private readonly JsonSerializerOptions _jsonOpts = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public ApiService(IHttpClientFactory http, IConfiguration cfg)
        {
            _http = http;
            _cfg = cfg;
        }

        private HttpClient Client() => _http.CreateClient("Api");

        // Rutas configurables (o defaults)
        private string SolicitarOtpPath => _cfg["Api:SolicitarOtpPath"] ?? "api/acceso/solicitar-otp";
        private string VerificarOtpPath => _cfg["Api:VerificarOtpPath"] ?? "api/acceso/verificar-otp";

        // ============================
        // DTOs OTP
        // ============================
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
        public class EmitirVotoDto
        {
            public string Cedula { get; set; } = "";
            public string CodigoPad { get; set; } = "";
            public int CandidatoId { get; set; } = 0;
        }
        public class EmitirVotoResultDto
        {
            public bool Ok { get; set; }
            public string? Error { get; set; }
            public string? Comprobante { get; set; }
        }
        public async Task<EmitirVotoResultDto?> EmitirVotoAsync(EmitirVotoDto dto, CancellationToken ct = default)
        {
            using var resp = await Client().PostAsJsonAsync("api/Votacion/emitir", dto, ct);
            return await resp.Content.ReadFromJsonAsync<EmitirVotoResultDto>(_jsonOpts, ct);
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

        // ============================
        // OTP - Solicitar
        // ============================
        public async Task<SolicitarOtpResponse> SolicitarOtpCorreoAsync(string cedula, CancellationToken ct)
        {
            var req = new SolicitarOtpRequest { Cedula = cedula, Metodo = 1 };
            var json = JsonSerializer.Serialize(req);

            for (int intento = 1; intento <= 3; intento++)
            {
                try
                {
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
                    await Task.Delay(1500 * intento, CancellationToken.None);
                }
                catch (HttpRequestException) when (intento < 3)
                {
                    await Task.Delay(1500 * intento, CancellationToken.None);
                }
            }

            return new SolicitarOtpResponse
            {
                Ok = false,
                Error = "⏳ La API está lenta (Render cold start) o no respondió. Intenta de nuevo."
            };
        }

        // ============================
        // OTP - Verificar
        // ============================
        public async Task<VerificarOtpResponse> VerificarOtpAsync(string cedula, string codigo, CancellationToken ct)
        {
            var req = new VerificarOtpRequest { Cedula = cedula, Codigo = codigo };

            var json = JsonSerializer.Serialize(req);
            using var message = new HttpRequestMessage(HttpMethod.Post, VerificarOtpPath)
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

        // ============================
        // Rol por Cédula
        // ============================
        public class RolPorCedulaResponse
        {
            public bool Ok { get; set; }
            public string? Error { get; set; }
            public int Rol { get; set; }
        }

        public async Task<RolPorCedulaResponse> GetRolPorCedulaAsync(string cedula, CancellationToken ct)
        {
            using var resp = await Client().GetAsync($"api/acceso/rol/{cedula}", ct);
            var raw = await resp.Content.ReadAsStringAsync(ct);

            try
            {
                var data = JsonSerializer.Deserialize<RolPorCedulaResponse>(raw, _jsonOpts);
                if (data == null)
                    return new RolPorCedulaResponse { Ok = false, Error = $"HTTP {(int)resp.StatusCode} - {raw}" };

                if (!resp.IsSuccessStatusCode && string.IsNullOrWhiteSpace(data.Error))
                    data.Error = $"HTTP {(int)resp.StatusCode} - {raw}";

                return data;
            }
            catch
            {
                return new RolPorCedulaResponse { Ok = false, Error = $"HTTP {(int)resp.StatusCode} - {raw}" };
            }
        }

        // ============================
        // Wrapper opcional (si lo usas)
        // ============================
        public class ApiWrap<T>
        {
            public bool ok { get; set; }
            public string? error { get; set; }
            public T? data { get; set; }
        }

        // ============================
        // DTO padrón (lo que te devuelve GET /api/Padron/cedula/{cedula})
        // ============================
        public class CiudadanoDto
        {
            public string cedula { get; set; } = "";
            public string nombres { get; set; } = "";
            public string apellidos { get; set; } = "";
            public string? correo { get; set; }
            public string? telefono { get; set; }
            public int rol { get; set; }
            public string provincia { get; set; } = "";
            public string canton { get; set; } = "";
            public string? parroquia { get; set; }
            public string? mesa { get; set; }
            public string? codigoPad { get; set; }
        }

        // Si usas este endpoint también (api/acceso/ciudadano/{cedula}) con wrap ok/error/data:
        public async Task<(bool Ok, string? Error, CiudadanoDto? Data)> GetCiudadanoAsync(string cedula, CancellationToken ct)
        {
            var resp = await Client().GetAsync($"api/acceso/ciudadano/{cedula}", ct);
            var raw = await resp.Content.ReadAsStringAsync(ct);

            try
            {
                var wrap = JsonSerializer.Deserialize<ApiWrap<CiudadanoDto>>(raw, _jsonOpts);

                if (wrap == null)
                    return (false, $"HTTP {(int)resp.StatusCode} - {raw}", null);

                if (!resp.IsSuccessStatusCode || !wrap.ok)
                    return (false, wrap.error ?? $"HTTP {(int)resp.StatusCode} - {raw}", null);

                return (true, null, wrap.data);
            }
            catch
            {
                return (false, $"HTTP {(int)resp.StatusCode} - {raw}", null);
            }
        }

        // ============================
        // ✅ GET padrón por cédula (Jefe y Votante)
        // ============================
        public Task<CiudadanoDto?> GetPadronPorCedulaAsync(string cedula, CancellationToken ct)
            => Client().GetFromJsonAsync<CiudadanoDto>($"api/Padron/cedula/{cedula}", _jsonOpts, ct);

        // ============================
        // ✅ Validar PAD usando GET (esto reemplaza el POST que te daba 404)
        // ============================
        public async Task<(bool Ok, string? Error, CiudadanoDto? Data)> ValidarPadConGetAsync(string cedula, string codigoPad, CancellationToken ct)
        {
            try
            {
                var data = await GetPadronPorCedulaAsync(cedula, ct);

                if (data == null)
                    return (false, "No existe en padrón.", null);

                // IMPORTANTE: la propiedad es codigoPad (minúsculas)
                if (string.IsNullOrWhiteSpace(data.codigoPad))
                    return (false, "No existe código PAD para este votante.", data);

                var padApi = LimpiarPad(data.codigoPad);
                var padUser = LimpiarPad(codigoPad);

                if (padApi != padUser)
                    return (false, "Código PAD incorrecto.", data);

                return (true, null, data);
            }
            catch (HttpRequestException ex)
            {
                return (false, "Error HTTP llamando API: " + ex.Message, null);
            }
            catch (TaskCanceledException)
            {
                return (false, "La API no respondió a tiempo (timeout).", null);
            }
            catch (Exception ex)
            {
                return (false, "Error llamando API: " + ex.Message, null);
            }
        }

        private static string LimpiarPad(string? pad)
        {
            return (pad ?? "")
                .Trim()
                .Replace("]", "")
                .Replace("[", "")
                .Replace(" ", "")
                .ToUpperInvariant();
        }
        public class CandidatoDto
        {
            public int Id { get; set; }
            public string Nombre { get; set; } = "";
            public string PartidoPolitico { get; set; } = "";
            public string Dignidad { get; set; } = "";
            public string? FotoUrl { get; set; }
            public bool activo { get; set; } = true;

        }

        // ============================
        // Helper Post genérico (por si lo usas en otros controladores)
        // ============================
        public async Task<TResp?> PostAsync<TReq, TResp>(string path, TReq body, CancellationToken ct)
        {
            using var resp = await Client().PostAsJsonAsync(path, body, ct);
            return await resp.Content.ReadFromJsonAsync<TResp>(_jsonOpts, ct);
        }
    

        // =========================
        // EMITIR VOTO (simple)
        // =========================
        public class EmitirVotoResponse
        {
            public bool Ok { get; set; }
            public string? Error { get; set; }
        }

        public async Task<EmitirVotoResponse> EmitirVotoSimpleAsync(string cedula, int candidatoId, CancellationToken ct)
        {
            // ⚠️ AJUSTA según tu API real:
            // Ejemplo: api/Votacion/emitir
            // body: { Cedula, CandidatoId }

            var path = "api/Votacion/emitir";

            var body = new
            {
                Cedula = cedula,
                CandidatoId = candidatoId
            };

            try
            {
                using var resp = await Client().PostAsJsonAsync(path, body, ct);
                var raw = await resp.Content.ReadAsStringAsync(ct);

                if (!resp.IsSuccessStatusCode)
                    return new EmitirVotoResponse { Ok = false, Error = $"HTTP {(int)resp.StatusCode} - {raw}" };

                // si tu API devuelve {ok,error} puedes deserializar aquí
                return new EmitirVotoResponse { Ok = true };
            }
            catch (Exception ex)
            {
                return new EmitirVotoResponse { Ok = false, Error = "Error llamando API: " + ex.Message };
            }
        }
        // VOTACIÓN (Proceso/Candidatos/Voto)
        // ======================

        public class ProcesoActivoResponse
        {
            public bool ok { get; set; }
            public string? error { get; set; }
            public ProcesoActivoDto? data { get; set; }
        }

        public class ProcesoActivoDto
        {
            public int id { get; set; }
            public string nombre { get; set; } = "";

            // 🔥 AQUÍ ESTÁ EL FIX
            public int estado { get; set; }  // antes lo tenías string

            public DateTime? inicioUtc { get; set; }
            public DateTime? finUtc { get; set; }
        }




        public class VotacionEmitirRequest
        {
            public int ProcesoElectoralId { get; set; }
            public int CandidatoId { get; set; }
            public string Cedula { get; set; } = "";
            public int TipoVoto { get; set; } = 1; // 1 = Candidato (ajusta si tu API usa otro)
        }

        public class VotacionEmitirResponse
        {
            public bool ok { get; set; }
            public string? error { get; set; }
            public object? data { get; set; } // si tu API devuelve comprobante, aquí lo capturas
        }

        // Rutas (ajústalas si tu swagger usa otras)
        private const string ProcesoActivoPath = "api/Proceso/activo";
        private const string CandidatosPath = "api/Candidatos";
        private const string EmitirVotoPath = "api/Votacion/emitir";




      

        public Task<List<CandidatoDto>?> GetCandidatosAsync(CancellationToken ct = default)
          => Client().GetFromJsonAsync<List<CandidatoDto>>("api/Candidatos", _jsonOpts, ct);

        public Task<ProcesoActivoResponse?> GetProcesoActivoAsync(CancellationToken ct = default)
            => Client().GetFromJsonAsync<ProcesoActivoResponse>("api/Proceso/activo", _jsonOpts, ct);

        public Task<ResultadosNacionalResponse?> GetResultadosNacionalAsync(CancellationToken ct = default)
            => Client().GetFromJsonAsync < ResultadosNacionalResponse>("api/Resultados/nacional", _jsonOpts, ct);

        public async Task<List<PadronItemDto>> GetPadronAsync(CancellationToken ct = default)
        {
            var res = await Client().GetAsync("api/Padron", ct);
            if (!res.IsSuccessStatusCode) return new List<PadronItemDto>();

            var data = await res.Content.ReadFromJsonAsync<List<PadronItemDto>>(_jsonOpts, ct);
            return data ?? new List<PadronItemDto>();
        }
        public class ProcesoCreateDto
        {
            public string Nombre { get; set; } = "";
            public int Tipo { get; set; }
            public DateTime InicioLocal { get; set; }
            public DateTime FinLocal { get; set; }
            public int Estado { get; set; }
        }

        public class CandidatoCreateDto
        {
            public int ProcesoElectoralId { get; set; }
            public string NombreCompleto { get; set; } = "";
            public string Partido { get; set; } = "";
            public string Binomio { get; set; } = "";
            public int NumeroLista { get; set; }
            public bool Activo { get; set; }
        }

        public class CandidatoAdminDto
        {
            public int Id { get; set; }
            public int ProcesoElectoralId { get; set; }
            public string NombreCompleto { get; set; } = "";
            public string Partido { get; set; } = "";
            public string Binomio { get; set; } = "";
            public int NumeroLista { get; set; }
            public bool Activo { get; set; }
        }
        public class ApiResp<T>
        {
            public bool ok { get; set; }
            public string? error { get; set; }
            public T? data { get; set; }
        }
        public async Task<ApiResp<int>?> CrearProcesoAsync(ProcesoCreateDto dto, CancellationToken ct)
        {
            using var resp = await Client().PostAsJsonAsync("api/Proceso/crear", dto, ct);
            return await resp.Content.ReadFromJsonAsync<ApiResp<int>>(_jsonOpts, ct);
        }

        public Task<List<CandidatoAdminDto>?> GetCandidatosAdminAsync(CancellationToken ct)
            => Client().GetFromJsonAsync<List<CandidatoAdminDto>>("api/Candidatos", _jsonOpts, ct);

        public async Task<(bool Ok, string? Error)> CrearCandidatoAsync(CandidatoCreateDto dto, CancellationToken ct)
        {
            using var resp = await Client().PostAsJsonAsync("api/Candidatos", dto, ct);
            var raw = await resp.Content.ReadAsStringAsync(ct);
            if (resp.IsSuccessStatusCode) return (true, null);
            return (false, raw);
        }

        public async Task<(bool Ok, string? Error)> GenerarPadDemoAsync(CancellationToken ct)
        {
            using var resp = await Client().PostAsync("api/Padron/generar-demo", content: null, ct);
            var raw = await resp.Content.ReadAsStringAsync(ct);
            if (resp.IsSuccessStatusCode) return (true, raw);
            return (false, raw);
        }
        public class EnviarResultadosCorreoRequest
        {
            public string Correo { get; set; } = "";
            public string? Asunto { get; set; }
        }

        public async Task<bool> EnviarResultadosAlCorreoAsync(string correo, CancellationToken ct)
        {
            var dto = new EnviarResultadosCorreoRequest
            {
                Correo = correo,
                Asunto = "Reporte de resultados - Voto Electrónico"
            };

            var resp = await Client().PostAsJsonAsync("api/Resultados/enviar-correo", dto, _jsonOpts, ct);

            if (!resp.IsSuccessStatusCode) return false;

            // si tu API devuelve { ok: true }, puedes leerlo:
            var json = await resp.Content.ReadFromJsonAsync<Dictionary<string, object>>(_jsonOpts, ct);
            return json != null && json.TryGetValue("ok", out var okVal) && okVal?.ToString() == "True";
        }

        public async Task<ResultadosPublicResponse?> GetResultadosNacionalPublicoAsync(CancellationToken ct)
        {
            var res = await Client().GetAsync("api/Resultados/nacional", ct);
            if (!res.IsSuccessStatusCode) return null;

            var json = await res.Content.ReadAsStringAsync(ct);
            return JsonSerializer.Deserialize<ResultadosPublicResponse>(json, _jsonOpts);
        }

        public async Task<List<ResultadoItemDto>?> GetResultadosProvinciaPublicoAsync(string provincia, CancellationToken ct)
        {
            var res = await Client().GetAsync($"api/Resultados/provincia/{provincia}", ct);
            if (!res.IsSuccessStatusCode) return null;

            var json = await res.Content.ReadAsStringAsync(ct);
            return JsonSerializer.Deserialize<List<ResultadoItemDto>>(json, _jsonOpts);
        }
        public async Task<ResultadosNacionalResponse?> GetResultadosFinalesAsync(CancellationToken ct)
        {
            var http = Client();
            return await http.GetFromJsonAsync<ResultadosNacionalResponse>("api/Resultados/final", _jsonOpts, ct);
        }

    }
}
