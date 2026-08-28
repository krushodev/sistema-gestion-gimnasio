namespace SGIG.Entidades
{
    /// <summary>Tipo de documento de identidad: DNI, Pasaporte, Cédula.</summary>
    public class TipoDocumento
    {
        public int IdTipoDocumento { get; set; }
        public string Descripcion { get; set; } = string.Empty;

        /// <summary>Baja lógica (RF#04): nunca se borra físicamente un catálogo.</summary>
        public bool Activo { get; set; } = true;
    }
}
