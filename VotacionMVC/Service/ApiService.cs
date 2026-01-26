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

        public ApiService(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        private HttpClient Client() => _factory.CreateClient("Api");

        // ========= GETs =========
        public Task<List<CandidatoDto>?> GetCandidatosAsync(CancellationToken ct = default)
            => Client().GetFromJsonAsync<List<CandidatoDto>>("api/Candidatos", _jsonOptions, ct);

        public Task<ProcesoActivoResponse?> GetProcesoActivoAsync(CancellationToken ct = default)
            => Client().GetFromJsonAsync<ProcesoActivoResponse>("api/Proceso/activo", _jsonOptions, ct);

        public Task<ResultadosNacionalResponse?> GetResultadosNacionalAsync(CancellationToken ct = default)
            => Client().GetFromJsonAsync<ResultadosNacionalResponse>("api/Resultados/nacional", _jsonOptions, ct);

        // ✅ PADRÓN (ya NO depende de AdminController)
        public async Task<List<PadronItemDto>> GetPadronAsync(CancellationToken ct = default)
        {
            // 👇 OJO: aquí está la ruta que estás intentando.
            // Si tu API usa otra, cámbiala aquí.
            var res = await Client().GetAsync("api/Padron", ct);

            if (res.StatusCode == System.Net.HttpStatusCode.NotFound)
                return new List<PadronItemDto>(); // ✅ no explota, solo vacío

            if (!res.IsSuccessStatusCode)
                return new List<PadronItemDto>(); // ✅ simple: no explota

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

        // POST genérico (el que estás usando en AccesoController)
        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest body, CancellationToken ct = default)
        {
            var res = await Client().PostAsJsonAsync(url, body, ct);
            if (!res.IsSuccessStatusCode) return default;

            return await res.Content.ReadFromJsonAsync<TResponse>(_jsonOptions, ct);
        }
        public async Task<byte[]?> TryDownloadResultadosPdfAsync(string provincia, CancellationToken ct = default)
        {
            // Cambia la ruta según tu API real
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



    }
}
