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
        /// Da de alta un usuario. La contraseña llega en texto plano y se guarda
        /// hasheada; nunca se persiste en claro (RNF#11). Si <see cref="Usuario.IdPersona"/>
        /// ya viene cargado (se reutilizó una Persona existente, por ejemplo alguien
        /// que ya era Socio) reactiva la fila de Usuario si ya la tenía, o la inserta
        /// si nunca fue usuario; si no, da de alta Persona + Usuario completos. Igual
        /// que ServicioSocio.Alta.
        /// </summary>
        public int Alta(Usuario usuario, string contrasenia)
        {
            Validar(usuario, contrasenia, esAlta: true);
            VerificarUnicidad(usuario, idPersonaExcluida: null);

            usuario.ContraseniaHash = Hash.Calcular(contrasenia);

            if (usuario.IdPersona > 0)
            {
                if (_repositorioUsuario.ExisteFilaUsuario(usuario.IdPersona))
                {
                    _repositorioUsuario.Reactivar(usuario);
                }
                else
                {
                    _repositorioUsuario.AltaSobrePersonaExistente(usuario);
                }

                return usuario.IdPersona;
            }

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
        /// Cambia la contraseña de la propia cuenta (frmConfiguracion): valida la
        /// contraseña actual contra el hash guardado antes de aceptar la nueva, con
        /// el mismo mecanismo que usa el login (<see cref="Hash.Coincide"/>).
        /// </summary>
        public void CambiarContrasenia(int idPersona, string contraseniaActual, string contraseniaNueva)
        {
            var usuario = _repositorioUsuario.ObtenerPorId(idPersona)
                ?? throw new NegocioException("No se encontró el usuario.");

            if (!Hash.Coincide(contraseniaActual, usuario.ContraseniaHash))
            {
                throw new NegocioException("La contraseña actual no es correcta.");
            }

            if (string.IsNullOrWhiteSpace(contraseniaNueva) || contraseniaNueva.Length < 4)
            {
                throw new NegocioException("La nueva contraseña debe tener al menos 4 caracteres.");
            }

            _repositorioUsuario.ActualizarContrasenia(idPersona, Hash.Calcular(contraseniaNueva));
        }

        /// <summary>
        /// Actualiza los datos de contacto de la propia cuenta (frmConfiguracion):
        /// nombre, apellido, email, teléfono, localidad. No permite tocar
        /// documento/tipo de documento ni rol/legajo desde acá.
        /// </summary>
        public void ActualizarDatosPropios(int idPersona, string nombre, string apellido,
            string? email, string? telefono, int? idLocalidad)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new NegocioException("El nombre es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(apellido))
            {
                throw new NegocioException("El apellido es obligatorio.");
            }

            if (!string.IsNullOrWhiteSpace(email) && !Validaciones.EsEmailValido(email))
            {
                throw new NegocioException($"El email \"{email}\" no tiene un formato válido. Debe ser del estilo nombre@dominio.com.");
            }

            if (!string.IsNullOrWhiteSpace(telefono) && !Validaciones.EsTelefonoValido(telefono))
            {
                throw new NegocioException("El teléfono sólo admite números, espacios, guiones y paréntesis.");
            }

            _repositorioUsuario.ActualizarDatosPropios(idPersona, nombre.Trim(), apellido.Trim(),
                email, telefono, idLocalidad);
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

            // Formato de documento, email y teléfono con expresiones regulares (RF#09, RNF#04).
            Validaciones.ValidarDatosDePersona(usuario.Documento, usuario.IdTipoDocumento, usuario.Email, usuario.Telefono);

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
            if (_repositorioUsuario.ExisteDocumentoUsuarioActivo(usuario.Documento, idPersonaExcluida))
            {
                throw new CampoDuplicadoException(nameof(usuario.Documento),
                    $"Ya existe un usuario activo registrado con el documento {usuario.Documento}.");
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
