using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.UI
{
    /// <summary>Alta/edición de un rol del catálogo (RF#04), en diálogo modal.</summary>
    //
    // ── CONTROLES (ver frmRolEditor.Designer.cs) ─────────────────────────────
    //   txtRol, txtDescripcion, btnGuardar, btnCancelar
    // ─────────────────────────────────────────────────────────────────────────
    public partial class frmRolEditor : Form
    {
        private readonly ServicioCatalogo _servicio = new();
        private readonly Rol? _existente;

        public frmRolEditor(Rol? existente)
        {
            InitializeComponent();
            _existente = existente;
            Text = existente is null ? "Nuevo rol" : "Editar rol";
        }

        private void frmRolEditor_Load(object sender, EventArgs e)
        {
            if (_existente is not null)
            {
                txtRol.Text = _existente.NombreRol;
                txtDescripcion.Text = _existente.Descripcion ?? string.Empty;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            var rol = new Rol
            {
                IdRol = _existente?.IdRol ?? 0,
                NombreRol = txtRol.Text.Trim(),
                Descripcion = string.IsNullOrWhiteSpace(txtDescripcion.Text) ? null : txtDescripcion.Text.Trim()
            };

            try
            {
                if (_existente is null)
                {
                    _servicio.AltaRol(rol);
                }
                else
                {
                    _servicio.ModificarRol(rol);
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SGIG", MessageBoxButtons.OK,
                    ex is NegocioException ? MessageBoxIcon.Warning : MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
