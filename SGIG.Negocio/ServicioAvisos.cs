using SGIG.Entidades;

namespace SGIG.Negocio
{
    /// <summary>
    /// Arma el bloque "Para atender hoy" del Inicio (Fase 13): qué pendientes tiene cada
    /// rol. Las reglas (qué es un pendiente, los 7 días de margen) viven acá y no en la
    /// pantalla. No agrega consultas: reutiliza los servicios de socios y mantenimientos.
    /// </summary>
    public class ServicioAvisos
    {
        /// <summary>Días de anticipación para avisar un vencimiento y días para considerar demorada una reparación.</summary>
        public const int DiasDeMargen = 7;

        public List<AvisoDTO> ObtenerAvisos(int idRol, DateTime hoy)
        {
            var avisos = new List<AvisoDTO>();
            hoy = hoy.Date;

            if (idRol == Roles.Administrador || idRol == Roles.Recepcionista)
            {
                var servicioSocio = new ServicioSocio();
                var activos = servicioSocio.ObtenerActivos().ToList();

                int vencidas = activos.Count(s => s.FechaVencimientoCuota.HasValue && s.FechaVencimientoCuota.Value.Date < hoy);
                if (vencidas > 0)
                    avisos.Add(Aviso("Peligro", Plural(vencidas, "socio con la cuota vencida", "socios con la cuota vencida"), "Cuotas"));

                int porVencer = servicioSocio.ObtenerPorVencer(hoy, DiasDeMargen).Count;
                if (porVencer > 0)
                    avisos.Add(Aviso("Aviso", Plural(porVencer, "cuota vence", "cuotas vencen") + $" en los próximos {DiasDeMargen} días", "Cuotas"));

                if (idRol == Roles.Recepcionista)
                {
                    var cumpleanieros = activos
                        .Where(s => ServicioSocio.EsCumpleanios(s.FechaNacimiento, hoy))
                        .Select(s => $"{s.Nombre} {s.Apellido}")
                        .ToList();
                    if (cumpleanieros.Count > 0)
                        avisos.Add(Aviso("Info", (cumpleanieros.Count == 1 ? "Hoy cumple años " : "Hoy cumplen años ")
                            + Enumerar(cumpleanieros), "Cumpleanios"));
                }
            }

            if (idRol == Roles.Administrador || idRol == Roles.Tecnico)
            {
                int demoradas = new ServicioMantenimiento().ObtenerActivos()
                    .Count(m => m.FechaInicio.Date <= hoy.AddDays(-DiasDeMargen));
                if (demoradas > 0)
                    avisos.Add(Aviso("Aviso", Plural(demoradas, "máquina", "máquinas")
                        + $" en reparación hace más de {DiasDeMargen} días", "Maquinas"));
            }

            if (avisos.Count == 0)
                avisos.Add(Aviso("Ok", "Todo al día: no hay pendientes", "General"));

            return avisos;
        }

        private static AvisoDTO Aviso(string nivel, string texto, string categoria) =>
            new() { Nivel = nivel, Texto = texto, Categoria = categoria };

        /// <summary>"1 socio con…" / "3 socios con…".</summary>
        private static string Plural(int cantidad, string singular, string plural) =>
            $"{cantidad} {(cantidad == 1 ? singular : plural)}";

        /// <summary>"Ana", "Ana y Juan", "Ana, Juan y Luis".</summary>
        private static string Enumerar(List<string> nombres) =>
            nombres.Count == 1 ? nombres[0] : string.Join(", ", nombres.Take(nombres.Count - 1)) + " y " + nombres[^1];
    }
}
