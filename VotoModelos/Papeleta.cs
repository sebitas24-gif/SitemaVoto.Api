using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotoModelos
{
    public class Papeleta
    {
        [Key]
        public int Id { get; set; }

        // Registra el momento exacto en que se emitió el comprobante
        public DateTime FechaEmision { get; set; }

        // Código único (ej. un GUID o Hash) para validar que el certificado es real
        public string? CodigoConfirmacion { get; set; } = Guid.NewGuid().ToString(); // Genera un código único automático

        // RELACIÓN DE IDENTIDAD:
        // Vincula el comprobante al ciudadano, pero NO al voto.
        // Así sabemos que 'Juan Pérez' ya tiene su papel, pero no por quién votó.
        public int VotanteId { get; set; }

        // Opcional: Para saber a qué elección pertenece este certificado
        public int ProcesoElectoralId { get; set; }
    }
}
