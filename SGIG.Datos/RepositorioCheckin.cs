using Dapper;
using Microsoft.Data.SqlClient;
using SGIG.Entidades;

namespace SGIG.Datos
{
    /// <summary>
    /// Acceso a datos de dbo.Checkin. La consulta por documento está pensada para
    /// resolverse en menos de 2 segundos (RNF#01): trae en una sola consulta lo
    /// mínimo que necesita el Check-in para decidir Concedido/Rechazado, leyendo
    /// <c>Socio.fecha_vencimiento_cuota</c> (el campo caché) en vez de resolver la
    /// Facturación vigente.
    /// </summary>
    public class RepositorioCheckin
    {
        /// <summary>
        /// Busca al socio por documento con los datos justos para decidir el acceso.
        /// Devuelve null si el documento no corresponde a ningún socio.
        /// </summary>
        public Socio? BuscarSocioPorDocumento(string documento)
        {
            const string sql = @"
                SELECT s.id_persona AS IdPersona, p.documento AS Documento,
                       p.nombre AS Nombre, p.apellido AS Apellido,
                       s.activo AS Activo, s.fecha_vencimiento_cuota AS FechaVencimientoCuota
                FROM dbo.Socio s
                INNER JOIN dbo.Persona p ON p.id_persona = s.id_persona
                WHERE p.documento = @Documento";

            try
            {
                using var connection = Conexion.ObtenerConexionAbierta();
                return connection.QueryFirstOrDefault<Socio>(sql, new { Documento = documento });
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("Error al buscar el socio para el check-in.", ex);
            }
        }

        /// <summary>Registra el intento de acceso, sea Concedido o Rechazado (RF#17).</summary>
        public void Registrar(Checkin checkin)
        {
            const string sql = @"
                INSERT INTO dbo.Checkin (id_persona, fecha_hora, resultado)
                VALUES (@IdPersona, @FechaHora, @Resultado);";

            try
            {
                using var connection = Conexion.ObtenerConexionAbierta();
                connection.Execute(sql, new { checkin.IdPersona, checkin.FechaHora, checkin.Resultado });
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo registrar el intento de acceso.", ex);
            }
        }
    }
}
