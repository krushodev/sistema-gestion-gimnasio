using System;

namespace SGIG.Entidades
{
    public class Socio : Persona
    {
        public int IdSocio { get; set; }
        public DateTime FechaAlta { get; set; }
        public bool Activo { get; set; }
    }
}