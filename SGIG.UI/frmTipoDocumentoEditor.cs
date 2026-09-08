using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.UI
{
    /// <summary>Alta/edición de un tipo de documento del catálogo (RF#04), en diálogo modal.</summary>
    //
    // ── CONTROLES (ver frmTipoDocumentoEditor.Designer.cs) ───────────────────
    //   txtDescripcion, btnGuardar, btnCancelar
    // ─────────────────────────────────────────────────────────────────────────
    public partial class frmTipoDocumentoEditor : Form
    {
        private readonly ServicioCatalogo _servicio = new();
        private readonly TipoDocumento? _existente;

        public frmTipoDocumentoEditor(TipoDocumento? existente)
        {
            InitializeComponent();
            _existente = existente;
            Text = existente is null ? "Nuevo tipo de documento" : "Editar tipo de documento";
        }

        private void frmTipoDocumentoEditor_Load(object sender, EventArgs e)
        {
            if (_existente is not null)
            {
                txtDescripcion.Text = _existente.Descripcion;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            var tipo = new TipoDocumento
            {
                IdTipoDocumento = _existente?.IdTipoDocumento ?? 0,
                Descripcion = txtDescripcion.Text.Trim()
            };

            try
            {
                if (_existente is null)
                {
                    _servicio.AltaTipoDocumento(tipo);
                }
                else
                {
                    _servicio.ModificarTipoDocumento(tipo);
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
