using System;
using System.Collections.Generic;
using Dapper;
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
                    NuevasAltasMes = (int)socios.AlDia, // Representa socios con cuota al día
                    CheckinsPromedioDia = Math.Round(checkinsProm, 1)
                };
            }
            catch (Exception ex)
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
            catch (Exception ex)
            {
                throw new AccesoDatosException("Error al consultar el reporte financiero: " + ex.Message, ex);
            }
        }
    }
}