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

        /// <summary>Activos y dados de baja, para que la grilla del ABM muestre el estado.</summary>
        public IEnumerable<Socio> ObtenerTodos() => _repositorioSocio.ObtenerTodos();

        public Socio? ObtenerPorDocumento(string documento)
        {
            if (string.IsNullOrWhiteSpace(documento))
                return null;

            return _repositorioSocio.ObtenerPorDocumento(documento.Trim());
        }

        public Socio? ObtenerPorId(int idPersona) => _repositorioSocio.ObtenerPorId(idPersona);

        /// <summary>
        /// Búsqueda por nombre/apellido/documento para el selector de Cobro de Cuotas
        /// y Check-in (RF#11, RF#15). Exige un mínimo de caracteres para no traer
        /// medio catálogo de socios con cada tecla.
        /// </summary>
        public IEnumerable<Socio> BuscarPorTexto(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto) || texto.Trim().Length < 2)
                return Enumerable.Empty<Socio>();

            return _repositorioSocio.BuscarActivosPorTexto(texto.Trim());
        }

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

        /// <summary>
        /// Perfil del socio (Fase 12): junta sus datos, el plan que usa, el estado de la
        /// cuota, los pagos y los accesos recientes. No agrega consultas: reutiliza los
        /// repositorios y servicios que ya existen y calcula acá lo derivado.
        /// La parte económica (pagos, montos, total pagado) sólo se devuelve si quien
        /// consulta es Administrador: el filtro está acá y no sólo en la pantalla.
        /// </summary>
        public PerfilSocioDTO ObtenerPerfil(int idPersona, int idRolSolicitante)
        {
            var socio = _repositorioSocio.ObtenerPorId(idPersona)
                ?? throw new NegocioException("No se encontró el socio.");

            var hoy = DateTime.Today;
            var catalogo = new ServicioCatalogo();

            var pagos = new ServicioTesoreria().ObtenerHistorialSocio(idPersona).ToList();
            // El historial de accesos filtra por documento con LIKE: se queda sólo con los de este socio.
            var accesos = new ServicioCheckin()
                .ObtenerHistorial(hoy.AddDays(-90), hoy, socio.Documento)
                .Where(c => c.IdPersona == idPersona)
                .ToList();

            var localidad = socio.IdLocalidad is int idLocalidad
                ? catalogo.ObtenerLocalidades().FirstOrDefault(l => l.IdLocalidad == idLocalidad)
                : null;
            var provincia = localidad is null
                ? null
                : catalogo.ObtenerProvincias().FirstOrDefault(p => p.IdProvincia == localidad.IdProvincia);

            int? diasParaVencer = socio.FechaVencimientoCuota.HasValue
                ? (socio.FechaVencimientoCuota.Value.Date - hoy).Days
                : null;

            // El plan actual sale del último pago, pero los pagos en sí (con montos) sólo los ve el Administrador.
            bool incluyeDatosEconomicos = idRolSolicitante == Roles.Administrador;
            string? planActual = pagos.FirstOrDefault()?.PlanNombre;
            if (!incluyeDatosEconomicos) pagos = new List<Pago>();

            return new PerfilSocioDTO
            {
                Socio = socio,
                TipoDocumento = catalogo.ObtenerTiposDocumento()
                    .FirstOrDefault(t => t.IdTipoDocumento == socio.IdTipoDocumento)?.Descripcion ?? string.Empty,
                Localidad = localidad is null
                    ? null
                    : provincia is null ? localidad.Nombre : $"{localidad.Nombre}, {provincia.Nombre}",
                Edad = CalcularEdad(socio.FechaNacimiento, hoy),
                PlanActual = planActual,
                PlanPreferido = socio.IdPlan is int idPlan ? new ServicioPlan().ObtenerPorId(idPlan)?.Nombre : null,
                DiasParaVencer = diasParaVencer,
                EstadoCuota = DescribirCuota(diasParaVencer),
                IncluyeDatosEconomicos = incluyeDatosEconomicos,
                Pagos = pagos,
                TotalPagado = pagos.Sum(p => p.Monto),
                Accesos = accesos,
                AccesosUltimos30Dias = accesos.Count(c => c.FechaHora >= hoy.AddDays(-30))
            };
        }

        /// <summary>Mismo criterio que el Check-in: al día mientras el vencimiento no sea anterior a hoy.</summary>
        public static string DescribirCuota(int? diasParaVencer) => diasParaVencer switch
        {
            null => "Sin cuotas",
            < 0 => "Vencida",
            0 => "Vence hoy",
            <= 7 => "Por vencer",
            _ => "Al día"
        };

        private static int? CalcularEdad(DateTime? fechaNacimiento, DateTime hoy)
        {
            if (fechaNacimiento is not DateTime nacimiento) return null;
            int edad = hoy.Year - nacimiento.Year;
            if (nacimiento.Date > hoy.AddYears(-edad)) edad--;
            return edad;
        }

        /// <summary>Baja lógica (RNF#03).</summary>
        public void DarDeBaja(int idPersona) => _repositorioSocio.BajaLogica(idPersona);

        /// <summary>
        /// Reactiva a un socio dado de baja, sin pasar por el diálogo de alta
        /// (los datos ya están cargados: sólo se vuelve a poner activo = 1).
        /// </summary>
        public void Reactivar(int idPersona)
        {
            var socio = _repositorioSocio.ObtenerPorId(idPersona)
                ?? throw new NegocioException("No se encontró el socio.");

            if (socio.Activo)
            {
                throw new NegocioException("El socio ya está activo.");
            }

            _repositorioSocio.Reactivar(socio);
        }

        private static void Validar(Socio socio)
        {
            if (string.IsNullOrWhiteSpace(socio.Documento))
            {
                throw new NegocioException("El documento es obligatorio.");
            }

            if (socio.IdTipoDocumento <= 0)
            {
                throw new NegocioException("Seleccioná un tipo de documento.");
            }

            // Formato de documento, email y teléfono con expresiones regulares (RF#09, RNF#04).
            Validaciones.ValidarDatosDePersona(socio.Documento, socio.IdTipoDocumento, socio.Email, socio.Telefono);

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
