using SGIG.Datos;
using SGIG.Entidades;

namespace SGIG.Negocio
{
    /// <summary>
    /// Lectura de las tablas paramétricas: Provincia, Localidad, TipoDocumento y
    /// MedioPago. Son catálogos sembrados con la aplicación (ver
    /// docs/SGIG_CreateDB.sql) y no tienen ABM en la UI; este servicio solo expone
    /// las consultas que siguen alimentando combos (alta de Persona, cobro de
    /// cuota, etc).
    /// </summary>
    public class ServicioCatalogo
    {
        private readonly RepositorioCatalogo _repositorio = new();

        public IEnumerable<Provincia> ObtenerProvincias() => _repositorio.ObtenerProvincias();

        public IEnumerable<Localidad> ObtenerLocalidades() => _repositorio.ObtenerLocalidades();

        public IEnumerable<TipoDocumento> ObtenerTiposDocumento() => _repositorio.ObtenerTiposDocumento();

        public IEnumerable<MedioPago> ObtenerMediosPago() => _repositorio.ObtenerMediosPago();
    }
}
