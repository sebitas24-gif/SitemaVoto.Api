using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using VotoModelos;

namespace VotoMVC.Services
{
    public class OpcionApiService
    {
        private readonly HttpClient _http;
        private readonly string _baseUrl;

        public OpcionApiService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _baseUrl = config["ApiSettings:BaseUrl"]!.TrimEnd('/');
        }

        private void SetAuth(string? token)
        {
            _http.DefaultRequestHeaders.Authorization = null;
            if (!string.IsNullOrWhiteSpace(token))
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        // Trae TODAS y filtra en MVC por IdProceso
        public async Task<List<OpcionElectoral>> GetByProcesoAsync(int idProceso)
        {
            var url = $"{_baseUrl}/api/OpcionElectorales";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return new List<OpcionElectoral>();

            var json = await resp.Content.ReadAsStringAsync();
            var all = JsonSerializer.Deserialize<List<OpcionElectoral>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<OpcionElectoral>();

            return all.Where(o => o.IdProceso == idProceso).ToList();
        }

        public async Task<bool> CreateAsync(OpcionElectoral model, string? token)
        {
            SetAuth(token);

            var url = $"{_baseUrl}/api/OpcionElectorales";
            var json = JsonSerializer.Serialize(model);
            var resp = await _http.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
            return resp.IsSuccessStatusCode;
        }
    }
}
