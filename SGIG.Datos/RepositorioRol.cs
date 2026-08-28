using Dapper;
using Microsoft.Data.SqlClient;
using SGIG.Entidades;

namespace SGIG.Datos
{
    /// <summary>
    /// Acceso a datos de la tabla paramétrica dbo.Rol. Como el resto de los
    /// catálogos, su baja es lógica (RF#04): nunca DELETE físico.
    /// </summary>
    public class RepositorioRol
    {
        private const string SelectBase = @"
            SELECT id_rol AS IdRol, nombre_rol AS NombreRol,
                   descripcion AS Descripcion, activo AS Activo
            FROM dbo.Rol";

        /// <summary>Roles vigentes, para los combos y la grilla del ABM.</summary>
        public IEnumerable<Rol> ObtenerActivos()
        {
            const string sql = SelectBase + @"
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

        public Rol? ObtenerPorId(int idRol)
        {
            const string sql = SelectBase + " WHERE id_rol = @IdRol";

            try
            {
                using var connection = Conexion.ObtenerConexionAbierta();
                return connection.QuerySingleOrDefault<Rol>(sql, new { IdRol = idRol });
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("Error al obtener el rol.", ex);
            }
        }

        public int Alta(Rol rol)
        {
            const string sql = @"
                INSERT INTO dbo.Rol (nombre_rol, descripcion)
                VALUES (@NombreRol, @Descripcion);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            try
            {
                using var connection = Conexion.ObtenerConexionAbierta();
                return connection.ExecuteScalar<int>(sql, new { rol.NombreRol, rol.Descripcion });
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("Error al dar de alta el rol.", ex);
            }
        }

        public void Modificar(Rol rol)
        {
            const string sql = @"
                UPDATE dbo.Rol
                SET nombre_rol = @NombreRol, descripcion = @Descripcion
                WHERE id_rol = @IdRol";

            try
            {
                using var connection = Conexion.ObtenerConexionAbierta();
                connection.Execute(sql, new { rol.IdRol, rol.NombreRol, rol.Descripcion });
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("Error al modificar el rol.", ex);
            }
        }

        /// <summary>Baja lógica (RF#04): nunca DELETE físico.</summary>
        public void BajaLogica(int idRol)
        {
            const string sql = "UPDATE dbo.Rol SET activo = 0 WHERE id_rol = @IdRol";

            try
            {
                using var connection = Conexion.ObtenerConexionAbierta();
                connection.Execute(sql, new { IdRol = idRol });
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo dar de baja el rol.", ex);
            }
        }

        /// <summary>
        /// Cuántos usuarios activos tienen asignado este rol. Dar de baja un rol en
        /// uso dejaría a esos usuarios sin permisos resolubles, así que se bloquea.
        /// </summary>
        public int ContarUsuariosActivos(int idRol)
        {
            const string sql = "SELECT COUNT(1) FROM dbo.Usuario WHERE id_rol = @IdRol AND activo = 1";

            try
            {
                using var connection = Conexion.ObtenerConexionAbierta();
                return connection.ExecuteScalar<int>(sql, new { IdRol = idRol });
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("Error al verificar los usuarios del rol.", ex);
            }
        }
    }
}
