using System;

namespace SGIG.Entidades
{
    /// <summary>
    /// Socio del gimnasio: una <see cref="Persona"/> que además puede tener una cuota
    /// activa. Hereda de Persona porque en la base es su especialización (comparten
    /// <see cref="Persona.IdPersona"/> como clave, patrón "tabla por subtipo").
    /// </summary>
    public class Socio : Persona
    {
        /// <summary>Observaciones médicas cargadas por recepción (texto libre).</summary>
        public string? AptoMedico { get; set; }

        /// <summary>
        /// Plan preferido del socio: sólo precarga el combo al facturar, no genera
        /// facturación por sí solo (ver nota histórica de la ERS v4.0 en CLAUDE.md).
        /// </summary>
        public int? IdPlan { get; set; }

        /// <summary>
        /// Caché de lectura rápida para el Check-in (RNF#01): la fuente de verdad es
        /// la Facturación vigente, esto sólo se actualiza al registrar un pago.
        /// </summary>
        public DateTime? FechaVencimientoCuota { get; set; }

        /// <summary>Baja lógica (RNF#03): nunca se borra físicamente un socio.</summary>
        public bool Activo { get; set; } = true;
    }
}
