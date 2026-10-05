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
}