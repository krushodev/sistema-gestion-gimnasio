using System;
using System.Collections.Generic;

namespace SGIG.Entidades
{
    /// <summary>
    /// Todo lo que muestra el perfil de un socio (Fase 12), armado por
    /// <c>ServicioSocio.ObtenerPerfil</c>. Sólo datos: los cálculos (estado de la cuota,
    /// edad, totales) los hace el servicio.
    /// </summary>
    public class PerfilSocioDTO
    {
        public Socio Socio { get; set; } = new();

        public string TipoDocumento { get; set; } = string.Empty;

        /// <summary>"Localidad, Provincia", o null si no la cargó.</summary>
        public string? Localidad { get; set; }

        public int? Edad { get; set; }

        /// <summary>Plan de la última facturación pagada (el que realmente está usando).</summary>
        public string? PlanActual { get; set; }

        /// <summary>Plan preferido (<see cref="Socio.IdPlan"/>), sólo precarga el cobro.</summary>
        public string? PlanPreferido { get; set; }

        /// <summary>Días hasta el vencimiento (negativo si ya venció); null si nunca pagó.</summary>
        public int? DiasParaVencer { get; set; }

        /// <summary>"Al día", "Por vencer", "Vence hoy", "Vencida" o "Sin cuotas".</summary>
        public string EstadoCuota { get; set; } = string.Empty;

        /// <summary>
        /// Si el perfil trae la parte económica (pagos, montos, total). Sólo el
        /// Administrador la recibe; para el Recepcionista viene en false y
        /// <see cref="Pagos"/> vacío.
        /// </summary>
        public bool IncluyeDatosEconomicos { get; set; }

        /// <summary>Pagos del socio, del más reciente al más antiguo (vacío sin datos económicos).</summary>
        public List<Pago> Pagos { get; set; } = new();

        public decimal TotalPagado { get; set; }

        /// <summary>Intentos de acceso de los últimos 90 días, del más reciente al más antiguo.</summary>
        public List<Checkin> Accesos { get; set; } = new();

        public int AccesosUltimos30Dias { get; set; }
    }
}
