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
                FechaNacimiento = new DateTime(1990, 1, 1)
            };
            var apiResult = Crud<Votante>.Create(nuevoVotante);
            var votantes = Crud<Votante>.ReadAll();
            nuevoVotante = apiResult.Data;
            nuevoVotante.Nombre= "MODIFICADO";
            Crud<Votante>.Update(nuevoVotante.Id.ToString(), nuevoVotante);
            var unVotante = Crud<Votante>.ReadBy("Id","12");
            Crud<Votante>.Delete("12");
            Console.WriteLine(apiResult);
            Console.WriteLine();
        }
    }
}
