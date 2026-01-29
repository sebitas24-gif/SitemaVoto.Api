using static VotoMVC_Login.Service.ApiService;

namespace VotoMVC_Login.Models
{
    public class JefePanelVm
    {
        public string? CedulaBuscada { get; set; }
        public CiudadanoDto? Ciudadano { get; set; }
        public string? Error { get; set; }
        public string? Msg { get; set; }
    }
}
