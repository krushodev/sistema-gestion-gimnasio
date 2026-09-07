using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.UI
{
    /// <summary>
    /// Historial de mantenimientos por máquina (RF#21). Accesible para
    /// Administrador y Técnico.
    /// </summary>
    //
    // ── CONTROLES (ver frmHistorialMantenimientos.Designer.cs) ──────────────
    //   cboMaquina, dgvHistorialMantenimientos
    // ───────────────────────────────────────────────────────────────────────────
    public partial class frmHistorialMantenimientos : Form
    {
        private readonly ServicioMantenimiento _servicioMantenimiento = new();
        private readonly ServicioMaquina _servicioMaquina = new();

        public frmHistorialMantenimientos()
        {
            InitializeComponent();
        }

        private void frmHistorialMantenimientos_Load(object sender, EventArgs e)
        {
            try
            {
                Grillas.Configurar(dgvHistorialMantenimientos,
                    (nameof(Mantenimiento.FechaInicio), "Fecha inicio", 90),
                    (nameof(Mantenimiento.FechaFin), "Fecha fin", 90),
                    (nameof(Mantenimiento.NombreTecnico), "Técnico", 120),
                    (nameof(Mantenimiento.DetalleTecnico), "Detalle", 200));

                cboMaquina.DisplayMember = nameof(Maquina.Nombre);
                cboMaquina.ValueMember = nameof(Maquina.IdMaquina);
                cboMaquina.DataSource = _servicioMaquina.ObtenerTodas().ToList();
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        private void cboMaquina_SelectedIndexChanged(object sender, EventArgs e)
        {
            var idMaquina = (int)(cboMaquina.SelectedValue ?? 0);
            if (idMaquina <= 0) return;

            try
            {
                dgvHistorialMantenimientos.DataSource = _servicioMantenimiento.ObtenerPorMaquina(idMaquina).ToList();
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        private static void MostrarError(Exception ex)
        {
            var esNegocio = ex is NegocioException;

            MessageBox.Show(ex.Message, "SGIG", MessageBoxButtons.OK,
                esNegocio ? MessageBoxIcon.Warning : MessageBoxIcon.Error);
        }
    }
}
