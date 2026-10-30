using System;
using System.Collections.Generic;

namespace SGIG.Entidades
{
    public class MetricasDashboardDTO
    {
        public decimal IngresosMes { get; set; }
        public int SociosActivos { get; set; }
        public int SociosAlDia { get; set; }
        public double CheckinsPromedioDia { get; set; }
    }

    /// <summary>Fila del reporte "Ingresos por plan": plan y mes, con su participación en el total.</summary>
    public class FilaReporteConceptoDTO
    {
        public string Concepto { get; set; } = string.Empty;
        public string Periodo { get; set; } = string.Empty;
        public int CantidadRegistros { get; set; }
        public decimal MontoTotal { get; set; }

        /// <summary>Porcentaje del total del período (0–100). Lo calcula el servicio.</summary>
        public decimal Porcentaje { get; set; }
    }

    /// <summary>Una barra/columna de un gráfico: rótulo y valor.</summary>
    public class PuntoGraficoDTO
    {
        public string Etiqueta { get; set; } = string.Empty;
        public decimal Valor { get; set; }
    }

    /// <summary>Cantidad de intentos de acceso en una hora del día (sólo horas con datos).</summary>
    public class AccesosPorHoraDTO
    {
        public int Hora { get; set; }
        public int Cantidad { get; set; }
    }

    /// <summary>
    /// Un total diario (sólo días con datos): cantidad de mantenimientos o monto facturado,
    /// según la consulta que lo devuelva. El servicio lo agrupa por mes si el rango es largo.
    /// </summary>
    public class TotalPorDiaDTO
    {
        public DateTime Fecha { get; set; }
        public decimal Valor { get; set; }
    }

    /// <summary>Un indicador del período para las tarjetas de Reportes ("Total cobrado", "$ 470.000", "en 12 cobros").</summary>
    public class IndicadorDTO
    {
        public string Titulo { get; set; } = string.Empty;
        public string Valor { get; set; } = string.Empty;
        public string Detalle { get; set; } = string.Empty;
    }

    /// <summary>Resultado de un reporte: sus filas y los indicadores del período.</summary>
    public class ReporteDTO<TFila>
    {
        public List<TFila> Filas { get; set; } = new();
        public List<IndicadorDTO> Indicadores { get; set; } = new();
    }

    /// <summary>Fila de "Ingresos por medio de pago".</summary>
    public class FilaTotalDTO
    {
        public string Concepto { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal Monto { get; set; }
        public decimal Porcentaje { get; set; }
    }

    /// <summary>Fila de "Detalle de cobros": un pago.</summary>
    public class FilaCobroDTO
    {
        public DateTime FechaPago { get; set; }
        public string Socio { get; set; } = string.Empty;
        public string Documento { get; set; } = string.Empty;
        public string Plan { get; set; } = string.Empty;
        public string MedioPago { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public DateTime CubreHasta { get; set; }
    }

    /// <summary>Fila de "Asistencia por día".</summary>
    public class FilaAsistenciaDTO
    {
        public DateTime Fecha { get; set; }
        public int Total { get; set; }
        public int Concedidos { get; set; }
        public int Rechazados { get; set; }
        public decimal PorcentajeRechazo { get; set; }
    }

    /// <summary>Fila de "Mantenimientos": una intervención iniciada en el período.</summary>
    public class FilaMantenimientoDTO
    {
        public string Maquina { get; set; } = string.Empty;
        public string? Marca { get; set; }
        public string Tecnico { get; set; } = string.Empty;
        public DateTime Inicio { get; set; }
        public DateTime? Fin { get; set; }

        /// <summary>Días fuera de servicio (hasta hoy si sigue en curso). Lo calcula el servicio.</summary>
        public int Dias { get; set; }

        /// <summary>"En curso" o "Finalizado". Lo calcula el servicio.</summary>
        public string Estado { get; set; } = string.Empty;

        public string? Detalle { get; set; }
    }
}
