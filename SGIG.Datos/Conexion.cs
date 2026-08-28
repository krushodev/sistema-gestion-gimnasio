using System.Configuration;
using Microsoft.Data.SqlClient;

namespace SGIG.Datos
{
    /// <summary>
    /// Punto único de acceso a la cadena de conexión del sistema.
    /// La cadena vive exclusivamente en el App.config de SGIG.UI (entrada "SGIG");
    /// ningún formulario ni repositorio debe hardcodearla.
    /// </summary>
    public static class Conexion
    {
        private const string NombreCadena = "SGIG";

        /// <summary>
        /// Devuelve la cadena de conexión configurada en App.config.
        /// </summary>
        public static string ObtenerCadena()
        {
            var configuracion = ConfigurationManager.ConnectionStrings[NombreCadena];

            if (configuracion is null || string.IsNullOrWhiteSpace(configuracion.ConnectionString))
            {
                throw new AccesoDatosException(
                    $"No se encontró la cadena de conexión \"{NombreCadena}\" en App.config.");
            }

            return configuracion.ConnectionString;
        }

        /// <summary>
        /// Crea y abre una conexión a SQL Server. El llamador es responsable de cerrarla
        /// (usar siempre con 'using var connection = Conexion.ObtenerConexionAbierta();').
        /// </summary>
        public static SqlConnection ObtenerConexionAbierta()
        {
            var connection = new SqlConnection(ObtenerCadena());

            try
            {
                connection.Open();
                return connection;
            }
            catch (SqlException ex)
            {
                connection.Dispose();
                throw new AccesoDatosException("No se pudo establecer la conexión con la base de datos.", ex);
            }
        }
    }
}
