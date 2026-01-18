using VotoMVC.ViewModelos.Admin;

namespace VotoMVC.Services
{
    public class AdminProcesosApiService
    {
        private readonly HttpClient _http;
        private readonly string _baseUrl;

        public AdminProcesosApiService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _baseUrl = config["ApiSettings:BaseUrl"]!;
        }

        public async Task<List<ProcesoAdminVM>> ListarProcesosAsync()
        {
            var res = await _http.GetAsync($"{_baseUrl}/api/admin/procesos");
            if (!res.IsSuccessStatusCode) return new List<ProcesoAdminVM>();
            return await res.Content.ReadFromJsonAsync<List<ProcesoAdminVM>>() ?? new();
        }

        public async Task<bool> CrearProcesoAsync(CrearProcesoVM vm)
        {
            var res = await _http.PostAsJsonAsync($"{_baseUrl}/api/admin/procesos", vm);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> ActivarProcesoAsync(int id)
        {
            var res = await _http.PutAsync($"{_baseUrl}/api/admin/procesos/{id}/activar", null);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> CerrarProcesoAsync(int id)
        {
            var res = await _http.PutAsync($"{_baseUrl}/api/admin/procesos/{id}/cerrar", null);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> CrearOpcionAsync(CrearOpcionVM vm)
        {
            var res = await _http.PostAsJsonAsync($"{_baseUrl}/api/admin/procesos/opciones", vm);
            return res.IsSuccessStatusCode;
        }

        public async Task<List<OpcionAdminVM>> ListarOpcionesAsync(int idProceso)
        {
            var res = await _http.GetAsync($"{_baseUrl}/api/admin/procesos/{idProceso}/opciones");
            if (!res.IsSuccessStatusCode) return new List<OpcionAdminVM>();
            return await res.Content.ReadFromJsonAsync<List<OpcionAdminVM>>() ?? new();
        }
    }
}
