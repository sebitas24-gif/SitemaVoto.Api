using System.Net.Http.Headers;
using VotoMVC.ViewModelos.Votacion;
using VotoMVC.ViewModelos;
namespace VotoMVC.Services
{
    public class VotacionApiService
    {
        private readonly HttpClient _http;
        private readonly string _baseUrl;

        public VotacionApiService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _baseUrl = config["ApiSettings:BaseUrl"]!;
        }

        public async Task<ProcesoActivoVM?> ObtenerProcesoActivoAsync()
        {
            var res = await _http.GetAsync($"{_baseUrl}/api/votacion/proceso-activo");
            if (!res.IsSuccessStatusCode) return null;
            return await res.Content.ReadFromJsonAsync<ProcesoActivoVM>();
        }

        public async Task<OpcionesActivoVM?> ObtenerOpcionesActivoAsync()
        {
            var res = await _http.GetAsync($"{_baseUrl}/api/votacion/opciones-activo");
            if (!res.IsSuccessStatusCode) return null;
            return await res.Content.ReadFromJsonAsync<OpcionesActivoVM>();
        }

        public async Task<ConfirmacionVotoVM?> EmitirVotoAsync(string cedula, int idOpcion)
        {
            var res = await _http.PostAsJsonAsync($"{_baseUrl}/api/votacion/emitir",
                new { Cedula = cedula, IdOpcion = idOpcion });

            if (!res.IsSuccessStatusCode) return null;
            return await res.Content.ReadFromJsonAsync<ConfirmacionVotoVM>();
        }
    }
}
