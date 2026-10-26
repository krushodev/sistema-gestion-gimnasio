using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

        /// <summary>Gráfico del Administrador: facturado por plan en el mes de <paramref name="fechaDelMes"/>.</summary>
        public List<PuntoGraficoDTO> ObtenerIngresosPorPlan(DateTime fechaDelMes)
        {
            var desde = new DateTime(fechaDelMes.Year, fechaDelMes.Month, 1);
            return _repoReporte.ObtenerIngresosPorPlan(desde, desde.AddMonths(1)).ToList();
        }

        /// <summary>
        /// Gráfico del Recepcionista: accesos del día por hora. Devuelve una columna por hora,
        /// de corrido, entre las 6 y las 22 (o más si hubo accesos fuera de esa franja).
        /// </summary>
        public List<PuntoGraficoDTO> ObtenerAccesosPorHora(DateTime dia)
        {
            var porHora = _repoReporte.ObtenerAccesosPorHora(dia).ToDictionary(a => a.Hora, a => a.Cantidad);

            int desde = porHora.Count > 0 ? Math.Min(HoraApertura, porHora.Keys.Min()) : HoraApertura;
            int hasta = porHora.Count > 0 ? Math.Max(HoraCierre, porHora.Keys.Max()) : HoraCierre;

            var puntos = new List<PuntoGraficoDTO>();
            for (int hora = desde; hora <= hasta; hora++)
            {
                puntos.Add(new PuntoGraficoDTO
                {
                    Etiqueta = hora.ToString("00"),
                    Valor = porHora.TryGetValue(hora, out var cantidad) ? cantidad : 0
                });
            }
            return puntos;
        }

        /// <summary>
        /// Gráfico del Técnico: mantenimientos iniciados por mes en los últimos
        /// <paramref name="meses"/> meses (incluido el de <paramref name="fechaDelUltimoMes"/>),
        /// con los meses sin registros en 0.
        /// </summary>
        public List<PuntoGraficoDTO> ObtenerMantenimientosPorMes(DateTime fechaDelUltimoMes, int meses = 6) =>
            PorMes(fechaDelUltimoMes, meses, _repoReporte.ObtenerMantenimientosPorMes);

        /// <summary>
        /// Segundo gráfico del Administrador: facturado por mes en los últimos
        /// <paramref name="meses"/> meses (incluido el de <paramref name="fechaDelUltimoMes"/>),
        /// con los meses sin facturación en 0. El último mes coincide con la tarjeta "Ingresos del mes".
        /// </summary>
        public List<PuntoGraficoDTO> ObtenerIngresosPorMes(DateTime fechaDelUltimoMes, int meses = 12) =>
            PorMes(fechaDelUltimoMes, meses, _repoReporte.ObtenerIngresosPorMes);

        /// <summary>
        /// Calcula el rango de meses, pide los totales al repositorio y devuelve una columna
        /// por mes, de corrido, completando con 0 los meses que la consulta no trajo.
        /// </summary>
        private static List<PuntoGraficoDTO> PorMes(DateTime fechaDelUltimoMes, int meses,
            Func<DateTime, DateTime, IEnumerable<TotalPorMesDTO>> consultar)
        {
            if (meses < 1 || meses > 24)
                throw new NegocioException("La cantidad de meses del gráfico debe estar entre 1 y 24.");

            var primerDiaUltimoMes = new DateTime(fechaDelUltimoMes.Year, fechaDelUltimoMes.Month, 1);
            var desde = primerDiaUltimoMes.AddMonths(-(meses - 1));
            var porMes = consultar(desde, primerDiaUltimoMes.AddMonths(1))
                .ToDictionary(m => (m.Anio, m.Mes), m => m.Valor);

            var puntos = new List<PuntoGraficoDTO>();
            for (var mes = desde; mes <= primerDiaUltimoMes; mes = mes.AddMonths(1))
            {
                puntos.Add(new PuntoGraficoDTO
                {
                    Etiqueta = NombreCortoMes(mes),
                    Valor = porMes.TryGetValue((mes.Year, mes.Month), out var cantidad) ? cantidad : 0
                });
            }
            return puntos;
        }

        private const int HoraApertura = 6;
        private const int HoraCierre = 22;

        /// <summary>"Oct 26": mes abreviado en la cultura actual (sin el punto final) y año corto.</summary>
        private static string NombreCortoMes(DateTime mes)
        {
            var nombre = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(mes.Month).TrimEnd('.');
            if (nombre.Length > 0) nombre = char.ToUpper(nombre[0], CultureInfo.CurrentCulture) + nombre[1..];
            return $"{nombre} {mes:yy}";
        }
    }
}