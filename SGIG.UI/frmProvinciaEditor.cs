using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.UI
{
    /// <summary>Alta/edición de una provincia del catálogo (RF#04), en diálogo modal.</summary>
    //
    // ── CONTROLES (ver frmProvinciaEditor.Designer.cs) ───────────────────────
    //   txtNombre, btnGuardar, btnCancelar
    // ─────────────────────────────────────────────────────────────────────────
    public partial class frmProvinciaEditor : Form
    {
        private readonly ServicioCatalogo _servicio = new();
        private readonly Provincia? _existente;

        public frmProvinciaEditor(Provincia? existente)
        {
            InitializeComponent();
            _existente = existente;
            Text = existente is null ? "Nueva provincia" : "Editar provincia";
        }

        private void frmProvinciaEditor_Load(object sender, EventArgs e)
        {
            if (_existente is not null)
            {
                txtNombre.Text = _existente.Nombre;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            var provincia = new Provincia
            {
                IdProvincia = _existente?.IdProvincia ?? 0,
                Nombre = txtNombre.Text.Trim()
            };

            try
            {
                if (_existente is null)
                {
                    _servicio.AltaProvincia(provincia);
                }
                else
                {
                    _servicio.ModificarProvincia(provincia);
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
