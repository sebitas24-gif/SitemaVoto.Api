using System.ComponentModel.DataAnnotations;

namespace VotoMVC_Login.Models.ViewModels
{
    public class AdminLoginPasswordVm
    {
        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = "";

        public string? Error { get; set; }
    }
}
