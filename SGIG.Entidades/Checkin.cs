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

        /// <summary>Resuelto por JOIN para el historial de accesos; no se persiste.</summary>
        public string? Documento { get; set; }

        /// <summary>Resuelto por JOIN para el historial de accesos; no se persiste.</summary>
        public string? NombreCompleto { get; set; }

        /// <summary>
        /// Días entre hoy y <c>Socio.fecha_vencimiento_cuota</c> al momento de la
        /// consulta (negativo si ya venció). Resuelto por JOIN; no se persiste.
        /// </summary>
        public int? DiasRestantesCuota { get; set; }

        /// <summary>Texto listo para grilla, derivado de <see cref="DiasRestantesCuota"/>.</summary>
        public string DescripcionVencimiento => DiasRestantesCuota switch
        {
            null => "Sin cuota registrada",
            0 => "Vence hoy",
            > 0 => $"Vence en {DiasRestantesCuota} día(s)",
            _ => $"Vencida hace {-DiasRestantesCuota} día(s)"
        };
    }
}
