using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.UI
{
    /// <summary>
    /// Alta/edición de un usuario del sistema (RF#03, RNF#11), en un diálogo modal
    /// separado de la grilla de <see cref="frmUsuarios"/>. Reutiliza
    /// <see cref="ucDatosPersona"/> para los campos de Persona: si el documento ya
    /// pertenece a una Persona existente (por ejemplo, ya es Socio) se autocompletan
    /// y bloquean sus datos en vez de rechazar el alta.
    /// </summary>
    //
    // ── CONTROLES (ver frmUsuarioEditor.Designer.cs) ─────────────────────────
    //   ucDatosPersona (campos de Persona + buscar/reutilizar, sin fecha de nacimiento)
    //   txtLegajo, dtpFechaIngreso, cboRol, txtNombreUsuario, txtContrasenia,
    //   lblAyudaContrasenia
    //   btnGuardar, btnCancelar
    // ─────────────────────────────────────────────────────────────────────────
    public partial class frmUsuarioEditor : Form
    {
        private readonly ServicioUsuario _servicioUsuario = new();
        private readonly ServicioCatalogo _servicioCatalogo = new();
        private readonly Usuario? _usuarioExistente;

        public frmUsuarioEditor(Usuario? usuarioExistente)
        {
            InitializeComponent();
            _usuarioExistente = usuarioExistente;
            Text = usuarioExistente is null ? "Nuevo usuario" : "Editar usuario";

            ucDatosPersona.MostrarFechaNacimiento = false;
        }

        private void frmUsuarioEditor_Load(object sender, EventArgs e)
        {
            try
            {
                ucDatosPersona.CargarCatalogos(
                    _servicioCatalogo.ObtenerTiposDocumento(),
                    _servicioCatalogo.ObtenerProvincias(),
                    _servicioCatalogo.ObtenerLocalidades());

                cboRol.DisplayMember = nameof(Rol.NombreRol);
                cboRol.ValueMember = nameof(Rol.IdRol);
                cboRol.DataSource = _servicioUsuario.ObtenerRoles().ToList();

                if (_usuarioExistente is null)
                {
                    ucDatosPersona.PrepararParaAlta();
                    txtLegajo.Clear();
                    dtpFechaIngreso.Value = DateTime.Today;
                    if (cboRol.Items.Count > 0) cboRol.SelectedIndex = 0;
                    txtNombreUsuario.Clear();
                    txtContrasenia.Clear();
                    lblAyudaContrasenia.Text = "Obligatoria.";
                }
                else
                {
                    ucDatosPersona.CargarParaEdicion(_usuarioExistente);
                    txtLegajo.Text = _usuarioExistente.Legajo;
                    dtpFechaIngreso.Value = _usuarioExistente.FechaIngreso ?? DateTime.Today;
                    cboRol.SelectedValue = _usuarioExistente.IdRol;
                    txtNombreUsuario.Text = _usuarioExistente.NombreUsuario;
                    txtContrasenia.Clear();
                    lblAyudaContrasenia.Text = "Dejar vacía para no cambiarla.";
                }
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            var usuario = new Usuario
            {
                IdPersona = _usuarioExistente?.IdPersona ?? ucDatosPersona.IdPersonaActual ?? 0,
                Documento = ucDatosPersona.Documento,
                IdTipoDocumento = ucDatosPersona.IdTipoDocumento,
                Nombre = ucDatosPersona.Nombre,
                Apellido = ucDatosPersona.Apellido,
                Email = ucDatosPersona.Email,
                Telefono = ucDatosPersona.Telefono,
                IdLocalidad = ucDatosPersona.IdLocalidad,
                Legajo = txtLegajo.Text.Trim(),
                FechaIngreso = dtpFechaIngreso.Value.Date,
                IdRol = (int)(cboRol.SelectedValue ?? 0),
                NombreUsuario = txtNombreUsuario.Text.Trim()
            };

            try
            {
                Cursor = Cursors.WaitCursor;

                if (_usuarioExistente is null)
                {
                    _servicioUsuario.Alta(usuario, txtContrasenia.Text);
                }
                else
                {
                    _servicioUsuario.Modificar(usuario, txtContrasenia.Text);
                }

                DialogResult = DialogResult.OK;
                Close();
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

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// Los errores de negocio se muestran como advertencia (el usuario puede
        /// corregirlos); los de acceso a datos, como error.
        /// </summary>
        private static void MostrarError(Exception ex)
        {
            var esNegocio = ex is NegocioException;

            MessageBox.Show(ex.Message, "SGIG", MessageBoxButtons.OK,
                esNegocio ? MessageBoxIcon.Warning : MessageBoxIcon.Error);
        }
    }
}
