using Dapper;
using Microsoft.Data.SqlClient;
using SGIG.Entidades;

namespace SGIG.Datos
{
    /// <summary>
    /// Acceso a datos de dbo.Maquina. Sin baja lógica: la tabla no tiene columna
    /// 'activo' (a diferencia de Socio/Usuario/Plan), así que la baja es DELETE físico,
    /// bloqueado si la máquina tiene mantenimientos registrados (FK_Mantenimiento_Maquina).
    /// </summary>
    public class RepositorioMaquina
    {
        private const string SelectBase = @"
            SELECT id_maquina AS IdMaquina, marca AS Marca, nombre AS Nombre,
                   fecha_compra AS FechaCompra, estado AS Estado
            FROM dbo.Maquina";

        public IEnumerable<Maquina> ObtenerTodas()
        {
            const string sql = SelectBase + " ORDER BY nombre";

            try
            {
                using var connection = Conexion.ObtenerConexionAbierta();
                return connection.Query<Maquina>(sql);
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("Error al obtener las máquinas.", ex);
            }
        }

        /// <summary>Máquinas que hoy están operativas, para ofrecer en el alta de un mantenimiento.</summary>
        public IEnumerable<Maquina> ObtenerOperativas()
        {
            const string sql = SelectBase + " WHERE estado = 'Operativa' ORDER BY nombre";

            try
            {
                using var connection = Conexion.ObtenerConexionAbierta();
                return connection.Query<Maquina>(sql);
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("Error al obtener las máquinas operativas.", ex);
            }
        }

        public int Alta(Maquina maquina)
        {
            const string sql = @"
                INSERT INTO dbo.Maquina (marca, nombre, fecha_compra, estado)
                VALUES (@Marca, @Nombre, @FechaCompra, @Estado);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            try
            {
                using var connection = Conexion.ObtenerConexionAbierta();
                return connection.ExecuteScalar<int>(sql, maquina);
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo dar de alta la máquina.", ex);
            }
        }

        public void Modificar(Maquina maquina)
        {
            const string sql = @"
                UPDATE dbo.Maquina
                SET marca = @Marca, nombre = @Nombre, fecha_compra = @FechaCompra, estado = @Estado
                WHERE id_maquina = @IdMaquina";

            try
            {
                using var connection = Conexion.ObtenerConexionAbierta();
                connection.Execute(sql, maquina);
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo modificar la máquina.", ex);
            }
        }

        /// <summary>DELETE físico: dbo.Maquina no tiene baja lógica.</summary>
        public void Eliminar(int idMaquina)
        {
            const string sql = "DELETE FROM dbo.Maquina WHERE id_maquina = @IdMaquina";

            try
            {
                using var connection = Conexion.ObtenerConexionAbierta();
                connection.Execute(sql, new { IdMaquina = idMaquina });
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                throw new AccesoDatosException(
                    "No se puede eliminar la máquina porque tiene mantenimientos registrados.", ex);
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo eliminar la máquina.", ex);
            }
        }
    }
}
