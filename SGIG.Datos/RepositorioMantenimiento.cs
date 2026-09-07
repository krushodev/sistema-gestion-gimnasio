using Dapper;
using Microsoft.Data.SqlClient;
using SGIG.Entidades;

namespace SGIG.Datos
{
    /// <summary>
    /// Acceso a datos de dbo.Mantenimiento. El alta y la finalización tocan también
    /// dbo.Maquina.estado, así que van siempre dentro de una transacción explícita
    /// (RF#20).
    /// </summary>
    public class RepositorioMantenimiento
    {
        private const string SelectBase = @"
            SELECT m.id_mantenimiento AS IdMantenimiento, m.id_maquina AS IdMaquina,
                   m.id_persona AS IdPersona, m.fecha_inicio AS FechaInicio,
                   m.fecha_fin AS FechaFin, m.detalle_tecnico AS DetalleTecnico,
                   maq.nombre AS NombreMaquina,
                   (p.nombre + ' ' + p.apellido) AS NombreTecnico
            FROM dbo.Mantenimiento m
            INNER JOIN dbo.Maquina maq ON maq.id_maquina = m.id_maquina
            INNER JOIN dbo.Persona p ON p.id_persona = m.id_persona";

        /// <summary>Mantenimientos en curso (fecha_fin todavía sin cargar).</summary>
        public IEnumerable<Mantenimiento> ObtenerActivos()
        {
            const string sql = SelectBase + " WHERE m.fecha_fin IS NULL ORDER BY m.fecha_inicio DESC";

            try
            {
                using var connection = Conexion.ObtenerConexionAbierta();
                return connection.Query<Mantenimiento>(sql);
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("Error al obtener los mantenimientos activos.", ex);
            }
        }

        /// <summary>Historial completo de una máquina (RF#21), más reciente primero.</summary>
        public IEnumerable<Mantenimiento> ObtenerPorMaquina(int idMaquina)
        {
            const string sql = SelectBase + " WHERE m.id_maquina = @IdMaquina ORDER BY m.fecha_inicio DESC";

            try
            {
                using var connection = Conexion.ObtenerConexionAbierta();
                return connection.Query<Mantenimiento>(sql, new { IdMaquina = idMaquina });
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("Error al obtener el historial de mantenimientos.", ex);
            }
        }

        /// <summary>Alguna vez se registró un mantenimiento para esa máquina (para bloquear su eliminación).</summary>
        public bool ExisteParaMaquina(int idMaquina)
        {
            const string sql = "SELECT COUNT(1) FROM dbo.Mantenimiento WHERE id_maquina = @IdMaquina";

            try
            {
                using var connection = Conexion.ObtenerConexionAbierta();
                return connection.ExecuteScalar<int>(sql, new { IdMaquina = idMaquina }) > 0;
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("Error al verificar los mantenimientos de la máquina.", ex);
            }
        }

        /// <summary>
        /// Alta transaccional: inserta el mantenimiento y pone la máquina "En Reparacion"
        /// (RF#20). Si falla cualquiera de los dos pasos, no queda ninguno aplicado.
        /// </summary>
        public int Registrar(Mantenimiento mantenimiento)
        {
            const string sqlInsert = @"
                INSERT INTO dbo.Mantenimiento (id_maquina, id_persona, fecha_inicio, detalle_tecnico)
                VALUES (@IdMaquina, @IdPersona, @FechaInicio, @DetalleTecnico);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            const string sqlEstadoMaquina =
                "UPDATE dbo.Maquina SET estado = 'En Reparacion' WHERE id_maquina = @IdMaquina";

            using var connection = Conexion.ObtenerConexionAbierta();
            using var transaction = connection.BeginTransaction();
            try
            {
                var idMantenimiento = connection.ExecuteScalar<int>(sqlInsert, new
                {
                    mantenimiento.IdMaquina,
                    mantenimiento.IdPersona,
                    mantenimiento.FechaInicio,
                    mantenimiento.DetalleTecnico
                }, transaction);

                connection.Execute(sqlEstadoMaquina, new { mantenimiento.IdMaquina }, transaction);

                transaction.Commit();
                return idMantenimiento;
            }
            catch (SqlException ex)
            {
                transaction.Rollback();
                throw new AccesoDatosException("No se pudo registrar el mantenimiento.", ex);
            }
        }

        /// <summary>Cierra el mantenimiento y devuelve la máquina a "Operativa".</summary>
        public void Finalizar(int idMantenimiento, int idMaquina, DateTime fechaFin)
        {
            const string sqlFinalizar =
                "UPDATE dbo.Mantenimiento SET fecha_fin = @FechaFin WHERE id_mantenimiento = @IdMantenimiento";

            const string sqlEstadoMaquina =
                "UPDATE dbo.Maquina SET estado = 'Operativa' WHERE id_maquina = @IdMaquina";

            using var connection = Conexion.ObtenerConexionAbierta();
            using var transaction = connection.BeginTransaction();
            try
            {
                connection.Execute(sqlFinalizar, new { IdMantenimiento = idMantenimiento, FechaFin = fechaFin }, transaction);
                connection.Execute(sqlEstadoMaquina, new { IdMaquina = idMaquina }, transaction);

                transaction.Commit();
            }
            catch (SqlException ex)
            {
                transaction.Rollback();
                throw new AccesoDatosException("No se pudo finalizar el mantenimiento.", ex);
            }
        }
    }
}
