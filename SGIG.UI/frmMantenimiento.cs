using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.UI
{
    /// <summary>
    /// Registro y finalización de mantenimientos (RF#19, RF#20). Accesible sólo
    /// para el rol Técnico. El técnico a cargo es siempre el usuario logueado.
    /// </summary>
    //
    // ── CONTROLES (ver frmMantenimiento.Designer.cs) ─────────────────────────
    //   Alta: cboMaquina, dtpFechaInicio, txtDetalleTecnico, btnRegistrar
    //   dgvMantenimientosActivos
    //   Finalizar: dtpFechaFin, btnFinalizar
    // ───────────────────────────────────────────────────────────────────────────
    public partial class frmMantenimiento : Form
    {
        private readonly ServicioMantenimiento _servicioMantenimiento = new();
        private readonly ServicioMaquina _servicioMaquina = new();
        private readonly Usuario _usuarioLogueado;

        public frmMantenimiento(Usuario usuarioLogueado)
        {
            InitializeComponent();
            _usuarioLogueado = usuarioLogueado ?? throw new ArgumentNullException(nameof(usuarioLogueado));
        }

        private void frmMantenimiento_Load(object sender, EventArgs e)
        {
            try
            {
                ConfigurarGrilla();
                CargarCombos();
                CargarGrilla();
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        private void ConfigurarGrilla()
        {
            Grillas.Configurar(dgvMantenimientosActivos,
                (nameof(Mantenimiento.NombreMaquina), "Máquina", 110),
                (nameof(Mantenimiento.NombreTecnico), "Técnico", 110),
                (nameof(Mantenimiento.FechaInicio), "Fecha inicio", 90),
                (nameof(Mantenimiento.DetalleTecnico), "Detalle", 160));
        }

        /// <summary>
        /// Sólo ofrece máquinas operativas: una que ya está "En Reparacion" tiene un
        /// mantenimiento en curso, no tiene sentido abrirle otro encima.
        /// </summary>
        private void CargarCombos()
        {
            cboMaquina.DisplayMember = nameof(Maquina.Nombre);
            cboMaquina.ValueMember = nameof(Maquina.IdMaquina);
            cboMaquina.DataSource = _servicioMaquina.ObtenerOperativas().ToList();

            dtpFechaInicio.Value = DateTime.Today;
            dtpFechaFin.Value = DateTime.Today;
        }

        private void CargarGrilla()
        {
            dgvMantenimientosActivos.DataSource = _servicioMantenimiento.ObtenerActivos().ToList();
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            var idMaquina = (int)(cboMaquina.SelectedValue ?? 0);

            try
            {
                Cursor = Cursors.WaitCursor;

                _servicioMantenimiento.Registrar(
                    idMaquina,
                    _usuarioLogueado.IdPersona,
                    dtpFechaInicio.Value,
                    txtDetalleTecnico.Text);

                txtDetalleTecnico.Clear();
                CargarCombos();
                CargarGrilla();
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void btnFinalizar_Click(object sender, EventArgs e)
        {
            if (dgvMantenimientosActivos.CurrentRow?.DataBoundItem is not Mantenimiento mantenimiento)
            {
                MessageBox.Show("Seleccioná un mantenimiento activo de la grilla.", "SGIG",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                Cursor = Cursors.WaitCursor;

                _servicioMantenimiento.Finalizar(mantenimiento, dtpFechaFin.Value);

                CargarCombos();
                CargarGrilla();
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
            finally
            {
                Cursor = Cursors.Default;
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
