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

            _repositorioCheckin.Registrar(new Checkin
            {
                IdPersona = socio.IdPersona,
                FechaHora = DateTime.Now,
                Resultado = concedido ? "Concedido" : "Rechazado"
            });

            var mensaje = concedido
                ? "Acceso concedido."
                : !socio.Activo
                    ? "El socio está dado de baja."
                    : !vencimiento.HasValue
                        ? "El socio no tiene ninguna cuota registrada."
                        : "La cuota está vencida.";

            return new ResultadoCheckin
            {
                Concedido = concedido,
                NombreCompleto = $"{socio.Nombre} {socio.Apellido}",
                Mensaje = mensaje
            };
        }
    }
}
