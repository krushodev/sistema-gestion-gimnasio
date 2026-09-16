using System.Collections.Generic;
using Dapper;
using Microsoft.Data.SqlClient;
using SGIG.Entidades;

namespace SGIG.Datos;

/// <summary>
/// Acceso a datos de dbo.[Plan]. Plan es un catálogo sembrado con la aplicación
/// (ver docs/SGIG_CreateDB.sql) y no tiene ABM en la UI; este repositorio solo
/// expone lectura, usada por Cobro de Cuotas y por la consulta de frmPlanes.
/// </summary>
public class RepositorioPlan
{
    public IEnumerable<Plan> ObtenerTodos(bool soloActivos = true)
    {
        string sql = @"
            SELECT id_plan AS IdPlan,
                   nombre AS Nombre,
                   precio AS Precio,
                   tipo_periodicidad AS TipoPeriodicidad,
                   activo AS Activo
            FROM dbo.[Plan]" +
            (soloActivos ? " WHERE activo = 1" : "") +
            " ORDER BY nombre";

        try
        {
            using var connection = Conexion.ObtenerConexionAbierta();
            return connection.Query<Plan>(sql);
        }
        catch (SqlException ex)
        {
            throw new AccesoDatosException("Error al obtener los planes.", ex);
        }
    }

    public Plan? ObtenerPorId(int idPlan)
    {
        const string sql = @"
            SELECT id_plan AS IdPlan,
                   nombre AS Nombre,
                   precio AS Precio,
                   tipo_periodicidad AS TipoPeriodicidad,
                   activo AS Activo
            FROM dbo.[Plan]
            WHERE id_plan = @IdPlan";

        try
        {
            using var connection = Conexion.ObtenerConexionAbierta();
            return connection.QueryFirstOrDefault<Plan>(sql, new { IdPlan = idPlan });
        }
        catch (SqlException ex)
        {
            throw new AccesoDatosException("Error al obtener el plan.", ex);
        }
    }
}
