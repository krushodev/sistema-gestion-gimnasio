using SGIG.Entidades;

namespace SGIG.UI
{
    /// <summary>
    /// Contenedor MDI y punto único de navegación del sistema (RNF#02).
    /// Todos los formularios se abren como hijos de este, nunca sueltos.
    /// Se abre desde <see cref="Program"/> tras un login exitoso, con el usuario
    /// autenticado: el menú se arma según su rol (RF#02).
    /// </summary>
    //
    // ── CONTROLES (ver frmMDIParent.Designer.cs) ─────────────────────────────
    //   mnuPrincipal (MenuStrip) con mnuSeguridad, mnuPersonas, mnuTesoreria,
    //   mnuControlAcceso, mnuActivos, mnuReportes y sus subítems.
    //   stsEstado (StatusStrip) con lblUsuarioLogueado y btnCerrarSesion.
    // ─────────────────────────────────────────────────────────────────────────
    public partial class frmMDIParent : Form
    {
        private readonly Usuario _usuario;

        public frmMDIParent(Usuario usuario)
        {
            InitializeComponent();
            _usuario = usuario ?? throw new ArgumentNullException(nameof(usuario));
        }

        private void frmMDIParent_Load(object sender, EventArgs e)
        {
            lblUsuarioLogueado.Text =
                $"{_usuario.Nombre} {_usuario.Apellido}  ·  {_usuario.Rol?.NombreRol}";

            AplicarPermisos(_usuario.Rol?.NombreRol);
        }

        /// <summary>
        /// Habilita cada pantalla según la matriz de permisos de la ERS (RF#02).
        /// Todo arranca deshabilitado y se prende sólo lo que el rol tiene permitido:
        /// si mañana aparece un rol nuevo, por omisión no ve nada.
        /// </summary>
        private void AplicarPermisos(string? nombreRol)
        {
            foreach (var item in mnuPrincipal.Items.OfType<ToolStripMenuItem>())
            {
                item.Enabled = false;
                foreach (var sub in item.DropDownItems.OfType<ToolStripMenuItem>())
                {
                    sub.Enabled = false;
                }
            }

            switch (nombreRol)
            {
                case "Administrador":
                    Habilitar(mnuUsuarios, mnuTablasParametricas, mnuSocios, mnuPlanes,
                              mnuHistorialPagos, mnuMaquinas, mnuHistorialMantenimientos,
                              mnuReporteIngresos, mnuBackup);
                    break;

                case "Recepcionista":
                    Habilitar(mnuSocios, mnuPagos, mnuHistorialPagos, mnuCheckin);
                    break;

                case "Tecnico":
                    Habilitar(mnuMaquinas, mnuMantenimiento, mnuHistorialMantenimientos);
                    break;
            }

            // Un menú de nivel superior sólo se muestra habilitado si alguno de sus
            // hijos lo está; si no, queda gris y el usuario no lo intenta abrir.
            foreach (var item in mnuPrincipal.Items.OfType<ToolStripMenuItem>())
            {
                item.Enabled = item.DropDownItems.OfType<ToolStripMenuItem>().Any(s => s.Enabled);
            }
        }

        private static void Habilitar(params ToolStripMenuItem[] items)
        {
            foreach (var item in items)
            {
                item.Enabled = true;
            }
        }

        /// <summary>
        /// Abre un formulario como hijo MDI. Si ya hay una instancia de ese tipo
        /// abierta la trae al frente en vez de duplicarla (RNF#02).
        /// </summary>
        private void AbrirFormularioHijo<T>() where T : Form, new()
        {
            var abierto = MdiChildren.OfType<T>().FirstOrDefault();

            if (abierto is not null)
            {
                abierto.Activate();
                return;
            }

            var hijo = new T
            {
                MdiParent = this,
                StartPosition = FormStartPosition.CenterParent
            };
            hijo.Show();
        }

        /// <summary>Igual que el anterior, para formularios que necesitan el usuario logueado.</summary>
        private void AbrirFormularioHijo<T>(Func<T> fabrica) where T : Form
        {
            var abierto = MdiChildren.OfType<T>().FirstOrDefault();

            if (abierto is not null)
            {
                abierto.Activate();
                return;
            }

            var hijo = fabrica();
            hijo.MdiParent = this;
            hijo.StartPosition = FormStartPosition.CenterParent;
            hijo.Show();
        }

        private void mnuUsuarios_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo(() => new frmUsuarios(_usuario));
        }

        private void mnuTablasParametricas_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo<frmTablasParametricas>();
        }

        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            var respuesta = MessageBox.Show(
                "¿Confirmás cerrar la sesión?", "SGIG",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (respuesta != DialogResult.Yes) return;

            // Basta con cerrar el contenedor: el bucle de Program.Main vuelve a
            // mostrar frmLogin. Al cerrarse el MDI, sus hijos se cierran con él.
            Close();
        }
    }
}
