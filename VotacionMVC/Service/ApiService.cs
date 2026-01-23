using System.Text.Json;
using System.Text;
using VotacionMVC.Models.DTOs;
using System.Net.Http.Json;

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
    }
}
