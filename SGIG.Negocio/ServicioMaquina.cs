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

        /// <summary>
        /// Aplica la barra de filtros de Máquinas (Fase 14) sobre una lista ya cargada: texto
        /// (nombre o marca), estado y marca exacta.
        /// </summary>
        public static List<Maquina> Filtrar(IEnumerable<Maquina> maquinas, FiltroMaquinas filtro)
        {
            var texto = filtro.Texto?.Trim();

            return maquinas.Where(m =>
                    (string.IsNullOrEmpty(texto)
                        || m.Nombre.Contains(texto, StringComparison.OrdinalIgnoreCase)
                        || (m.Marca?.Contains(texto, StringComparison.OrdinalIgnoreCase) ?? false))
                    && (filtro.Estado is null || string.Equals(m.Estado, filtro.Estado, StringComparison.OrdinalIgnoreCase))
                    && (filtro.Marca is null || string.Equals(m.Marca, filtro.Marca, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        /// <summary>"Menos de 1 año", "1 año", "8 años" desde la fecha de compra; "—" si no la tiene.</summary>
        public static string Antiguedad(DateTime? fechaCompra, DateTime hoy)
        {
            if (fechaCompra is not DateTime compra) return "—";
            int anios = hoy.Year - compra.Year;
            if (compra.Date > hoy.AddYears(-anios)) anios--;
            return anios switch { < 1 => "Menos de 1 año", 1 => "1 año", _ => $"{anios} años" };
        }
    }
}
