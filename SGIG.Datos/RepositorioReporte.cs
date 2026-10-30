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


        /// <summary>Intentos de acceso (concedidos y rechazados) entre dos fechas (hasta exclusiva), agrupados por hora del día.</summary>
        public IEnumerable<AccesosPorHoraDTO> ObtenerAccesosPorHora(DateTime desde, DateTime hastaExclusiva)
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
                return conn.Query<AccesosPorHoraDTO>(sql, new { Desde = desde.Date, Hasta = hastaExclusiva.Date });
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("Error al consultar los accesos por hora: " + ex.Message, ex);
            }
        }

        /// <summary>Mantenimientos iniciados entre dos fechas (hasta exclusiva), contados por día.</summary>
        public IEnumerable<TotalPorDiaDTO> ObtenerMantenimientosPorDia(DateTime desde, DateTime hastaExclusiva)
        {
            const string sql = @"
                SELECT fecha_inicio AS Fecha, COUNT(*) AS Valor
                FROM dbo.Mantenimiento
                WHERE fecha_inicio >= @Desde AND fecha_inicio < @Hasta
                GROUP BY fecha_inicio
                ORDER BY Fecha;";

            try
            {
                using var conn = Conexion.ObtenerConexionAbierta();
                return conn.Query<TotalPorDiaDTO>(sql, new { Desde = desde.Date, Hasta = hastaExclusiva.Date });
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("Error al consultar los mantenimientos por día: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Facturado por día entre dos fechas (hasta exclusiva). Mismo criterio que la tarjeta
        /// "Ingresos del mes": Facturacion.monto_total por fecha_emision.
        /// </summary>
        public IEnumerable<TotalPorDiaDTO> ObtenerIngresosPorDia(DateTime desde, DateTime hastaExclusiva)
        {
            const string sql = @"
                SELECT fecha_emision AS Fecha, SUM(monto_total) AS Valor
                FROM dbo.Facturacion
                WHERE fecha_emision >= @Desde AND fecha_emision < @Hasta
                GROUP BY fecha_emision
                ORDER BY Fecha;";

            try
            {
                using var conn = Conexion.ObtenerConexionAbierta();
                return conn.Query<TotalPorDiaDTO>(sql, new { Desde = desde.Date, Hasta = hastaExclusiva.Date });
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("Error al consultar los ingresos por día: " + ex.Message, ex);
            }
        }

        // ── Reportes por período (Fase 14) ───────────────────────────────────

        /// <summary>Cobrado por medio de pago entre dos fechas (hasta exclusiva), por fecha de pago.</summary>
        public IEnumerable<FilaTotalDTO> ObtenerIngresosPorMedioPago(DateTime desde, DateTime hastaExclusiva)
        {
            const string sql = @"
                SELECT mp.descripcion AS Concepto, COUNT(*) AS Cantidad, SUM(p.monto) AS Monto
                FROM dbo.Pago p
                INNER JOIN dbo.MedioPago mp ON mp.id_medio_pago = p.id_medio_pago
                WHERE p.fecha_pago >= @Desde AND p.fecha_pago < @Hasta
                GROUP BY mp.descripcion
                ORDER BY Monto DESC;";

            try
            {
                using var conn = Conexion.ObtenerConexionAbierta();
                return conn.Query<FilaTotalDTO>(sql, new { Desde = desde.Date, Hasta = hastaExclusiva.Date });
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("Error al consultar los ingresos por medio de pago: " + ex.Message, ex);
            }
        }

        /// <summary>Cada pago del período (hasta exclusiva), del más reciente al más antiguo.</summary>
        public IEnumerable<FilaCobroDTO> ObtenerDetalleCobros(DateTime desde, DateTime hastaExclusiva)
        {
            const string sql = @"
                SELECT p.fecha_pago AS FechaPago,
                       pe.apellido + ', ' + pe.nombre AS Socio,
                       pe.documento AS Documento,
                       pl.nombre AS [Plan],
                       mp.descripcion AS MedioPago,
                       p.monto AS Monto,
                       f.fecha_vencimiento AS CubreHasta
                FROM dbo.Pago p
                INNER JOIN dbo.Facturacion f ON f.id_facturacion = p.id_facturacion
                INNER JOIN dbo.Persona pe ON pe.id_persona = f.id_persona
                INNER JOIN dbo.[Plan] pl ON pl.id_plan = f.id_plan
                INNER JOIN dbo.MedioPago mp ON mp.id_medio_pago = p.id_medio_pago
                WHERE p.fecha_pago >= @Desde AND p.fecha_pago < @Hasta
                ORDER BY p.fecha_pago DESC, p.id_pago DESC;";

            try
            {
                using var conn = Conexion.ObtenerConexionAbierta();
                return conn.Query<FilaCobroDTO>(sql, new { Desde = desde.Date, Hasta = hastaExclusiva.Date });
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("Error al consultar el detalle de cobros: " + ex.Message, ex);
            }
        }

        /// <summary>Intentos de acceso por día del período (hasta exclusiva), separados por resultado.</summary>
        public IEnumerable<FilaAsistenciaDTO> ObtenerAsistenciaPorDia(DateTime desde, DateTime hastaExclusiva)
        {
            const string sql = @"
                SELECT CAST(fecha_hora AS DATE) AS Fecha,
                       COUNT(*) AS Total,
                       SUM(CASE WHEN resultado = 'Concedido' THEN 1 ELSE 0 END) AS Concedidos,
                       SUM(CASE WHEN resultado = 'Rechazado' THEN 1 ELSE 0 END) AS Rechazados
                FROM dbo.Checkin
                WHERE fecha_hora >= @Desde AND fecha_hora < @Hasta
                GROUP BY CAST(fecha_hora AS DATE)
                ORDER BY Fecha DESC;";

            try
            {
                using var conn = Conexion.ObtenerConexionAbierta();
                return conn.Query<FilaAsistenciaDTO>(sql, new { Desde = desde.Date, Hasta = hastaExclusiva.Date });
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("Error al consultar la asistencia por día: " + ex.Message, ex);
            }
        }

        /// <summary>Mantenimientos iniciados en el período (hasta exclusiva), del más reciente al más antiguo.</summary>
        public IEnumerable<FilaMantenimientoDTO> ObtenerMantenimientosDelPeriodo(DateTime desde, DateTime hastaExclusiva)
        {
            const string sql = @"
                SELECT maq.nombre AS Maquina, maq.marca AS Marca,
                       pe.apellido + ', ' + pe.nombre AS Tecnico,
                       m.fecha_inicio AS Inicio, m.fecha_fin AS Fin,
                       m.detalle_tecnico AS Detalle
                FROM dbo.Mantenimiento m
                INNER JOIN dbo.Maquina maq ON maq.id_maquina = m.id_maquina
                INNER JOIN dbo.Persona pe ON pe.id_persona = m.id_persona
                WHERE m.fecha_inicio >= @Desde AND m.fecha_inicio < @Hasta
                ORDER BY m.fecha_inicio DESC, m.id_mantenimiento DESC;";

            try
            {
                using var conn = Conexion.ObtenerConexionAbierta();
                return conn.Query<FilaMantenimientoDTO>(sql, new { Desde = desde.Date, Hasta = hastaExclusiva.Date });
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("Error al consultar los mantenimientos del período: " + ex.Message, ex);
            }
        }
    }
}
