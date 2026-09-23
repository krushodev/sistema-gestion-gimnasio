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