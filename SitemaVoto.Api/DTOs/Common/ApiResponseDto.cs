namespace SitemaVoto.Api.DTOs.Common
{
    public class ApiResponseDto<T>
    {
        public bool Ok { get; set; }
        public string? Error { get; set; }
        public T? Data { get; set; }

        public static ApiResponseDto<T> Success(T data) => new() { Ok = true, Data = data };
        public static ApiResponseDto<T> Fail(string error) => new() { Ok = false, Error = error };
    }
}
