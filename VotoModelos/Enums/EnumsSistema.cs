using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotoModelos.Enums
{

    public enum RolUsuario
    {
        Admin = 1,
        JefeJunta = 2,
        Votante = 3,
        Candidato = 4
    }
    public enum MetodoOtp
    {
        Correo = 1,
        Sms = 2
    }

    public enum EstadoProceso
    {
        Configuracion = 1, // No iniciado / preparando
        Activo = 2,
        Cerrado = 3
    }
    public enum TipoEleccion
    {
        Nominal = 1,       // 1 candidato
        Plancha = 2,       // binomio
        Plurinominal = 3   // varios escaños
    }
    public enum TipoVoto
    {
        Candidato = 1,
        Blanco = 2
    }

}
