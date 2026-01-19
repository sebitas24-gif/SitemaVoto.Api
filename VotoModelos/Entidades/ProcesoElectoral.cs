using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using VotoModelos.Enums;
using System.Threading.Tasks;

namespace VotoModelos.Entidades
{
    public class ProcesoElectoral
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Nombre { get; set; } = null!;

        [MaxLength(600)]
        public string? Descripcion { get; set; }

        public TipoEleccion Tipo { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        public bool PermitirVotoEnBlanco { get; set; } = true;

        public bool CerradoManual { get; set; } = false;

        public List<Candidato> Candidatos { get; set; } = new();
    }
}
