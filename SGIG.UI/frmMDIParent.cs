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

        public frmMDIParent(Usuario usuario)
        {
            InitializeComponent();
            _usuario = usuario ?? throw new ArgumentNullException(nameof(usuario));
        }

        private void frmMDIParent_Load(object sender, EventArgs e)
        {
            lblUsuarioLogueado.Text = $"👤  {_usuario.Nombre} {_usuario.Apellido}   |   Rol: {_usuario.Rol?.NombreRol}";

            ConstruirDashboard();
            CargarTarjetasSegunRol(_usuario.Rol?.NombreRol);
        }

        private void ConstruirDashboard()
        {
            // Barra inferior
            stsEstado.BackColor = Color.FromArgb(15, 23, 42);
            stsEstado.ForeColor = Color.FromArgb(226, 232, 240);
            stsEstado.Font = new Font("Segoe UI", 9.5F);
            stsEstado.Padding = new Padding(16, 6, 16, 6);

            btnCerrarSesion.ForeColor = Color.FromArgb(248, 113, 113);
            btnCerrarSesion.LinkColor = Color.FromArgb(248, 113, 113);

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

            // 4 columnas iguales (25% cada una)
            tlpGrilla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            tlpGrilla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            tlpGrilla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            tlpGrilla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));

            // 2 filas iguales (50% cada una)
            tlpGrilla.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
            tlpGrilla.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));

            pnlDashboard.Controls.Add(tlpGrilla);
            pnlDashboard.Controls.Add(pnlHeader);

            pnlContenedor.Controls.Add(pnlDashboard);
        }

        private void CargarTarjetasSegunRol(string? rol)
        {
            tlpGrilla.Controls.Clear();

            // 1. 👥 Socios
            if (rol is "Administrador" or "Recepcionista")
            {
                AgregarTarjetaGrilla(
                    "Gestión de Socios",
                    "Alta, modificación, bajas, fichas médicas y listado general de alumnos.",
                    "👥",
                    Color.FromArgb(37, 99, 235),
                    () => AbrirFormularioEnPanel(new frmSocios())
                );
            }

            // 2. 🛡️ Usuarios
            if (rol == "Administrador")
            {
                AgregarTarjetaGrilla(
                    "Seguridad y Usuarios",
                    "Gestión de cuentas del personal, roles, permisos y credenciales.",
                    "🛡️",
                    Color.FromArgb(79, 70, 229),
                    () => AbrirFormularioEnPanel(new frmUsuarios(_usuario))
                );

                // 3. ⚙️ Tablas Paramétricas
                AgregarTarjetaGrilla(
                    "Tablas Paramétricas",
                    "Catálogo base de localidades, provincias, tipos de documento y planes.",
                    "⚙️",
                    Color.FromArgb(71, 85, 105),
                    () => AbrirFormularioEnPanel(new frmTablasParametricas())
                );
            }

            // 4. 💳 Planes y Cobros
            if (rol is "Administrador" or "Recepcionista")
            {
                AgregarTarjetaGrilla(
                    "Planes y Cobros",
                    "Facturación, cobro de aranceles mensuales y emisión de comprobantes.",
                    "💳",
                    Color.FromArgb(16, 185, 129),
                    () => MessageBox.Show("Módulo de Cobros en desarrollo.", "SGIG", MessageBoxButtons.OK, MessageBoxIcon.Information)
                );

                // 5. ⏱️ Control de Acceso
                AgregarTarjetaGrilla(
                    "Control de Acceso",
                    "Monitoreo de entradas en recepción, molinete y estado de cuotas al día.",
                    "⏱️",
                    Color.FromArgb(245, 158, 11),
                    () => MessageBox.Show("Módulo de Check-in en desarrollo.", "SGIG", MessageBoxButtons.OK, MessageBoxIcon.Information)
                );
            }

            // 6. 🏋️ Activos y Máquinas
            if (rol is "Administrador" or "Tecnico")
            {
                AgregarTarjetaGrilla(
                    "Activos y Máquinas",
                    "Inventario de equipamiento, registro de fallas y mantenimiento preventivo.",
                    "🏋️",
                    Color.FromArgb(236, 72, 153),
                    () => MessageBox.Show("Módulo de Máquinas en desarrollo.", "SGIG", MessageBoxButtons.OK, MessageBoxIcon.Information)
                );
            }

            // 7. 📊 Reportes
            if (rol == "Administrador")
            {
                AgregarTarjetaGrilla(
                    "Reportes e Ingresos",
                    "Métricas de concurrencia, balance financiero, altas y bajas periódicas.",
                    "📊",
                    Color.FromArgb(14, 165, 233),
                    () => MessageBox.Show("Módulo de Reportes en desarrollo.", "SGIG", MessageBoxButtons.OK, MessageBoxIcon.Information)
                );

                // 8. 💾 Copias de Seguridad
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

            // Eventos de clic e interactividad
            void EjecutarAccion(object? s, EventArgs e) => accion();

            card.Click += EjecutarAccion;
            lblIcono.Click += EjecutarAccion;
            lblTitulo.Click += EjecutarAccion;
            lblDesc.Click += EjecutarAccion;

            card.MouseEnter += (s, e) => card.BackColor = Color.FromArgb(248, 250, 252);
            card.MouseLeave += (s, e) => card.BackColor = Color.White;

            tlpGrilla.Controls.Add(card);
        }

        private void AbrirFormularioEnPanel(Form hijo)
        {
            pnlDashboard.Visible = false;

            var pnlBarraVolver = new Panel
            {
                Dock = DockStyle.Top,
                Height = 46,
                BackColor = Color.FromArgb(226, 232, 240),
                Name = "pnlBarraVolver"
            };

            var btnVolver = new Button
            {
                Text = "⬅  Volver al Inicio",
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                BackColor = Color.FromArgb(15, 23, 42),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(150, 32),
                Location = new Point(14, 7),
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

            pnlBarraVolver.Controls.Add(btnVolver);

            hijo.TopLevel = false;
            hijo.FormBorderStyle = FormBorderStyle.None;
            hijo.Dock = DockStyle.Fill;

            pnlContenedor.Controls.Add(hijo);
            pnlContenedor.Controls.Add(pnlBarraVolver);

            pnlBarraVolver.BringToFront();
            hijo.BringToFront();
            hijo.Show();
        }

        private void btnCerrarSesion_Click(object sender, EventArgs e)
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