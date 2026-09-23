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

        /// <summary>
        /// Historial de accesos entre <paramref name="desde"/> y <paramref name="hasta"/>
        /// (inclusive), opcionalmente filtrado por documento (búsqueda parcial). Usa
        /// <c>IX_Checkin_Persona_Fecha</c> vía el rango de fecha_hora, ordenado del más
        /// reciente al más antiguo. Incluye los días restantes a la cuota vigente de
        /// cada socio, calculados al momento de la consulta (no al momento del acceso).
        /// </summary>
        public IEnumerable<Checkin> ObtenerHistorial(DateTime desde, DateTime hasta, string? documento = null)
        {
            const string sql = @"
                SELECT c.id_checkin AS IdCheckin, c.id_persona AS IdPersona,
                       c.fecha_hora AS FechaHora, c.resultado AS Resultado,
                       p.documento AS Documento,
                       p.nombre + ' ' + p.apellido AS NombreCompleto,
                       DATEDIFF(DAY, CAST(GETDATE() AS DATE), s.fecha_vencimiento_cuota) AS DiasRestantesCuota
                FROM dbo.Checkin c
                INNER JOIN dbo.Persona p ON p.id_persona = c.id_persona
                INNER JOIN dbo.Socio s ON s.id_persona = c.id_persona
                WHERE c.fecha_hora >= @Desde
                  AND c.fecha_hora < DATEADD(DAY, 1, @Hasta)
                  AND (@Documento IS NULL OR p.documento LIKE '%' + @Documento + '%')
                ORDER BY c.fecha_hora DESC";

            try
            {
                using var connection = Conexion.ObtenerConexionAbierta();
                return connection.Query<Checkin>(sql, new
                {
                    Desde = desde.Date,
                    Hasta = hasta.Date,
                    Documento = string.IsNullOrWhiteSpace(documento) ? null : documento.Trim()
                }).ToList();
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("Error al consultar el historial de accesos.", ex);
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
