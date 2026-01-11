using System.Net.Http.Headers;
using VotoMVC.ViewModelos.Votacion;
namespace VotoMVC.Services
{
    public class VotacionApiService
    {
        private readonly HttpClient _http;
        private readonly string _baseUrl;

        public VotacionApiService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _baseUrl = config["ApiSettings:BaseUrl"]!.TrimEnd('/');
        }

        private void SetBearer(string? token)
        {
            _http.DefaultRequestHeaders.Authorization = null;
            if (!string.IsNullOrWhiteSpace(token))
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        // LECTURA
        public async Task<dynamic?> GetProcesoActivoAsync()
        {
            return await _http.GetFromJsonAsync<dynamic>($"{_baseUrl}/api/votacion/proceso-activo");
        }

        // LECTURA
        public async Task<List<dynamic>?> GetOpcionesAsync(int idProceso)
        {
            return await _http.GetFromJsonAsync<List<dynamic>>($"{_baseUrl}/api/votacion/opciones/{idProceso}");
        }

        // ESCRITURA
        public async Task<dynamic?> EmitirAsync(string cedula, int idProceso, int idOpcion, string? token)
        {
            SetBearer(token);
            var res = await _http.PostAsJsonAsync($"{_baseUrl}/api/votacion/emitir", new
            {
                cedula,
                idProceso,
                idOpcion
            });

            var json = await res.Content.ReadFromJsonAsync<dynamic>();
            return json;
        }
    }
}
