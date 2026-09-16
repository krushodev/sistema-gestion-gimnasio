using System;
using System.Collections.Generic;
using Dapper;
using Microsoft.Data.SqlClient;
using SGIG.Entidades;

namespace SGIG.Datos;

public class RepositorioTesoreria
{
    public IEnumerable<Pago> ObtenerHistorialPorSocio(int idPersona)
    {
        const string sql = @"
            SELECT 
                p.id_pago AS IdPago,
                p.id_facturacion AS IdFacturacion,
                p.fecha_pago AS FechaPago,
                p.monto AS Monto,
                f.fecha_vencimiento AS FechaVencimiento,
                pl.nombre AS PlanNombre,
                mp.descripcion AS MedioPagoDesc
            FROM dbo.Pago p
            INNER JOIN dbo.Facturacion f ON p.id_facturacion = f.id_facturacion
            INNER JOIN dbo.[Plan] pl ON f.id_plan = pl.id_plan
            INNER JOIN dbo.MedioPago mp ON p.id_medio_pago = mp.id_medio_pago
            WHERE f.id_persona = @IdPersona
            ORDER BY p.id_pago DESC";

        try
        {
            using var connection = Conexion.ObtenerConexionAbierta();
            return connection.Query<Pago>(sql, new { IdPersona = idPersona });
        }
        catch (SqlException ex)
        {
            throw new AccesoDatosException("Error al consultar el historial de pagos del socio.", ex);
        }
    }

    public void RegistrarCobroTransaccional(Facturacion facturacion, Pago pago, DateTime nuevoVencimiento)
    {
        const string sqlFacturacion = @"
            INSERT INTO dbo.Facturacion 
                (id_persona, id_plan, fecha_emision, fecha_vencimiento, monto_total, estado)
            VALUES 
                (@IdPersona, @IdPlan, @FechaEmision, @FechaVencimiento, @MontoTotal, @Estado);
            SELECT CAST(SCOPE_IDENTITY() AS int);";

        const string sqlPago = @"
            INSERT INTO dbo.Pago 
                (id_facturacion, id_medio_pago, fecha_pago, monto)
            VALUES 
                (@IdFacturacion, @IdMedioPago, @FechaPago, @Monto);";

        const string sqlSocio = @"
            UPDATE dbo.Socio
            SET id_plan = @IdPlan,
                fecha_vencimiento_cuota = @FechaVencimientoCuota
            WHERE id_persona = @IdPersona;";

        using var connection = Conexion.ObtenerConexionAbierta();
        using var transaction = connection.BeginTransaction();

        try
        {
            int idFacturacion = connection.ExecuteScalar<int>(sqlFacturacion, facturacion, transaction);

            pago.IdFacturacion = idFacturacion;
            connection.Execute(sqlPago, pago, transaction);

            connection.Execute(sqlSocio, new
            {
                facturacion.IdPlan,
                FechaVencimientoCuota = nuevoVencimiento,
                facturacion.IdPersona
            }, transaction);

            transaction.Commit();
        }
        catch (SqlException ex)
        {
            transaction.Rollback();
            throw new AccesoDatosException("No se pudo completar el cobro transaccional del socio.", ex);
        }
    }
}