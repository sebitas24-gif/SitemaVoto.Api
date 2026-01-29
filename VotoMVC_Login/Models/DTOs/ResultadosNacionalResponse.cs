namespace VotoMVC_Login.Models.DTOs
{
    public class ResultadosNacionalResponse
    {
        public string estadoProceso { get; set; } = "";
        public List<PorCandidato> porCandidato { get; set; } = new();
        public List<LiderProvincia> lideresPorProvincia { get; set; } = new();

        public class PorCandidato
        {
            public string nombre { get; set; } = "";
            public int votos { get; set; }
        }

        public class LiderProvincia
        {
            public string provincia { get; set; } = "";
            public string lider { get; set; } = "";
            public int votosLider { get; set; }
        }

    }
}
