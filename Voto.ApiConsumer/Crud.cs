using VotoModelos;
using Newtonsoft.Json;

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
                    // invocar al servicio web
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                    var response = httpClient.PostAsync(UrlBase, content).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        json = response.Content.ReadAsStringAsync().Result;
                        // deserializar la respuesta
                        var newData = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResult<T>>(json);
                        return newData;
                    }
                    else
                    {
                        return ApiResult<T>.Fail($"Error: {response.StatusCode}");
                    }
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
            using (var client = new HttpClient())
            {
                // Usamos UrlBase que ya tiene la IP de ZeroTier y el puerto 5050
                var response = client.GetAsync($"{UrlBase}/{id}").Result;

                var result = new ApiResult<T>();
                if (response.IsSuccessStatusCode)
                {
                    var json = response.Content.ReadAsStringAsync().Result;
                    // Deserializamos el JSON plano que manda tu API
                    result.Data = JsonConvert.DeserializeObject<T>(json);
                }
                else
                {
                    result.Message = $"Error: {response.StatusCode}";
                }
                return result;
            }
        }
        public static ApiResult<List<T>> GetAll()
        {
            return ReadAll();
        }
        public static ApiResult<T> Update(int id, T data)
        {
            using (var client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = client.PutAsync($"{UrlBase}/{id}", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    return new ApiResult<T> { Data = data }; // Retornamos los datos actualizados
                }
                return new ApiResult<T> { Message = "Error al actualizar" };
            }
        }
        public static ApiResult<bool> Delete(string id)
        {
            // consumir una API y ejecutar el verbo DELETE
            try
            {
                using (var httpClient = new HttpClient())
                {
                    // invocar al servicio web
                    var response = httpClient.DeleteAsync($"{UrlBase}/{id}").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        return ApiResult<bool>.Ok(true);
                    }
                    else
                    {
                        return ApiResult<bool>.Fail($"Error: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Fail(ex.Message);
            }
        }
        public static ApiResult<bool> Delete(int id)
        {
            using (var client = new HttpClient())
            {
                var response = client.DeleteAsync($"{UrlBase}/{id}").Result;
                return new ApiResult<bool> { Data = response.IsSuccessStatusCode };
            }
        }
    }
}
