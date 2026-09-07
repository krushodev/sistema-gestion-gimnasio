using System;

namespace SGIG.Entidades
{
    /// <summary>
    /// Registro de un intento de acceso de un socio (RF#15, RF#16). Se genera uno por
    /// cada intento, tanto si fue concedido como rechazado (RF#17).
    /// </summary>
    public class Checkin
    {
        public int IdCheckin { get; set; }
        public int IdPersona { get; set; }
        public DateTime FechaHora { get; set; }

        /// <summary>"Concedido" o "Rechazado" (CK_Checkin_Resultado).</summary>
        public string Resultado { get; set; } = string.Empty;
    }
}
