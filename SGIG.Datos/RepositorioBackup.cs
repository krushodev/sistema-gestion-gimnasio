using System;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

namespace SGIG.Datos
{
    public class RepositorioBackup
    {
        // SQL Server no permite parametrizar nombres de base de datos en BACKUP/ALTER DATABASE,
        // asi que se valida el identificador antes de interpolarlo para evitar inyeccion SQL.
        private static readonly Regex PatronIdentificadorValido = new(@"^[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.Compiled);

        private static void ValidarNombreBaseDatos(string nombreBaseDatos)
        {
            if (string.IsNullOrWhiteSpace(nombreBaseDatos) || !PatronIdentificadorValido.IsMatch(nombreBaseDatos))
            {
                throw new AccesoDatosException($"Nombre de base de datos inválido: '{nombreBaseDatos}'.");
            }
        }

        public void RealizarBackup(string rutaDestino, string nombreBaseDatos)
        {
            ValidarNombreBaseDatos(nombreBaseDatos);

            try
            {
                var builder = new SqlConnectionStringBuilder(Conexion.ObtenerCadena())
                {
                    InitialCatalog = "master"
                };

                using (var conn = new SqlConnection(builder.ConnectionString))
                {
                    conn.Open();
                    string query = $@"
                        BACKUP DATABASE [{nombreBaseDatos}]
                        TO DISK = @Ruta
                        WITH FORMAT, INIT, NAME = 'SGIG_Full_Backup', SKIP, NOREWIND, NOUNLOAD, STATS = 10;";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandTimeout = 120;
                        cmd.Parameters.AddWithValue("@Ruta", rutaDestino);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex) when (EsSinPermisoDeEscritura(ex))
            {
                // El archivo lo escribe el proceso de SQL Server, no la app: si SQL Server corre como
                // servicio, puede no tener permiso sobre carpetas del usuario (Escritorio, Documentos).
                throw new AccesoDatosException(
                    "SQL Server no tiene permiso para escribir en esa carpeta. " +
                    @"Elegí otra ubicación (por ejemplo, C:\Backups) e intentá de nuevo.", ex);
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("Error al generar el backup: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Error 3201 ("Cannot open backup device") con error 5 del sistema operativo (acceso
        /// denegado): SQL Server no puede escribir en la carpeta elegida.
        /// </summary>
        private static bool EsSinPermisoDeEscritura(SqlException ex)
        {
            foreach (SqlError error in ex.Errors)
            {
                if (error.Number == 3201 && error.Message.Contains("error 5(", StringComparison.Ordinal))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Lee el encabezado del archivo (RESTORE HEADERONLY, no modifica nada) y devuelve el
        /// nombre de la base de la que es copia, o null si el archivo no es un backup de SQL Server.
        /// </summary>
        public string? ObtenerBaseDelBackup(string rutaOrigen)
        {
            try
            {
                var builder = new SqlConnectionStringBuilder(Conexion.ObtenerCadena())
                {
                    InitialCatalog = "master"
                };

                using var conn = new SqlConnection(builder.ConnectionString);
                conn.Open();
                using var cmd = new SqlCommand("RESTORE HEADERONLY FROM DISK = @Ruta;", conn);
                cmd.CommandTimeout = 60;
                cmd.Parameters.AddWithValue("@Ruta", rutaOrigen);
                using var lector = cmd.ExecuteReader();
                return lector.Read() ? Convert.ToString(lector["DatabaseName"]) : null;
            }
            catch (SqlException)
            {
                // SQL Server no lo reconoce como backup (archivo dañado o de otro tipo).
                return null;
            }
        }

        public void RealizarRestore(string rutaOrigen, string nombreBaseDatos)
        {
            ValidarNombreBaseDatos(nombreBaseDatos);

            try
            {
                var builder = new SqlConnectionStringBuilder(Conexion.ObtenerCadena())
                {
                    InitialCatalog = "master"
                };

                using (var conn = new SqlConnection(builder.ConnectionString))
                {
                    conn.Open();
                    string query = $@"
                        ALTER DATABASE [{nombreBaseDatos}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                        RESTORE DATABASE [{nombreBaseDatos}] FROM DISK = @Ruta WITH REPLACE;
                        ALTER DATABASE [{nombreBaseDatos}] SET MULTI_USER;";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandTimeout = 180;
                        cmd.Parameters.AddWithValue("@Ruta", rutaOrigen);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("Error al restaurar la base de datos: " + ex.Message, ex);
            }
        }
    }
}