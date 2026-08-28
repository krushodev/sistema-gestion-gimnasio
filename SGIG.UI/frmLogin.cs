using SGIG.Datos;
using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.UI
{
    /// <summary>
    /// Pantalla de autenticación (RF#01). Primer formulario de la aplicación:
    /// se abre desde <see cref="Program"/> y, si las credenciales son correctas,
    /// cierra con <see cref="DialogResult.OK"/> dejando el usuario en
    /// <see cref="UsuarioAutenticado"/> para que Program abra el contenedor MDI.
    /// </summary>
    //
    // ── CONTROLES (ver frmLogin.Designer.cs) ─────────────────────────────────
    //   lblUsuario  txtUsuario  lblContrasenia  txtContrasenia (PasswordChar='*')
    //   btnIngresar  lblMensajeError (oculto)
    // ─────────────────────────────────────────────────────────────────────────
    public partial class frmLogin : Form
    {
        private readonly ServicioAutenticacion _servicioAutenticacion = new();

        /// <summary>Usuario que inició sesión. Sólo tiene valor si el diálogo cerró con OK.</summary>
        public Usuario? UsuarioAutenticado { get; private set; }

        public frmLogin()
        {
            InitializeComponent();
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            LimpiarError();
            txtUsuario.Focus();
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            LimpiarError();

            if (string.IsNullOrWhiteSpace(txtUsuario.Text) ||
                string.IsNullOrWhiteSpace(txtContrasenia.Text))
            {
                MostrarError("Ingresá usuario y contraseña.");
                return;
            }

            try
            {
                Cursor = Cursors.WaitCursor;
                btnIngresar.Enabled = false;

                var usuario = _servicioAutenticacion.Autenticar(txtUsuario.Text, txtContrasenia.Text);

                if (usuario is null)
                {
                    // Mensaje único a propósito: no se revela si falló el usuario o la contraseña.
                    MostrarError("Usuario o contraseña incorrectos.");
                    txtContrasenia.Clear();
                    txtContrasenia.Focus();
                    return;
                }

                UsuarioAutenticado = usuario;
                DialogResult = DialogResult.OK;
            }
            catch (AccesoDatosException ex)
            {
                MessageBox.Show(
                    $"{ex.Message}\n\nVerificá que el servidor de base de datos esté disponible.",
                    "SGIG", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
                btnIngresar.Enabled = true;
            }
        }

        /// <summary>Oculta el mensaje de error apenas el usuario corrige los campos.</summary>
        private void Campos_TextChanged(object sender, EventArgs e)
        {
            LimpiarError();
        }

        private void MostrarError(string mensaje)
        {
            lblMensajeError.Text = mensaje;
            lblMensajeError.Visible = true;
        }

        private void LimpiarError()
        {
            lblMensajeError.Text = string.Empty;
            lblMensajeError.Visible = false;
        }
    }
}
