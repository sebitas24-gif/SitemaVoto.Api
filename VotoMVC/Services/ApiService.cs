namespace VotoMVC.Services
{
    public class ApiService
    {
        protected readonly HttpClient _http;
        protected readonly string _baseUrl;

        public ApiService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _baseUrl = config["ApiSettings:BaseUrl"];
        }
    }
}
