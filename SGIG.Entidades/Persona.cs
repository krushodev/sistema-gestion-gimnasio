namespace SGIG.Entidades
{
    /// <summary>
    /// Supertipo de <see cref="Socio"/> y <see cref="Usuario"/>. Guarda los datos
    /// personales comunes; las especializaciones comparten su <see cref="IdPersona"/>
    /// como clave (patrón "tabla por subtipo").
    /// </summary>
    public class Persona
    {
        public int IdPersona { get; set; }
        public string Documento { get; set; } = string.Empty;
        public int IdTipoDocumento { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public int? IdLocalidad { get; set; }
        public DateTime? FechaNacimiento { get; set; }
    }
}
