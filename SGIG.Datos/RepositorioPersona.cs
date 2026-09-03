using Dapper;
using Microsoft.Data.SqlClient;
using SGIG.Entidades;

namespace SGIG.Datos
{
    public class RepositorioPersona
    {
        public Persona? ObtenerPorDocumento(int idTipoDocumento, string documento)
        {
            try
            {
                using var db = Conexion.ObtenerConexionAbierta();
                const string sql = @"
                    SELECT id_persona AS IdPersona, 
                           nombre AS Nombre, 
                           apellido AS Apellido, 
                           id_tipo_doc AS IdTipoDocumento, 
                           nro_doc AS Documento, 
                           fecha_nac AS FechaNacimiento, 
                           telefono AS Telefono, 
                           email AS Email, 
                           id_localidad AS IdLocalidad
                    FROM Personas
                    WHERE id_tipo_doc = @idTipoDocumento AND nro_doc = @documento";

                return db.QueryFirstOrDefault<Persona>(sql, new { idTipoDocumento, documento });
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("Error al consultar persona por documento.", ex);
            }
        }
    }
}