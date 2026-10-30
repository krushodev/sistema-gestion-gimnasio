using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SGIG.Datos;
using SGIG.Entidades;

namespace SGIG.Negocio
{
    /// <summary>
    /// Métricas, gráficos (Fase 11) y reportes por período (RF#24, ampliados en la Fase 14).
    /// Todo lo que recibe un rango lo valida acá y calcula acá lo derivado (agrupación por
    /// día o mes, porcentajes, indicadores): la pantalla sólo muestra.
    /// </summary>
    public class ServicioReporte
    {
        /// <summary>Hasta este largo (en días) los gráficos de evolución van por día; si no, por mes.</summary>
        public const int MaxDiasAgrupadoPorDia = 31;

        /// <summary>Tope de columnas para los gráficos agrupados por mes.</summary>
        public const int MaxMesesGrafico = 24;

        private const int HoraApertura = 6;
        private const int HoraCierre = 22;

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
            ValidarRango(desde, hasta);
            return _repoReporte.ObtenerReportePorPeriodo(desde, hasta);
        }

        /// <summary>"Desde" no puede ser posterior a "Hasta" (ambas fechas inclusive).</summary>
        public static void ValidarRango(DateTime desde, DateTime hasta)
        {
            if (desde.Date > hasta.Date)
                throw new NegocioException("La fecha \"Desde\" no puede ser posterior a la fecha \"Hasta\".");
        }

        /// <summary>Si un gráfico de evolución en este rango se agrupa por día (true) o por mes (false).</summary>
        public static bool AgrupaPorDia(DateTime desde, DateTime hasta) =>
            (hasta.Date - desde.Date).Days + 1 <= MaxDiasAgrupadoPorDia;

        // ── Gráficos ─────────────────────────────────────────────────────────

        /// <summary>Facturado por plan en el rango (ambas fechas inclusive), de mayor a menor.</summary>
        public List<PuntoGraficoDTO> ObtenerIngresosPorPlan(DateTime desde, DateTime hasta)
        {
            ValidarRango(desde, hasta);
            return _repoReporte.ObtenerIngresosPorPlan(desde.Date, hasta.Date.AddDays(1)).ToList();
        }

        /// <summary>
        /// Accesos del rango sumados por hora del día. Devuelve una columna por hora, de
        /// corrido, entre las 6 y las 22 (o más si hubo accesos fuera de esa franja).
        /// </summary>
        public List<PuntoGraficoDTO> ObtenerAccesosPorHora(DateTime desde, DateTime hasta)
        {
            ValidarRango(desde, hasta);
            var porHora = _repoReporte.ObtenerAccesosPorHora(desde.Date, hasta.Date.AddDays(1))
                .ToDictionary(a => a.Hora, a => a.Cantidad);

            int primera = porHora.Count > 0 ? Math.Min(HoraApertura, porHora.Keys.Min()) : HoraApertura;
            int ultima = porHora.Count > 0 ? Math.Max(HoraCierre, porHora.Keys.Max()) : HoraCierre;

            var puntos = new List<PuntoGraficoDTO>();
            for (int hora = primera; hora <= ultima; hora++)
            {
                puntos.Add(new PuntoGraficoDTO
                {
                    Etiqueta = hora.ToString("00"),
                    Valor = porHora.TryGetValue(hora, out var cantidad) ? cantidad : 0
                });
            }
            return puntos;
        }

        /// <summary>Facturado en el rango, por día (≤ 31 días) o por mes. El total coincide con la tarjeta "Ingresos del mes".</summary>
        public List<PuntoGraficoDTO> ObtenerIngresosEnElTiempo(DateTime desde, DateTime hasta) =>
            EnElTiempo(desde, hasta, _repoReporte.ObtenerIngresosPorDia);

        /// <summary>Mantenimientos iniciados en el rango, por día (≤ 31 días) o por mes.</summary>
        public List<PuntoGraficoDTO> ObtenerMantenimientosEnElTiempo(DateTime desde, DateTime hasta) =>
            EnElTiempo(desde, hasta, _repoReporte.ObtenerMantenimientosPorDia);

        /// <summary>
        /// Pide los totales diarios del rango y devuelve una columna por día o por mes, de
        /// corrido, completando con 0 los días o meses que la consulta no trajo.
        /// </summary>
        private static List<PuntoGraficoDTO> EnElTiempo(DateTime desde, DateTime hasta,
            Func<DateTime, DateTime, IEnumerable<TotalPorDiaDTO>> consultarPorDia)
        {
            ValidarRango(desde, hasta);
            desde = desde.Date;
            hasta = hasta.Date;

            var porDia = consultarPorDia(desde, hasta.AddDays(1)).ToList();
            var puntos = new List<PuntoGraficoDTO>();

            if (AgrupaPorDia(desde, hasta))
            {
                var totales = porDia.ToDictionary(d => d.Fecha.Date, d => d.Valor);
                for (var dia = desde; dia <= hasta; dia = dia.AddDays(1))
                {
                    puntos.Add(new PuntoGraficoDTO
                    {
                        Etiqueta = dia.ToString("dd/MM", CultureInfo.CurrentCulture),
                        Valor = totales.TryGetValue(dia, out var valor) ? valor : 0
                    });
                }
                return puntos;
            }

            var primerMes = new DateTime(desde.Year, desde.Month, 1);
            var ultimoMes = new DateTime(hasta.Year, hasta.Month, 1);
            int meses = (ultimoMes.Year - primerMes.Year) * 12 + ultimoMes.Month - primerMes.Month + 1;
            if (meses > MaxMesesGrafico)
                throw new NegocioException($"El rango no puede superar los {MaxMesesGrafico} meses para el gráfico.");

            var porMes = porDia
                .GroupBy(d => new DateTime(d.Fecha.Year, d.Fecha.Month, 1))
                .ToDictionary(g => g.Key, g => g.Sum(d => d.Valor));
            for (var mes = primerMes; mes <= ultimoMes; mes = mes.AddMonths(1))
            {
                puntos.Add(new PuntoGraficoDTO
                {
                    Etiqueta = NombreCortoMes(mes),
                    Valor = porMes.TryGetValue(mes, out var valor) ? valor : 0
                });
            }
            return puntos;
        }

        /// <summary>"Oct 26": mes abreviado en la cultura actual (sin el punto final) y año corto.</summary>
        private static string NombreCortoMes(DateTime mes)
        {
            var nombre = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(mes.Month).TrimEnd('.');
            if (nombre.Length > 0) nombre = char.ToUpper(nombre[0], CultureInfo.CurrentCulture) + nombre[1..];
            return $"{nombre} {mes:yy}";
        }

        // ── Reportes por período (Fase 14) ───────────────────────────────────

        /// <summary>Ingresos por plan y mes (RF#24), con el % de cada fila sobre el total del período.</summary>
        public ReporteDTO<FilaReporteConceptoDTO> ReporteIngresosPorPlan(DateTime desde, DateTime hasta)
        {
            var filas = GenerarReporte(desde, hasta).ToList();
            decimal total = filas.Sum(f => f.MontoTotal);
            int cuotas = filas.Sum(f => f.CantidadRegistros);
            foreach (var fila in filas) fila.Porcentaje = Porcentaje(fila.MontoTotal, total);

            var masVendido = filas
                .GroupBy(f => f.Concepto)
                .Select(g => (Plan: g.Key, Cantidad: g.Sum(f => f.CantidadRegistros)))
                .OrderByDescending(p => p.Cantidad)
                .FirstOrDefault();

            return new ReporteDTO<FilaReporteConceptoDTO>
            {
                Filas = filas,
                Indicadores =
                {
                    Indicador("Total facturado", Pesos(total), Plural(cuotas, "cuota emitida", "cuotas emitidas")),
                    Indicador("Ticket promedio", Pesos(cuotas == 0 ? 0 : total / cuotas), "Monto promedio por cuota"),
                    Indicador("Plan más vendido", masVendido.Plan ?? "—",
                        masVendido.Plan is null ? "Sin ventas en el período" : Plural(masVendido.Cantidad, "cuota", "cuotas")),
                }
            };
        }

        /// <summary>Cobrado por medio de pago, con su participación en el total.</summary>
        public ReporteDTO<FilaTotalDTO> ReporteIngresosPorMedioPago(DateTime desde, DateTime hasta)
        {
            ValidarRango(desde, hasta);
            var filas = _repoReporte.ObtenerIngresosPorMedioPago(desde.Date, hasta.Date.AddDays(1)).ToList();
            decimal total = filas.Sum(f => f.Monto);
            int cobros = filas.Sum(f => f.Cantidad);
            foreach (var fila in filas) fila.Porcentaje = Porcentaje(fila.Monto, total);
            var masUsado = filas.OrderByDescending(f => f.Cantidad).FirstOrDefault();

            return new ReporteDTO<FilaTotalDTO>
            {
                Filas = filas,
                Indicadores =
                {
                    Indicador("Total cobrado", Pesos(total), Plural(cobros, "cobro", "cobros")),
                    Indicador("Medio más usado", masUsado?.Concepto ?? "—",
                        masUsado is null ? "Sin cobros en el período" : $"{masUsado.Porcentaje:N1} % del total cobrado"),
                    Indicador("Medios utilizados", filas.Count.ToString(), "Distintos en el período"),
                }
            };
        }

        /// <summary>Cada cobro del período, con totales, socios distintos y ticket promedio.</summary>
        public ReporteDTO<FilaCobroDTO> ReporteDetalleCobros(DateTime desde, DateTime hasta)
        {
            ValidarRango(desde, hasta);
            var filas = _repoReporte.ObtenerDetalleCobros(desde.Date, hasta.Date.AddDays(1)).ToList();
            decimal total = filas.Sum(f => f.Monto);
            int socios = filas.Select(f => f.Documento).Distinct().Count();

            return new ReporteDTO<FilaCobroDTO>
            {
                Filas = filas,
                Indicadores =
                {
                    Indicador("Total cobrado", Pesos(total), Plural(filas.Count, "cobro", "cobros")),
                    Indicador("Socios que pagaron", socios.ToString(), "Distintos en el período"),
                    Indicador("Ticket promedio", Pesos(filas.Count == 0 ? 0 : total / filas.Count), "Monto promedio por cobro"),
                }
            };
        }

        /// <summary>Accesos por día, con el % de rechazos y el promedio diario sobre todos los días del rango.</summary>
        public ReporteDTO<FilaAsistenciaDTO> ReporteAsistencia(DateTime desde, DateTime hasta)
        {
            ValidarRango(desde, hasta);
            var filas = _repoReporte.ObtenerAsistenciaPorDia(desde.Date, hasta.Date.AddDays(1)).ToList();
            foreach (var fila in filas) fila.PorcentajeRechazo = Porcentaje(fila.Rechazados, fila.Total);

            int total = filas.Sum(f => f.Total);
            int concedidos = filas.Sum(f => f.Concedidos);
            int rechazados = filas.Sum(f => f.Rechazados);
            int dias = (hasta.Date - desde.Date).Days + 1;
            var pico = filas.OrderByDescending(f => f.Total).FirstOrDefault();

            return new ReporteDTO<FilaAsistenciaDTO>
            {
                Filas = filas,
                Indicadores =
                {
                    Indicador("Accesos", total.ToString("N0"), $"Promedio {(decimal)total / dias:N1} por día"),
                    Indicador("Concedidos", concedidos.ToString("N0"), $"{Porcentaje(concedidos, total):N1} % del total"),
                    Indicador("Rechazados", rechazados.ToString("N0"), $"{Porcentaje(rechazados, total):N1} % del total"),
                    Indicador("Día de más concurrencia", pico is null ? "—" : pico.Fecha.ToString("dd/MM/yyyy"),
                        pico is null ? "Sin accesos en el período" : Plural(pico.Total, "acceso", "accesos")),
                }
            };
        }

        /// <summary>Mantenimientos iniciados en el período, con días fuera de servicio y estado.</summary>
        public ReporteDTO<FilaMantenimientoDTO> ReporteMantenimientos(DateTime desde, DateTime hasta, DateTime hoy)
        {
            ValidarRango(desde, hasta);
            var filas = _repoReporte.ObtenerMantenimientosDelPeriodo(desde.Date, hasta.Date.AddDays(1)).ToList();
            foreach (var fila in filas)
            {
                fila.Estado = fila.Fin.HasValue ? "Finalizado" : "En curso";
                fila.Dias = ((fila.Fin ?? hoy).Date - fila.Inicio.Date).Days;
            }

            var finalizados = filas.Where(f => f.Fin.HasValue).ToList();
            var masIntervenida = filas
                .GroupBy(f => f.Maquina)
                .Select(g => (Maquina: g.Key, Cantidad: g.Count()))
                .OrderByDescending(m => m.Cantidad)
                .FirstOrDefault();

            return new ReporteDTO<FilaMantenimientoDTO>
            {
                Filas = filas,
                Indicadores =
                {
                    Indicador("Mantenimientos", filas.Count.ToString(), $"{filas.Count - finalizados.Count} en curso · {finalizados.Count} finalizados"),
                    Indicador("Reparación promedio",
                        finalizados.Count == 0 ? "—" : $"{finalizados.Average(f => f.Dias):N1} días",
                        "De los finalizados"),
                    Indicador("Máquina más intervenida", masIntervenida.Maquina ?? "—",
                        masIntervenida.Maquina is null ? "Sin mantenimientos en el período" : Plural(masIntervenida.Cantidad, "intervención", "intervenciones")),
                }
            };
        }

        // ── Ayudas de formato ────────────────────────────────────────────────

        private static IndicadorDTO Indicador(string titulo, string valor, string detalle) =>
            new() { Titulo = titulo, Valor = valor, Detalle = detalle };

        /// <summary>Porcentaje (0–100, un decimal) de una parte sobre el total; 0 si el total es 0.</summary>
        public static decimal Porcentaje(decimal parte, decimal total) =>
            total == 0 ? 0 : Math.Round(parte * 100 / total, 1);

        private static string Pesos(decimal monto) => $"$ {monto:N0}";

        private static string Plural(int cantidad, string singular, string plural) =>
            $"{cantidad:N0} {(cantidad == 1 ? singular : plural)}";
    }
}
