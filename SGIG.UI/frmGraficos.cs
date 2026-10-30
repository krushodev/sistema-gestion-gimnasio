// Formulario armado sólo por código (no tiene .Designer.cs: no hay controles que arrastrar).
#pragma warning disable WFO1000
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.UI
{
    /// <summary>
    /// Sección propia de gráficos (Fase 11). Desde la Fase 14 todos se filtran por fecha con
    /// <see cref="ucFiltroFechas"/> y muestran una línea de resumen (total, promedio, máximo).
    /// Cada rol ve los suyos, uno debajo del otro ocupando toda la pantalla:
    /// Administrador → ingresos en el tiempo e ingresos por plan; Recepcionista → accesos por
    /// hora; Técnico → mantenimientos en el tiempo. Se abre dentro de <see cref="frmMDIParent"/>.
    /// </summary>
    //
    // ── CONTROLES (creados por código) ───────────────────────────────────────
    //   ucFiltroFechas (cboPeriodo, dtpDesde, dtpHasta) y una TarjetaGrafico por gráfico
    // ───────────────────────────────────────────────────────────────────────────
    public class frmGraficos : Form
    {
        /// <summary>
        /// Un gráfico: la consulta que lo llena para un rango, cómo se describe el período
        /// (subtítulo) y cómo se resume lo que muestra.
        /// </summary>
        private sealed record Definicion(
            TarjetaGrafico Grafico,
            Func<DateTime, DateTime, List<PuntoGraficoDTO>> Cargar,
            Func<DateTime, DateTime, string> Subtitulo,
            Func<List<PuntoGraficoDTO>, DateTime, DateTime, string> Resumir);

        private readonly List<Definicion> _graficos = new();
        private readonly ucFiltroFechas _filtro;
        private int _versionCarga;

        public frmGraficos(Usuario usuarioLogueado)
        {
            Text = "Gráficos";
            BackColor = Tema.Fondo;
            ClientSize = new Size(900, 600);

            var reportes = new ServicioReporte();
            ucFiltroFechas.Periodo periodoInicial;
            switch (usuarioLogueado.IdRol)
            {
                case Roles.Administrador:
                    periodoInicial = ucFiltroFechas.Periodo.Ultimos12Meses;
                    _graficos.Add(new Definicion(
                        new TarjetaGrafico("Ingresos en el tiempo", "", TarjetaGrafico.Orientacion.Columnas, Pesos),
                        reportes.ObtenerIngresosEnElTiempo,
                        (d, h) => $"Facturado {Agrupacion(d, h)}",
                        (p, d, h) => ResumenTemporal(p, d, h, Pesos, "Mejor")));
                    _graficos.Add(new Definicion(
                        new TarjetaGrafico("Ingresos por plan", "", TarjetaGrafico.Orientacion.Barras, Pesos),
                        reportes.ObtenerIngresosPorPlan,
                        (d, h) => "Facturado en el período, de mayor a menor",
                        (p, d, h) => ResumenPorPlan(p)));
                    break;

                case Roles.Recepcionista:
                    periodoInicial = ucFiltroFechas.Periodo.Hoy;
                    _graficos.Add(new Definicion(
                        new TarjetaGrafico("Accesos por hora", "", TarjetaGrafico.Orientacion.Columnas, Cantidad),
                        reportes.ObtenerAccesosPorHora,
                        (d, h) => d == h
                            ? "Intentos de acceso (concedidos y rechazados) del día, por franja horaria"
                            : "Intentos de acceso (concedidos y rechazados) del período, sumados por franja horaria",
                        ResumenPorHora));
                    break;

                default: // Técnico
                    periodoInicial = ucFiltroFechas.Periodo.Ultimos6Meses;
                    _graficos.Add(new Definicion(
                        new TarjetaGrafico("Mantenimientos en el tiempo", "", TarjetaGrafico.Orientacion.Columnas, Cantidad),
                        reportes.ObtenerMantenimientosEnElTiempo,
                        (d, h) => $"Fallas registradas {Agrupacion(d, h)}",
                        (p, d, h) => ResumenTemporal(p, d, h, Cantidad, "Con más fallas")));
                    break;
            }

            _filtro = new ucFiltroFechas(periodoInicial) { Location = new Point(0, 8) };
            _filtro.RangoCambiado += (s, e) => _ = CargarAsync();

            var barra = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = Tema.Fondo };
            barra.Controls.Add(_filtro);

            // Dock=Top se apila al revés del orden de alta: primero el Fill.
            Controls.Add(ConstruirGrilla());
            Controls.Add(barra);
        }

        private static string Pesos(decimal valor) => $"$ {valor:N0}";
        private static string Cantidad(decimal valor) => valor.ToString("N0");

        /// <summary>"por día" o "por mes", según el largo del rango (lo decide el servicio).</summary>
        private static string Agrupacion(DateTime desde, DateTime hasta) =>
            ServicioReporte.AgrupaPorDia(desde, hasta) ? "por día" : "por mes";

        /// <summary>Un gráfico por fila, todas del mismo alto, separadas por 16 px.</summary>
        private Control ConstruirGrilla()
        {
            var tabla = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = _graficos.Count,
                BackColor = Tema.Fondo
            };
            tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            for (int i = 0; i < _graficos.Count; i++)
            {
                tabla.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / _graficos.Count));
                var grafico = _graficos[i].Grafico;
                grafico.Margin = new Padding(0, 0, 0, i < _graficos.Count - 1 ? 16 : 0);
                tabla.Controls.Add(grafico, 0, i);
            }
            return tabla;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _ = CargarAsync();
        }

        /// <summary>
        /// Recarga todos los gráficos con el rango actual. Cada uno se carga por separado: si una
        /// consulta falla (o el rango no es válido), ese gráfico muestra el error en su lugar.
        /// </summary>
        private async Task CargarAsync()
        {
            int version = ++_versionCarga;
            DateTime desde = _filtro.Desde, hasta = _filtro.Hasta;

            await Task.WhenAll(_graficos.Select(async definicion =>
            {
                var grafico = definicion.Grafico;
                grafico.Subtitulo = $"{definicion.Subtitulo(desde, hasta)} · {_filtro.Descripcion}";
                grafico.MostrarMensaje("Cargando…");
                try
                {
                    var puntos = await Task.Run(() => definicion.Cargar(desde, hasta));
                    if (IsDisposed || version != _versionCarga) return;
                    grafico.Mostrar(puntos, definicion.Resumir(puntos, desde, hasta));
                }
                catch (Exception ex)
                {
                    if (IsDisposed || version != _versionCarga) return;
                    grafico.MostrarMensaje(ex is NegocioException ? ex.Message : "No se pudo cargar el gráfico: " + ex.Message);
                }
            }));
        }

        // ── Resúmenes (sólo describen lo que ya muestra cada gráfico) ────────

        private static string ResumenTemporal(List<PuntoGraficoDTO> puntos, DateTime desde, DateTime hasta,
            Func<decimal, string> formato, string rotuloMaximo)
        {
            if (puntos.Count == 0) return string.Empty;
            string unidad = ServicioReporte.AgrupaPorDia(desde, hasta) ? "día" : "mes";
            var maximo = puntos.OrderByDescending(p => p.Valor).First();
            return $"Total {formato(puntos.Sum(p => p.Valor))}  ·  Promedio por {unidad} {formato(puntos.Average(p => p.Valor))}"
                 + $"  ·  {rotuloMaximo} {unidad}: {maximo.Etiqueta} ({formato(maximo.Valor)})";
        }

        private static string ResumenPorPlan(List<PuntoGraficoDTO> puntos)
        {
            decimal total = puntos.Sum(p => p.Valor);
            if (puntos.Count == 0 || total == 0) return string.Empty;
            var primero = puntos.OrderByDescending(p => p.Valor).First();
            int conVentas = puntos.Count(p => p.Valor > 0);
            return $"Total {Pesos(total)}  ·  Plan que más facturó: {primero.Etiqueta} ({ServicioReporte.Porcentaje(primero.Valor, total):N1} %)"
                 + $"  ·  {conVentas} de {puntos.Count} planes con ventas";
        }

        private static string ResumenPorHora(List<PuntoGraficoDTO> puntos, DateTime desde, DateTime hasta)
        {
            decimal total = puntos.Sum(p => p.Valor);
            if (total == 0) return string.Empty;
            var pico = puntos.OrderByDescending(p => p.Valor).First();
            int dias = (hasta - desde).Days + 1;
            string resumen = $"Total {Cantidad(total)} accesos  ·  Hora pico: {pico.Etiqueta} h ({Cantidad(pico.Valor)})";
            return dias > 1 ? resumen + $"  ·  Promedio {total / dias:N1} por día" : resumen;
        }
    }
}
