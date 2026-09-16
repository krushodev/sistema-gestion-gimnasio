using SGIG.Datos;
using SGIG.Entidades;

namespace SGIG.Negocio
{
    /// <summary>
    /// Reglas de negocio del ABM de socios (RF#05, RF#06, RF#07, RF#09): validación
    /// de campos obligatorios, reutilización de una Persona ya cargada, unicidad de
    /// documento entre socios activos y baja lógica.
    /// </summary>
    public class ServicioSocio
    {
        private readonly RepositorioSocio _repositorioSocio = new();
        private readonly ServicioPersona _servicioPersona = new();

        public IEnumerable<Socio> ObtenerActivos() => _repositorioSocio.ObtenerActivos();

        public Socio? ObtenerPorDocumento(string documento)
        {
            if (string.IsNullOrWhiteSpace(documento))
                return null;

            return _repositorioSocio.ObtenerPorDocumento(documento.Trim());
        }

        public Socio? ObtenerPorId(int idPersona) => _repositorioSocio.ObtenerPorId(idPersona);

        /// <summary>
        /// Busca una Persona ya cargada por documento, para reutilizarla al dar de
        /// alta un socio (RF#06) en vez de duplicar sus datos personales.
        /// </summary>
        public Persona? BuscarPersonaPorDocumento(string documento) =>
            _servicioPersona.BuscarPorDocumento(documento);

        /// <summary>
        /// Da de alta un socio. Si <see cref="Socio.IdPersona"/> ya viene cargado
        /// (RF#06: se reutilizó una Persona existente) reactiva la fila de Socio si
        /// ya la tenía, o la inserta si nunca fue socio; si no, da de alta Persona +
        /// Socio completos.
        /// </summary>
        public int Alta(Socio socio)
        {
            Validar(socio);
            VerificarUnicidad(socio, idPersonaExcluida: null);

            if (socio.IdPersona > 0)
            {
                if (_repositorioSocio.ExisteFilaSocio(socio.IdPersona))
                {
                    _repositorioSocio.Reactivar(socio);
                }
                else
                {
                    _repositorioSocio.AltaSobrePersonaExistente(socio);
                }

                return socio.IdPersona;
            }

            return _repositorioSocio.Alta(socio);
        }

        public void Modificar(Socio socio)
        {
            Validar(socio);
            VerificarUnicidad(socio, idPersonaExcluida: socio.IdPersona);
            _repositorioSocio.Modificar(socio);
        }

        /// <summary>Baja lógica (RNF#03).</summary>
        public void DarDeBaja(int idPersona) => _repositorioSocio.BajaLogica(idPersona);

        private static void Validar(Socio socio)
        {
            if (string.IsNullOrWhiteSpace(socio.Documento))
            {
                throw new NegocioException("El documento es obligatorio.");
            }

            // Formato de documento, email y teléfono con expresiones regulares (RF#09, RNF#04).
            Validaciones.ValidarDatosDePersona(socio.Documento, socio.Email, socio.Telefono);

            if (socio.IdTipoDocumento <= 0)
            {
                throw new NegocioException("Seleccioná un tipo de documento.");
            }

            if (string.IsNullOrWhiteSpace(socio.Nombre))
            {
                throw new NegocioException("El nombre es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(socio.Apellido))
            {
                throw new NegocioException("El apellido es obligatorio.");
            }
        }

        private void VerificarUnicidad(Socio socio, int? idPersonaExcluida)
        {
            if (_repositorioSocio.ExisteDocumentoSocioActivo(socio.Documento, idPersonaExcluida))
            {
                throw new CampoDuplicadoException(nameof(socio.Documento),
                    $"Ya existe un socio activo registrado con el documento {socio.Documento}.");
            }
        }
    }
}
