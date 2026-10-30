using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.UI
{
    /// <summary>
    /// Reportes por período (RF#24, ampliado en la Fase 14 por la devolución de la cátedra).
    /// Sólo Administrador. Un selector con 5 reportes (ingresos por plan, por medio de pago,
    /// detalle de cobros, asistencia y mantenimientos), todos filtrados por fecha con
    /// <see cref="ucFiltroFechas"/>, con tarjetas de indicadores del período calculados en
    /// <see cref="ServicioReporte"/> y exportación a CSV de lo que muestra la grilla.
    /// </summary>
    //
    // ── CONTROLES (creados por código; frmReportes.Designer.cs no declara ninguno) ──
    //   cboTipoReporte, ucFiltroFechas (cboPeriodo, dtpDesde, dtpHasta), btnExportar,
    //   pnlIndicadores (TarjetaMetrica por indicador), dgvReporte
    // ───────────────────────────────────────────────────────────────────────────
    public partial class frmReportes : Form
    {
        /// <summary>Un reporte: su ícono, sus columnas y cómo se genera para un rango.</summary>
        private sealed record TipoReporte(
            string Nombre, string Icono, Action<DataGridView> ConfigurarColumnas,
            Func<DateTime, DateTime, (IList Filas, List<IndicadorDTO> Indicadores)> Generar);

        private readonly ServicioReporte _servicioReporte = new();
        private readonly List<TipoReporte> _tipos;

        private ComboBox cboTipoReporte = null!;
        private ucFiltroFechas _filtro = null!;
        private Button btnExportar = null!;
        private PanelTarjetas pnlIndicadores = null!;
        private DataGridView dgvReporte = null!;
        private Label lblResultado = null!;
        private int _versionCarga;

        public frmReportes()
        {
            Text = "SGIG — Reportes";
            Size = new Size(740, 520);
            StartPosition = FormStartPosition.CenterParent;

            _tipos = new List<TipoReporte>
            {
                new("Ingresos por plan", Iconos.Planes, ColumnasIngresosPorPlan,
                    (d, h) => Desarmar(_servicioReporte.ReporteIngresosPorPlan(d, h))),
                new("Ingresos por medio de pago", Iconos.Cobro, ColumnasMedioPago,
                    (d, h) => Desarmar(_servicioReporte.ReporteIngresosPorMedioPago(d, h))),
                new("Detalle de cobros", Iconos.Cobro, ColumnasCobros,
                    (d, h) => Desarmar(_servicioReporte.ReporteDetalleCobros(d, h))),
                new("Asistencia por día", Iconos.Acceso, ColumnasAsistencia,
                    (d, h) => Desarmar(_servicioReporte.ReporteAsistencia(d, h))),
                new("Mantenimientos", Iconos.Mantenimiento, ColumnasMantenimientos,
                    (d, h) => Desarmar(_servicioReporte.ReporteMantenimientos(d, h, DateTime.Today))),
            };

            Tema.EstilizarFormulario(this);
            InicializarComponentes();
            Shown += async (s, e) => await GenerarAsync();
        }

        private static (IList, List<IndicadorDTO>) Desarmar<T>(ReporteDTO<T> reporte) => (reporte.Filas, reporte.Indicadores);

        /// <summary>
        /// Layout por Dock: encabezado, barra de filtros (tipo de reporte + fechas, exportar a
        /// la derecha), tarjetas de indicadores y, ocupando el resto, la grilla.
        /// </summary>
        private void InicializarComponentes()
        {
            Padding = new Padding(0);

            var pnlEncabezado = new Panel { Dock = DockStyle.Top, Height = 72, BackColor = Tema.Fondo };
            pnlEncabezado.Controls.Add(new Label
            {
                Text = "Reportes por período",
                Font = Tema.Fuente(16F, FontStyle.Bold),
                ForeColor = Tema.TextoPrimario,
                Location = new Point(0, 0),
                AutoSize = true
            });
            pnlEncabezado.Controls.Add(new Label
            {
                Text = "Elegí un reporte y el período: los indicadores y el detalle se actualizan solos.",
                Font = Tema.FuenteSubtitulo,
                ForeColor = Tema.TextoSecundario,
                Location = new Point(2, 42),
                AutoSize = true
            });

            // Barra de filtros: reporte y fechas a la izquierda (pueden pasar a dos filas), exportar a la derecha.
            var pnlFiltro = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = Tema.Fondo };
            var flpFiltro = new FlowLayoutPanel
            {
                Location = new Point(0, 8),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = true,
                BackColor = Tema.Fondo
            };
            cboTipoReporte = new ComboBox
            {
                Name = "cboTipoReporte",
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 210,
                Margin = new Padding(0, 4, 0, 0)
            };
            cboTipoReporte.Items.AddRange(_tipos.Select(t => (object)t.Nombre).ToArray());
            cboTipoReporte.SelectedIndex = 0;
            cboTipoReporte.SelectedIndexChanged += async (s, e) => await GenerarAsync();

            _filtro = new ucFiltroFechas(ucFiltroFechas.Periodo.EsteMes) { Margin = new Padding(16, 0, 0, 0) };
            _filtro.RangoCambiado += async (s, e) => await GenerarAsync();

            flpFiltro.Controls.Add(new Label
            {
                Text = "Reporte",
                AutoSize = true,
                Font = Tema.Fuente(9F, FontStyle.Bold),
                ForeColor = Tema.TextoSecundario,
                Margin = new Padding(0, 9, 6, 0)
            });
            flpFiltro.Controls.Add(cboTipoReporte);
            flpFiltro.Controls.Add(_filtro);

            btnExportar = new Button
            {
                Name = "btnExportarCerrar", // variante secundaria por convención de nombre
                Text = "Exportar a CSV",
                Size = new Size(150, 34),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnExportar.Click += btnExportar_Click;

            pnlFiltro.Controls.AddRange(new Control[] { flpFiltro, btnExportar });
            void AcomodarFiltro()
            {
                btnExportar.Location = new Point(pnlFiltro.ClientSize.Width - btnExportar.Width, 10);
                flpFiltro.MaximumSize = new Size(Math.Max(200, pnlFiltro.ClientSize.Width - btnExportar.Width - 16), 0);
                pnlFiltro.Height = Math.Max(52, flpFiltro.Bottom + 8);
            }
            pnlFiltro.Resize += (s, e) => AcomodarFiltro();
            flpFiltro.SizeChanged += (s, e) => AcomodarFiltro();

            pnlIndicadores = new PanelTarjetas();

            lblResultado = new Label
            {
                Dock = DockStyle.Top,
                Height = 28,
                Font = Tema.Fuente(9F),
                ForeColor = Tema.TextoSecundario,
                TextAlign = ContentAlignment.MiddleLeft
            };

            dgvReporte = new DataGridView
            {
                Name = "dgvReporte",
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToResizeRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            Tema.EstilizarGrilla(dgvReporte);

            // Dock=Top se apila al revés del orden de alta: de abajo hacia arriba.
            Controls.Add(dgvReporte);
            Controls.Add(lblResultado);
            Controls.Add(pnlIndicadores);
            Controls.Add(pnlFiltro);
            Controls.Add(pnlEncabezado);
            dgvReporte.BringToFront();
        }

        // ── Generación ───────────────────────────────────────────────────────

        private TipoReporte TipoActual => _tipos[cboTipoReporte.SelectedIndex];

        private async Task GenerarAsync()
        {
            int version = ++_versionCarga;
            var tipo = TipoActual;
            DateTime desde = _filtro.Desde, hasta = _filtro.Hasta;

            try
            {
                Cursor = Cursors.WaitCursor;
                var (filas, indicadores) = await Task.Run(() => tipo.Generar(desde, hasta));
                if (IsDisposed || version != _versionCarga) return;

                dgvReporte.DataSource = null;
                tipo.ConfigurarColumnas(dgvReporte);
                dgvReporte.DataSource = filas;

                pnlIndicadores.SuspendLayout();
                foreach (var c in pnlIndicadores.Controls.Cast<Control>().ToArray()) c.Dispose();
                foreach (var indicador in indicadores)
                {
                    pnlIndicadores.Controls.Add(new TarjetaMetrica(
                        tipo.Icono, indicador.Titulo, indicador.Valor, indicador.Detalle, Tema.TextoPrimario));
                }
                pnlIndicadores.ResumeLayout();

                lblResultado.Text = filas.Count == 0
                    ? $"{tipo.Nombre}: no hay datos {_filtro.Descripcion}."
                    : $"{tipo.Nombre} {_filtro.Descripcion} · {filas.Count} fila(s)";
            }
            catch (Exception ex)
            {
                if (IsDisposed || version != _versionCarga) return;
                MessageBox.Show(ex.Message, "Reportes", MessageBoxButtons.OK,
                    ex is NegocioException ? MessageBoxIcon.Warning : MessageBoxIcon.Error);
            }
            finally
            {
                if (!IsDisposed) Cursor = Cursors.Default;
            }
        }

        // ── Columnas de cada reporte ─────────────────────────────────────────

        private const string FormatoPesos = "$ #,##0.00";
        private const string FormatoFecha = "dd/MM/yyyy";
        private const string FormatoPorcentaje = "0.0' %'";

        private static void ColumnasIngresosPorPlan(DataGridView dgv) => Columnas(dgv,
            (nameof(FilaReporteConceptoDTO.Concepto), "Plan", 30, null, false),
            (nameof(FilaReporteConceptoDTO.Periodo), "Período", 22, null, false),
            (nameof(FilaReporteConceptoDTO.CantidadRegistros), "Cuotas", 14, "N0", true),
            (nameof(FilaReporteConceptoDTO.MontoTotal), "Monto total", 20, FormatoPesos, true),
            (nameof(FilaReporteConceptoDTO.Porcentaje), "% del total", 14, FormatoPorcentaje, true));

        private static void ColumnasMedioPago(DataGridView dgv) => Columnas(dgv,
            (nameof(FilaTotalDTO.Concepto), "Medio de pago", 34, null, false),
            (nameof(FilaTotalDTO.Cantidad), "Cobros", 18, "N0", true),
            (nameof(FilaTotalDTO.Monto), "Monto", 26, FormatoPesos, true),
            (nameof(FilaTotalDTO.Porcentaje), "% del total", 18, FormatoPorcentaje, true));

        private static void ColumnasCobros(DataGridView dgv) => Columnas(dgv,
            (nameof(FilaCobroDTO.FechaPago), "Fecha", 12, FormatoFecha, false),
            (nameof(FilaCobroDTO.Socio), "Socio", 24, null, false),
            (nameof(FilaCobroDTO.Documento), "Documento", 13, null, false),
            (nameof(FilaCobroDTO.Plan), "Plan", 16, null, false),
            (nameof(FilaCobroDTO.MedioPago), "Medio de pago", 14, null, false),
            (nameof(FilaCobroDTO.Monto), "Monto", 13, FormatoPesos, true),
            (nameof(FilaCobroDTO.CubreHasta), "Cubre hasta", 12, FormatoFecha, false));

        private static void ColumnasAsistencia(DataGridView dgv) => Columnas(dgv,
            (nameof(FilaAsistenciaDTO.Fecha), "Fecha", 22, "dddd dd/MM/yyyy", false),
            (nameof(FilaAsistenciaDTO.Total), "Accesos", 16, "N0", true),
            (nameof(FilaAsistenciaDTO.Concedidos), "Concedidos", 16, "N0", true),
            (nameof(FilaAsistenciaDTO.Rechazados), "Rechazados", 16, "N0", true),
            (nameof(FilaAsistenciaDTO.PorcentajeRechazo), "% rechazo", 14, FormatoPorcentaje, true));

        private static void ColumnasMantenimientos(DataGridView dgv) => Columnas(dgv,
            (nameof(FilaMantenimientoDTO.Maquina), "Máquina", 18, null, false),
            (nameof(FilaMantenimientoDTO.Marca), "Marca", 12, null, false),
            (nameof(FilaMantenimientoDTO.Tecnico), "Técnico", 16, null, false),
            (nameof(FilaMantenimientoDTO.Inicio), "Inicio", 11, FormatoFecha, false),
            (nameof(FilaMantenimientoDTO.Fin), "Fin", 11, FormatoFecha, false),
            (nameof(FilaMantenimientoDTO.Dias), "Días", 7, "N0", true),
            (nameof(FilaMantenimientoDTO.Estado), "Estado", 10, null, false),
            (nameof(FilaMantenimientoDTO.Detalle), "Detalle", 22, null, false));

        private static void Columnas(DataGridView dgv,
            params (string Propiedad, string Encabezado, int Peso, string? Formato, bool Derecha)[] columnas)
        {
            dgv.AutoGenerateColumns = false;
            dgv.Columns.Clear();
            foreach (var (propiedad, encabezado, peso, formato, derecha) in columnas)
            {
                var estilo = new DataGridViewCellStyle();
                if (formato != null) estilo.Format = formato;
                if (derecha) estilo.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgv.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "col" + propiedad,
                    DataPropertyName = propiedad,
                    HeaderText = encabezado,
                    FillWeight = peso,
                    DefaultCellStyle = estilo
                });
            }
        }

        // ── Exportación ──────────────────────────────────────────────────────

        private void btnExportar_Click(object? sender, EventArgs e)
        {
            if (dgvReporte.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos en la grilla para exportar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string nombre = TipoActual.Nombre.Replace(" ", "_");
            using SaveFileDialog sfd = new()
            {
                Filter = "Archivo CSV compatible con Excel (*.csv)|*.csv",
                FileName = $"Reporte_{nombre}_{_filtro.Desde:yyyyMMdd}_{_filtro.Hasta:yyyyMMdd}.csv",
                Title = "Guardar Reporte"
            };

            if (sfd.ShowDialog() != DialogResult.OK) return;

            try
            {
                var sb = new StringBuilder();
                sb.AppendLine($"\"{TipoActual.Nombre} {_filtro.Descripcion}\"");

                // Indicadores del período, antes del detalle.
                foreach (var tarjeta in pnlIndicadores.Controls.OfType<TarjetaMetrica>())
                    sb.AppendLine(string.Join(";", tarjeta.Textos().Select(Celda)));
                sb.AppendLine();

                sb.AppendLine(string.Join(";", dgvReporte.Columns.Cast<DataGridViewColumn>().Select(c => Celda(c.HeaderText))));
                foreach (DataGridViewRow fila in dgvReporte.Rows)
                {
                    if (fila.IsNewRow) continue;
                    sb.AppendLine(string.Join(";", fila.Cells.Cast<DataGridViewCell>()
                        .Select(c => Celda(c.FormattedValue?.ToString() ?? string.Empty))));
                }

                File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                MessageBox.Show($"Reporte exportado exitosamente en:\n{sfd.FileName}", "Exportación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error al exportar el archivo: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string Celda(string valor) => $"\"{valor.Replace("\"", "\"\"")}\"";
    }
}
