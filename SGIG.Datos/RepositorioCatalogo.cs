using Dapper;
using Microsoft.Data.SqlClient;
using SGIG.Entidades;

namespace SGIG.Datos
{
    /// <summary>
    /// Acceso a datos de las tablas paramétricas simples: Provincia, Localidad,
    /// TipoDocumento y MedioPago. Son catálogos sembrados con la aplicación (ver
    /// docs/SGIG_CreateDB.sql) y no tienen ABM en la UI; este repositorio solo
    /// expone lectura, usada para poblar combos en otras pantallas.
    /// </summary>
    public class RepositorioCatalogo
    {
        // ── Provincia ────────────────────────────────────────────────────────

        public IEnumerable<Provincia> ObtenerProvincias()
        {
            const string sql = @"
                SELECT id_provincia AS IdProvincia, nombre AS Nombre, activo AS Activo
                FROM dbo.Provincia
                WHERE activo = 1
                ORDER BY nombre";

            return Consultar<Provincia>(sql, "Error al obtener las provincias.");
        }

        // ── Localidad ────────────────────────────────────────────────────────

        /// <summary>Localidades con el nombre de su provincia resuelto, para los combos.</summary>
        public IEnumerable<Localidad> ObtenerLocalidades()
        {
            const string sql = @"
                SELECT l.id_localidad AS IdLocalidad, l.nombre AS Nombre,
                       l.id_provincia AS IdProvincia, l.activo AS Activo,
                       p.nombre AS NombreProvincia
                FROM dbo.Localidad l
                INNER JOIN dbo.Provincia p ON p.id_provincia = l.id_provincia
                WHERE l.activo = 1
                ORDER BY p.nombre, l.nombre";

            return Consultar<Localidad>(sql, "Error al obtener las localidades.");
        }

        // ── TipoDocumento ────────────────────────────────────────────────────

        public IEnumerable<TipoDocumento> ObtenerTiposDocumento()
        {
            const string sql = @"
                SELECT id_tipo_documento AS IdTipoDocumento, descripcion AS Descripcion,
                       activo AS Activo
                FROM dbo.TipoDocumento
                WHERE activo = 1
                ORDER BY descripcion";

            return Consultar<TipoDocumento>(sql, "Error al obtener los tipos de documento.");
        }

        // ── MedioPago ────────────────────────────────────────────────────────

        public IEnumerable<MedioPago> ObtenerMediosPago()
        {
            const string sql = @"
                SELECT id_medio_pago AS IdMedioPago, descripcion AS Descripcion,
                       activo AS Activo
                FROM dbo.MedioPago
                WHERE activo = 1
                ORDER BY descripcion";

            return Consultar<MedioPago>(sql, "Error al obtener los medios de pago.");
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        private static IEnumerable<T> Consultar<T>(string sql, string mensajeError)
        {
            try
            {
                using var connection = Conexion.ObtenerConexionAbierta();
                return connection.Query<T>(sql);
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException(mensajeError, ex);
            }
        }
    }
}
