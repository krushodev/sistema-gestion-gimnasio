using System;
using System.Drawing;
using System.Windows.Forms;
using SGIG.Entidades;

namespace SGIG.UI
{
    public partial class frmMDIParent : Form
    {
        private readonly Usuario _usuario;
        private Panel pnlContenedor = null!;
        private Panel pnlDashboard = null!;
        private TableLayoutPanel tlpGrilla = null!;
        private Label _lblUsuarioSuperior = null!;

        public frmMDIParent(Usuario usuario)
        {
            InitializeComponent();
            _usuario = usuario ?? throw new ArgumentNullException(nameof(usuario));
        }

        private void frmMDIParent_Load(object sender, EventArgs e)
        {
            ConstruirDashboard();
            CargarTarjetasSegunRol(_usuario.IdRol);
        }

        /// <summary>Texto que muestra la barra superior con los datos del usuario logueado.</summary>
        private string TextoUsuarioLogueado() =>
            $"👤  {_usuario.Nombre} {_usuario.Apellido}   |   Rol: {_usuario.Rol?.NombreRol}";

        /// <summary>
        /// Barra persistente arriba a la derecha (usuario logueado, Configuración,
        /// Cerrar sesión), visible siempre -- a diferencia de pnlHeader, que vive
        /// dentro de pnlDashboard y se oculta al abrir cualquier módulo. Reemplaza a
        /// la franja stsEstado de abajo, que quedaba casi invisible.
        /// </summary>
        private void ConstruirBarraSuperior()
        {
            var pnlBarraSuperior = new Panel
            {
                Dock = DockStyle.Top,
                Height = 44,
                BackColor = Tema.SlateOscuro,
                Name = "pnlBarraSuperior"
            };

            _lblUsuarioSuperior = new Label
            {
                Text = TextoUsuarioLogueado(),
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(226, 232, 240),
                AutoSize = true
            };

            var btnConfiguracion = new Button
            {
                Text = "⚙ Configuración",
                Font = Tema.FuenteBoton,
                BackColor = Tema.SlateMedio,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(140, 30),
                Cursor = Cursors.Hand
            };
            btnConfiguracion.FlatAppearance.BorderSize = 0;
            btnConfiguracion.Click += (s, e) =>
            {
                using var frm = new frmConfiguracion(_usuario);
                frm.ShowDialog(this);
                _lblUsuarioSuperior.Text = TextoUsuarioLogueado();
            };

            var btnCerrarSesionSuperior = new Button
            {
                Text = "Cerrar sesión",
                Font = Tema.FuenteBoton,
                BackColor = Tema.Peligro,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 30),
                Cursor = Cursors.Hand
            };
            btnCerrarSesionSuperior.FlatAppearance.BorderSize = 0;
            btnCerrarSesionSuperior.Click += (s, e) => CerrarSesion();

            pnlBarraSuperior.Controls.Add(_lblUsuarioSuperior);
            pnlBarraSuperior.Controls.Add(btnConfiguracion);
            pnlBarraSuperior.Controls.Add(btnCerrarSesionSuperior);

            Controls.Add(pnlBarraSuperior);
            pnlBarraSuperior.BringToFront();

            // Recién tiene ancho real una vez agregada al formulario (igual que
            // pnlBarraVolver en AbrirFormularioEnPanel): se ubica todo a la derecha
            // después de agregarla.
            const int margen = 16;
            const int separacion = 10;
            btnCerrarSesionSuperior.Location = new Point(
                pnlBarraSuperior.ClientSize.Width - margen - btnCerrarSesionSuperior.Width, 7);
            btnConfiguracion.Location = new Point(
                btnCerrarSesionSuperior.Left - separacion - btnConfiguracion.Width, 7);
            _lblUsuarioSuperior.Location = new Point(
                btnConfiguracion.Left - separacion - _lblUsuarioSuperior.Width, 13);
        }

        private void ConstruirDashboard()
        {
            // La franja inferior original (stsEstado) queda oculta: el usuario logueado
            // y cerrar sesión casi no se veían ahí abajo. Se reemplaza por una barra
            // persistente arriba a la derecha (ver ConstruirBarraSuperior), visible
            // tanto en el dashboard como con cualquier pantalla hija abierta.
            stsEstado.Visible = false;

            ConstruirBarraSuperior();

            // Contenedor principal
            pnlContenedor = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(241, 245, 249)
            };
            this.Controls.Add(pnlContenedor);
            pnlContenedor.BringToFront();

            // Vista del Dashboard con padding perimetral
            pnlDashboard = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(241, 245, 249),
                Padding = new Padding(50, 30, 50, 40)
            };

            // Encabezado superior
            var pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 85,
                BackColor = Color.Transparent
            };

            var lblHola = new Label
            {
                Text = $"¡Hola, {_usuario.Nombre}!",
                Font = new Font("Segoe UI", 26F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(0, 0),
                AutoSize = true
            };

            var lblSub = new Label
            {
                Text = "Bienvenido al panel principal de SGIG. Seleccioná el módulo con el que deseás trabajar hoy:",
                Font = new Font("Segoe UI", 12F, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(2, 50),
                AutoSize = true
            };

            pnlHeader.Controls.Add(lblHola);
            pnlHeader.Controls.Add(lblSub);

            // Grilla responsiva de 4 columnas x 2 filas
            tlpGrilla = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 2,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 20, 0, 0)
            };

            tlpGrilla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            tlpGrilla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            tlpGrilla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            tlpGrilla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));

            tlpGrilla.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
            tlpGrilla.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));

            pnlDashboard.Controls.Add(tlpGrilla);
            pnlDashboard.Controls.Add(pnlHeader);

            pnlContenedor.Controls.Add(pnlDashboard);
        }

        private void CargarTarjetasSegunRol(int idRol)
        {
            tlpGrilla.Controls.Clear();

            // 1. 👥 Socios
            if (idRol is Roles.Administrador or Roles.Recepcionista)
            {
                AgregarTarjetaGrilla(
                    "Gestión de Socios",
                    "Alta, modificación, bajas, fichas médicas y listado general de alumnos.",
                    "👥",
                    Color.FromArgb(37, 99, 235),
                    () => AbrirFormularioEnPanel(new frmSocios(), "👥  Gestión de Socios")
                );
            }

            // 2. 🛡️ Usuarios
            if (idRol == Roles.Administrador)
            {
                AgregarTarjetaGrilla(
                    "Seguridad y Usuarios",
                    "Gestión de cuentas del personal, roles, permisos y credenciales.",
                    "🛡️",
                    Color.FromArgb(79, 70, 229),
                    () => AbrirFormularioEnPanel(new frmUsuarios(_usuario), "🛡️  Seguridad y Usuarios")
                );
            }

            // 3. 💳 Tesorería: Cobro de Cuotas y Planes
            if (idRol is Roles.Administrador or Roles.Recepcionista)
            {
                AgregarTarjetaGrilla(
                    "Cobro de Cuotas",
                    "Registrar pago de socios, emisión de facturación y cálculo de vencimiento.",
                    "💳",
                    Color.FromArgb(5, 150, 105),
                    () => AbrirFormularioEnPanel(new frmCobroCuota(), "💳  Cobro de Cuotas")
                );

                if (idRol == Roles.Administrador)
                {
                    AgregarTarjetaGrilla(
                        "Planes de Membresía",
                        "Consulta de tarifas y periodicidad vigentes (catálogo sembrado con la aplicación).",
                        "📋",
                        Color.FromArgb(16, 185, 129),
                        () => AbrirFormularioEnPanel(new frmPlanes(), "📋  Planes de Membresía")
                    );
                }
            }

            // 4. ⏱️ Control de Acceso (Recepcionista)
            if (idRol == Roles.Recepcionista)
            {
                AgregarTarjetaGrilla(
                    "Control de Acceso",
                    "Monitoreo de entradas en recepción, molinete y estado de cuotas al día.",
                    "⏱️",
                    Color.FromArgb(245, 158, 11),
                    () => AbrirFormularioEnPanel(new frmCheckin(), "⏱️  Control de Acceso")
                );
            }

            // 5. 🏋️ Activos y Máquinas (Administrador y Técnico)
            if (idRol is Roles.Administrador or Roles.Tecnico)
            {
                AgregarTarjetaGrilla(
                    "Máquinas",
                    "Consulta del inventario y estado de las máquinas (catálogo sembrado con la aplicación).",
                    "🏋️",
                    Color.FromArgb(236, 72, 153),
                    () => AbrirFormularioEnPanel(new frmMaquinas(), "🏋️  Máquinas")
                );

                AgregarTarjetaGrilla(
                    "Historial de Mantenimientos",
                    "Consulta de intervenciones técnicas registradas por máquina.",
                    "🛠️",
                    Color.FromArgb(190, 24, 93),
                    () => AbrirFormularioEnPanel(new frmHistorialMantenimientos(), "🛠️  Historial de Mantenimientos")
                );
            }

            // Mantenimiento (Técnico)
            if (idRol == Roles.Tecnico)
            {
                AgregarTarjetaGrilla(
                    "Mantenimiento",
                    "Registrar una falla y finalizar mantenimientos en curso.",
                    "🔧",
                    Color.FromArgb(217, 119, 6),
                    () => AbrirFormularioEnPanel(new frmMantenimiento(_usuario), "🔧  Mantenimiento")
                );
            }

            // 7. 📊 Reportes (Administrador)
            if (idRol == Roles.Administrador)
            {
                AgregarTarjetaGrilla(
                    "Reportes e Ingresos",
                    "Métricas de concurrencia, balance financiero, altas y bajas periódicas.",
                    "📊",
                    Color.FromArgb(14, 165, 233),
                    () => MessageBox.Show("Módulo de Reportes en desarrollo.", "SGIG", MessageBoxButtons.OK, MessageBoxIcon.Information)
                );

                // 8. 💾 Copias de Seguridad (Administrador)
                AgregarTarjetaGrilla(
                    "Copia de Seguridad",
                    "Generación y restauración de backups para la base de datos SQL Server.",
                    "💾",
                    Color.FromArgb(100, 116, 139),
                    () => MessageBox.Show("Módulo de Backup en desarrollo.", "SGIG", MessageBoxButtons.OK, MessageBoxIcon.Information)
                );
            }
        }

        private void AgregarTarjetaGrilla(string titulo, string descripcion, string icono, Color colorAcento, Action accion)
        {
            var card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Margin = new Padding(12),
                Cursor = Cursors.Hand
            };

            var barraSuperior = new Panel
            {
                Dock = DockStyle.Top,
                Height = 6,
                BackColor = colorAcento
            };

            var lblIcono = new Label
            {
                Text = icono,
                Font = new Font("Segoe UI Emoji", 34F),
                Location = new Point(22, 20),
                AutoSize = true
            };

            var lblTitulo = new Label
            {
                Text = titulo,
                Font = new Font("Segoe UI", 13.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(22, 85),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Height = 30
            };

            var lblDesc = new Label
            {
                Text = descripcion,
                Font = new Font("Segoe UI", 9.75F),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(24, 120),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            card.Controls.AddRange(new Control[] { barraSuperior, lblIcono, lblTitulo, lblDesc });

            void EjecutarAccion(object? s, EventArgs e) => accion();

            card.Click += EjecutarAccion;
            lblIcono.Click += EjecutarAccion;
            lblTitulo.Click += EjecutarAccion;
            lblDesc.Click += EjecutarAccion;

            card.MouseEnter += (s, e) => card.BackColor = Color.FromArgb(248, 250, 252);
            card.MouseLeave += (s, e) => card.BackColor = Color.White;

            tlpGrilla.Controls.Add(card);
        }

        /// <summary>
        /// Embebe <paramref name="hijo"/> dentro del panel principal, con una barra
        /// de cabecera consistente (título de la sección + volver) que vive afuera
        /// del formulario hijo -- así toda pantalla tiene la misma "estructura"
        /// alrededor sin tener que tocar el .Designer.cs de cada una.
        /// </summary>
        private void AbrirFormularioEnPanel(Form hijo, string titulo)
        {
            pnlDashboard.Visible = false;

            var pnlBarraVolver = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Tema.FondoClaro,
                Name = "pnlBarraVolver"
            };

            var lblTitulo = new Label
            {
                Text = titulo,
                Font = Tema.FuenteTitulo,
                ForeColor = Tema.SlateOscuro,
                AutoSize = true,
                Location = new Point(16, 12)
            };

            var btnVolver = new Button
            {
                Text = "⬅  Volver al Inicio",
                Font = Tema.FuenteBoton,
                BackColor = Tema.SlateOscuro,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(150, 32),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = Cursors.Hand
            };
            btnVolver.FlatAppearance.BorderSize = 0;

            btnVolver.Click += (s, e) =>
            {
                hijo.Close();
                pnlContenedor.Controls.Remove(pnlBarraVolver);
                pnlContenedor.Controls.Remove(hijo);
                pnlDashboard.Visible = true;
                pnlDashboard.BringToFront();
            };

            pnlBarraVolver.Controls.Add(lblTitulo);
            pnlBarraVolver.Controls.Add(btnVolver);

            hijo.TopLevel = false;
            hijo.FormBorderStyle = FormBorderStyle.None;
            hijo.Dock = DockStyle.Fill;

            Tema.EstilizarFormulario(hijo);
            Tema.EstilizarControles(hijo);

            pnlContenedor.Controls.Add(hijo);
            pnlContenedor.Controls.Add(pnlBarraVolver);

            // Recién acá pnlBarraVolver tiene su ancho real (quedó dockeada al
            // ancho de pnlContenedor), así que el botón se ubica después de agregarla.
            btnVolver.Location = new Point(pnlBarraVolver.ClientSize.Width - btnVolver.Width - 14, 9);

            pnlBarraVolver.BringToFront();
            hijo.BringToFront();
            hijo.Show();
        }

        // Sigue acá porque frmMDIParent.Designer.cs engancha el evento del
        // ToolStripStatusLabel original, pero ese control está oculto (ver
        // ConstruirBarraSuperior): el botón "Cerrar sesión" real ahora es el de la
        // barra superior, que llama a CerrarSesion() directamente.
        private void btnCerrarSesion_Click(object sender, EventArgs e) => CerrarSesion();

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