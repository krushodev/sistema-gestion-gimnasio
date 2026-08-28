namespace SGIG.Entidades
{
    /// <summary>Localidad, perteneciente a una <see cref="Provincia"/>.</summary>
    public class Localidad
    {
        public int IdLocalidad { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int IdProvincia { get; set; }

        /// <summary>Baja lógica (RF#04): nunca se borra físicamente un catálogo.</summary>
        public bool Activo { get; set; } = true;

        /// <summary>Nombre de la provincia, para mostrar en grillas sin una consulta extra.</summary>
        public string? NombreProvincia { get; set; }
    }
}
