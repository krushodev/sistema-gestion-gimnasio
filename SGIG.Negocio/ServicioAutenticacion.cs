using SGIG.Datos;
using SGIG.Entidades;

namespace SGIG.Negocio
{
    /// <summary>
    /// Autenticación de usuarios contra la base (RF#01). Valida las credenciales
    /// comparando el hash SHA256 de lo que se tipeó contra el hash guardado.
    /// </summary>
    public class ServicioAutenticacion
    {
        private readonly RepositorioUsuario _repositorioUsuario = new();

        /// <summary>
        /// Valida usuario y contraseña. Devuelve el <see cref="Usuario"/> con su
        /// <see cref="Rol"/> cargado si son correctos, o <c>null</c> si no lo son.
        /// </summary>
        /// <remarks>
        /// Devuelve null indistintamente si el usuario no existe, está dado de baja
        /// o la contraseña es incorrecta: la UI muestra un único mensaje genérico para
        /// no revelar cuál de las tres cosas pasó.
        /// </remarks>
        public Usuario? Autenticar(string nombreUsuario, string contrasenia)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario) || string.IsNullOrWhiteSpace(contrasenia))
            {
                return null;
            }

            var usuario = _repositorioUsuario.ObtenerPorNombreUsuario(nombreUsuario.Trim());

            if (usuario is null)
            {
                return null;
            }

            return Hash.Coincide(contrasenia, usuario.ContraseniaHash) ? usuario : null;
        }
    }
}
