using Dapper;
using Microsoft.Data.SqlClient;
using SGIG.Entidades;

namespace SGIG.Datos
{
    /// <summary>
    /// Acceso a datos de la tabla paramétrica dbo.Rol. Rol es un catálogo sembrado
    /// con la aplicación (los 3 roles fijos: Administrador, Recepcionista,
    /// Técnico) y no tiene ABM en la UI; este repositorio solo expone lectura,
    /// usada para poblar el combo de rol al dar de alta un Usuario.
    /// </summary>
    public class RepositorioRol
    {
        /// <summary>Roles vigentes, para el combo de asignación de rol en frmUsuarioEditor.</summary>
        public IEnumerable<Rol> ObtenerActivos()
        {
            const string sql = @"
                SELECT id_rol AS IdRol, nombre_rol AS NombreRol,
                       descripcion AS Descripcion, activo AS Activo
                FROM dbo.Rol
                WHERE activo = 1
                ORDER BY nombre_rol";

            try
            {
                using var connection = Conexion.ObtenerConexionAbierta();
                return connection.Query<Rol>(sql);
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("Error al obtener los roles.", ex);
            }
        }
    }
}
