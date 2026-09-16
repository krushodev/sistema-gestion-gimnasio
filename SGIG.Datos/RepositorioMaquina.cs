using Dapper;
using Microsoft.Data.SqlClient;
using SGIG.Entidades;

namespace SGIG.Datos
{
    /// <summary>
    /// Acceso a datos de dbo.Maquina. Maquina es un catálogo sembrado con la
    /// aplicación (ver docs/SGIG_CreateDB.sql) y no tiene ABM en la UI; este
    /// repositorio solo expone lectura, usada por la consulta de frmMaquinas y
    /// por los combos de Mantenimiento/Historial de Mantenimientos.
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

    }
}
