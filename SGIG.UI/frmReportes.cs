using System;
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
    public partial class frmReportes : Form
    {
        private readonly ServicioReporte _servicioReporte;

        // Referencias a los controles dinámicos para actualización
        private TarjetaMetrica _tarjetaIngresos = null!;
        private TarjetaMetrica _tarjetaSocios = null!;
        private TarjetaMetrica _tarjetaCheckins = null!;
        private DateTimePicker _dtpDesde = null!;
        private DateTimePicker _dtpHasta = null!;
        private Button _btnConsultar = null!;
        private Button _btnExportar = null!;
        private DataGridView _dgv = null!;

        public frmReportes()
        {
            _servicioReporte = new ServicioReporte();

            Text = "SGIG — Métricas y Reportes de Ingresos";
            Size = new Size(740, 520);
            StartPosition = FormStartPosition.CenterParent;

            Tema.EstilizarFormulario(this);
            InicializarComponentes();
            ConfigurarColumnasGrilla();

            // Cargar datos reales al abrir
            this.Shown += async (s, e) => await CargarDatosAsync();
        }

        /// <summary>
        /// Layout por Dock (sin posiciones fijas): encabezado, fila de tarjetas, barra de
        /// filtro con las acciones y, ocupando el resto, la grilla. Así se adapta a
        /// cualquier tamaño del área de contenido.
        /// </summary>
        private void InicializarComponentes()
        {
            Padding = new Padding(0);

            // Encabezado
            var pnlEncabezado = new Panel { Dock = DockStyle.Top, Height = 64, BackColor = Tema.Fondo };
            pnlEncabezado.Controls.Add(new Label
            {
                Text = "Reportes financieros y concurrencia",
                Font = Tema.Fuente(16F, FontStyle.Bold),
                ForeColor = Tema.TextoPrimario,
                Location = new Point(0, 0),
                AutoSize = true
            });
            pnlEncabezado.Controls.Add(new Label
            {
                Text = "Métricas del mes y detalle de ingresos por período.",
                Font = Tema.FuenteSubtitulo,
                ForeColor = Tema.TextoSecundario,
                Location = new Point(2, 34),
                AutoSize = true
            });

            // Tarjetas de métricas (mismo componente que el inicio)
            var flpTarjetas = new PanelTarjetas();
            _tarjetaIngresos = new TarjetaMetrica(Iconos.Cobro, "Ingresos del mes", "$ 0", "Calculando...", Tema.TextoPrimario);
            _tarjetaSocios = new TarjetaMetrica(Iconos.Socios, "Socios activos", "0", "Calculando...", Tema.TextoPrimario);
            _tarjetaCheckins = new TarjetaMetrica(Iconos.Acceso, "Check-ins promedio", "0 / día", "Últimos 30 días", Tema.TextoPrimario);
            flpTarjetas.Controls.AddRange(new Control[] { _tarjetaIngresos, _tarjetaSocios, _tarjetaCheckins });

            // Barra de filtro: rango de fechas a la izquierda, exportar a la derecha
            var pnlFiltro = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = Tema.Fondo };

            var lblDesde = new Label { Text = "Desde", Font = Tema.Fuente(9F, FontStyle.Bold), ForeColor = Tema.TextoSecundario, Location = new Point(2, 18), AutoSize = true };
            _dtpDesde = new DateTimePicker { Format = DateTimePickerFormat.Short, Location = new Point(52, 14), Width = 130, Value = DateTime.Today.AddDays(-30) };
            var lblHasta = new Label { Text = "Hasta", Font = Tema.Fuente(9F, FontStyle.Bold), ForeColor = Tema.TextoSecundario, Location = new Point(204, 18), AutoSize = true };
            _dtpHasta = new DateTimePicker { Format = DateTimePickerFormat.Short, Location = new Point(252, 14), Width = 130, Value = DateTime.Today };

            _btnConsultar = new Button { Name = "btnGenerar", Text = "Generar vista", Location = new Point(402, 10), Size = new Size(130, 34) };
            _btnConsultar.Click += async (s, e) => await ActualizarGrillaAsync();

            _btnExportar = new Button
            {
                Name = "btnExportarCerrar", // variante secundaria por convención de nombre
                Text = "Exportar a CSV",
                Size = new Size(150, 34),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            _btnExportar.Click += btnExportar_Click;

            pnlFiltro.Controls.AddRange(new Control[] { lblDesde, _dtpDesde, lblHasta, _dtpHasta, _btnConsultar, _btnExportar });
            pnlFiltro.Resize += (s, e) => _btnExportar.Location = new Point(pnlFiltro.ClientSize.Width - _btnExportar.Width, 10);

            // Grilla de movimientos (ocupa el resto)
            _dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToResizeRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            Tema.EstilizarGrilla(_dgv);

            // Dock=Top se apila al revés del orden de alta: de abajo hacia arriba.
            Controls.Add(_dgv);
            Controls.Add(pnlFiltro);
            Controls.Add(flpTarjetas);
            Controls.Add(pnlEncabezado);
            _dgv.BringToFront();
        }

        private void ConfigurarColumnasGrilla()
        {
            _dgv.AutoGenerateColumns = false;
            _dgv.Columns.Clear();

            _dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(FilaReporteConceptoDTO.Concepto),
                HeaderText = "Concepto",
                FillWeight = 40
            });

            _dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(FilaReporteConceptoDTO.Periodo),
                HeaderText = "Período",
                FillWeight = 25
            });

            _dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(FilaReporteConceptoDTO.CantidadRegistros),
                HeaderText = "Cantidad",
                FillWeight = 20,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            _dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = nameof(FilaReporteConceptoDTO.MontoTotal),
                HeaderText = "Monto Total",
                FillWeight = 25,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "C2" }
            });
        }

        private async Task CargarDatosAsync()
        {
            await CargarMetricasKpiAsync();
            await ActualizarGrillaAsync();
        }

        private async Task CargarMetricasKpiAsync()
        {
            try
            {
                var metricas = await Task.Run(() => _servicioReporte.ObtenerMetricas());

                _tarjetaIngresos.Actualizar($"$ {metricas.IngresosMes:N0}", "Total facturado este mes");
                _tarjetaSocios.Actualizar(metricas.SociosActivos.ToString(), $"{metricas.SociosAlDia} con cuota al día");
                _tarjetaCheckins.Actualizar($"{metricas.CheckinsPromedioDia:F1} / día", "Promedio últimos 30 días");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar métricas del panel: {ex.Message}", "Reportes", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task ActualizarGrillaAsync()
        {
            if (_dtpDesde.Value.Date > _dtpHasta.Value.Date)
            {
                MessageBox.Show("La fecha \"Desde\" no puede ser posterior a la fecha \"Hasta\".",
                    "Reportes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Cursor = Cursors.WaitCursor;
                _btnConsultar.Enabled = false;

                DateTime desde = _dtpDesde.Value.Date;
                DateTime hasta = _dtpHasta.Value.Date;

                var datos = await Task.Run(() => _servicioReporte.GenerarReporte(desde, hasta).ToList());
                _dgv.DataSource = datos;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar el reporte: {ex.Message}", "Reportes", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _btnConsultar.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private void btnExportar_Click(object? sender, EventArgs e)
        {
            if (_dgv.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos en la grilla para exportar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using SaveFileDialog sfd = new()
            {
                Filter = "Archivo CSV compatible con Excel (*.csv)|*.csv",
                FileName = $"Reporte_Ingresos_{DateTime.Now:yyyyMMdd_HHmm}.csv",
                Title = "Guardar Reporte"
            };

            if (sfd.ShowDialog() != DialogResult.OK) return;

            try
            {
                var sb = new StringBuilder();

                // Encabezados
                var columnas = _dgv.Columns.Cast<DataGridViewColumn>().Select(c => $"\"{c.HeaderText}\"");
                sb.AppendLine(string.Join(";", columnas));

                // Filas
                foreach (DataGridViewRow fila in _dgv.Rows)
                {
                    if (fila.IsNewRow) continue;
                    var celdas = fila.Cells.Cast<DataGridViewCell>().Select(c =>
                    {
                        string val = c.FormattedValue?.ToString() ?? string.Empty;
                        return $"\"{val.Replace("\"", "\"\"")}\"";
                    });
                    sb.AppendLine(string.Join(";", celdas));
                }

                File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                MessageBox.Show($"Reporte exportado exitosamente en:\n{sfd.FileName}", "Exportación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error al exportar el archivo: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}