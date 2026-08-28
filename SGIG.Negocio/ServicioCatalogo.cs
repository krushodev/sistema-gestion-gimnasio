using SGIG.Datos;
using SGIG.Entidades;

namespace SGIG.Negocio
{
    /// <summary>
    /// Reglas de negocio de las tablas paramétricas (RF#04). Son catálogos simples:
    /// la única regla real es que la descripción/nombre no venga vacío y que la
    /// localidad tenga una provincia asociada.
    /// </summary>
    public class ServicioCatalogo
    {
        private readonly RepositorioCatalogo _repositorio = new();
        private readonly RepositorioRol _repositorioRol = new();

        // ── Rol ──────────────────────────────────────────────────────────────

        public IEnumerable<Rol> ObtenerRoles() => _repositorioRol.ObtenerActivos();

        public int AltaRol(Rol rol)
        {
            ValidarTexto(rol.NombreRol, "El nombre del rol es obligatorio.");
            return _repositorioRol.Alta(rol);
        }

        public void ModificarRol(Rol rol)
        {
            ValidarTexto(rol.NombreRol, "El nombre del rol es obligatorio.");
            _repositorioRol.Modificar(rol);
        }

        /// <summary>
        /// Da de baja un rol. No se permite si todavía lo tiene asignado algún
        /// usuario activo: ese usuario quedaría sin permisos resolubles al loguearse.
        /// </summary>
        public void BajaLogicaRol(int idRol)
        {
            var usuarios = _repositorioRol.ContarUsuariosActivos(idRol);

            if (usuarios > 0)
            {
                throw new NegocioException(
                    $"No se puede dar de baja el rol porque {usuarios} usuario(s) activo(s) lo tienen asignado. " +
                    "Reasignálos a otro rol primero.");
            }

            _repositorioRol.BajaLogica(idRol);
        }

        // ── Provincia ────────────────────────────────────────────────────────

        public IEnumerable<Provincia> ObtenerProvincias() => _repositorio.ObtenerProvincias();

        public int AltaProvincia(Provincia provincia)
        {
            ValidarTexto(provincia.Nombre, "El nombre de la provincia es obligatorio.");
            return _repositorio.AltaProvincia(provincia);
        }

        public void ModificarProvincia(Provincia provincia)
        {
            ValidarTexto(provincia.Nombre, "El nombre de la provincia es obligatorio.");
            _repositorio.ModificarProvincia(provincia);
        }

        // ── Localidad ────────────────────────────────────────────────────────

        public IEnumerable<Localidad> ObtenerLocalidades() => _repositorio.ObtenerLocalidades();

        public int AltaLocalidad(Localidad localidad)
        {
            ValidarLocalidad(localidad);
            return _repositorio.AltaLocalidad(localidad);
        }

        public void ModificarLocalidad(Localidad localidad)
        {
            ValidarLocalidad(localidad);
            _repositorio.ModificarLocalidad(localidad);
        }

        // ── TipoDocumento ────────────────────────────────────────────────────

        public IEnumerable<TipoDocumento> ObtenerTiposDocumento() => _repositorio.ObtenerTiposDocumento();

        public int AltaTipoDocumento(TipoDocumento tipoDocumento)
        {
            ValidarTexto(tipoDocumento.Descripcion, "La descripción del tipo de documento es obligatoria.");
            return _repositorio.AltaTipoDocumento(tipoDocumento);
        }

        public void ModificarTipoDocumento(TipoDocumento tipoDocumento)
        {
            ValidarTexto(tipoDocumento.Descripcion, "La descripción del tipo de documento es obligatoria.");
            _repositorio.ModificarTipoDocumento(tipoDocumento);
        }

        // ── MedioPago ────────────────────────────────────────────────────────

        public IEnumerable<MedioPago> ObtenerMediosPago() => _repositorio.ObtenerMediosPago();

        public int AltaMedioPago(MedioPago medioPago)
        {
            ValidarTexto(medioPago.Descripcion, "La descripción del medio de pago es obligatoria.");
            return _repositorio.AltaMedioPago(medioPago);
        }

        public void ModificarMedioPago(MedioPago medioPago)
        {
            ValidarTexto(medioPago.Descripcion, "La descripción del medio de pago es obligatoria.");
            _repositorio.ModificarMedioPago(medioPago);
        }

        // ── Bajas ────────────────────────────────────────────────────────────
        //
        // Baja LOGICA (RF#04, ERS v3.2): el registro se marca inactivo y desaparece
        // de grillas y combos, pero la fila queda para no invalidar el historico.

        /// <summary>
        /// Da de baja una provincia. No se permite si todavia tiene localidades
        /// activas: quedarian colgando de un catalogo que ya no se ve.
        /// </summary>
        public void BajaLogicaProvincia(int idProvincia)
        {
            var localidades = _repositorio.ContarLocalidadesActivas(idProvincia);

            if (localidades > 0)
            {
                throw new NegocioException(
                    $"No se puede dar de baja la provincia porque tiene {localidades} localidad(es) activa(s). " +
                    "Dá de baja primero esas localidades.");
            }

            _repositorio.BajaLogicaProvincia(idProvincia);
        }

        public void BajaLogicaLocalidad(int idLocalidad) =>
            _repositorio.BajaLogicaLocalidad(idLocalidad);

        public void BajaLogicaTipoDocumento(int idTipoDocumento) =>
            _repositorio.BajaLogicaTipoDocumento(idTipoDocumento);

        public void BajaLogicaMedioPago(int idMedioPago) =>
            _repositorio.BajaLogicaMedioPago(idMedioPago);

        // ── Validaciones ─────────────────────────────────────────────────────

        private static void ValidarTexto(string valor, string mensaje)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                throw new NegocioException(mensaje);
            }
        }

        private static void ValidarLocalidad(Localidad localidad)
        {
            ValidarTexto(localidad.Nombre, "El nombre de la localidad es obligatorio.");

            if (localidad.IdProvincia <= 0)
            {
                throw new NegocioException("Seleccioná la provincia a la que pertenece la localidad.");
            }
        }
    }
}
