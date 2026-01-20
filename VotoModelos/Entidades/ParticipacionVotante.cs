using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotoModelos.Entidades
{
    public class ParticipacionVotante
    {
        [Key] public int Id { get; set; }

        public int ProcesoElectoralId { get; set; }
        public ProcesoElectoral ProcesoElectoral { get; set; } = default!;

        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = default!;

        // Comprobante (receipt) sin voto
        [Required, MaxLength(40)]
        public string CodigoComprobante { get; set; } = default!; // Ej: CONF-XXXX

        public DateTime EmitidoUtc { get; set; }

        // Para auditoría simple (opcional)
        [MaxLength(80)] public string Provincia { get; set; } = default!;
        [MaxLength(80)] public string Canton { get; set; } = default!;
        [MaxLength(50)] public string? CodigoMesa { get; set; }
    }
}
