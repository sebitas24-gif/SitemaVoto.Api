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
        public ProcesoElectoral ProcesoElectoral { get; set; } = null!;

        public int VotanteUsuarioId { get; set; }
        public Usuario VotanteUsuario { get; set; } = null!;

        public DateTime VotoEn { get; set; } = DateTime.UtcNow;

        public string CodigoComprobante { get; set; } = null!;
    }
}
