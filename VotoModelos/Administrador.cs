using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotoModelos
{
    internal class Administrador
    {
        [Key] public int Id { get; set; }
        public int IdVotante {  get; set; }
    }
}
