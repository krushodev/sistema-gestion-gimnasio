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
        private Label _lblValorIngresos = null!;
        private Label _lblDetalleIngresos = null!;
        private Label _lblValorSocios = null!;
        private Label _lblDetalleSocios = null!;
        private Label _lblValorCheckins = null!;
        private Label _lblDetalleCheckins = null!;
        private DateTimePicker _dtpDesde = null!;
        private DateTimePicker _dtpHasta = null!;
        private Button _btnConsultar = null!;
        private Button _btnExportar = null!;
        private DataGridView _dgv = null!;

        public frmReportes()
        {
            _servicioReporte = new ServicioReporte();

            Text = "SGIG — Métricas y Reportes de Ingresos";
            Size = new Size(820, 530);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            Tema.EstilizarFormulario(this);
            InicializarComponentes();
            ConfigurarColumnasGrilla();

            // Cargar datos reales al abrir
            this.Shown += async (s, e) => await CargarDatosAsync();
        }

        private void InicializarComponentes()
        {
            // Encabezado
            Label lblTitulo = new()
            {
                Text = "Reportes Financieros y Concurrencia",
                Font = Tema.FuenteTitulo,
                ForeColor = Tema.SlateOscuro,
                Location = new Point(24, 20),
                AutoSize = true
            };

            Label lblSub = new()
            {
                Text = "Métricas mensuales estimadas, altas/bajas de membresías y balance general.",
                Font = Tema.FuenteSubtitulo,
                ForeColor = Tema.SlateTexto,
                Location = new Point(25, 52),
                AutoSize = true
            };

            // Contenedores de KPIs (Tarjetas estadísticas)
            Panel kpi1 = CrearTarjetaKpi("Ingresos del Mes", "$ 0", "Calculando...", Tema.Exito, 24, 90, out _lblValorIngresos, out _lblDetalleIngresos);
            Panel kpi2 = CrearTarjetaKpi("Socios Activos", "0", "Calculando...", Tema.Primario, 284, 90, out _lblValorSocios, out _lblDetalleSocios);
            Panel kpi3 = CrearTarjetaKpi("Check-ins Promedio", "0 / día", "Últimos 30 días", Tema.SlateOscuro, 544, 90, out _lblValorCheckins, out _lblDetalleCheckins);

            // Filtro de fechas
            GroupBox grpPeriodo = new()
            {
                Text = " Rango de consulta ",
                Font = Tema.FuenteLabel,
                ForeColor = Tema.SlateTexto,
                Location = new Point(24, 210),
                Size = new Size(756, 70)
            };

            Label lblDesde = new() { Text = "Desde:", Location = new Point(20, 30), AutoSize = true };
            _dtpDesde = new() { Location = new Point(70, 26), Width = 130, Value = DateTime.Today.AddDays(-30) };

            Label lblHasta = new() { Text = "Hasta:", Location = new Point(230, 30), AutoSize = true };
            _dtpHasta = new() { Location = new Point(280, 26), Width = 130, Value = DateTime.Today };

            _btnConsultar = new()
            {
                Text = "Generar Vista",
                Location = new Point(440, 24),
                Size = new Size(130, 30),
                BackColor = Tema.Primario,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _btnConsultar.FlatAppearance.BorderSize = 0;
            _btnConsultar.Click += async (s, e) => await ActualizarGrillaAsync();

            grpPeriodo.Controls.AddRange(new Control[] { lblDesde, _dtpDesde, lblHasta, _dtpHasta, _btnConsultar });

            // Grilla de movimientos
            _dgv = new()
            {
                Location = new Point(24, 295),
                Size = new Size(756, 130),
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            // Botón de exportación
            _btnExportar = new()
            {
                Text = "Exportar Reporte (CSV / Excel)",
                Location = new Point(540, 438),
                Size = new Size(240, 34),
                BackColor = Tema.SlateOscuro,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _btnExportar.FlatAppearance.BorderSize = 0;
            _btnExportar.Click += btnExportar_Click;

            Controls.AddRange(new Control[] { lblTitulo, lblSub, kpi1, kpi2, kpi3, grpPeriodo, _dgv, _btnExportar });
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
                HeaderText = "Cantidad / Registros",
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

        private Panel CrearTarjetaKpi(
            string titulo,
            string valorInicial,
            string detalleInicial,
            Color colorAcento,
            int x,
            int y,
            out Label lblValor,
            out Label lblDetalle)
        {
            Panel p = new()
            {
                Location = new Point(x, y),
                Size = new Size(236, 100),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Panel barrita = new() { Dock = DockStyle.Top, Height = 4, BackColor = colorAcento };
            Label lblT = new() { Text = titulo, Font = Tema.FuenteLabel, ForeColor = Tema.SlateTexto, Location = new Point(14, 14), AutoSize = true };
            lblValor = new() { Text = valorInicial, Font = new Font("Segoe UI", 16f, FontStyle.Bold), ForeColor = Tema.SlateOscuro, Location = new Point(12, 34), AutoSize = true };
            lblDetalle = new() { Text = detalleInicial, Font = new Font("Segoe UI", 7.5f), ForeColor = Color.Gray, Location = new Point(14, 70), AutoSize = true, MaximumSize = new Size(208, 0) };

            p.Controls.AddRange(new Control[] { barrita, lblT, lblValor, lblDetalle });
            return p;
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

                _lblValorIngresos.Text = $"$ {metricas.IngresosMes:N0}";
                _lblDetalleIngresos.Text = "Total facturado este mes";

                _lblValorSocios.Text = metricas.SociosActivos.ToString();
                _lblDetalleSocios.Text = $"{metricas.SociosAlDia} con cuota al día";

                _lblValorCheckins.Text = $"{metricas.CheckinsPromedioDia:F1} / día";
                _lblDetalleCheckins.Text = "Promedio últimos 30 días";
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