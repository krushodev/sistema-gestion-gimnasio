using SGIG.Datos;
using SGIG.Entidades;

namespace SGIG.Negocio
{
    /// <summary>
    /// Reglas de negocio de Mantenimiento (RF#19, RF#20, RF#21). El técnico a cargo
    /// siempre es el usuario logueado, nunca un dato que se tipee a mano.
    /// </summary>
    public class ServicioMantenimiento
    {
        private readonly RepositorioMantenimiento _repositorioMantenimiento = new();

        public IEnumerable<Mantenimiento> ObtenerActivos() => _repositorioMantenimiento.ObtenerActivos();

        public IEnumerable<Mantenimiento> ObtenerPorMaquina(int idMaquina) =>
            _repositorioMantenimiento.ObtenerPorMaquina(idMaquina);

        /// <summary>
        /// Registra un mantenimiento nuevo. <paramref name="idTecnicoLogueado"/> es
        /// siempre el id_persona del Usuario con la sesión abierta (RF#20).
        /// </summary>
        public int Registrar(int idMaquina, int idTecnicoLogueado, DateTime fechaInicio, string? detalleTecnico)
        {
            if (idMaquina <= 0)
            {
                throw new NegocioException("Seleccioná una máquina.");
            }

            var mantenimiento = new Mantenimiento
            {
                IdMaquina = idMaquina,
                IdPersona = idTecnicoLogueado,
                FechaInicio = fechaInicio.Date,
                DetalleTecnico = string.IsNullOrWhiteSpace(detalleTecnico) ? null : detalleTecnico.Trim()
            };

            return _repositorioMantenimiento.Registrar(mantenimiento);
        }

        /// <summary>Cierra el mantenimiento seleccionado y libera la máquina (RF#20).</summary>
        public void Finalizar(Mantenimiento mantenimiento, DateTime fechaFin)
        {
            if (fechaFin.Date < mantenimiento.FechaInicio.Date)
            {
                throw new NegocioException("La fecha de fin no puede ser anterior a la fecha de inicio.");
            }

            _repositorioMantenimiento.Finalizar(mantenimiento.IdMantenimiento, mantenimiento.IdMaquina, fechaFin.Date);
        }
    }
}
