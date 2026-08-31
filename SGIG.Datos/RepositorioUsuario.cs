using Dapper;
using Microsoft.Data.SqlClient;
using SGIG.Entidades;

namespace SGIG.Datos
{
    /// <summary>
    /// Acceso a datos de dbo.Usuario y su supertipo dbo.Persona. Toda escritura toca
    /// las dos tablas, así que va siempre dentro de una transacción explícita.
    /// </summary>
    public class RepositorioUsuario
    {
        /// <summary>Columnas de Persona + Usuario, con los alias que espera la entidad.</summary>
        private const string SelectBase = @"
            SELECT p.id_persona AS IdPersona, p.documento AS Documento,
                   p.id_tipo_documento AS IdTipoDocumento, p.nombre AS Nombre,
                   p.apellido AS Apellido, p.email AS Email, p.telefono AS Telefono,
                   p.id_localidad AS IdLocalidad, p.fecha_nacimiento AS FechaNacimiento,
                   u.nombre_usuario AS NombreUsuario, u.contrasenia_hash AS ContraseniaHash,
                   u.id_rol AS IdRol, u.legajo AS Legajo, u.fecha_ingreso AS FechaIngreso,
                   u.activo AS Activo
            FROM dbo.Usuario u
            INNER JOIN dbo.Persona p ON p.id_persona = u.id_persona";

        /// <summary>
        /// Busca un usuario por su nombre de usuario y trae su Rol en la misma consulta.
        /// Sólo devuelve usuarios activos: uno dado de baja no puede iniciar sesión (RF#01).
        /// </summary>
        public Usuario? ObtenerPorNombreUsuario(string nombreUsuario)
        {
            // Esta consulta no reutiliza SelectBase porque necesita columnas extra (las de
            // Rol) y SelectBase ya incluye su propio FROM/JOIN: concatenarle columnas las
            // dejaría después del FROM y el SQL sería inválido.
            //
            // Las columnas de Rol van al final y arrancan en NombreRol: ese es el punto de
            // corte (splitOn) donde Dapper empieza a armar el segundo objeto. No se puede
            // cortar en IdRol porque Usuario ya trae una columna con ese mismo alias.
            const string sql = @"
                SELECT p.id_persona AS IdPersona, p.documento AS Documento,
                       p.id_tipo_documento AS IdTipoDocumento, p.nombre AS Nombre,
                       p.apellido AS Apellido, p.email AS Email, p.telefono AS Telefono,
                       p.id_localidad AS IdLocalidad, p.fecha_nacimiento AS FechaNacimiento,
                       u.nombre_usuario AS NombreUsuario, u.contrasenia_hash AS ContraseniaHash,
                       u.id_rol AS IdRol, u.legajo AS Legajo, u.fecha_ingreso AS FechaIngreso,
                       u.activo AS Activo,
                       r.nombre_rol AS NombreRol, r.descripcion AS Descripcion, r.id_rol AS IdRol
                FROM dbo.Usuario u
                INNER JOIN dbo.Persona p ON p.id_persona = u.id_persona
                INNER JOIN dbo.Rol r ON r.id_rol = u.id_rol
                WHERE u.nombre_usuario = @NombreUsuario AND u.activo = 1";

            try
            {
                using var connection = Conexion.ObtenerConexionAbierta();
                return connection.Query<Usuario, Rol, Usuario>(
                    sql,
                    (usuario, rol) => { usuario.Rol = rol; return usuario; },
                    new { NombreUsuario = nombreUsuario },
                    splitOn: "NombreRol").SingleOrDefault();
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("Error al buscar el usuario por nombre de usuario.", ex);
            }
        }

        public Usuario? ObtenerPorId(int idPersona)
        {
            const string sql = SelectBase + " WHERE u.id_persona = @IdPersona";

            try
            {
                using var connection = Conexion.ObtenerConexionAbierta();
                return connection.QuerySingleOrDefault<Usuario>(sql, new { IdPersona = idPersona });
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("Error al obtener el usuario.", ex);
            }
        }

        /// <summary>Usuarios no dados de baja, para la grilla del ABM.</summary>
        public IEnumerable<Usuario> ObtenerActivos()
        {
            // Trae el nombre del rol en la misma consulta para que la grilla no tenga
            // que resolverlo fila por fila.
            const string sql = @"
                SELECT p.id_persona AS IdPersona, p.documento AS Documento,
                       p.id_tipo_documento AS IdTipoDocumento, p.nombre AS Nombre,
                       p.apellido AS Apellido, p.email AS Email, p.telefono AS Telefono,
                       p.id_localidad AS IdLocalidad, p.fecha_nacimiento AS FechaNacimiento,
                       u.nombre_usuario AS NombreUsuario, u.contrasenia_hash AS ContraseniaHash,
                       u.id_rol AS IdRol, u.legajo AS Legajo, u.fecha_ingreso AS FechaIngreso,
                       u.activo AS Activo, r.nombre_rol AS NombreRol
                FROM dbo.Usuario u
                INNER JOIN dbo.Persona p ON p.id_persona = u.id_persona
                INNER JOIN dbo.Rol r ON r.id_rol = u.id_rol
                WHERE u.activo = 1
                ORDER BY p.apellido, p.nombre";

            try
            {
                using var connection = Conexion.ObtenerConexionAbierta();
                return connection.Query<Usuario>(sql);
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("Error al obtener los usuarios activos.", ex);
            }
        }

        /// <summary>
        /// Alta unificada de Persona + Usuario en una sola transacción: si falla el insert
        /// del Usuario, la Persona tampoco queda registrada. Devuelve el id_persona nuevo.
        /// </summary>
        public int Alta(Usuario usuario)
        {
            const string sqlPersona = @"
                INSERT INTO dbo.Persona
                    (documento, id_tipo_documento, nombre, apellido, email, telefono, id_localidad, fecha_nacimiento)
                VALUES
                    (@Documento, @IdTipoDocumento, @Nombre, @Apellido, @Email, @Telefono, @IdLocalidad, @FechaNacimiento);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            const string sqlUsuario = @"
                INSERT INTO dbo.Usuario
                    (id_persona, nombre_usuario, contrasenia_hash, id_rol, legajo, fecha_ingreso, activo)
                VALUES
                    (@IdPersona, @NombreUsuario, @ContraseniaHash, @IdRol, @Legajo, @FechaIngreso, 1);";

            using var connection = Conexion.ObtenerConexionAbierta();
            using var transaction = connection.BeginTransaction();
            try
            {
                var idPersona = connection.ExecuteScalar<int>(sqlPersona, new
                {
                    usuario.Documento,
                    usuario.IdTipoDocumento,
                    usuario.Nombre,
                    usuario.Apellido,
                    usuario.Email,
                    usuario.Telefono,
                    usuario.IdLocalidad,
                    usuario.FechaNacimiento
                }, transaction);

                connection.Execute(sqlUsuario, new
                {
                    IdPersona = idPersona,
                    usuario.NombreUsuario,
                    usuario.ContraseniaHash,
                    usuario.IdRol,
                    usuario.Legajo,
                    usuario.FechaIngreso
                }, transaction);

                transaction.Commit();
                return idPersona;
            }
            catch (SqlException ex)
            {
                transaction.Rollback();
                throw new AccesoDatosException("No se pudo dar de alta el usuario.", ex);
            }
        }

        /// <summary>
        /// Modifica Persona + Usuario en una transacción. La contraseña sólo se pisa si el
        /// usuario trae un hash nuevo; si viene vacío se conserva la que ya estaba.
        /// </summary>
        public void Modificar(Usuario usuario)
        {
            const string sqlPersona = @"
                UPDATE dbo.Persona
                SET documento = @Documento, id_tipo_documento = @IdTipoDocumento,
                    nombre = @Nombre, apellido = @Apellido, email = @Email,
                    telefono = @Telefono, id_localidad = @IdLocalidad,
                    fecha_nacimiento = @FechaNacimiento
                WHERE id_persona = @IdPersona;";

            const string sqlUsuario = @"
                UPDATE dbo.Usuario
                SET nombre_usuario = @NombreUsuario, id_rol = @IdRol,
                    legajo = @Legajo, fecha_ingreso = @FechaIngreso
                WHERE id_persona = @IdPersona;";

            const string sqlContrasenia = @"
                UPDATE dbo.Usuario
                SET contrasenia_hash = @ContraseniaHash
                WHERE id_persona = @IdPersona;";

            using var connection = Conexion.ObtenerConexionAbierta();
            using var transaction = connection.BeginTransaction();
            try
            {
                connection.Execute(sqlPersona, new
                {
                    usuario.IdPersona,
                    usuario.Documento,
                    usuario.IdTipoDocumento,
                    usuario.Nombre,
                    usuario.Apellido,
                    usuario.Email,
                    usuario.Telefono,
                    usuario.IdLocalidad,
                    usuario.FechaNacimiento
                }, transaction);

                connection.Execute(sqlUsuario, new
                {
                    usuario.IdPersona,
                    usuario.NombreUsuario,
                    usuario.IdRol,
                    usuario.Legajo,
                    usuario.FechaIngreso
                }, transaction);

                if (usuario.ContraseniaHash.Length > 0)
                {
                    connection.Execute(sqlContrasenia,
                        new { usuario.IdPersona, usuario.ContraseniaHash }, transaction);
                }

                transaction.Commit();
            }
            catch (SqlException ex)
            {
                transaction.Rollback();
                throw new AccesoDatosException("No se pudo modificar el usuario.", ex);
            }
        }

        /// <summary>Baja lógica (RNF#03): nunca DELETE físico.</summary>
        public void BajaLogica(int idPersona)
        {
            const string sql = "UPDATE dbo.Usuario SET activo = 0 WHERE id_persona = @IdPersona";

            try
            {
                using var connection = Conexion.ObtenerConexionAbierta();
                connection.Execute(sql, new { IdPersona = idPersona });
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo dar de baja el usuario.", ex);
            }
        }

        /// <summary>
        /// Verifica que el nombre de usuario no esté tomado. idPersonaExcluida permite
        /// excluir al propio registro cuando se está editando.
        /// </summary>
        public bool ExisteNombreUsuario(string nombreUsuario, int? idPersonaExcluida = null)
        {
            const string sql = @"
                SELECT COUNT(1) FROM dbo.Usuario
                WHERE nombre_usuario = @NombreUsuario
                  AND (@IdPersonaExcluida IS NULL OR id_persona <> @IdPersonaExcluida)";

            return Existe(sql, new { NombreUsuario = nombreUsuario, IdPersonaExcluida = idPersonaExcluida },
                "Error al verificar el nombre de usuario.");
        }

        public bool ExisteLegajo(string legajo, int? idPersonaExcluida = null)
        {
            const string sql = @"
                SELECT COUNT(1) FROM dbo.Usuario
                WHERE legajo = @Legajo
                  AND (@IdPersonaExcluida IS NULL OR id_persona <> @IdPersonaExcluida)";

            return Existe(sql, new { Legajo = legajo, IdPersonaExcluida = idPersonaExcluida },
                "Error al verificar el legajo.");
        }

        public bool ExisteDocumento(string documento, int? idPersonaExcluida = null)
        {
            const string sql = @"
                SELECT COUNT(1) FROM dbo.Persona
                WHERE documento = @Documento
                  AND (@IdPersonaExcluida IS NULL OR id_persona <> @IdPersonaExcluida)";

            return Existe(sql, new { Documento = documento, IdPersonaExcluida = idPersonaExcluida },
                "Error al verificar el documento.");
        }

        private static bool Existe(string sql, object parametros, string mensajeError)
        {
            try
            {
                using var connection = Conexion.ObtenerConexionAbierta();
                return connection.ExecuteScalar<int>(sql, parametros) > 0;
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException(mensajeError, ex);
            }
        }
    }
}
