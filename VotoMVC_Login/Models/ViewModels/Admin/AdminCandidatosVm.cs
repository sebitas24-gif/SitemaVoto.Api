using System.ComponentModel.DataAnnotations;

namespace VotoMVC_Login.Models.ViewModels.Admin
{
    public class AdminCandidatosVm
    {
        public string? Ok { get; set; }
        public string? Error { get; set; }

        public int ProcesoElectoralId { get; set; }
        public List<CandidatoRowVm> Lista { get; set; } = new();

        public CandidatoCrearVm Nuevo { get; set; } = new();
    }
    public class CandidatoRowVm
    {
        public int Id { get; set; }
        public int ProcesoElectoralId { get; set; }
        public string NombreCompleto { get; set; } = "";
        public string Partido { get; set; } = "";
        public string Binomio { get; set; } = "";
        public int NumeroLista { get; set; }
        public bool Activo { get; set; }
    }

    public class CandidatoCrearVm
    {
        [Required] public int ProcesoElectoralId { get; set; }
        [Required] public string NombreCompleto { get; set; } = "";
        [Required] public string Partido { get; set; } = "";
        public string Binomio { get; set; } = "";
        public int NumeroLista { get; set; } = 0;
        public bool Activo { get; set; } = true;
    }
}
