namespace SGIG.Negocio
{
    /// <summary>
    /// Resultado de un intento de check-in (RF#16), listo para mostrar en
    /// <c>frmCheckin</c> sin que la UI tenga que repetir la lógica de negocio.
    /// </summary>
    public class ResultadoCheckin
    {
        public bool Concedido { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
    }
}
