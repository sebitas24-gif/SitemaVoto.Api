using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotoModelos
{
    internal class Candidato
    {
        [Key] public int Id { get; set; }
        public int IdVotante { get; set; }

        public string Partido {  get; set; }
        
        public string Eslogan {  get; set; }
    }
}
