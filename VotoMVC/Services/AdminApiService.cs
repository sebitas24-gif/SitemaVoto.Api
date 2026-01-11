using System.Net.Http.Headers;

namespace VotoMVC.Services
{
    public class AdminApiService
    {

        private readonly HttpClient _http;
        private readonly string _baseUrl;

        public AdminApiService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _baseUrl = config["ApiSettings:BaseUrl"]!.TrimEnd('/');
        }

        private void AttachJwt(string? token)
        {
            _http.DefaultRequestHeaders.Authorization = null;
            if (!string.IsNullOrWhiteSpace(token))
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<bool> HacerAdminAsync(int idVotante, string? token)
        {
            AttachJwt(token);
            var res = await _http.PostAsJsonAsync($"{_baseUrl}/api/admin/roles/hacer-admin", new { IdVotante = idVotante });
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> QuitarAdminAsync(int idVotante, string? token)
        {
            AttachJwt(token);
            var res = await _http.DeleteAsync($"{_baseUrl}/api/admin/roles/quitar-admin/{idVotante}");
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> HacerCandidatoAsync(int idVotante, string partido, string eslogan, string? token)
        {
            AttachJwt(token);
            var res = await _http.PostAsJsonAsync($"{_baseUrl}/api/admin/roles/hacer-candidato",
                new { IdVotante = idVotante, Partido = partido, Eslogan = eslogan });

            return res.IsSuccessStatusCode;
        }

        public async Task<bool> QuitarCandidatoAsync(int idVotante, string? token)
        {
            AttachJwt(token);
            var res = await _http.DeleteAsync($"{_baseUrl}/api/admin/roles/quitar-candidato/{idVotante}");
            return res.IsSuccessStatusCode;
        }
    }
}
