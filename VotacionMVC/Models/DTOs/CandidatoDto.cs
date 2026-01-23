namespace VotacionMVC.Models.DTOs
{
    public class CandidatoDto
    {
        public int id { get; set; }
        public int procesoElectoralId { get; set; }
        public string nombreCompleto { get; set; } = "";
        public string partido { get; set; } = "";
        public string binomio { get; set; } = "";
        public int numeroLista { get; set; }
        public bool activo { get; set; }
    }
}
