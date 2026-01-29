namespace VotoMVC_Login.Models.DTOs
{
    public class VotacionEmitirRequest
    {

        public string cedula { get; set; } = "";
        public string codigoPad { get; set; } = "";
        public int candidatoId { get; set; }
    }
}
