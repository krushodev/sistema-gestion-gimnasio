// Formulario armado sólo por código (no tiene .Designer.cs: no hay controles que arrastrar).
#pragma warning disable WFO1000
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.UI
{
    /// <summary>
    /// Sección propia de gráficos (Fase 11, revisada a pedido de la cátedra: antes estaban
    /// en el Inicio). Cada rol ve los suyos, uno debajo del otro ocupando toda la pantalla:
    /// Administrador → ingresos por mes e ingresos por plan; Recepcionista → accesos de hoy
    /// por hora; Técnico → mantenimientos por mes. Se abre dentro de <see cref="frmMDIParent"/>.
    /// </summary>
    public class frmGraficos : Form
    {
        /// <summary>Un gráfico y la consulta que lo llena (se ejecuta en segundo plano).</summary>
        private sealed record Definicion(TarjetaGrafico Grafico, Func<List<PuntoGraficoDTO>> Cargar);

        private readonly List<Definicion> _graficos = new();

        public frmGraficos(Usuario usuarioLogueado)
        {
            Text = "Gráficos";
            BackColor = Tema.Fondo;
            ClientSize = new Size(900, 600);

            var reportes = new ServicioReporte();
            switch (usuarioLogueado.IdRol)
            {
                case Roles.Administrador:
                    Agregar(new TarjetaGrafico("Ingresos por mes", "Facturado en los últimos 12 meses",
                            TarjetaGrafico.Orientacion.Columnas, Pesos),
                        () => reportes.ObtenerIngresosPorMes(DateTime.Today));
                    Agregar(new TarjetaGrafico("Ingresos por plan", "Facturado este mes, de mayor a menor",
                            TarjetaGrafico.Orientacion.Barras, Pesos),
                        () => reportes.ObtenerIngresosPorPlan(DateTime.Today));
                    break;

                case Roles.Recepcionista:
                    Agregar(new TarjetaGrafico("Accesos de hoy por hora", "Intentos de acceso (concedidos y rechazados)",
                            TarjetaGrafico.Orientacion.Columnas, Cantidad),
                        () => reportes.ObtenerAccesosPorHora(DateTime.Today));
                    break;

                default: // Técnico
                    Agregar(new TarjetaGrafico("Mantenimientos por mes", "Fallas registradas en los últimos 6 meses",
                            TarjetaGrafico.Orientacion.Columnas, Cantidad),
                        () => reportes.ObtenerMantenimientosPorMes(DateTime.Today));
                    break;
            }

            Controls.Add(ConstruirGrilla());
        }

        private static string Pesos(decimal valor) => $"$ {valor:N0}";
        private static string Cantidad(decimal valor) => valor.ToString("N0");

        private void Agregar(TarjetaGrafico grafico, Func<List<PuntoGraficoDTO>> cargar) =>
            _graficos.Add(new Definicion(grafico, cargar));

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
            foreach (var definicion in _graficos) _ = CargarAsync(definicion);
        }

        /// <summary>
        /// Cada gráfico se carga por separado: si una consulta falla, ese gráfico muestra el
        /// error en su lugar y los demás se ven igual (sin MessageBox).
        /// </summary>
        private async Task CargarAsync(Definicion definicion)
        {
            try
            {
                var puntos = await Task.Run(definicion.Cargar);
                if (!IsDisposed) definicion.Grafico.Mostrar(puntos);
            }
            catch (Exception ex)
            {
                if (!IsDisposed) definicion.Grafico.MostrarMensaje("No se pudo cargar el gráfico: " + ex.Message);
            }
        }
    }
}
