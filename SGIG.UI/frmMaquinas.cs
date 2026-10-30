using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.UI
{
    /// <summary>
    /// Consulta de máquinas (RF#18). Maquina es un catálogo sembrado con la
    /// aplicación (ver docs/SGIG_CreateDB.sql): esta pantalla es de solo lectura,
    /// sin alta/edición/baja desde la UI.
    /// </summary>
    //
    // ── CONTROLES (ver frmMaquinas.Designer.cs) ──────────────────────────────
    //   dgvMaquinas. btnNuevo/btnGuardar/btnEliminar y grpDatos siguen
    //   declarados en el Designer pero se ocultan en tiempo de ejecución.
    //   Barra de filtros (Fase 14, creada por código con BarraFiltros): txtBuscar,
    //   cboFiltroEstado, cboMarca, lblResultado; y la columna "Antigüedad" (colAntiguedad).
    // ───────────────────────────────────────────────────────────────────────────
    public partial class frmMaquinas : Form
    {
        private const string ColumnaAntiguedad = "colAntiguedad";

        private readonly ServicioMaquina _servicioMaquina = new();
        private List<Maquina> _maquinas = new();

        private readonly TextBox txtBuscar = new() { Name = "txtBuscar", Width = 200, PlaceholderText = "Nombre o marca" };
        private readonly ComboBox cboFiltroEstado = BarraFiltros.Combo("cboFiltroEstado", 130, "Todas", "Operativa", "En reparación");
        private readonly ComboBox cboMarca = BarraFiltros.Combo("cboMarca", 150, "Todas las marcas");
        private readonly Label lblResultado = BarraFiltros.Resultado("lblResultado");

        public frmMaquinas()
        {
            InitializeComponent();

            // Fase 14: filtros sobre la grilla. Sin el panel de edición (oculto), la grilla
            // ocupa todo el alto disponible: queda a 16 px del borde inferior.
            BarraFiltros.Crear(dgvMaquinas, 16,
                BarraFiltros.Rotulo("Buscar", 0), BarraFiltros.ComoCampo(txtBuscar),
                BarraFiltros.Rotulo("Estado"), cboFiltroEstado,
                BarraFiltros.Rotulo("Marca"), cboMarca,
                lblResultado);
            txtBuscar.TextChanged += (s, e) => AplicarFiltro();
            cboFiltroEstado.SelectedIndexChanged += (s, e) => AplicarFiltro();
            cboMarca.SelectedIndexChanged += (s, e) => AplicarFiltro();
            dgvMaquinas.CellFormatting += DgvMaquinas_CellFormatting;
        }

        private void frmMaquinas_Load(object sender, EventArgs e)
        {
            try
            {
                btnNuevo.Visible = false;
                btnGuardar.Visible = false;
                btnEliminar.Visible = false;
                grpDatos.Visible = false;

                ConfigurarGrilla();
                CargarGrilla();
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        private void ConfigurarGrilla()
        {
            Grillas.Configurar(dgvMaquinas,
                (nameof(Maquina.Nombre), "Nombre", 110),
                (nameof(Maquina.Marca), "Marca", 90),
                (nameof(Maquina.FechaCompra), "Fecha compra", 90),
                (nameof(Maquina.Estado), "Estado", 90));
            dgvMaquinas.Columns["col" + nameof(Maquina.FechaCompra)]!.DefaultCellStyle.Format = "dd/MM/yyyy";

            // Antigüedad: se calcula a partir de la fecha de compra (ver DgvMaquinas_CellFormatting).
            dgvMaquinas.Columns.Insert(3, new DataGridViewTextBoxColumn
            {
                Name = ColumnaAntiguedad,
                DataPropertyName = nameof(Maquina.FechaCompra),
                HeaderText = "Antigüedad",
                FillWeight = 80,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
        }

        private void CargarGrilla()
        {
            _maquinas = _servicioMaquina.ObtenerTodas().ToList();

            cboMarca.Items.Clear();
            cboMarca.Items.Add("Todas las marcas");
            cboMarca.Items.AddRange(_maquinas
                .Select(m => m.Marca)
                .Where(m => !string.IsNullOrWhiteSpace(m))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(m => m)
                .Cast<object>()
                .ToArray());
            cboMarca.SelectedIndex = 0;

            AplicarFiltro();
        }

        /// <summary>
        /// Aplica la barra de filtros sobre la lista ya cargada. El criterio vive en
        /// <see cref="ServicioMaquina.Filtrar"/>.
        /// </summary>
        private void AplicarFiltro()
        {
            var filtro = new FiltroMaquinas
            {
                Texto = txtBuscar.Text,
                Estado = cboFiltroEstado.SelectedIndex switch { 1 => "Operativa", 2 => "En Reparacion", _ => null },
                Marca = cboMarca.SelectedIndex > 0 ? cboMarca.SelectedItem?.ToString() : null
            };

            var filtradas = ServicioMaquina.Filtrar(_maquinas, filtro);
            dgvMaquinas.DataSource = filtradas;
            lblResultado.Text = $"Mostrando {filtradas.Count} de {_maquinas.Count} máquinas";
        }

        /// <summary>La columna Antigüedad muestra "8 años" en vez de la fecha de compra.</summary>
        private void DgvMaquinas_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || dgvMaquinas.Columns[e.ColumnIndex].Name != ColumnaAntiguedad) return;
            e.Value = ServicioMaquina.Antiguedad(e.Value as DateTime?, DateTime.Today);
            e.FormattingApplied = true;
        }

        // Nuevo/Guardar/Eliminar quedan inertes (RF#18 pasó a ser consulta de un
        // catálogo sembrado, ver docs/SGIG_CreateDB.sql): los métodos siguen acá
        // porque frmMaquinas.Designer.cs los engancha por evento, pero los
        // controles están ocultos (ver frmMaquinas_Load) y nunca se disparan.
        private void dgvMaquinas_SelectionChanged(object sender, EventArgs e)
        {
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
        }

        // ── Helpers de UI ────────────────────────────────────────────────────

        private static void MostrarError(Exception ex)
        {
            var esNegocio = ex is NegocioException;

            MessageBox.Show(ex.Message, "SGIG", MessageBoxButtons.OK,
                esNegocio ? MessageBoxIcon.Warning : MessageBoxIcon.Error);
        }
    }
}
