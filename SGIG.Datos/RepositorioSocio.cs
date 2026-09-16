using Dapper;
using Microsoft.Data.SqlClient;
using SGIG.Datos;
using SGIG.Entidades;

namespace SGIG.Datos
{
    /// <summary>
    /// Acceso a datos de dbo.Socio y su supertipo dbo.Persona. Toda escritura que
    /// toca las dos tablas va siempre dentro de una transacción explícita.
    /// </summary>
    public class RepositorioSocio
    {
        /// <summary>Columnas de Persona + Socio, con los alias que espera la entidad.</summary>
        private const string SelectBase = @"
            SELECT p.id_persona AS IdPersona, p.documento AS Documento,
                   p.id_tipo_documento AS IdTipoDocumento, p.nombre AS Nombre,
                   p.apellido AS Apellido, p.email AS Email, p.telefono AS Telefono,
                   p.id_localidad AS IdLocalidad, p.fecha_nacimiento AS FechaNacimiento,
                   s.apto_medico AS AptoMedico, s.id_plan AS IdPlan,
                   s.fecha_vencimiento_cuota AS FechaVencimientoCuota, s.activo AS Activo
            FROM dbo.Socio s
            INNER JOIN dbo.Persona p ON p.id_persona = s.id_persona";

        public Socio? ObtenerPorId(int idPersona)
        {
            const string sql = SelectBase + " WHERE s.id_persona = @IdPersona";

            try
            {
                using var connection = Conexion.ObtenerConexionAbierta();
                return connection.QuerySingleOrDefault<Socio>(sql, new { IdPersona = idPersona });
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("Error al obtener el socio.", ex);
            }
        }

        /// <summary>Socios no dados de baja, para la grilla del ABM.</summary>
        public IEnumerable<Socio> ObtenerActivos()
        {
            const string sql = SelectBase + " WHERE s.activo = 1 ORDER BY p.apellido, p.nombre";

            try
            {
                using var connection = Conexion.ObtenerConexionAbierta();
                return connection.Query<Socio>(sql);
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("Error al obtener los socios activos.", ex);
            }
        }

        /// <summary>
        /// Alta completa: inserta Persona + Socio en una sola transacción. Se usa
        /// cuando el documento no pertenecía a ninguna Persona ya cargada.
        /// </summary>
        public int Alta(Socio socio)
        {
            const string sqlPersona = @"
                INSERT INTO dbo.Persona
                    (documento, id_tipo_documento, nombre, apellido, email, telefono, id_localidad, fecha_nacimiento)
                VALUES
                    (@Documento, @IdTipoDocumento, @Nombre, @Apellido, @Email, @Telefono, @IdLocalidad, @FechaNacimiento);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            const string sqlSocio = @"
                INSERT INTO dbo.Socio (id_persona, apto_medico, id_plan, fecha_vencimiento_cuota, activo)
                VALUES (@IdPersona, @AptoMedico, @IdPlan, NULL, 1);";

            using var connection = Conexion.ObtenerConexionAbierta();
            using var transaction = connection.BeginTransaction();
            try
            {
                var idPersona = connection.ExecuteScalar<int>(sqlPersona, new
                {
                    socio.Documento,
                    socio.IdTipoDocumento,
                    socio.Nombre,
                    socio.Apellido,
                    socio.Email,
                    socio.Telefono,
                    socio.IdLocalidad,
                    socio.FechaNacimiento
                }, transaction);

                connection.Execute(sqlSocio, new { IdPersona = idPersona, socio.AptoMedico, socio.IdPlan }, transaction);

                transaction.Commit();
                return idPersona;
            }
            catch (SqlException ex)
            {
                transaction.Rollback();
                throw new AccesoDatosException("No se pudo dar de alta el socio.", ex);
            }
        }

        /// <summary>
        /// Existe una fila en dbo.Socio para esa Persona (fue socio alguna vez, activo
        /// o dado de baja). Determina si el alta debe insertar o reactivar (ver RF#06).
        /// </summary>
        public bool ExisteFilaSocio(int idPersona)
        {
            const string sql = "SELECT COUNT(1) FROM dbo.Socio WHERE id_persona = @IdPersona";
            return Existe(sql, new { IdPersona = idPersona }, "Error al verificar si la persona ya fue socio.");
        }

        /// <summary>
        /// Da de alta el Socio sobre una Persona ya existente (RF#06: por ejemplo,
        /// alguien que ya estaba cargado como Usuario) que nunca tuvo fila en Socio.
        /// </summary>
        public void AltaSobrePersonaExistente(Socio socio)
        {
            const string sql = @"
                INSERT INTO dbo.Socio (id_persona, apto_medico, id_plan, fecha_vencimiento_cuota, activo)
                VALUES (@IdPersona, @AptoMedico, @IdPlan, NULL, 1);";

            try
            {
                using var connection = Conexion.ObtenerConexionAbierta();
                connection.Execute(sql, new { socio.IdPersona, socio.AptoMedico, socio.IdPlan });
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo registrar el socio sobre la persona existente.", ex);
            }
        }

        /// <summary>
        /// Reactiva a un socio que ya tenía fila en dbo.Socio pero estaba dado de baja
        /// (RF#06). Como id_persona es la PK de Socio, no se puede volver a insertar.
        /// </summary>
        public void Reactivar(Socio socio)
        {
            const string sql = @"
                UPDATE dbo.Socio
                SET apto_medico = @AptoMedico, id_plan = @IdPlan, activo = 1
                WHERE id_persona = @IdPersona;";

            try
            {
                using var connection = Conexion.ObtenerConexionAbierta();
                connection.Execute(sql, new { socio.IdPersona, socio.AptoMedico, socio.IdPlan });
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo reactivar el socio.", ex);
            }
        }

        /// <summary>Modifica Persona + Socio en una transacción.</summary>
        public void Modificar(Socio socio)
        {
            const string sqlPersona = @"
                UPDATE dbo.Persona
                SET documento = @Documento, id_tipo_documento = @IdTipoDocumento,
                    nombre = @Nombre, apellido = @Apellido, email = @Email,
                    telefono = @Telefono, id_localidad = @IdLocalidad,
                    fecha_nacimiento = @FechaNacimiento
                WHERE id_persona = @IdPersona;";

            const string sqlSocio = @"
                UPDATE dbo.Socio
                SET apto_medico = @AptoMedico, id_plan = @IdPlan
                WHERE id_persona = @IdPersona;";

            using var connection = Conexion.ObtenerConexionAbierta();
            using var transaction = connection.BeginTransaction();
            try
            {
                connection.Execute(sqlPersona, new
                {
                    socio.IdPersona,
                    socio.Documento,
                    socio.IdTipoDocumento,
                    socio.Nombre,
                    socio.Apellido,
                    socio.Email,
                    socio.Telefono,
                    socio.IdLocalidad,
                    socio.FechaNacimiento
                }, transaction);

                connection.Execute(sqlSocio, new { socio.IdPersona, socio.AptoMedico, socio.IdPlan }, transaction);

                transaction.Commit();
            }
            catch (SqlException ex)
            {
                transaction.Rollback();
                throw new AccesoDatosException("No se pudo modificar el socio.", ex);
            }
        }

        public Socio? ObtenerPorDocumento(string documento)
        {
            const string sql = SelectBase + " WHERE p.documento = @Documento";

            try
            {
                using var connection = Conexion.ObtenerConexionAbierta();
                return connection.QuerySingleOrDefault<Socio>(sql, new { Documento = documento });
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("Error al obtener el socio por documento.", ex);
            }
        }

        /// <summary>
        /// Socios activos cuyo nombre, apellido o documento contiene <paramref name="texto"/>.
        /// Para el selector de búsqueda por nombre de Cobro de Cuotas y Check-in.
        /// </summary>
        public IEnumerable<Socio> BuscarActivosPorTexto(string texto)
        {
            const string sql = SelectBase + @"
                WHERE s.activo = 1
                  AND (p.nombre LIKE @Texto OR p.apellido LIKE @Texto OR p.documento LIKE @Texto)
                ORDER BY p.apellido, p.nombre";

            try
            {
                using var connection = Conexion.ObtenerConexionAbierta();
                return connection.Query<Socio>(sql, new { Texto = $"%{texto}%" });
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("Error al buscar socios.", ex);
            }
        }



        /// <summary>Baja lógica (RNF#03): nunca DELETE físico.</summary>
        public void BajaLogica(int idPersona)
        {
            const string sql = "UPDATE dbo.Socio SET activo = 0 WHERE id_persona = @IdPersona";

            try
            {
                using var connection = Conexion.ObtenerConexionAbierta();
                connection.Execute(sql, new { IdPersona = idPersona });
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo dar de baja al socio.", ex);
            }
        }

        /// <summary>
        /// Verifica que el documento no pertenezca a otro socio activo (RF#09).
        /// idPersonaExcluida permite excluir al propio registro cuando se está editando.
        /// </summary>
        public bool ExisteDocumentoSocioActivo(string documento, int? idPersonaExcluida = null)
        {
            const string sql = @"
                SELECT COUNT(1) FROM dbo.Socio s
                INNER JOIN dbo.Persona p ON p.id_persona = s.id_persona
                WHERE p.documento = @Documento AND s.activo = 1
                  AND (@IdPersonaExcluida IS NULL OR s.id_persona <> @IdPersonaExcluida)";

            return Existe(sql, new { Documento = documento, IdPersonaExcluida = idPersonaExcluida },
                "Error al verificar el documento del socio.");
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
