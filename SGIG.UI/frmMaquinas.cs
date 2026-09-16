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
    // ───────────────────────────────────────────────────────────────────────────
    public partial class frmMaquinas : Form
    {
        private readonly ServicioMaquina _servicioMaquina = new();

        public frmMaquinas()
        {
            InitializeComponent();
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
        }

        private void CargarGrilla()
        {
            dgvMaquinas.DataSource = _servicioMaquina.ObtenerTodas().ToList();
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
