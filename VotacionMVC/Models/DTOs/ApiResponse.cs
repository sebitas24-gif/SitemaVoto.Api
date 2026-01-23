namespace VotacionMVC.Models.DTOs
{
    public class ApiResponse<T>
    {

        public bool ok { get; set; }
        public string? error { get; set; }
        public T? data { get; set; }
    }
}
