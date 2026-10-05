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
            catch (SqlException ex)
            {
                throw new AccesoDatosException("Error al generar el backup: " + ex.Message, ex);
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