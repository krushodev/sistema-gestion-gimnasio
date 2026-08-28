namespace SGIG.Negocio
{
    /// <summary>
    /// Error de regla de negocio: algo que el usuario hizo mal y puede corregir
    /// (un campo obligatorio vacío, un nombre de usuario repetido). A diferencia de
    /// AccesoDatosException, su mensaje está pensado para mostrarse tal cual en la UI.
    /// </summary>
    public class NegocioException : Exception
    {
        public NegocioException(string mensaje)
            : base(mensaje)
        {
        }

        public NegocioException(string mensaje, Exception innerException)
            : base(mensaje, innerException)
        {
        }
    }

    /// <summary>
    /// Un campo que debe ser único ya está tomado por otro registro
    /// (nombre de usuario, legajo, documento).
    /// </summary>
    public class CampoDuplicadoException : NegocioException
    {
        /// <summary>Nombre del campo duplicado, para que la UI pueda enfocarlo.</summary>
        public string Campo { get; }

        public CampoDuplicadoException(string campo, string mensaje)
            : base(mensaje)
        {
            Campo = campo;
        }
    }
}
