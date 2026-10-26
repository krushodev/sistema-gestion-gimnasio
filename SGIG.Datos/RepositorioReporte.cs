using System;
using System.Collections.Generic;
using Dapper;
using Microsoft.Data.SqlClient;
using SGIG.Entidades;

namespace SGIG.Datos
{
    public class RepositorioReporte
    {
        public MetricasDashboardDTO ObtenerMetricasDashboard()
        {
            try
            {
                using var conn = Conexion.ObtenerConexionAbierta();

                // 1. Ingresos totales cobrados/facturados este mes
                string sqlIngresos = @"
                    SELECT ISNULL(SUM(monto_total), 0) 
                    FROM Facturacion 
                    WHERE MONTH(fecha_emision) = MONTH(GETDATE()) 
                      AND YEAR(fecha_emision) = YEAR(GETDATE());";

                // 2. Socios: Activos totales y con cuota al día este mes
                string sqlSocios = @"
                    SELECT 
                        COUNT(CASE WHEN activo = 1 THEN 1 END) AS Activos,
                        COUNT(CASE WHEN activo = 1 AND fecha_vencimiento_cuota >= CAST(GETDATE() AS DATE) THEN 1 END) AS AlDia
                    FROM Socio;";

                // 3. Check-ins promedio diario en el último mes
                string sqlCheckins = @"
                    SELECT ISNULL(CAST(COUNT(*) AS FLOAT) / NULLIF(COUNT(DISTINCT CAST(fecha_hora AS DATE)), 0), 0)
                    FROM Checkin
                    WHERE fecha_hora >= DATEADD(DAY, -30, GETDATE());";

                decimal ingresos = conn.ExecuteScalar<decimal>(sqlIngresos);
                var socios = conn.QuerySingle(sqlSocios);
                double checkinsProm = conn.ExecuteScalar<double>(sqlCheckins);

                return new MetricasDashboardDTO
                {
                    IngresosMes = ingresos,
                    SociosActivos = (int)socios.Activos,
                    SociosAlDia = (int)socios.AlDia,
                    CheckinsPromedioDia = Math.Round(checkinsProm, 1)
                };
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("Error al obtener las métricas del dashboard: " + ex.Message, ex);
            }
        }

        public IEnumerable<FilaReporteConceptoDTO> ObtenerReportePorPeriodo(DateTime desde, DateTime hasta)
        {
            try
            {
                using var conn = Conexion.ObtenerConexionAbierta();

                string sql = @"
                    SELECT 
                        p.nombre AS Concepto,
                        FORMAT(f.fecha_emision, 'MMMM yyyy', 'es-ES') AS Periodo,
                        COUNT(f.id_facturacion) AS CantidadRegistros,
                        SUM(f.monto_total) AS MontoTotal
                    FROM Facturacion f
                    INNER JOIN [Plan] p ON f.id_plan = p.id_plan
                    WHERE f.fecha_emision >= @Desde AND f.fecha_emision <= @Hasta
                    GROUP BY p.nombre, FORMAT(f.fecha_emision, 'MMMM yyyy', 'es-ES')
                    ORDER BY MontoTotal DESC;";

                return conn.Query<FilaReporteConceptoDTO>(sql, new
                {
                    Desde = desde.Date,
                    Hasta = hasta.Date.AddDays(1).AddTicks(-1)
                });
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("Error al consultar el reporte financiero: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Facturado por plan entre dos fechas (hasta exclusiva). Mismo criterio que la tarjeta
        /// "Ingresos del mes" (Facturacion.monto_total por fecha_emision). Incluye los planes
        /// activos sin facturación, en 0.
        /// </summary>
        public IEnumerable<PuntoGraficoDTO> ObtenerIngresosPorPlan(DateTime desde, DateTime hastaExclusiva)
        {
            const string sql = @"
                SELECT p.nombre AS Etiqueta, ISNULL(SUM(f.monto_total), 0) AS Valor
                FROM dbo.[Plan] p
                LEFT JOIN dbo.Facturacion f
                       ON f.id_plan = p.id_plan
                      AND f.fecha_emision >= @Desde
                      AND f.fecha_emision < @Hasta
                WHERE p.activo = 1 OR f.id_facturacion IS NOT NULL
                GROUP BY p.nombre
                ORDER BY Valor DESC, p.nombre;";

            try
            {
                using var conn = Conexion.ObtenerConexionAbierta();
                return conn.Query<PuntoGraficoDTO>(sql, new { Desde = desde.Date, Hasta = hastaExclusiva.Date });
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("Error al consultar los ingresos por plan: " + ex.Message, ex);
            }
        }

        /// <summary>Intentos de acceso (concedidos y rechazados) de un día, agrupados por hora.</summary>
        public IEnumerable<AccesosPorHoraDTO> ObtenerAccesosPorHora(DateTime dia)
        {
            const string sql = @"
                SELECT DATEPART(HOUR, fecha_hora) AS Hora, COUNT(*) AS Cantidad
                FROM dbo.Checkin
                WHERE fecha_hora >= @Desde AND fecha_hora < @Hasta
                GROUP BY DATEPART(HOUR, fecha_hora)
                ORDER BY Hora;";

            try
            {
                using var conn = Conexion.ObtenerConexionAbierta();
                return conn.Query<AccesosPorHoraDTO>(sql, new { Desde = dia.Date, Hasta = dia.Date.AddDays(1) });
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("Error al consultar los accesos por hora: " + ex.Message, ex);
            }
        }

        /// <summary>Mantenimientos iniciados entre dos fechas (hasta exclusiva), agrupados por año y mes.</summary>
        public IEnumerable<TotalPorMesDTO> ObtenerMantenimientosPorMes(DateTime desde, DateTime hastaExclusiva)
        {
            const string sql = @"
                SELECT YEAR(fecha_inicio) AS Anio, MONTH(fecha_inicio) AS Mes, COUNT(*) AS Valor
                FROM dbo.Mantenimiento
                WHERE fecha_inicio >= @Desde AND fecha_inicio < @Hasta
                GROUP BY YEAR(fecha_inicio), MONTH(fecha_inicio)
                ORDER BY Anio, Mes;";

            try
            {
                using var conn = Conexion.ObtenerConexionAbierta();
                return conn.Query<TotalPorMesDTO>(sql, new { Desde = desde.Date, Hasta = hastaExclusiva.Date });
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("Error al consultar los mantenimientos por mes: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Facturado por mes entre dos fechas (hasta exclusiva). Mismo criterio que la tarjeta
        /// "Ingresos del mes": Facturacion.monto_total por fecha_emision.
        /// </summary>
        public IEnumerable<TotalPorMesDTO> ObtenerIngresosPorMes(DateTime desde, DateTime hastaExclusiva)
        {
            const string sql = @"
                SELECT YEAR(fecha_emision) AS Anio, MONTH(fecha_emision) AS Mes, SUM(monto_total) AS Valor
                FROM dbo.Facturacion
                WHERE fecha_emision >= @Desde AND fecha_emision < @Hasta
                GROUP BY YEAR(fecha_emision), MONTH(fecha_emision)
                ORDER BY Anio, Mes;";

            try
            {
                using var conn = Conexion.ObtenerConexionAbierta();
                return conn.Query<TotalPorMesDTO>(sql, new { Desde = desde.Date, Hasta = hastaExclusiva.Date });
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("Error al consultar los ingresos por mes: " + ex.Message, ex);
            }
        }
    }
}