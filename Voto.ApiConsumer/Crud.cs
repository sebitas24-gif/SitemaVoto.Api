using VotoModelos;
using Newtonsoft.Json;
using System.Text;

namespace Voto.ApiConsumer
{
    public static class Crud<T>
    {
        public static string UrlBase = "";
        //Consumir una API y ejecutarel verbo POST
        public static ApiResult<T> Create(T data)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    // Forzamos el formato ISO que Swagger usa exitosamente
                    var settings = new JsonSerializerSettings
                    {
                        DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffZ",
                        NullValueHandling = NullValueHandling.Ignore
                    };

                    var json = JsonConvert.SerializeObject(data, settings);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = httpClient.PostAsync(UrlBase, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var resJson = response.Content.ReadAsStringAsync().Result;
                        return JsonConvert.DeserializeObject<ApiResult<T>>(resJson);
                    }

                    return ApiResult<T>.Fail($"Error {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return ApiResult<T>.Fail(ex.Message);
            }
        }
        public static ApiResult<List<T>> ReadAll()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    // Agrega el puerto 5050 que vimos en tu navegador
                    var response = httpClient.GetAsync(UrlBase).Result;
                    var json = response.Content.ReadAsStringAsync().Result;

                    // IMPORTANTE: Si la API devuelve la lista directo (como se ve en tu imagen),
                    // primero la guardamos en una lista y luego la metemos en el ApiResult.
                    var lista = Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>(json);

                    return new ApiResult<List<T>> { Data = lista };
                }
            }
            catch (Exception ex)
            {
                return new ApiResult<List<T>> { Message = ex.Message };
            }
        }
        public static ApiResult<T> GetByCedula(string cedula)
{
    try
    {
        using (var client = new HttpClient())
        {
            // Nota: Verifica si tu API tiene esta ruta, si no, usaremos ReadAll
            var response = client.GetAsync($"{UrlBase}/cedula/{cedula}").Result;

            if (response.IsSuccessStatusCode)
            {
                var json = response.Content.ReadAsStringAsync().Result;
                var data = JsonConvert.DeserializeObject<T>(json);
                return new ApiResult<T> { Data = data };
            }
            return ApiResult<T>.Fail("No encontrado");
        }
    }
    catch (Exception ex) { return ApiResult<T>.Fail(ex.Message); }
}

        public static ApiResult<T> ReadBy(string field, string value)
        {
            // consumir una API y ejecutar el verbo GET con parámetros
            try
            {
                using (var httpClient = new HttpClient())
                {
                    // invocar al servicio web
                    var response = httpClient.GetAsync($"{UrlBase}/{field}/{value}").Result;
                    var json = response.Content.ReadAsStringAsync().Result;
                    // deserializar la respuesta
                    var data = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResult<T>>(json);
                    return data;
                }
            }
            catch (Exception ex)
            {
                return ApiResult<T>.Fail(ex.Message);
            }
        }
        public static ApiResult<T> GetById(int id)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    // La URL ahora se formará correctamente: .../api/Votantes/5
                    var response = client.GetAsync($"{UrlBase}/{id}").Result;

                    if (!response.IsSuccessStatusCode)
                        return ApiResult<T>.Fail($"Error: {response.StatusCode}");

                    var json = response.Content.ReadAsStringAsync().Result;
                    // Deserializamos directamente al objeto T
                    var data = JsonConvert.DeserializeObject<T>(json);
                    return new ApiResult<T> { Data = data };
                }
            }
            catch (Exception ex) { return ApiResult<T>.Fail(ex.Message); }
        }
        public static ApiResult<List<T>> GetAll()
        {
            using var client = new HttpClient();
            var response = client.GetAsync(UrlBase).Result;

            if (!response.IsSuccessStatusCode)
                return ApiResult<List<T>>.Fail("Error API");

            var json = response.Content.ReadAsStringAsync().Result;
            var data = JsonConvert.DeserializeObject<List<T>>(json);

            return ApiResult<List<T>>.Ok(data);
        }
        private static JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffZ",
            NullValueHandling = NullValueHandling.Ignore
        };
        public static ApiResult<T> Update(int id, T data)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(data, _settings);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = client.PutAsync($"{UrlBase}/{id}", content).Result;

                    return response.IsSuccessStatusCode
                        ? ApiResult<T>.Ok(data)
                        : ApiResult<T>.Fail($"Error API: {response.StatusCode}");
                }
            }
            catch (Exception ex) { return ApiResult<T>.Fail(ex.Message); }
        }
        public static ApiResult<bool> Delete(int id)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = client.DeleteAsync($"{UrlBase}/{id}").Result;
                    return response.IsSuccessStatusCode
                        ? ApiResult<bool>.Ok(true)
                        : ApiResult<bool>.Fail($"No se pudo eliminar: {response.StatusCode}");
                }
            }
            catch (Exception ex) { return ApiResult<bool>.Fail(ex.Message); }
        }
    }
}
