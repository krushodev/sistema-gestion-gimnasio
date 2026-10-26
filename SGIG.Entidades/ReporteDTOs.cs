using System;

namespace SGIG.Entidades
{
    public class MetricasDashboardDTO
    {
        public decimal IngresosMes { get; set; }
        public int SociosActivos { get; set; }
        public int SociosAlDia { get; set; }
        public double CheckinsPromedioDia { get; set; }
    }

    public class FilaReporteConceptoDTO
    {
        public string Concepto { get; set; } = string.Empty;
        public string Periodo { get; set; } = string.Empty;
        public int CantidadRegistros { get; set; }
        public decimal MontoTotal { get; set; }
    }

    /// <summary>Una barra/columna de un gráfico del Inicio: rótulo y valor.</summary>
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
    /// Un total mensual (sólo meses con datos): cantidad de mantenimientos o monto
    /// facturado, según la consulta que lo devuelva.
    /// </summary>
    public class TotalPorMesDTO
    {
        public int Anio { get; set; }
        public int Mes { get; set; }
        public decimal Valor { get; set; }
    }
}