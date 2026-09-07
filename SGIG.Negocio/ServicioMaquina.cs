using SGIG.Datos;
using SGIG.Entidades;

namespace SGIG.Negocio
{
    /// <summary>Reglas de negocio del ABM de máquinas (RF#18).</summary>
    public class ServicioMaquina
    {
        public const string EstadoOperativa = "Operativa";
        public const string EstadoEnReparacion = "En Reparacion";

        private readonly RepositorioMaquina _repositorioMaquina = new();
        private readonly RepositorioMantenimiento _repositorioMantenimiento = new();

        public IEnumerable<Maquina> ObtenerTodas() => _repositorioMaquina.ObtenerTodas();

        public IEnumerable<Maquina> ObtenerOperativas() => _repositorioMaquina.ObtenerOperativas();

        /// <summary>Toda máquina nueva arranca operativa, sin importar lo que traiga el combo.</summary>
        public int Alta(Maquina maquina)
        {
            Validar(maquina);
            maquina.Estado = EstadoOperativa;
            return _repositorioMaquina.Alta(maquina);
        }

        public void Modificar(Maquina maquina)
        {
            Validar(maquina);
            _repositorioMaquina.Modificar(maquina);
        }

        /// <summary>
        /// Sin baja lógica (dbo.Maquina no tiene columna 'activo'): se bloquea el
        /// DELETE si la máquina ya tiene mantenimientos, para no perder ese historial.
        /// </summary>
        public void Eliminar(int idMaquina)
        {
            if (_repositorioMantenimiento.ExisteParaMaquina(idMaquina))
            {
                throw new NegocioException(
                    "No se puede eliminar la máquina porque tiene mantenimientos registrados.");
            }

            _repositorioMaquina.Eliminar(idMaquina);
        }

        private static void Validar(Maquina maquina)
        {
            if (string.IsNullOrWhiteSpace(maquina.Nombre))
            {
                throw new NegocioException("El nombre de la máquina es obligatorio.");
            }
        }
    }
}
