using Newtonsoft.Json;
using Voto.ApiConsumer;
using VotoModelos;
namespace ApiTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Crud<Votante>.UrlBase = "http://10.241.253.223/api/Votantes";
            var nuevoVotante = new Votante
            {
                Nombre = "Juan",
                Apellido = "Perez",
                Cedula = "12345678",
                FechaNacimiento = new DateTime(1990, 1, 1),
                EstaHabilitado = true,
                YaVoto = false
            };
            var apiResult = Crud<Votante>.Create(nuevoVotante);

            // VALIDACIÓN: Solo entramos si apiResult y Data no son nulos
            if (apiResult != null && apiResult.Data != null)
            {
                nuevoVotante = apiResult.Data;
                nuevoVotante.Nombre = "Perez";
                // Actualizar usando el ID real devuelto por la API
                Crud<Votante>.Update(nuevoVotante.Id.ToString(), nuevoVotante);
                Console.WriteLine("Votante actualizado correctamente.");
            }
            else
            {
                // Esto captura el error que ves en tu consola
                Console.WriteLine("Error: La API no devolvió datos. Revisa la conexión o el servidor.");
            }
            var votantes = Crud<Votante>.ReadAll();
            var unVotante = Crud<Votante>.ReadBy("Id", "12");
            Crud<Votante>.Delete("12");
            Console.WriteLine(apiResult);
        }
    }
}
