using System;
using System.Collections.Generic;
using SGIG.Datos;
using SGIG.Entidades;

namespace SGIG.Negocio
{
    public class ServicioReporte
    {
        private readonly RepositorioReporte _repoReporte;

        public ServicioReporte()
        {
            _repoReporte = new RepositorioReporte();
        }

        public MetricasDashboardDTO ObtenerMetricas()
        {
            return _repoReporte.ObtenerMetricasDashboard();
        }

        public IEnumerable<FilaReporteConceptoDTO> GenerarReporte(DateTime desde, DateTime hasta)
        {
            if (desde > hasta)
                throw new NegocioException("La fecha 'Desde' no puede ser posterior a la fecha 'Hasta'.");

            return _repoReporte.ObtenerReportePorPeriodo(desde, hasta);
        }
    }
}