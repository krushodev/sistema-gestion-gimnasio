using SGIG.Datos;
using SGIG.Entidades;

namespace SGIG.Negocio
{
    /// <summary>
    /// Reglas de negocio del ABM de usuarios (RF#03): validación de campos
    /// obligatorios, unicidad de documento/legajo/nombre de usuario, hash de la
    /// contraseña y baja lógica.
    /// </summary>
    public class ServicioUsuario
    {
        private readonly RepositorioUsuario _repositorioUsuario = new();
        private readonly RepositorioRol _repositorioRol = new();

        public IEnumerable<Usuario> ObtenerActivos() => _repositorioUsuario.ObtenerActivos();

        public Usuario? ObtenerPorId(int idPersona) => _repositorioUsuario.ObtenerPorId(idPersona);

        public IEnumerable<Rol> ObtenerRoles() => _repositorioRol.ObtenerActivos();

        /// <summary>
        /// Da de alta Persona + Usuario. La contraseña llega en texto plano y se
        /// guarda hasheada; nunca se persiste en claro (RNF#11).
        /// </summary>
        public int Alta(Usuario usuario, string contrasenia)
        {
            Validar(usuario, contrasenia, esAlta: true);
            VerificarUnicidad(usuario, idPersonaExcluida: null);

            usuario.ContraseniaHash = Hash.Calcular(contrasenia);
            return _repositorioUsuario.Alta(usuario);
        }

        /// <summary>
        /// Modifica Persona + Usuario. Si <paramref name="contrasenia"/> viene vacía
        /// se conserva la contraseña actual; sólo se recalcula el hash si se tipeó una nueva.
        /// </summary>
        public void Modificar(Usuario usuario, string contrasenia)
        {
            Validar(usuario, contrasenia, esAlta: false);
            VerificarUnicidad(usuario, idPersonaExcluida: usuario.IdPersona);

            usuario.ContraseniaHash = string.IsNullOrWhiteSpace(contrasenia)
                ? Array.Empty<byte>()
                : Hash.Calcular(contrasenia);

            _repositorioUsuario.Modificar(usuario);
        }

        /// <summary>
        /// Baja lógica (RNF#03). No permite que un administrador se dé de baja a sí
        /// mismo, para no dejar el sistema sin sesión activa.
        /// </summary>
        public void DarDeBaja(int idPersona, int idPersonaLogueada)
        {
            if (idPersona == idPersonaLogueada)
            {
                throw new NegocioException("No podés darte de baja a vos mismo mientras tenés la sesión abierta.");
            }

            _repositorioUsuario.BajaLogica(idPersona);
        }

        private static void Validar(Usuario usuario, string contrasenia, bool esAlta)
        {
            if (string.IsNullOrWhiteSpace(usuario.Documento))
            {
                throw new NegocioException("El documento es obligatorio.");
            }

            if (usuario.IdTipoDocumento <= 0)
            {
                throw new NegocioException("Seleccioná un tipo de documento.");
            }

            if (string.IsNullOrWhiteSpace(usuario.Nombre))
            {
                throw new NegocioException("El nombre es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(usuario.Apellido))
            {
                throw new NegocioException("El apellido es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(usuario.NombreUsuario))
            {
                throw new NegocioException("El nombre de usuario es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(usuario.Legajo))
            {
                throw new NegocioException("El legajo es obligatorio.");
            }

            if (usuario.IdRol <= 0)
            {
                throw new NegocioException("Seleccioná un rol.");
            }

            // En el alta la contraseña es obligatoria; en la edición, opcional.
            if (esAlta && string.IsNullOrWhiteSpace(contrasenia))
            {
                throw new NegocioException("La contraseña es obligatoria.");
            }

            if (!string.IsNullOrWhiteSpace(contrasenia) && contrasenia.Length < 4)
            {
                throw new NegocioException("La contraseña debe tener al menos 4 caracteres.");
            }
        }

        private void VerificarUnicidad(Usuario usuario, int? idPersonaExcluida)
        {
            if (_repositorioUsuario.ExisteDocumento(usuario.Documento, idPersonaExcluida))
            {
                throw new CampoDuplicadoException(nameof(usuario.Documento),
                    $"Ya existe una persona con el documento {usuario.Documento}.");
            }

            if (_repositorioUsuario.ExisteNombreUsuario(usuario.NombreUsuario, idPersonaExcluida))
            {
                throw new CampoDuplicadoException(nameof(usuario.NombreUsuario),
                    $"El nombre de usuario \"{usuario.NombreUsuario}\" ya está en uso.");
            }

            if (_repositorioUsuario.ExisteLegajo(usuario.Legajo, idPersonaExcluida))
            {
                throw new CampoDuplicadoException(nameof(usuario.Legajo),
                    $"El legajo {usuario.Legajo} ya está asignado a otro usuario.");
            }
        }
    }
}
