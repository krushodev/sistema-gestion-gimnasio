using SGIG.Datos;
using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.UI
{
    public partial class frmLogin : Form
    {
        private readonly ServicioAutenticacion _servicioAutenticacion = new();

        public Usuario? UsuarioAutenticado { get; private set; }

        private Panel pnlFormulario = null!;
        private Panel pnlCard = null!;
        private RoundedPanel campoUsuario = null!;
        private RoundedPanel campoContrasenia = null!;
        private Label lblSubtituloLogin = null!;
        private Panel barraAcento = null!;

        private const int AnchoCard = 340;
        private const int AnchoMinimoConMarca = 680;

        public frmLogin()
        {
            InitializeComponent();
            ConstruirLayout();
            Tema.AgregarToggleContrasenia(txtContrasenia);
        }

        /// <summary>
        /// Rediseño de la Fase 9.5: panel de marca a la izquierda y formulario centrado a
        /// la derecha. Se hace por código reubicando los controles que ya declara el
        /// .Designer.cs (mismos nombres y mismo flujo de autenticación), así el diseñador
        /// de Visual Studio no se toca. En ventana angosta se oculta el panel de marca.
        /// </summary>
        private void ConstruirLayout()
        {
            BackColor = Tema.Superficie;
            Font = Tema.FuenteLabel;
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            ClientSize = new Size(860, 520);
            MinimumSize = new Size(480, 480);

            // ── Panel de marca ───────────────────────────────────────────────
            pnlLateral.BackColor = Tema.SidebarFondo;
            pnlLateral.Dock = DockStyle.Left;

            barraAcento = new Panel { Size = new Size(40, 4), BackColor = Tema.Acento };
            pnlLateral.Controls.Add(barraAcento);

            lblTituloApp.AutoSize = true;
            lblTituloApp.Font = Tema.Fuente(34F, FontStyle.Bold);
            lblTituloApp.ForeColor = Color.White;
            lblTituloApp.BackColor = Color.Transparent;
            lblTituloApp.TextAlign = ContentAlignment.MiddleLeft;

            lblSubtitulo.AutoSize = true;
            lblSubtitulo.Font = Tema.FuenteSubtitulo;
            lblSubtitulo.ForeColor = Tema.SidebarTexto;
            lblSubtitulo.BackColor = Color.Transparent;
            lblSubtitulo.TextAlign = ContentAlignment.TopLeft;
            lblSubtitulo.Text = "Sistema de Gestión Integral para Gimnasios";

            // ── Formulario ───────────────────────────────────────────────────
            pnlFormulario = new Panel { Dock = DockStyle.Fill, BackColor = Tema.Superficie };
            pnlCard = new Panel { Size = new Size(AnchoCard, 372), BackColor = Tema.Superficie };
            pnlFormulario.Controls.Add(pnlCard);
            Controls.Add(pnlFormulario);
            pnlFormulario.BringToFront();

            lblIniciarSesion.AutoSize = true;
            lblIniciarSesion.Font = Tema.Fuente(22F, FontStyle.Bold);
            lblIniciarSesion.ForeColor = Tema.TextoPrimario;
            lblIniciarSesion.Location = new Point(0, 0);
            pnlCard.Controls.Add(lblIniciarSesion);

            lblSubtituloLogin = new Label
            {
                Text = "Ingresá tus credenciales para continuar.",
                Font = Tema.FuenteTexto,
                ForeColor = Tema.TextoSecundario,
                AutoSize = true,
                Location = new Point(2, 46)
            };
            pnlCard.Controls.Add(lblSubtituloLogin);

            EstilizarEtiquetaCampo(lblUsuario, 96);
            campoUsuario = CrearCampo(txtUsuario, Iconos.Usuarios, 118, tabIndex: 0);

            EstilizarEtiquetaCampo(lblContrasenia, 176);
            campoContrasenia = CrearCampo(txtContrasenia, Iconos.Acceso, 198, tabIndex: 1);

            lblMensajeError.AutoSize = false;
            lblMensajeError.Font = Tema.Fuente(9.5F);
            lblMensajeError.ForeColor = Tema.Peligro;
            lblMensajeError.BackColor = Tema.PeligroFondo;
            lblMensajeError.TextAlign = ContentAlignment.MiddleLeft;
            lblMensajeError.Padding = new Padding(12, 0, 12, 0);
            lblMensajeError.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            lblMensajeError.SetBounds(0, 258, AnchoCard, 36);
            pnlCard.Controls.Add(lblMensajeError);

            btnIngresar.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnIngresar.SetBounds(0, 310, AnchoCard, 46);
            btnIngresar.TabIndex = 2;
            Tema.EstilizarBoton(btnIngresar);
            pnlCard.Controls.Add(btnIngresar);

            Resize += (s, e) => Reubicar();
            Reubicar();
        }

        private void EstilizarEtiquetaCampo(Label etiqueta, int y)
        {
            etiqueta.AutoSize = true;
            etiqueta.Font = Tema.Fuente(9F, FontStyle.Bold);
            etiqueta.ForeColor = Tema.TextoSecundario;
            etiqueta.Location = new Point(2, y);
            pnlCard.Controls.Add(etiqueta);
        }

        /// <summary>
        /// Campo de texto con ícono de línea a la izquierda dentro de un contenedor con
        /// borde que se resalta con el foco. El TextBox queda sin borde propio.
        /// </summary>
        private RoundedPanel CrearCampo(TextBox txt, string icono, int y, int tabIndex)
        {
            var campo = new RoundedPanel
            {
                Radio = 8,
                Size = new Size(AnchoCard, 44),
                Location = new Point(0, y),
                TabIndex = tabIndex
            };

            var lblIcono = new Label
            {
                Text = icono,
                Font = Tema.FuenteIconos(11F),
                ForeColor = Tema.TextoSecundario,
                BackColor = Color.Transparent,
                AutoSize = false,
                Size = new Size(36, 44),
                Location = new Point(2, 0),
                TextAlign = ContentAlignment.MiddleCenter
            };

            txt.BorderStyle = BorderStyle.None;
            txt.Font = Tema.Fuente(11F);
            txt.BackColor = Tema.Superficie;
            txt.ForeColor = Tema.TextoPrimario;
            txt.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            txt.Width = AnchoCard - 40 - 8;
            txt.Location = new Point(40, (campo.Height - txt.Height) / 2);
            txt.TabIndex = 0;

            campo.Controls.Add(lblIcono);
            campo.Controls.Add(txt);
            pnlCard.Controls.Add(campo);

            txt.Enter += (s, e) => { campo.ColorBorde = Tema.Acento; campo.Invalidate(); };
            txt.Leave += (s, e) => { campo.ColorBorde = Tema.Borde; campo.Invalidate(); };
            return campo;
        }

        private void Reubicar()
        {
            bool conMarca = ClientSize.Width >= AnchoMinimoConMarca;
            pnlLateral.Visible = conMarca;

            if (conMarca)
            {
                pnlLateral.Width = Math.Clamp((int)(ClientSize.Width * 0.42), 300, 380);

                const int margen = 44;
                lblSubtitulo.MaximumSize = new Size(pnlLateral.Width - 2 * margen, 0);
                int altoGrupo = barraAcento.Height + 24 + lblTituloApp.Height + 6 + lblSubtitulo.Height;
                int y = Math.Max(24, (pnlLateral.Height - altoGrupo) / 2);

                barraAcento.Location = new Point(margen, y);
                lblTituloApp.Location = new Point(margen - 4, y + barraAcento.Height + 24);
                lblSubtitulo.Location = new Point(margen, lblTituloApp.Bottom + 6);
            }

            pnlCard.Location = new Point(
                Math.Max(0, (pnlFormulario.ClientSize.Width - pnlCard.Width) / 2),
                Math.Max(0, (pnlFormulario.ClientSize.Height - pnlCard.Height) / 2));
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
                btnIngresar.Text = "Ingresando…";
                btnIngresar.Refresh();

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
                btnIngresar.Text = "Ingresar";
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
            lblMensajeError.BringToFront(); // Garantiza que no quede detrás de ningún panel
        }

        private void LimpiarError()
        {
            lblMensajeError.Text = string.Empty;
            lblMensajeError.Visible = false;
        }
    }
}