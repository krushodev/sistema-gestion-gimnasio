namespace SGIG.Datos
{
    /// <summary>
    /// Excepción propia de la capa de acceso a datos (RNF#06).
    /// Todo método público de SGIG.Datos captura SqlException y la relanza envuelta acá,
    /// para que ninguna excepción de base de datos llegue sin controlar a la UI.
    /// </summary>
    public class AccesoDatosException : Exception
    {
        public AccesoDatosException(string mensaje)
            : base(mensaje)
        {
        }

        public AccesoDatosException(string mensaje, Exception innerException)
            : base(mensaje, innerException)
        {
        }
    }
}
