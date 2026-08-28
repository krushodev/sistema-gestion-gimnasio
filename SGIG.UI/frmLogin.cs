namespace SGIG.UI
{
    /// <summary>
    /// Pantalla de autenticación (RF#01). Primer formulario de la aplicación:
    /// se abre desde <see cref="Program"/> y, tras un login exitoso, da paso a
    /// <see cref="frmMDIParent"/>. No requiere rol: es previa al login.
    /// </summary>
    //
    // ── CONTROLES A AGREGAR CON EL DISEÑADOR DE VISUAL STUDIO ────────────────
    //
    //   Nombre              Tipo       Propiedades a fijar
    //   ------------------  ---------  ---------------------------------------
    //   lblUsuario          Label      Text = "Usuario:"
    //   txtUsuario          TextBox    MaxLength = 50
    //   lblContrasenia      Label      Text = "Contraseña:"
    //   txtContrasenia      TextBox    PasswordChar = '*'   MaxLength = 50
    //   btnIngresar         Button     Text = "Ingresar"
    //   lblMensajeError     Label      Visible = False   ForeColor = Color.Firebrick
    //
    // Propiedades del formulario (frmLogin):
    //   Text            = "SGIG — Iniciar sesión"
    //   FormBorderStyle = FixedDialog
    //   MaximizeBox     = False   MinimizeBox = False
    //   StartPosition   = CenterScreen
    //   AcceptButton    = btnIngresar   (así Enter dispara el login)
    //
    // Eventos a suscribir desde el diseñador (o dejar el auto-wire del Designer):
    //   btnIngresar.Click     -> btnIngresar_Click
    //   txtUsuario.TextChanged, txtContrasenia.TextChanged -> Campos_TextChanged
    // ─────────────────────────────────────────────────────────────────────────
    public partial class frmLogin : Form
    {
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

            // TODO (Fase 2.4): autenticar contra ServicioAutenticacion (hash SHA256).
            // Si las credenciales son válidas, guardar el Usuario en UsuarioAutenticado
            // y cerrar con DialogResult.OK — Program se encarga de abrir frmMDIParent.
            // Si no, MostrarError("Usuario o contraseña incorrectos.").
            //
            //     DialogResult = DialogResult.OK;
            //
            // Por ahora la pantalla es sólo el esqueleto visual.
            MostrarError("La autenticación se implementa en la Fase 2.");
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
