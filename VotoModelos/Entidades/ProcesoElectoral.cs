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
        [Key] public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Nombre { get; set; } = default!;

        [MaxLength(400)]
        public string? Descripcion { get; set; }

        public TipoEleccion Tipo { get; set; }
        public EstadoProceso Estado { get; set; } = EstadoProceso.Configuracion;

        public DateTime InicioLocal { get; set; }
        public DateTime FinLocal { get; set; }

        public ICollection<Candidato> Candidatos { get; set; } = new List<Candidato>();
    }
}
