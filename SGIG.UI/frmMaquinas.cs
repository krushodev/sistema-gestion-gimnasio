using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.UI
{
    /// <summary>
    /// ABM de máquinas (RF#18). Accesible para Administrador y Técnico. Sin baja
    /// lógica: dbo.Maquina no tiene columna 'activo', así que Eliminar es DELETE
    /// físico, bloqueado si la máquina ya tiene mantenimientos registrados.
    /// </summary>
    //
    // ── CONTROLES (ver frmMaquinas.Designer.cs) ──────────────────────────────
    //   dgvMaquinas, btnNuevo, btnGuardar, btnEliminar
    //   Datos: txtMarca, txtNombre, dtpFechaCompra, cboEstado
    // ───────────────────────────────────────────────────────────────────────────
    public partial class frmMaquinas : Form
    {
        private readonly ServicioMaquina _servicioMaquina = new();

        /// <summary>id_maquina en edición; null cuando se está dando un alta.</summary>
        private int? _idEnEdicion;

        public frmMaquinas()
        {
            InitializeComponent();
        }

        private void frmMaquinas_Load(object sender, EventArgs e)
        {
            try
            {
                ConfigurarGrilla();
                CargarCombos();
                CargarGrilla();
                LimpiarPanel();
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

        private void CargarCombos()
        {
            cboEstado.Items.Clear();
            cboEstado.Items.Add(ServicioMaquina.EstadoOperativa);
            cboEstado.Items.Add(ServicioMaquina.EstadoEnReparacion);
        }

        private void CargarGrilla()
        {
            dgvMaquinas.DataSource = _servicioMaquina.ObtenerTodas().ToList();
        }

        // ── ABM ──────────────────────────────────────────────────────────────

        /// <summary>Seleccionar una fila carga sus datos en el panel para editarla.</summary>
        private void dgvMaquinas_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvMaquinas.CurrentRow?.DataBoundItem is not Maquina maquina) return;

            _idEnEdicion = maquina.IdMaquina;
            txtMarca.Text = maquina.Marca ?? string.Empty;
            txtNombre.Text = maquina.Nombre;
            dtpFechaCompra.Value = maquina.FechaCompra ?? DateTime.Today;
            cboEstado.SelectedItem = maquina.Estado;
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            dgvMaquinas.ClearSelection();
            LimpiarPanel();
            txtNombre.Focus();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            var maquina = new Maquina
            {
                IdMaquina = _idEnEdicion ?? 0,
                Marca = string.IsNullOrWhiteSpace(txtMarca.Text) ? null : txtMarca.Text.Trim(),
                Nombre = txtNombre.Text.Trim(),
                FechaCompra = dtpFechaCompra.Value.Date,
                Estado = cboEstado.SelectedItem?.ToString() ?? ServicioMaquina.EstadoOperativa
            };

            try
            {
                Cursor = Cursors.WaitCursor;

                if (_idEnEdicion is null)
                {
                    _servicioMaquina.Alta(maquina);
                }
                else
                {
                    _servicioMaquina.Modificar(maquina);
                }

                CargarGrilla();
                LimpiarPanel();
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

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvMaquinas.CurrentRow?.DataBoundItem is not Maquina maquina)
            {
                MessageBox.Show("Seleccioná una máquina de la grilla.", "SGIG",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var respuesta = MessageBox.Show(
                $"¿Confirmás eliminar la máquina \"{maquina.Nombre}\"?",
                "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (respuesta != DialogResult.Yes) return;

            try
            {
                _servicioMaquina.Eliminar(maquina.IdMaquina);
                CargarGrilla();
                LimpiarPanel();
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        // ── Helpers de UI ────────────────────────────────────────────────────

        private void LimpiarPanel()
        {
            _idEnEdicion = null;
            txtMarca.Clear();
            txtNombre.Clear();
            dtpFechaCompra.Value = DateTime.Today;
            cboEstado.SelectedItem = ServicioMaquina.EstadoOperativa;
        }

        private static void MostrarError(Exception ex)
        {
            var esNegocio = ex is NegocioException;

            MessageBox.Show(ex.Message, "SGIG", MessageBoxButtons.OK,
                esNegocio ? MessageBoxIcon.Warning : MessageBoxIcon.Error);
        }
    }
}
