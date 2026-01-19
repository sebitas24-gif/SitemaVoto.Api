using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotoModelos.Entidades
{
    public class Candidato
    {
       [Key] public int Id { get; set; }

        public int ProcesoElectoralId { get; set; }
        public ProcesoElectoral ProcesoElectoral { get; set; } = null!;

        [Required, MaxLength(120)]
        public string NombreCompleto { get; set; } = null!;

        [MaxLength(120)]
        public string? Partido { get; set; }

        public int? NumeroLista { get; set; }

        [MaxLength(120)]
        public string? NombreBinomio { get; set; }

        public bool Activo { get; set; } = true;
    }
}
