namespace SGIG.Entidades
{
    /// <summary>
    /// Usuario del sistema: una <see cref="Persona"/> que además puede iniciar sesión.
    /// Hereda de Persona porque en la base es su especialización (comparten
    /// <see cref="Persona.IdPersona"/> como clave), así una sola consulta con JOIN
    /// mapea a esta clase sin partirla en dos objetos.
    /// </summary>
    public class Usuario : Persona
    {
        public string NombreUsuario { get; set; } = string.Empty;

        /// <summary>Hash SHA256 de la contraseña (RNF#11). Nunca la contraseña en texto plano.</summary>
        public byte[] ContraseniaHash { get; set; } = Array.Empty<byte>();

        public int IdRol { get; set; }
        public string Legajo { get; set; } = string.Empty;
        public DateTime? FechaIngreso { get; set; }

        /// <summary>Baja lógica (RNF#03): nunca se borra físicamente un usuario.</summary>
        public bool Activo { get; set; } = true;

        /// <summary>Rol asociado, resuelto por el repositorio al autenticar.</summary>
        public Rol? Rol { get; set; }

        /// <summary>
        /// Nombre del rol resuelto por el JOIN, para mostrarlo en la grilla sin una
        /// consulta extra. Lo llena <c>ObtenerActivos</c>; en el camino del login se
        /// usa <see cref="Rol"/>, que trae el objeto completo.
        /// </summary>
        public string? NombreRol { get; set; }
    }
}
