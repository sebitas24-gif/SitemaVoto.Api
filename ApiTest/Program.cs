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
            Crud<Votante>.UrlBase = "http://10.241.253.223:5203/api/Votantes";

            Console.WriteLine("Iniciando prueba de conexión a la API en ZeroTier...");

            var nuevoVotante = new Votante
            {
                Nombre = "Juan",
                Apellido = "Perez",
                Cedula = "12345678",
                FechaNacimiento = new DateTime(1990, 1, 1),
                EstaHabilitado = true,
                YaVoto = false
            };

            // 2. Intento de creación
            var apiResult = Crud<Votante>.Create(nuevoVotante);

            // VALIDACIÓN: Verificamos si la API respondió correctamente
            if (apiResult != null && apiResult.Data != null)
            {
                Console.WriteLine("Votante creado con éxito.");

                nuevoVotante = apiResult.Data;
                nuevoVotante.Nombre = "Perez Actualizado";

                // 3. Actualizar usando el ID devuelto por PostgreSQL en Render
                var updateResult = Crud<Votante>.Update(nuevoVotante.Id, nuevoVotante);

                if (updateResult != null)
                {
                    Console.WriteLine("Votante actualizado correctamente en la nube.");
                }
            }
            else
            {
                // Si sale este mensaje, el puerto 5203 sigue bloqueado en el Firewall
                Console.WriteLine("Error: La API en 10.241.253.223:5203 no responde.");
                Console.WriteLine("Revisa: 1. IIS iniciado, 2. Firewall puerto 5203 abierto.");
            }

            // 4. Lectura general
            var votantes = Crud<Votante>.ReadAll();
            if (votantes != null)
            {
                Console.WriteLine($"Total de votantes en la base de datos: {votantes}");
            }

            Console.WriteLine("Prueba finalizada. Presione cualquier tecla para salir.");
            Console.ReadKey();
        }
    }
}
