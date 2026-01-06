using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using BCrypt.Net;

    namespace VotoModelos
    {
        public class Voto
        {
            [Key] public int Id { get; set; }

            public int IdOpcionElectoral{ get; set; }

            public string? VotoEncriptado { get; set; }

            public DateTime FechaVoto { get; set; }
            public ProcesoElectoral ProcesoElectoral{ get;set; }
         
        }
    }
