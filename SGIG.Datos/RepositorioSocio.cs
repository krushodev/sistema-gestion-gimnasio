using System.Collections.Generic;
using Dapper;
using Microsoft.Data.SqlClient;
using SGIG.Entidades;

namespace SGIG.Datos
{
    public class RepositorioSocio
    {
        public int Guardar(Socio socio)
        {
            using var db = Conexion.ObtenerConexionAbierta();
            using var tran = db.BeginTransaction();

            try
            {
                if (socio.IdPersona == 0)
                {
                    // 1. Insertamos en dbo.Persona
                    const string sqlPersona = @"
                        INSERT INTO dbo.Persona (documento, id_tipo_documento, nombre, apellido, email, telefono, id_localidad, fecha_nacimiento)
                        VALUES (@Documento, @IdTipoDocumento, @Nombre, @Apellido, @Email, @Telefono, @IdLocalidad, @FechaNacimiento);
                        SELECT CAST(SCOPE_IDENTITY() as int);";

                    socio.IdPersona = db.QuerySingle<int>(sqlPersona, socio, tran);

                    // 2. Insertamos en dbo.Socio (con las columnas reales)
                    const string sqlSocio = @"
                        INSERT INTO dbo.Socio (id_persona, apto_medico, id_plan, fecha_vencimiento_cuota, activo)
                        VALUES (@IdPersona, NULL, NULL, NULL, 1);";

                    db.Execute(sqlSocio, new { socio.IdPersona }, tran);
                }
                else
                {
                    // Actualizamos Persona
                    const string sqlUpdatePersona = @"
                        UPDATE dbo.Persona 
                        SET nombre = @Nombre, apellido = @Apellido, telefono = @Telefono, 
                            email = @Email, documento = @Documento
                        WHERE id_persona = @IdPersona";

                    db.Execute(sqlUpdatePersona, socio, tran);
                }

                tran.Commit();
                return socio.IdPersona;
            }
            catch (SqlException ex)
            {
                tran.Rollback();
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    throw new AccesoDatosException($"Ya existe una persona registrada con el documento '{socio.Documento}'.", ex);
                }
                throw new AccesoDatosException($"Error al registrar o actualizar el socio: {ex.Message}", ex);
            }
        }

        public IEnumerable<Socio> ListarTodos()
        {
            try
            {
                using var db = Conexion.ObtenerConexionAbierta();
                const string sql = @"
                    SELECT s.id_persona AS IdPersona, s.activo AS Activo,
                           p.nombre AS Nombre, p.apellido AS Apellido, 
                           p.id_tipo_documento AS IdTipoDocumento, p.documento AS Documento, 
                           p.fecha_nacimiento AS FechaNacimiento, p.telefono AS Telefono, 
                           p.email AS Email, p.id_localidad AS IdLocalidad
                    FROM dbo.Socio s
                    INNER JOIN dbo.Persona p ON s.id_persona = p.id_persona
                    WHERE s.activo = 1
                    ORDER BY p.apellido, p.nombre";

                return db.Query<Socio>(sql);
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("Error al listar los socios.", ex);
            }
        }

        public Socio? ObtenerPorDocumento(int idTipoDocumento, string documento)
        {
            try
            {
                using var db = Conexion.ObtenerConexionAbierta();
                const string sql = @"
                    SELECT s.id_persona AS IdPersona, s.activo AS Activo,
                           p.nombre AS Nombre, p.apellido AS Apellido, 
                           p.id_tipo_documento AS IdTipoDocumento, p.documento AS Documento, 
                           p.fecha_nacimiento AS FechaNacimiento, p.telefono AS Telefono, 
                           p.email AS Email, p.id_localidad AS IdLocalidad
                    FROM dbo.Socio s
                    INNER JOIN dbo.Persona p ON s.id_persona = p.id_persona
                    WHERE p.id_tipo_documento = @idTipoDocumento AND p.documento = @documento";

                return db.QueryFirstOrDefault<Socio>(sql, new { idTipoDocumento, documento });
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("Error al verificar documento del socio.", ex);
            }
        }

        public void BajaLogica(int idPersona)
        {
            try
            {
                using var db = Conexion.ObtenerConexionAbierta();
                const string sql = "UPDATE dbo.Socio SET activo = 0 WHERE id_persona = @idPersona";
                db.Execute(sql, new { idPersona });
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("Error al dar de baja al socio.", ex);
            }
        }
    }
}