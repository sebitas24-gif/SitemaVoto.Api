using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotoModelos
{
    public class MesaVotacion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string NumeroMesa { get; set; }

        [Required]
        public string Provincia { get; set; }

        [Required]
        public string Canton { get; set; }

        [Required]
        public string Parroquia { get; set; }

        [Required]
        [StringLength(200)]
        public string RecintoElectoral { get; set; }

        [StringLength(500)]
        public string DireccionRecinto { get; set; }

        public int? JefeJuntaId { get; set; }

        [ForeignKey("JefeJuntaId")]
        public virtual Usuario JefeJunta { get; set; }

        public bool Activa { get; set; }

        // Relaciones
        public virtual ICollection<PadronElectoral> Votantes { get; set; }
    
}
}
