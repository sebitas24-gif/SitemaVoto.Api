using Newtonsoft.Json;
using Voto.ApiConsumer;
using VotoModelos;
namespace ApiTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 1. URL EXACTA: Se agrega el puerto 5203 que configuraste en IIS
            // Asegúrate de que la ruta termine en /api/Votantes (según tus controladores)
            //Crud<Votante>.UrlBase = "http://10.241.253.223:5203/api/Votantes";

            Console.WriteLine("Iniciando prueba de conexión a la API en ZeroTier...");

        }
    }
}
