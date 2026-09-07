using Dapper;
using Microsoft.Data.SqlClient;
using SGIG.Entidades;

namespace SGIG.Datos
{
    /// <summary>
    /// Acceso a datos de dbo.Persona, el supertipo común a Socio y Usuario. Se usa
    /// principalmente para reutilizar una Persona ya cargada al dar de alta una de
    /// sus especializaciones (RF#06).
    /// </summary>
    public class RepositorioPersona
    {
        /// <summary>
        /// Busca una Persona por documento. La columna es UNIQUE en dbo.Persona, así
        /// que alcanza con el documento solo (no hace falta el tipo de documento).
        /// </summary>
        public Persona? ObtenerPorDocumento(string documento)
        {
            const string sql = @"
                SELECT id_persona AS IdPersona, documento AS Documento,
                       id_tipo_documento AS IdTipoDocumento, nombre AS Nombre,
                       apellido AS Apellido, email AS Email, telefono AS Telefono,
                       id_localidad AS IdLocalidad, fecha_nacimiento AS FechaNacimiento
                FROM dbo.Persona
                WHERE documento = @Documento";

            try
            {
                using var connection = Conexion.ObtenerConexionAbierta();
                return connection.QueryFirstOrDefault<Persona>(sql, new { Documento = documento });
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("Error al consultar la persona por documento.", ex);
            }
        }
    }
}
