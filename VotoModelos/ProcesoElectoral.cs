using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotoModelos
{
   public class ProcesoElectoral
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Nombre { get; set; }

        [StringLength(1000)]
        public string Descripcion { get; set; }

        [Required]
        public TipoEleccion TipoEleccion { get; set; }

        [Required]
        public DateTime FechaInicio { get; set; }

        [Required]
        public DateTime FechaFin { get; set; }

        [Required]
        public EstadoProceso Estado { get; set; }

        public DateTime FechaCreacion { get; set; }

        public int AdministradorId { get; set; }

        [ForeignKey("AdministradorId")]
        public virtual Usuario Administrador { get; set; }

        // Método de adjudicación de escaños
        public MetodoAdjudicacion? MetodoAdjudicacion { get; set; }

        // Configuración
        public bool PermitirVotoBlanco { get; set; }

        public int? NumeroEscanos { get; set; } // Para elecciones plurinominales

        // Relaciones
        public virtual ICollection<Candidato> Candidatos { get; set; }
        public virtual ICollection<PadronElectoral> PadronElectoral { get; set; }
        public virtual ICollection<VotoAnonimo> Votos { get; set; }
        public virtual ICollection<RegistroVotacion> RegistrosVotacion { get; set; }
    }

    public enum TipoEleccion
    {
        Nominal = 1,      // Voto por un solo candidato
        Plancha = 2,      // Binomio o lista completa (presidente-vice)
        Plurinominal = 3  // Varios escaños (asambleístas)
    }

    public enum EstadoProceso
    {
        Configuracion = 1,  // En proceso de configuración
        Activo = 2,         // Votación en curso
        Finalizado = 3,     // Proceso terminado
        Cancelado = 4       // Proceso cancelado
    }

    public enum MetodoAdjudicacion
    {
        Webster = 1,   // Método Webster (Sainte-Laguë)
        DHondt = 2,    // Método D'Hondt
        HareNiemeyer = 3  // Método Hare-Niemeyer
    }

}
