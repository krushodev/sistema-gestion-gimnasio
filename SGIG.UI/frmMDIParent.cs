namespace SGIG.UI
{
    /// <summary>
    /// Contenedor MDI y punto único de navegación del sistema (RNF#02).
    /// Todos los formularios del sistema se abren como hijos de este, nunca sueltos.
    /// Se abre desde <see cref="frmLogin"/> tras un login exitoso.
    /// </summary>
    //
    // ── CONTROLES A AGREGAR CON EL DISEÑADOR DE VISUAL STUDIO ────────────────
    //
    // 1) MenuStrip llamado 'mnuPrincipal' (Dock = Top), con esta jerarquía.
    //    Cada ítem lleva su nombre en notación húngara, no el que autogenera VS:
    //
    //   mnuSeguridad        "&Seguridad"
    //     ├─ mnuUsuarios            "&Usuarios"
    //     └─ mnuTablasParametricas  "Tablas &paramétricas"
    //   mnuPersonas         "&Personas"
    //     └─ mnuSocios              "&Socios"
    //   mnuTesoreria        "&Tesorería"
    //     ├─ mnuPlanes              "P&lanes"
    //     ├─ mnuPagos               "Registrar &pago"
    //     └─ mnuHistorialPagos      "&Historial de pagos"
    //   mnuControlAcceso    "Control de &acceso"
    //     └─ mnuCheckin             "&Check-in"
    //   mnuActivos          "Ac&tivos"
    //     ├─ mnuMaquinas                  "&Máquinas"
    //     ├─ mnuMantenimiento             "Registrar &mantenimiento"
    //     └─ mnuHistorialMantenimientos   "&Historial de mantenimientos"
    //   mnuGastos           "&Gastos"
    //     ├─ mnuGastosAbm           "&Gastos"
    //     ├─ mnuReporteBalance      "Reporte de &balance"
    //     └─ mnuBackup              "&Backup"
    //
    // 2) StatusStrip llamado 'stsEstado' (Dock = Bottom) con:
    //   lblUsuarioLogueado  ToolStripStatusLabel   Text = ""  (nombre y rol activo)
    //   btnCerrarSesion     ToolStripStatusLabel   Text = "Cerrar sesión"
    //                       IsLink = True   Alignment = Right
    //
    // Propiedades del formulario (frmMDIParent):
    //   IsMdiContainer  = True        ← imprescindible
    //   Text            = "SGIG — Sistema de Gestión Integral para Gimnasios"
    //   WindowState     = Maximized
    //   MainMenuStrip   = mnuPrincipal
    //
    // Eventos a suscribir desde el diseñador:
    //   btnCerrarSesion.Click -> btnCerrarSesion_Click
    // ─────────────────────────────────────────────────────────────────────────
    public partial class frmMDIParent : Form
    {
        public frmMDIParent()
        {
            InitializeComponent();
        }

        private void frmMDIParent_Load(object sender, EventArgs e)
        {
            // TODO (Fase 2.4): recibir el Usuario autenticado, mostrar su nombre y rol
            // en lblUsuarioLogueado y habilitar los ítems de menú según la matriz de
            // permisos. Hasta entonces todo el menú arranca deshabilitado.
            lblUsuarioLogueado.Text = string.Empty;
            DeshabilitarTodoElMenu();
        }

        /// <summary>
        /// Deja todos los ítems de menú deshabilitados. En la Fase 2.4 se habilitan
        /// selectivamente según el rol del usuario logueado.
        /// </summary>
        private void DeshabilitarTodoElMenu()
        {
            foreach (var menu in mnuPrincipal.Items.OfType<ToolStripMenuItem>())
            {
                menu.Enabled = false;
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
