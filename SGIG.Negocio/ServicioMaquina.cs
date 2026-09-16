using SGIG.Datos;
using SGIG.Entidades;

namespace SGIG.Negocio
{
    /// <summary>
    /// Maquina es un catálogo sembrado con la aplicación (ver
    /// docs/SGIG_CreateDB.sql) y no tiene ABM en la UI; este servicio solo expone
    /// lectura, usada por la consulta de frmMaquinas y por los combos de
    /// Mantenimiento/Historial de Mantenimientos.
    /// </summary>
    public class ServicioMaquina
    {
        private readonly RepositorioMaquina _repositorioMaquina = new();

        public IEnumerable<Maquina> ObtenerTodas() => _repositorioMaquina.ObtenerTodas();

        public IEnumerable<Maquina> ObtenerOperativas() => _repositorioMaquina.ObtenerOperativas();
    }
}
