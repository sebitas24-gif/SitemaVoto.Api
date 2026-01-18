using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotoModelos
{
    public class RegistroVotacion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; }

        [Required]
        public int ProcesoElectoralId { get; set; }

        [ForeignKey("ProcesoElectoralId")]
        public virtual ProcesoElectoral ProcesoElectoral { get; set; }

        [Required]
        public DateTime FechaHoraVoto { get; set; }

        [Required]
        [StringLength(50)]
        public string CodigoConfirmacion { get; set; } // CONF-XXXXXXXXX

        public bool ComprobanteEnviado { get; set; }

        public DateTime? FechaEnvioComprobante { get; set; }

        // Dirección IP para auditoría
        [StringLength(50)]
        public string DireccionIP { get; set; }

    }
}
