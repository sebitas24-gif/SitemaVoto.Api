using System.Text.Json;
using System.Text;
using VotacionMVC.Models.DTOs;
using System.Net.Http.Json;
using VotacionMVC.Controllers;

namespace VotacionMVC.Service
{
    public class ApiService
    {
        private readonly IHttpClientFactory _factory;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public ApiService(IHttpClientFactory factory) => _factory = factory;

        private HttpClient Client() => _factory.CreateClient("Api");

        // ========= GETs =========
        public Task<List<CandidatoDto>?> GetCandidatosAsync(CancellationToken ct = default)
            => Client().GetFromJsonAsync<List<CandidatoDto>>("api/Candidatos", _jsonOptions, ct);

        public Task<ProcesoActivoResponse?> GetProcesoActivoAsync(CancellationToken ct = default)
            => Client().GetFromJsonAsync<ProcesoActivoResponse>("api/Proceso/activo", _jsonOptions, ct);

        public Task<ResultadosNacionalResponse?> GetResultadosNacionalAsync(CancellationToken ct = default)
            => Client().GetFromJsonAsync<ResultadosNacionalResponse>("api/Resultados/nacional", _jsonOptions, ct);

        public async Task<List<PadronItemDto>> GetPadronAsync(CancellationToken ct = default)
        {
            var res = await Client().GetAsync("api/Padron", ct);
            if (!res.IsSuccessStatusCode) return new List<PadronItemDto>();

            var data = await res.Content.ReadFromJsonAsync<List<PadronItemDto>>(_jsonOptions, ct);
            return data ?? new List<PadronItemDto>();
        }

        // ========= POSTs =========
        public async Task<VotacionEmitirResponse?> EmitirVotoAsync(VotacionEmitirRequest req, CancellationToken ct = default)
        {
            var res = await Client().PostAsJsonAsync("api/Votacion/emitir", req, ct);
            if (!res.IsSuccessStatusCode) return null;

            return await res.Content.ReadFromJsonAsync<VotacionEmitirResponse>(_jsonOptions, ct);
        }

        public string? LastError { get; private set; }
        public string? LastJsonSent { get; private set; }
        public string? LastRawResponse { get; private set; }

        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest body, CancellationToken ct = default)
        {
            using var res = await Client().PostAsJsonAsync(url, body, _jsonOptions, ct);

            // Si el servidor respondió (aunque no mande JSON), NO es "API no respondió"
            var raw = await res.Content.ReadAsStringAsync(ct);

            if (!res.IsSuccessStatusCode)
                throw new HttpRequestException($"POST {url} falló: HTTP {(int)res.StatusCode} {res.StatusCode} - {raw}");

            // ✅ Si está vacío, igual consideramos éxito y devolvemos default
            if (string.IsNullOrWhiteSpace(raw))
                return default;

            try
            {
                return JsonSerializer.Deserialize<TResponse>(raw, _jsonOptions);
            }
            catch
            {
                // ✅ Si no coincide el JSON con TResponse, igual no lo tratamos como error
                return default;
            }
        }


        // ✅ IMPORTANTE: tu API es POST api/Proceso/crear
        public Task<ProcesoCrearResponse?> CrearProcesoAsync(ProcesoCrearRequest req, CancellationToken ct = default)
            => PostAsync<ProcesoCrearRequest, ProcesoCrearResponse>("api/Proceso/crear", req, ct);

        public async Task<JsonElement?> GetResultadosRawAsync(string modo, CancellationToken ct = default)
        {
            var url = (modo ?? "vivo").ToLower() == "final"
                ? "api/Resultados/final"
                : "api/Resultados/nacional";

            using var res = await Client().GetAsync(url, ct);
            if (!res.IsSuccessStatusCode) return null;

            await using var stream = await res.Content.ReadAsStreamAsync(ct);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);
            return doc.RootElement.Clone();
        }

        public async Task<byte[]?> TryDownloadResultadosPdfAsync(string provincia, CancellationToken ct = default)
        {
            var url = $"api/Resultados/pdf?provincia={Uri.EscapeDataString(provincia)}";
            var res = await Client().GetAsync(url, ct);
            if (!res.IsSuccessStatusCode) return null;

            return await res.Content.ReadAsByteArrayAsync(ct);
        }
        public async Task<JefeVerificacionDto?> GetVotantePorCedulaAsync(string cedula, CancellationToken ct = default)
        {
            var res = await Client().GetAsync($"api/Padron/cedula/{cedula}", ct);
            if (!res.IsSuccessStatusCode) return null;

            return await res.Content.ReadFromJsonAsync<JefeVerificacionDto>(_jsonOptions, ct);
        }
        public Task<CandidatoCrearApiResponse?> CrearCandidatoAsync(CandidatoCrearApiRequest req, CancellationToken ct = default)
           => PostAsync<CandidatoCrearApiRequest, CandidatoCrearApiResponse>("api/Candidatos", req, ct);

    }
}
