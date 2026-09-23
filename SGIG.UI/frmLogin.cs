using SGIG.Datos;
using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.UI
{
    public partial class frmLogin : Form
    {
        private readonly ServicioAutenticacion _servicioAutenticacion = new();

        public Usuario? UsuarioAutenticado { get; private set; }

        public frmLogin()
        {
            InitializeComponent();
            Tema.AgregarToggleContrasenia(txtContrasenia);
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
                MostrarError("Usuario o contraseña incorrectos.");
                return;
            }

            try
            {
                Cursor = Cursors.WaitCursor;
                btnIngresar.Enabled = false;

                var usuario = _servicioAutenticacion.Autenticar(txtUsuario.Text, txtContrasenia.Text);

                if (usuario is null)
                {
                    // Desuscribimos temporalmente TextChanged para que el Clear() no borre el error
                    txtContrasenia.TextChanged -= Campos_TextChanged;
                    txtContrasenia.Clear();
                    txtContrasenia.TextChanged += Campos_TextChanged;

                    MostrarError("Usuario o contraseña incorrectos.");
                    txtContrasenia.Focus();
                    return;
                }

                UsuarioAutenticado = usuario;
                DialogResult = DialogResult.OK;
            }
            catch (AccesoDatosException)
            {
                MostrarError("Error de conexión al servidor de base de datos.");
            }
            finally
            {
                Cursor = Cursors.Default;
                btnIngresar.Enabled = true;
            }
        }

        private void Campos_TextChanged(object sender, EventArgs e)
        {
            LimpiarError();
        }

        private void MostrarError(string mensaje)
        {
            lblMensajeError.ForeColor = Tema.Peligro;
            lblMensajeError.Text = mensaje;
            lblMensajeError.Visible = true;
            lblMensajeError.BringToFront(); // Garantiza que no quede detrás de ningún panel
        }

        private void LimpiarError()
        {
            lblMensajeError.Text = string.Empty;
            lblMensajeError.Visible = false;
        }
    }
}