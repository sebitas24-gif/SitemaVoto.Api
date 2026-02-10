using System.ComponentModel.DataAnnotations;
using VotoModelos.Enums;

namespace VotoMVC_Login.Models.ViewModels.Admin
{
    public class AdminCandidatosVm
    {
        public string? Ok { get; set; }
        public string? Error { get; set; }

        public int ProcesoElectoralId { get; set; }
        public TipoEleccion TipoProceso { get; set; }
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
    public class NuevoCandidatoVm
    {
        public int ProcesoElectoralId { get; set; }
        // Comunes
        public string Partido { get; set; } = "";
        public int NumeroLista { get; set; }
        public bool Activo { get; set; } = true;

        // Nominal (y también sirve para Plurinominal como "candidato")
        public string NombreCompleto { get; set; } = "";
        public string? Cargo { get; set; } // opcional en tu demo

        // Plancha (binomio)
        public string? Vicepresidente { get; set; } // ✅ para plancha

        // Plurinominal
        public int? Orden { get; set; } // ✅ orden dentro de la lista
    }

    public class CandidatoItemVm
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; } = "";
        public string Partido { get; set; } = "";
        public int NumeroLista { get; set; }
        public bool Activo { get; set; }

        public string? Vicepresidente { get; set; } // plancha
        public int? Orden { get; set; } // plurinominal
        public string? Cargo { get; set; } // nominal
    }
}
