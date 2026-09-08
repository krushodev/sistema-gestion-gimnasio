using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.UI
{
    /// <summary>Alta/edición de una localidad del catálogo (RF#04), en diálogo modal.</summary>
    //
    // ── CONTROLES (ver frmLocalidadEditor.Designer.cs) ───────────────────────
    //   txtNombre, cboProvincia, btnGuardar, btnCancelar
    // ─────────────────────────────────────────────────────────────────────────
    public partial class frmLocalidadEditor : Form
    {
        private readonly ServicioCatalogo _servicio = new();
        private readonly Localidad? _existente;

        public frmLocalidadEditor(Localidad? existente)
        {
            InitializeComponent();
            _existente = existente;
            Text = existente is null ? "Nueva localidad" : "Editar localidad";
        }

        private void frmLocalidadEditor_Load(object sender, EventArgs e)
        {
            try
            {
                cboProvincia.DisplayMember = nameof(Provincia.Nombre);
                cboProvincia.ValueMember = nameof(Provincia.IdProvincia);
                cboProvincia.DataSource = _servicio.ObtenerProvincias().ToList();

                if (_existente is not null)
                {
                    txtNombre.Text = _existente.Nombre;
                    cboProvincia.SelectedValue = _existente.IdProvincia;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SGIG", MessageBoxButtons.OK,
                    ex is NegocioException ? MessageBoxIcon.Warning : MessageBoxIcon.Error);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            var localidad = new Localidad
            {
                IdLocalidad = _existente?.IdLocalidad ?? 0,
                Nombre = txtNombre.Text.Trim(),
                IdProvincia = (int)(cboProvincia.SelectedValue ?? 0)
            };

            try
            {
                if (_existente is null)
                {
                    _servicio.AltaLocalidad(localidad);
                }
                else
                {
                    _servicio.ModificarLocalidad(localidad);
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
