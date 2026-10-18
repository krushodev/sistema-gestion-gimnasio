using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SGIG.Entidades;

namespace SGIG.UI
{
    /// <summary>
    /// Contenedor principal: sidebar de navegación a la izquierda, barra superior con el
    /// título de la sección y el usuario, y el área de contenido donde se embebe la
    /// pantalla activa (RNF#02). Todo se arma por código; el .Designer.cs sólo conserva
    /// la franja de estado original, oculta.
    /// </summary>
    public partial class frmMDIParent : Form
    {
        private const int AnchoMinimoExpandido = 1100;
        private const string ClaveInicio = "inicio";

        private readonly Usuario _usuario;

        private ucSidebar _sidebar = null!;
        private Panel pnlPrincipal = null!;
        private Panel pnlBarraSuperior = null!;
        private Panel pnlContenido = null!;
        private ucInicio pnlInicio = null!;
        private Label lblTituloSeccion = null!;
        private Label lblNombreUsuario = null!;
        private Label lblRolUsuario = null!;

        private Form? _hijoActual;
        private string _claveActual = string.Empty;
        private bool _ultimoAutoColapsado;

        private sealed record Destino(
            string Seccion, string Clave, string Titulo, string Icono, int[] Roles, Func<Form> Crear);

        private readonly Dictionary<string, Destino> _destinos = new();

        public frmMDIParent(Usuario usuario)
        {
            InitializeComponent();
            _usuario = usuario ?? throw new ArgumentNullException(nameof(usuario));
            MinimumSize = new Size(900, 600);
        }

        private void frmMDIParent_Load(object? sender, EventArgs e)
        {
            // La franja inferior original (stsEstado) queda oculta: el usuario y
            // "Cerrar sesión" ahora viven en la barra superior y en el sidebar.
            stsEstado.Visible = false;

            ConstruirLayout();
            ConstruirNavegacion();
            Navegar(ClaveInicio);

            Resize += (s, ev) => AdaptarSidebarAlAncho();
            AdaptarSidebarAlAncho(forzar: true);
        }

        // ── Layout general ───────────────────────────────────────────────────
        private void ConstruirLayout()
        {
            BackColor = Tema.Fondo;

            _sidebar = new ucSidebar { Dock = DockStyle.Left };
            _sidebar.ItemSeleccionado += Navegar;
            _sidebar.ConfiguracionSolicitada += (s, e) => AbrirConfiguracion();
            _sidebar.CerrarSesionSolicitado += (s, e) => CerrarSesion();

            pnlPrincipal = new Panel { Dock = DockStyle.Fill, BackColor = Tema.Fondo };

            pnlBarraSuperior = new Panel
            {
                Dock = DockStyle.Top,
                Height = 64,
                BackColor = Tema.Superficie
            };
            pnlBarraSuperior.Paint += (s, e) =>
            {
                using var lapiz = new Pen(Tema.Borde);
                e.Graphics.DrawLine(lapiz, 0, pnlBarraSuperior.Height - 1,
                    pnlBarraSuperior.Width, pnlBarraSuperior.Height - 1);
            };

            // Usuario: avatar con iniciales + nombre y rol, anclado a la derecha.
            var pnlUsuario = new Panel { Dock = DockStyle.Right, Width = 280, BackColor = Tema.Superficie };
            var avatar = new AvatarIniciales(Iniciales()) { Location = new Point(12, 14) };
            lblNombreUsuario = new Label
            {
                Text = $"{_usuario.Nombre} {_usuario.Apellido}",
                Font = Tema.Fuente(10F, FontStyle.Bold),
                ForeColor = Tema.TextoPrimario,
                AutoEllipsis = true,
                Location = new Point(58, 13),
                Size = new Size(210, 20),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            lblRolUsuario = new Label
            {
                Text = _usuario.Rol?.NombreRol ?? string.Empty,
                Font = Tema.Fuente(9F),
                ForeColor = Tema.TextoSecundario,
                AutoEllipsis = true,
                Location = new Point(58, 33),
                Size = new Size(210, 18),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            pnlUsuario.Controls.AddRange(new Control[] { avatar, lblNombreUsuario, lblRolUsuario });

            lblTituloSeccion = new Label
            {
                Dock = DockStyle.Fill,
                Font = Tema.FuenteTitulo,
                ForeColor = Tema.TextoPrimario,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(24, 0, 0, 0),
                AutoEllipsis = true
            };

            pnlBarraSuperior.Controls.Add(pnlUsuario);
            pnlBarraSuperior.Controls.Add(lblTituloSeccion);
            lblTituloSeccion.BringToFront();

            pnlContenido = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Tema.Fondo,
                Padding = new Padding(24)
            };

            pnlInicio = new ucInicio(_usuario, Navegar);
            pnlContenido.Controls.Add(pnlInicio);

            pnlPrincipal.Controls.Add(pnlContenido);
            pnlPrincipal.Controls.Add(pnlBarraSuperior);
            pnlContenido.BringToFront();

            Controls.Add(pnlPrincipal);
            Controls.Add(_sidebar);
            pnlPrincipal.BringToFront();
        }

        private string Iniciales()
        {
            string n = string.IsNullOrWhiteSpace(_usuario.Nombre) ? "" : _usuario.Nombre.Trim()[..1];
            string a = string.IsNullOrWhiteSpace(_usuario.Apellido) ? "" : _usuario.Apellido.Trim()[..1];
            string iniciales = n + a;
            return iniciales.Length > 0 ? iniciales : "?";
        }

        // ── Navegación ───────────────────────────────────────────────────────
        /// <summary>
        /// Qué ve cada rol. La matriz de permisos es la de siempre (Fase 2.4): sólo
        /// cambia dónde se muestra, no quién puede entrar a qué.
        /// </summary>
        private void ConstruirNavegacion()
        {
            const int A = Roles.Administrador, R = Roles.Recepcionista, T = Roles.Tecnico;

            var catalogo = new Destino[]
            {
                new("Gestión", "socios", "Socios", Iconos.Socios, new[] { A, R }, () => new frmSocios()),
                new("Gestión", "cobro", "Cobro de cuotas", Iconos.Cobro, new[] { A, R }, () => new frmCobroCuota()),
                new("Gestión", "planes", "Planes", Iconos.Planes, new[] { A }, () => new frmPlanes()),

                new("Acceso", "checkin", "Control de acceso", Iconos.Acceso, new[] { R }, () => new frmCheckin()),
                new("Acceso", "historialAccesos", "Historial de accesos", Iconos.Historial, new[] { R }, () => new frmHistorialAccesos()),

                new("Activos", "maquinas", "Máquinas", Iconos.Maquinas, new[] { A, T }, () => new frmMaquinas()),
                new("Activos", "mantenimiento", "Mantenimiento", Iconos.Mantenimiento, new[] { T }, () => new frmMantenimiento(_usuario)),
                new("Activos", "historialMantenimientos", "Historial de mantenimientos", Iconos.Historial, new[] { A, T }, () => new frmHistorialMantenimientos()),

                new("Administración", "usuarios", "Usuarios", Iconos.Usuarios, new[] { A }, () => new frmUsuarios(_usuario)),
                new("Administración", "reportes", "Reportes", Iconos.Reportes, new[] { A }, () => new frmReportes()),
                new("Administración", "backup", "Copia de seguridad", Iconos.Backup, new[] { A }, () => new frmBackup()),
            };

            _sidebar.AgregarItem(ClaveInicio, "Inicio", Iconos.Inicio);

            string? seccionActual = null;
            foreach (var destino in catalogo)
            {
                if (Array.IndexOf(destino.Roles, _usuario.IdRol) < 0) continue;

                if (destino.Seccion != seccionActual)
                {
                    _sidebar.AgregarSeccion(destino.Seccion);
                    seccionActual = destino.Seccion;
                }

                _destinos[destino.Clave] = destino;
                _sidebar.AgregarItem(destino.Clave, destino.Titulo, destino.Icono);
            }
        }

        /// <summary>
        /// Reemplaza la pantalla activa por la indicada (cerrando la anterior en vez de
        /// apilarla) y marca el ítem activo del sidebar.
        /// </summary>
        private void Navegar(string clave)
        {
            if (clave == _claveActual) return;

            CerrarHijoActual();

            if (clave == ClaveInicio)
            {
                pnlInicio.Visible = true;
                pnlInicio.BringToFront();
                _ = pnlInicio.RecargarAsync();
                lblTituloSeccion.Text = "Inicio";
            }
            else if (_destinos.TryGetValue(clave, out var destino))
            {
                pnlInicio.Visible = false;

                var hijo = destino.Crear();
                hijo.TopLevel = false;
                hijo.FormBorderStyle = FormBorderStyle.None;
                hijo.Dock = DockStyle.Fill;

                Tema.EstilizarFormulario(hijo);
                Tema.EstilizarControles(hijo);

                pnlContenido.Controls.Add(hijo);
                hijo.BringToFront();
                hijo.Show();

                _hijoActual = hijo;
                lblTituloSeccion.Text = destino.Titulo;
            }
            else
            {
                return;
            }

            _claveActual = clave;
            _sidebar.Seleccionar(clave);
        }

        private void CerrarHijoActual()
        {
            if (_hijoActual == null) return;

            pnlContenido.Controls.Remove(_hijoActual);
            _hijoActual.Close();
            _hijoActual.Dispose();
            _hijoActual = null;
        }

        // ── Adaptabilidad ────────────────────────────────────────────────────
        /// <summary>
        /// Colapsa el sidebar a sólo íconos por debajo de <see cref="AnchoMinimoExpandido"/>.
        /// Sólo reacciona cuando la ventana cruza el umbral, así el botón de hamburguesa
        /// puede contradecirlo a mano sin que el próximo resize lo pise.
        /// </summary>
        private void AdaptarSidebarAlAncho(bool forzar = false)
        {
            bool debeColapsar = ClientSize.Width < AnchoMinimoExpandido;
            if (!forzar && debeColapsar == _ultimoAutoColapsado) return;

            _ultimoAutoColapsado = debeColapsar;
            _sidebar.Colapsado = debeColapsar;
        }

        // ── Cuenta ───────────────────────────────────────────────────────────
        private void AbrirConfiguracion()
        {
            using var frm = new frmConfiguracion(_usuario);
            frm.ShowDialog(this);
            lblNombreUsuario.Text = $"{_usuario.Nombre} {_usuario.Apellido}";
        }

        // Sigue acá porque frmMDIParent.Designer.cs engancha el evento del
        // ToolStripStatusLabel original, pero ese control está oculto: el acceso real a
        // "Cerrar sesión" ahora está en el sidebar, que llama a CerrarSesion() directamente.
        private void btnCerrarSesion_Click(object? sender, EventArgs e) => CerrarSesion();

        private void CerrarSesion()
        {
            var respuesta = MessageBox.Show(
                "¿Confirmás cerrar la sesión actual?", "SGIG",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (respuesta == DialogResult.Yes)
            {
                Close();
            }
        }
    }
}
