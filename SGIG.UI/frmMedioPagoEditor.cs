using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.UI
{
    /// <summary>Alta/edición de un medio de pago del catálogo (RF#04), en diálogo modal.</summary>
    //
    // ── CONTROLES (ver frmMedioPagoEditor.Designer.cs) ───────────────────────
    //   txtDescripcion, btnGuardar, btnCancelar
    // ─────────────────────────────────────────────────────────────────────────
    public partial class frmMedioPagoEditor : Form
    {
        private readonly ServicioCatalogo _servicio = new();
        private readonly MedioPago? _existente;

        public frmMedioPagoEditor(MedioPago? existente)
        {
            InitializeComponent();
            _existente = existente;
            Text = existente is null ? "Nuevo medio de pago" : "Editar medio de pago";
        }

        private void frmMedioPagoEditor_Load(object sender, EventArgs e)
        {
            if (_existente is not null)
            {
                txtDescripcion.Text = _existente.Descripcion;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            var medio = new MedioPago
            {
                IdMedioPago = _existente?.IdMedioPago ?? 0,
                Descripcion = txtDescripcion.Text.Trim()
            };

            try
            {
                if (_existente is null)
                {
                    _servicio.AltaMedioPago(medio);
                }
                else
                {
                    _servicio.ModificarMedioPago(medio);
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
