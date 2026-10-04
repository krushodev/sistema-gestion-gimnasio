using System;
using System.IO;
using Microsoft.Data.SqlClient;
using SGIG.Datos;

namespace SGIG.Negocio
{
    public class ServicioBackup
    {
        private readonly RepositorioBackup _repoBackup;

        public ServicioBackup()
        {
            _repoBackup = new RepositorioBackup();
        }

        private string ObtenerNombreBaseDatos()
        {
            var builder = new SqlConnectionStringBuilder(Conexion.ObtenerCadena());
            if (string.IsNullOrWhiteSpace(builder.InitialCatalog))
            {
                throw new NegocioException("No se pudo determinar el nombre de la base de datos desde la cadena de conexión.");
            }
            return builder.InitialCatalog;
        }

        public void GenerarCopiaSeguridad(string rutaArchivo)
        {
            if (string.IsNullOrWhiteSpace(rutaArchivo))
                throw new NegocioException("Debe seleccionar una ruta de archivo válida.");

            string directorio = Path.GetDirectoryName(rutaArchivo);
            if (!string.IsNullOrEmpty(directorio) && !Directory.Exists(directorio))
                throw new NegocioException("El directorio de destino no existe.");

            string nombreBd = ObtenerNombreBaseDatos();
            _repoBackup.RealizarBackup(rutaArchivo, nombreBd);
        }

        public void RestaurarCopiaSeguridad(string rutaArchivo)
        {
            if (string.IsNullOrWhiteSpace(rutaArchivo) || !File.Exists(rutaArchivo))
                throw new NegocioException("El archivo de respaldo seleccionado no existe o no es accesible.");

            string nombreBd = ObtenerNombreBaseDatos();
            _repoBackup.RealizarRestore(rutaArchivo, nombreBd);
        }
    }
}