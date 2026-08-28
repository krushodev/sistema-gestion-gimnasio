namespace SGIG.Entidades
{
    /// <summary>Rol de un usuario del sistema: Administrador, Recepcionista o Técnico.</summary>
    public class Rol
    {
        public int IdRol { get; set; }
        public string NombreRol { get; set; } = string.Empty;
        public string? Descripcion { get; set; }

        /// <summary>Baja lógica (RF#04): nunca se borra físicamente un catálogo.</summary>
        public bool Activo { get; set; } = true;
    }
}
