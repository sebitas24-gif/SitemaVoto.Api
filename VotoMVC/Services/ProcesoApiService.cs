using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using VotoModelos;

namespace VotoMVC.Services
{
    public class ProcesoApiService
    {
        private readonly HttpClient _http;
        private readonly string _baseUrl;

        public ProcesoApiService(HttpClient http, IConfiguration config)
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

        public async Task<List<ProcesoElectoral>> GetAllAsync()
        {
            var url = $"{_baseUrl}/api/ProcesoElectorales";
            var json = await _http.GetStringAsync(url);
            return JsonSerializer.Deserialize<List<ProcesoElectoral>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<ProcesoElectoral>();
        }

        public async Task<ProcesoElectoral?> GetByIdAsync(int id)
        {
            var url = $"{_baseUrl}/api/ProcesoElectorales/{id}";
            var resp = await _http.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return null;

            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ProcesoElectoral>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public async Task<bool> CreateAsync(ProcesoElectoral model, string? token)
        {
            SetAuth(token);

            var url = $"{_baseUrl}/api/ProcesoElectorales";
            var json = JsonSerializer.Serialize(model);
            var resp = await _http.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(int id, ProcesoElectoral model, string? token)
        {
            SetAuth(token);

            var url = $"{_baseUrl}/api/ProcesoElectorales/{id}";
            var json = JsonSerializer.Serialize(model);
            var resp = await _http.PutAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> CerrarAsync(int id, string? token)
        {
            // Traer, poner Estado=false, y hacer PUT
            var proc = await GetByIdAsync(id);
            if (proc == null) return false;

            proc.Estado = false;
            return await UpdateAsync(id, proc, token);
        }
    }
}
