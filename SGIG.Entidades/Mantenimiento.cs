using System;

namespace SGIG.Entidades
{
    /// <summary>
    /// Intervención técnica sobre una <see cref="Maquina"/> (RF#19, RF#20, RF#21).
    /// El técnico a cargo es un <see cref="Usuario"/> con rol Técnico.
    /// </summary>
    public class Mantenimiento
    {
        public int IdMantenimiento { get; set; }
        public int IdMaquina { get; set; }

        /// <summary>id_persona del técnico a cargo (dbo.Usuario).</summary>
        public int IdPersona { get; set; }

        public DateTime FechaInicio { get; set; }

        /// <summary>Null mientras el mantenimiento está en curso.</summary>
        public DateTime? FechaFin { get; set; }

        public string? DetalleTecnico { get; set; }

        /// <summary>Resuelto por JOIN para grillas; no se persiste.</summary>
        public string? NombreMaquina { get; set; }

        /// <summary>Resuelto por JOIN para grillas; no se persiste.</summary>
        public string? NombreTecnico { get; set; }
    }
}
