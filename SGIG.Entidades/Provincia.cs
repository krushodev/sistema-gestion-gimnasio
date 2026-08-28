namespace SGIG.Entidades
{
    /// <summary>Provincia. Una provincia agrupa muchas localidades.</summary>
    public class Provincia
    {
        public int IdProvincia { get; set; }
        public string Nombre { get; set; } = string.Empty;

        /// <summary>Baja lógica (RF#04): nunca se borra físicamente un catálogo.</summary>
        public bool Activo { get; set; } = true;
    }
}
