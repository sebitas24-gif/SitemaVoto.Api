using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ApiTest
{
    internal class Program
    {
        // Cambia a tu API (local o ZeroTier)
        static readonly string BaseUrl = "http://localhost:5203/";
        // static readonly string BaseUrl = "http://10.241.253.223:5203";

        static async Task Main(string[] args)
        {
            Console.WriteLine("Prueba GET Usuarios");
            Console.WriteLine($"BaseUrl: {BaseUrl}");
            Console.WriteLine();

            try
            {
                using var http = new HttpClient
                {
                    BaseAddress = new Uri(BaseUrl),
                    Timeout = TimeSpan.FromSeconds(15)
                };

                // Endpoint: revisa tu Swagger si es /api/Usuarios o /api/Votantes
                var endpoint = "/api/Usuarios";

                Console.WriteLine($"GET {endpoint}");
                var resp = await http.GetAsync(endpoint);
                var json = await resp.Content.ReadAsStringAsync();

                Console.WriteLine($"Status: {(int)resp.StatusCode} {resp.ReasonPhrase}");
                Console.WriteLine();

                if (!resp.IsSuccessStatusCode)
                {
                    Console.WriteLine("Error body:");
                    Console.WriteLine(json);
                    return;
                }

                var usuarios = JsonConvert.DeserializeObject<List<UsuarioDto>>(json) ?? new List<UsuarioDto>();

                Console.WriteLine($"Usuarios recibidos: {usuarios.Count}");
                Console.WriteLine(new string('=', 60));

                foreach (var u in usuarios)
                {
                    Console.WriteLine($"Id: {u.Id} | Rol: {u.Rol} | JuntaId: {u.JuntaId}");
                    Console.WriteLine($"Cedula: {u.Cedula}");
                    Console.WriteLine($"Nombre: {u.Nombres} {u.Apellidos}");
                    Console.WriteLine($"Correo: {u.Correo} | Tel: {u.Telefono}");
                    Console.WriteLine($"Ubicación: {u.Provincia} - {u.Canton} - {u.Parroquia}");
                    Console.WriteLine($"Habilitado: {u.HabilitadoLegalamente}");
                    Console.WriteLine(new string('-', 60));
                }

                // Extra: solo votantes (rol 3)
                Console.WriteLine();
                Console.WriteLine("=== SOLO VOTANTES (rol 3) ===");
                foreach (var u in usuarios)
                {
                    if (u.Rol == 3)
                        Console.WriteLine($"[{u.Id}] {u.Cedula} - {u.Nombres} {u.Apellidos} (JuntaId {u.JuntaId})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error conectando/leyendo:");
                Console.WriteLine(ex);
            }
            Console.WriteLine();
            Console.WriteLine("Presiona una tecla para salir...");
            Console.ReadKey();
        }
    }

    public class UsuarioDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("cedula")]
        public string Cedula { get; set; } = "";

        [JsonProperty("nombres")]
        public string Nombres { get; set; } = "";

        [JsonProperty("apellidos")]
        public string Apellidos { get; set; } = "";

        [JsonProperty("correo")]
        public string Correo { get; set; } = "";

        [JsonProperty("telefono")]
        public string Telefono { get; set; } = "";

        [JsonProperty("rol")]
        public int Rol { get; set; }

        [JsonProperty("provincia")]
        public string Provincia { get; set; } = "";

        [JsonProperty("canton")]
        public string Canton { get; set; } = "";

        [JsonProperty("parroquia")]
        public string? Parroquia { get; set; }

        [JsonProperty("juntaId")]
        public int? JuntaId { get; set; }

        [JsonProperty("imagenUrl")]
        public string? ImagenUrl { get; set; }

        [JsonProperty("habilitadoLegalamente")]
        public bool HabilitadoLegalamente { get; set; }
    }
}


