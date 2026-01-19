using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotoModelos.Entidades
{
    public class CodigoPadron
    {
        public int Id { get; set; }

        public int ProcesoElectoralId { get; set; }
        public ProcesoElectoral ProcesoElectoral { get; set; } = null!;

        public int VotanteUsuarioId { get; set; }
        public Usuario VotanteUsuario { get; set; } = null!;

        public int? EmitidoPorUsuarioId { get; set; }
        public Usuario? EmitidoPorUsuario { get; set; }

        [Required, MaxLength(20)]
        public string Codigo { get; set; } = null!; // PAD-123456

        public DateTime EmitidoEn { get; set; } = DateTime.UtcNow;
        public DateTime? UsadoEn { get; set; }
    }
}
