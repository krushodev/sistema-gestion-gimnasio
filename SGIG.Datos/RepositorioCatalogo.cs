using Dapper;
using Microsoft.Data.SqlClient;
using SGIG.Entidades;

namespace SGIG.Datos
{
    /// <summary>
    /// Acceso a datos de las tablas paramétricas simples: Provincia, Localidad,
    /// TipoDocumento y MedioPago (RF#04). Son catálogos chicos, de estructura casi
    /// idéntica, así que viven en un único repositorio en vez de cuatro archivos
    /// con el mismo CRUD repetido.
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

        public int AltaProvincia(Provincia provincia)
        {
            const string sql = @"
                INSERT INTO dbo.Provincia (nombre) VALUES (@Nombre);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            return EjecutarEscalar(sql, new { provincia.Nombre }, "Error al dar de alta la provincia.");
        }

        public void ModificarProvincia(Provincia provincia)
        {
            const string sql = "UPDATE dbo.Provincia SET nombre = @Nombre WHERE id_provincia = @IdProvincia";

            Ejecutar(sql, new { provincia.IdProvincia, provincia.Nombre },
                "Error al modificar la provincia.");
        }

        // ── Localidad ────────────────────────────────────────────────────────

        /// <summary>Localidades con el nombre de su provincia resuelto, para la grilla.</summary>
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

        public int AltaLocalidad(Localidad localidad)
        {
            const string sql = @"
                INSERT INTO dbo.Localidad (nombre, id_provincia) VALUES (@Nombre, @IdProvincia);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            return EjecutarEscalar(sql, new { localidad.Nombre, localidad.IdProvincia },
                "Error al dar de alta la localidad.");
        }

        public void ModificarLocalidad(Localidad localidad)
        {
            const string sql = @"
                UPDATE dbo.Localidad
                SET nombre = @Nombre, id_provincia = @IdProvincia
                WHERE id_localidad = @IdLocalidad";

            Ejecutar(sql, new { localidad.IdLocalidad, localidad.Nombre, localidad.IdProvincia },
                "Error al modificar la localidad.");
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

        public int AltaTipoDocumento(TipoDocumento tipoDocumento)
        {
            const string sql = @"
                INSERT INTO dbo.TipoDocumento (descripcion) VALUES (@Descripcion);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            return EjecutarEscalar(sql, new { tipoDocumento.Descripcion },
                "Error al dar de alta el tipo de documento.");
        }

        public void ModificarTipoDocumento(TipoDocumento tipoDocumento)
        {
            const string sql = @"
                UPDATE dbo.TipoDocumento SET descripcion = @Descripcion
                WHERE id_tipo_documento = @IdTipoDocumento";

            Ejecutar(sql, new { tipoDocumento.IdTipoDocumento, tipoDocumento.Descripcion },
                "Error al modificar el tipo de documento.");
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

        public int AltaMedioPago(MedioPago medioPago)
        {
            const string sql = @"
                INSERT INTO dbo.MedioPago (descripcion) VALUES (@Descripcion);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            return EjecutarEscalar(sql, new { medioPago.Descripcion },
                "Error al dar de alta el medio de pago.");
        }

        public void ModificarMedioPago(MedioPago medioPago)
        {
            const string sql = @"
                UPDATE dbo.MedioPago SET descripcion = @Descripcion
                WHERE id_medio_pago = @IdMedioPago";

            Ejecutar(sql, new { medioPago.IdMedioPago, medioPago.Descripcion },
                "Error al modificar el medio de pago.");
        }

        // ── Bajas ────────────────────────────────────────────────────────────
        //
        // Baja LOGICA (RF#04, ERS v3.2): nunca DELETE fisico. Un catalogo dado de
        // baja desaparece de las grillas y de los combos, pero la fila sigue ahi
        // para que los registros historicos que la referencian sigan siendo validos.

        public void BajaLogicaProvincia(int idProvincia) =>
            Ejecutar("UPDATE dbo.Provincia SET activo = 0 WHERE id_provincia = @Id",
                new { Id = idProvincia }, "No se pudo dar de baja la provincia.");

        public void BajaLogicaLocalidad(int idLocalidad) =>
            Ejecutar("UPDATE dbo.Localidad SET activo = 0 WHERE id_localidad = @Id",
                new { Id = idLocalidad }, "No se pudo dar de baja la localidad.");

        public void BajaLogicaTipoDocumento(int idTipoDocumento) =>
            Ejecutar("UPDATE dbo.TipoDocumento SET activo = 0 WHERE id_tipo_documento = @Id",
                new { Id = idTipoDocumento }, "No se pudo dar de baja el tipo de documento.");

        public void BajaLogicaMedioPago(int idMedioPago) =>
            Ejecutar("UPDATE dbo.MedioPago SET activo = 0 WHERE id_medio_pago = @Id",
                new { Id = idMedioPago }, "No se pudo dar de baja el medio de pago.");

        /// <summary>
        /// Cuenta las localidades activas de una provincia. Dar de baja una provincia
        /// que todavia tiene localidades vigentes dejaria esas localidades apuntando a
        /// un catalogo invisible, asi que el servicio lo bloquea.
        /// </summary>
        public int ContarLocalidadesActivas(int idProvincia)
        {
            const string sql = @"
                SELECT COUNT(1) FROM dbo.Localidad
                WHERE id_provincia = @Id AND activo = 1";

            try
            {
                using var connection = Conexion.ObtenerConexionAbierta();
                return connection.ExecuteScalar<int>(sql, new { Id = idProvincia });
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("Error al verificar las localidades de la provincia.", ex);
            }
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

        private static int EjecutarEscalar(string sql, object parametros, string mensajeError)
        {
            try
            {
                using var connection = Conexion.ObtenerConexionAbierta();
                return connection.ExecuteScalar<int>(sql, parametros);
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException(mensajeError, ex);
            }
        }

        private static void Ejecutar(string sql, object parametros, string mensajeError)
        {
            try
            {
                using var connection = Conexion.ObtenerConexionAbierta();
                connection.Execute(sql, parametros);
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException(mensajeError, ex);
            }
        }
    }
}
