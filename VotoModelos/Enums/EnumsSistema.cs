using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotoModelos.Enums
{
  
        public enum RolUsuario
        {
            Votante = 1,
            Administrador = 2,
            JefeJunta = 3
        }

        public enum TipoEleccion
        {
            Nominal = 1,
            Plancha = 2,
            Plurinominal = 3
        }

        public enum EstadoEleccion
        {
            Configuracion = 1,
            Activa = 2,
            Cerrada = 3
        }
    
}
