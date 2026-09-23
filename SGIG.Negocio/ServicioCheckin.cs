using SGIG.Datos;
using SGIG.Entidades;

namespace SGIG.Negocio
{
    /// <summary>
    /// Lógica de negocio del Check-in (RF#15, RF#16, RF#17, RNF#01). Decide
    /// Concedido/Rechazado comparando la fecha de hoy contra
    /// <see cref="Socio.FechaVencimientoCuota"/> — el campo caché de <c>dbo.Socio</c>,
    /// no la Facturación vigente — para poder responder en menos de 2 segundos.
    /// </summary>
    public class ServicioCheckin
    {
        private readonly RepositorioCheckin _repositorioCheckin = new();

        /// <summary>
        /// Busca al socio por documento, decide el resultado y registra el intento
        /// (RF#17: se registra siempre, tanto si se concede como si se rechaza).
        /// </summary>
        public ResultadoCheckin RegistrarIntento(string documento)
        {
            if (string.IsNullOrWhiteSpace(documento))
            {
                throw new NegocioException("Ingresá un documento.");
            }

            var socio = _repositorioCheckin.BuscarSocioPorDocumento(documento.Trim());

            if (socio is null)
            {
                throw new NegocioException($"No existe ningún socio registrado con el documento {documento.Trim()}.");
            }

            var vencimiento = socio.FechaVencimientoCuota;
            var cuotaAlDia = vencimiento.HasValue && vencimiento.Value.Date >= DateTime.Today;
            var concedido = socio.Activo && cuotaAlDia;
            var diasRestantes = vencimiento.HasValue ? (int?)(vencimiento.Value.Date - DateTime.Today).Days : null;

            _repositorioCheckin.Registrar(new Checkin
            {
                IdPersona = socio.IdPersona,
                FechaHora = DateTime.Now,
                Resultado = concedido ? "Concedido" : "Rechazado"
            });

            var mensaje = concedido
                ? diasRestantes == 0
                    ? "Acceso concedido. La cuota vence hoy."
                    : $"Acceso concedido. Vence en {diasRestantes} día(s)."
                : !socio.Activo
                    ? "El socio está dado de baja."
                    : !vencimiento.HasValue
                        ? "El socio no tiene ninguna cuota registrada."
                        : $"La cuota está vencida hace {-diasRestantes} día(s).";

            return new ResultadoCheckin
            {
                Concedido = concedido,
                NombreCompleto = $"{socio.Nombre} {socio.Apellido}",
                Mensaje = mensaje,
                DiasRestantesCuota = diasRestantes
            };
        }

        /// <summary>
        /// Historial de accesos para seguimiento de recepción (fuera del alcance
        /// original de RF#15-17, agregado para poder ver quién entró y cuántos días
        /// le quedan antes de vencer su cuota). Valida que el rango no esté invertido.
        /// </summary>
        public IEnumerable<Checkin> ObtenerHistorial(DateTime desde, DateTime hasta, string? documento = null)
        {
            if (desde.Date > hasta.Date)
            {
                throw new NegocioException("La fecha \"Desde\" no puede ser posterior a la fecha \"Hasta\".");
            }

            return _repositorioCheckin.ObtenerHistorial(desde, hasta, documento);
        }
    }
}
