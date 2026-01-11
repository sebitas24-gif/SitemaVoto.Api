namespace SitemaVoto.Api.DTOs
{
    public class ConfirmacionVotoDto
    {
        public int IdProceso { get; set; }
        public string CodigoVerificacion { get; set; } = null!;
        public DateTime FechaEmision { get; set; }
        public string Mensaje { get; set; } = "Voto registrado correctamente.";
    }
}
