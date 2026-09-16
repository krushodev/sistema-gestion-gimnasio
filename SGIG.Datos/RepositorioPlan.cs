using System.Collections.Generic;
using Dapper;
using SGIG.Entidades;

namespace SGIG.Datos;

public class RepositorioPlan
{
    public IEnumerable<Plan> ObtenerTodos(bool soloActivos = true)
    {
        using var db = Conexion.ObtenerConexionAbierta();
        string sql = @"
            SELECT id_plan AS IdPlan, 
                   nombre AS Nombre, 
                   precio AS Precio, 
                   tipo_periodicidad AS TipoPeriodicidad, 
                   activo AS Activo 
            FROM dbo.[Plan]" +
            (soloActivos ? " WHERE activo = 1" : "") +
            " ORDER BY nombre";

        return db.Query<Plan>(sql);
    }

    public Plan? ObtenerPorId(int idPlan)
    {
        using var db = Conexion.ObtenerConexionAbierta();
        string sql = @"
            SELECT id_plan AS IdPlan, 
                   nombre AS Nombre, 
                   precio AS Precio, 
                   tipo_periodicidad AS TipoPeriodicidad, 
                   activo AS Activo 
            FROM dbo.[Plan] 
            WHERE id_plan = @IdPlan";

        return db.QueryFirstOrDefault<Plan>(sql, new { IdPlan = idPlan });
    }

    public int Insertar(Plan plan)
    {
        using var db = Conexion.ObtenerConexionAbierta();
        string sql = @"
            INSERT INTO dbo.[Plan] (nombre, precio, tipo_periodicidad, activo)
            VALUES (@Nombre, @Precio, @TipoPeriodicidad, 1);
            SELECT CAST(SCOPE_IDENTITY() as int);";

        return db.ExecuteScalar<int>(sql, plan);
    }

    public void Modificar(Plan plan)
    {
        using var db = Conexion.ObtenerConexionAbierta();
        string sql = @"
            UPDATE dbo.[Plan] 
            SET nombre = @Nombre, 
                precio = @Precio, 
                tipo_periodicidad = @TipoPeriodicidad 
            WHERE id_plan = @IdPlan";

        db.Execute(sql, plan);
    }

    public void CambiarEstado(int idPlan, bool activo)
    {
        using var db = Conexion.ObtenerConexionAbierta();
        string sql = "UPDATE dbo.[Plan] SET activo = @Activo WHERE id_plan = @IdPlan";

        db.Execute(sql, new { IdPlan = idPlan, Activo = activo });
    }
}