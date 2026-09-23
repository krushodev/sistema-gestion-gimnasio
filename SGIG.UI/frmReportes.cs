using System;
using System.Drawing;
using System.Windows.Forms;

namespace SGIG.UI
{
    public partial class frmReportes : Form
    {
        public frmReportes()
        {
            Text = "SGIG — Métricas y Reportes de Ingresos";
            Size = new Size(820, 520);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            Tema.EstilizarFormulario(this);
            InicializarComponentes();
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
            Panel kpi1 = CrearTarjetaKpi("Ingresos del Mes", "$ 1.845.000", "+12.4% vs mes anterior", Tema.Exito, 24, 90);
            Panel kpi2 = CrearTarjetaKpi("Socios Activos", "318", "14 nuevas altas este mes", Tema.Primario, 284, 90);
            Panel kpi3 = CrearTarjetaKpi("Check-ins Promedio", "142 / día", "Pico habitual: 19:00 a 21:00 hs", Tema.SlateOscuro, 544, 90);

            // Filtro simulado
            GroupBox grpPeriodo = new()
            {
                Text = " Rango de consulta ",
                Font = Tema.FuenteLabel,
                ForeColor = Tema.SlateTexto,
                Location = new Point(24, 210),
                Size = new Size(756, 70)
            };

            Label lblDesde = new() { Text = "Desde:", Location = new Point(20, 30), AutoSize = true };
            DateTimePicker dtpDesde = new() { Location = new Point(70, 26), Width = 130, Value = DateTime.Today.AddDays(-30) };
            Label lblHasta = new() { Text = "Hasta:", Location = new Point(230, 30), AutoSize = true };
            DateTimePicker dtpHasta = new() { Location = new Point(280, 26), Width = 130, Value = DateTime.Today };

            Button btnConsultar = new()
            {
                Text = "Generar Vista",
                Location = new Point(440, 24),
                Size = new Size(130, 30)
            };

            btnConsultar.Click += (s, e) =>
            {
                Cursor = Cursors.WaitCursor;
                System.Threading.Thread.Sleep(350); // Simulación de carga
                Cursor = Cursors.Default;
                MessageBox.Show("Métricas actualizadas para el rango seleccionado.", "Reportes", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            grpPeriodo.Controls.AddRange(new Control[] { lblDesde, dtpDesde, lblHasta, dtpHasta, btnConsultar });

            // Grilla mockeada de movimientos
            DataGridView dgv = new()
            {
                Location = new Point(24, 295),
                Size = new Size(756, 130),
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            dgv.Columns.Add("Concepto", "Concepto");
            dgv.Columns.Add("Periodo", "Período");
            dgv.Columns.Add("Cantidad", "Cantidad / Registros");
            dgv.Columns.Add("Total", "Monto Total");

            dgv.Rows.Add("Cuotas Plan Full Libre", "Septiembre 2026", "210", "$ 1.260.000");
            dgv.Rows.Add("Cuotas Plan 3 Días", "Septiembre 2026", "84", "$ 420.000");
            dgv.Rows.Add("Pases Diarios / Visitas", "Septiembre 2026", "55", "$ 165.000");

            // Botón de exportación simulada
            Button btnExportar = new()
            {
                Text = "Exportar Reporte (PDF / Excel)",
                Location = new Point(540, 438),
                Size = new Size(240, 34)
            };

            btnExportar.Click += (s, e) =>
            {
                SaveFileDialog sfd = new()
                {
                    Filter = "Archivo PDF (*.pdf)|*.pdf|Documento Excel (*.xlsx)|*.xlsx",
                    FileName = $"Reporte_Ingresos_{DateTime.Now:yyyyMM}"
                };

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    Cursor = Cursors.WaitCursor;
                    System.Threading.Thread.Sleep(400);
                    Cursor = Cursors.Default;
                    MessageBox.Show($"Reporte simulado generado con éxito en:\n{sfd.FileName}", "Exportación Completa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };

            Controls.AddRange(new Control[] { lblTitulo, lblSub, kpi1, kpi2, kpi3, grpPeriodo, dgv, btnExportar });
        }

        private Panel CrearTarjetaKpi(string titulo, string valor, string detalle, Color colorAcento, int x, int y)
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
            Label lblV = new() { Text = valor, Font = new Font("Segoe UI", 16f, FontStyle.Bold), ForeColor = Tema.SlateOscuro, Location = new Point(12, 34), AutoSize = true };
            Label lblD = new() { Text = detalle, Font = new Font("Segoe UI", 7.5f), ForeColor = Color.Gray, Location = new Point(14, 70), AutoSize = true };

            p.Controls.AddRange(new Control[] { barrita, lblT, lblV, lblD });
            return p;
        }
    }
}