using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace SGIG.UI
{
    /// <summary>
    /// Sistema de diseño de la aplicación: tokens de color, escala tipográfica y
    /// estilizado central de controles. Ningún formulario debería usar
    /// <c>Color.FromArgb</c> sueltos: todo sale de acá (ver Fase 9 del plan).
    /// </summary>
    public static class Tema
    {
        // ── Tokens de color ──────────────────────────────────────────────────
        public static readonly Color SidebarFondo = Color.FromArgb(15, 23, 42);        // #0F172A
        public static readonly Color SidebarTexto = Color.FromArgb(148, 163, 184);     // #94A3B8
        public static readonly Color SidebarActivoFondo = Color.FromArgb(30, 41, 59);  // #1E293B
        public static readonly Color Fondo = Color.FromArgb(248, 250, 252);            // #F8FAFC
        public static readonly Color Superficie = Color.White;                         // #FFFFFF
        public static readonly Color Borde = Color.FromArgb(226, 232, 240);            // #E2E8F0
        public static readonly Color TextoPrimario = Color.FromArgb(15, 23, 42);       // #0F172A
        public static readonly Color TextoSecundario = Color.FromArgb(100, 116, 139);  // #64748B
        public static readonly Color Acento = Color.FromArgb(37, 99, 235);             // #2563EB
        public static readonly Color AcentoHover = Color.FromArgb(29, 78, 216);        // #1D4ED8
        public static readonly Color Exito = Color.FromArgb(22, 163, 74);              // #16A34A
        public static readonly Color Peligro = Color.FromArgb(220, 38, 38);            // #DC2626
        public static readonly Color Aviso = Color.FromArgb(217, 119, 6);              // #D97706

        // Fila alterna de las grillas y hover de superficies: apenas más oscuro que Fondo.
        public static readonly Color FilaAlterna = Color.FromArgb(241, 245, 249);      // #F1F5F9

        // ── Alias de la paleta anterior (se retiran al migrar cada pantalla) ──
        public static Color SlateOscuro => TextoPrimario;
        public static Color SlateMedio => SidebarActivoFondo;
        public static Color SlateTexto => TextoSecundario;
        public static Color BordeClaro => Borde;
        public static Color FondoClaro => Fondo;
        public static Color Primario => Acento;

        // ── Tipografía ───────────────────────────────────────────────────────
        // "Segoe UI Variable" viene con Windows 11; en Windows 10 se cae a "Segoe UI".
        private static readonly string FamiliaTexto = ElegirFamilia("Segoe UI Variable Text", "Segoe UI");
        private static readonly string FamiliaTitulos = ElegirFamilia("Segoe UI Variable Display", "Segoe UI Semibold", "Segoe UI");

        public static Font Fuente(float tamanio, FontStyle estilo = FontStyle.Regular) =>
            new Font(FamiliaTexto, tamanio, estilo);

        public static readonly Font FuenteTitulo = new Font(FamiliaTitulos, 20F, FontStyle.Bold);
        public static readonly Font FuenteSubtitulo = new Font(FamiliaTexto, 12F, FontStyle.Regular);
        public static readonly Font FuenteTexto = new Font(FamiliaTexto, 10F, FontStyle.Regular);
        public static readonly Font FuenteLabel = new Font(FamiliaTexto, 9F, FontStyle.Regular);
        public static readonly Font FuenteBoton = new Font(FamiliaTexto, 10F, FontStyle.Bold);

        // ── Íconos (glifos de fuente) ────────────────────────────────────────
        private static readonly string FamiliaIconos = ElegirFamilia("Segoe Fluent Icons", "Segoe MDL2 Assets");

        /// <summary>Fuente de íconos (Segoe Fluent Icons, o Segoe MDL2 Assets en Windows 10).</summary>
        public static Font FuenteIconos(float tamanio) => new Font(FamiliaIconos, tamanio, FontStyle.Regular);

        private static string ElegirFamilia(params string[] candidatas)
        {
            using var instaladas = new InstalledFontCollection();
            foreach (var candidata in candidatas)
            {
                foreach (var familia in instaladas.Families)
                {
                    if (string.Equals(familia.Name, candidata, StringComparison.OrdinalIgnoreCase))
                        return candidata;
                }
            }
            return candidatas[^1];
        }

        // ── Métodos de estilización ─────────────────────────────────────────
        public static void EstilizarFormulario(Form form)
        {
            form.BackColor = Fondo;
            form.Font = FuenteLabel;
            EstilizarControles(form.Controls);
        }

        public static void EstilizarControles(Control contenedor)
        {
            if (contenedor == null) return;
            EstilizarControles(contenedor.Controls);
        }

        public static void EstilizarControles(Control.ControlCollection controls)
        {
            foreach (Control c in controls)
            {
                if (c is Button btn)
                {
                    EstilizarBoton(btn);
                }
                else if (c is TextBox txt)
                {
                    txt.BorderStyle = BorderStyle.FixedSingle;
                    txt.Font = FuenteTexto;
                }
                else if (c is ComboBox cbo)
                {
                    cbo.FlatStyle = FlatStyle.Flat;
                    cbo.Font = FuenteTexto;
                }
                else if (c is DataGridView dgv)
                {
                    EstilizarGrilla(dgv);
                }

                if (c.HasChildren)
                {
                    EstilizarControles(c.Controls);
                }
            }
        }

        // ── Botones ─────────────────────────────────────────────────────────
        private enum VarianteBoton { Primario, Secundario, Exito, Peligro }

        /// <summary>
        /// Elige la variante del botón por convención de nombre (notación húngara):
        /// btnEliminar/btnDarDeBaja/btnRechazar → peligro; btnCancelar/btnCerrar/btnVolver/
        /// btnLimpiar → secundario; btnGuardar/btnRegistrar*/btnConfirmar → éxito;
        /// cualquier otro → primario.
        /// </summary>
        private static VarianteBoton VarianteSegunNombre(string nombre)
        {
            string n = nombre.ToLowerInvariant();
            if (n.Contains("eliminar") || n.Contains("dardebaja") || n.Contains("rechazar"))
                return VarianteBoton.Peligro;
            if (n.Contains("cancelar") || n.Contains("cerrar") || n.Contains("volver") || n.Contains("limpiar"))
                return VarianteBoton.Secundario;
            if (n.Contains("guardar") || n.Contains("registrar") || n.Contains("confirmar"))
                return VarianteBoton.Exito;
            return VarianteBoton.Primario;
        }

        public static void EstilizarBoton(Button btn)
        {
            var variante = VarianteSegunNombre(btn.Name);

            btn.FlatStyle = FlatStyle.Flat;
            btn.Font = FuenteBoton;
            btn.Cursor = Cursors.Hand;
            btn.UseVisualStyleBackColor = false;

            if (variante == VarianteBoton.Secundario)
            {
                btn.BackColor = Superficie;
                btn.ForeColor = TextoPrimario;
                btn.FlatAppearance.BorderSize = 1;
                btn.FlatAppearance.BorderColor = Borde;
                btn.FlatAppearance.MouseOverBackColor = FilaAlterna;
                btn.FlatAppearance.MouseDownBackColor = Borde;
                return;
            }

            Color baseColor = variante switch
            {
                VarianteBoton.Peligro => Peligro,
                VarianteBoton.Exito => Exito,
                _ => Acento
            };

            btn.BackColor = baseColor;
            btn.ForeColor = Color.White;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Oscurecer(baseColor, 0.12f);
            btn.FlatAppearance.MouseDownBackColor = Oscurecer(baseColor, 0.22f);
        }

        private static Color Oscurecer(Color color, float factor) => Color.FromArgb(
            color.A,
            (int)(color.R * (1 - factor)),
            (int)(color.G * (1 - factor)),
            (int)(color.B * (1 - factor)));

        // ── Grillas ─────────────────────────────────────────────────────────
        /// <summary>
        /// Look uniforme para cualquier DataGridView: filas altas, sin líneas verticales,
        /// cabecera sin relleno, fila alterna sutil y selección en el color de acento
        /// suavizado. No toca columnas ni datos (eso lo hace <c>Grillas.Configurar</c>).
        /// </summary>
        public static void EstilizarGrilla(DataGridView dgv)
        {
            dgv.BorderStyle = BorderStyle.None;
            dgv.BackgroundColor = Superficie;
            dgv.GridColor = Borde;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.EnableHeadersVisualStyles = false;
            dgv.RowHeadersVisible = false;
            dgv.RowTemplate.Height = 36;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.ColumnHeadersHeight = 38;

            dgv.ColumnHeadersDefaultCellStyle.BackColor = Superficie;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = TextoSecundario;
            dgv.ColumnHeadersDefaultCellStyle.Font = Fuente(9F, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = Superficie;
            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = TextoSecundario;
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 0, 0);

            dgv.DefaultCellStyle.BackColor = Superficie;
            dgv.DefaultCellStyle.ForeColor = TextoPrimario;
            dgv.DefaultCellStyle.Font = FuenteTexto;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254); // #DBEAFE
            dgv.DefaultCellStyle.SelectionForeColor = TextoPrimario;
            dgv.DefaultCellStyle.Padding = new Padding(8, 0, 0, 0);

            dgv.AlternatingRowsDefaultCellStyle.BackColor = Fondo;
        }

        // ── Visualizador de contraseña (Toggle) ─────────────────────────────
        public static void AgregarToggleContrasenia(TextBox txt)
        {
            if (txt == null) return;

            if (txt.Parent == null)
            {
                txt.ParentChanged += (s, e) => ConfigurarBoton(txt);
                return;
            }

            ConfigurarBoton(txt);
        }

        private static void ConfigurarBoton(TextBox txt)
        {
            Control? parent = txt.Parent;
            if (parent == null) return;

            string btnNombre = "btnToggle_" + txt.Name;
            if (parent.Controls.ContainsKey(btnNombre)) return;

            char caracterOculto = txt.PasswordChar != '\0' ? txt.PasswordChar : '●';

            Button btnToggle = new()
            {
                Name = btnNombre,
                Text = Iconos.Ver,
                Font = FuenteIconos(10f),
                ForeColor = TextoSecundario,
                Cursor = Cursors.Hand,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(26, txt.Height - 4),
                Location = new Point(txt.Right - 28, txt.Top + 2),
                TabStop = false,
                BackColor = Superficie
            };

            btnToggle.FlatAppearance.BorderSize = 0;
            btnToggle.FlatAppearance.MouseOverBackColor = FilaAlterna;

            txt.LocationChanged += (s, e) =>
            {
                btnToggle.Location = new Point(txt.Right - 28, txt.Top + 2);
            };

            btnToggle.Click += (s, e) =>
            {
                if (txt.PasswordChar != '\0')
                {
                    txt.PasswordChar = '\0';
                    btnToggle.Text = Iconos.Ocultar;
                }
                else
                {
                    txt.PasswordChar = caracterOculto;
                    btnToggle.Text = Iconos.Ver;
                }

                txt.Focus();
                txt.SelectionStart = txt.TextLength;
            };

            parent.Controls.Add(btnToggle);
            btnToggle.BringToFront();
        }
    }

    /// <summary>
    /// Mapa centralizado de glifos de <c>Segoe Fluent Icons</c> / <c>Segoe MDL2 Assets</c>
    /// (mismos puntos de código en ambas fuentes). Se usan con <see cref="Tema.FuenteIconos"/>.
    /// Para cambiar un ícono basta con tocar el código acá.
    /// </summary>
    public static class Iconos
    {
        public const string Menu = "";
        public const string Inicio = "";
        public const string Socios = "";
        public const string Usuarios = "";
        public const string Cobro = "";
        public const string Acceso = "";
        public const string Historial = "";
        public const string Maquinas = "";
        public const string Mantenimiento = "";
        public const string Reportes = "";
        public const string Backup = "";
        public const string Configuracion = "";
        public const string Salir = "";
        public const string Buscar = "";
        public const string Ver = "";
        public const string Ocultar = "";
        public const string Aviso = "";
        public const string Ok = "";
    }

    /// <summary>
    /// Panel con esquinas redondeadas y borde de 1 px, para tarjetas y bloques de
    /// formulario. Se dibuja por código (<c>OnPaint</c>): no involucra ningún .Designer.cs.
    /// </summary>
    public class RoundedPanel : Panel
    {
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public int Radio { get; set; } = 10;
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Color ColorBorde { get; set; } = Tema.Borde;
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Color ColorFondoPanel { get; set; } = Tema.Superficie;

        public RoundedPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            BackColor = Color.Transparent;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // El fondo real de la tarjeta se pinta redondeado; las esquinas muestran
            // lo que haya detrás (el padre).
            if (Parent != null)
            {
                using var fondoPadre = new SolidBrush(Parent.BackColor);
                e.Graphics.FillRectangle(fondoPadre, ClientRectangle);
            }

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using var camino = CrearCamino(rect, Radio);
            using var relleno = new SolidBrush(ColorFondoPanel);
            using var lapiz = new Pen(ColorBorde, 1f);
            e.Graphics.FillPath(relleno, camino);
            e.Graphics.DrawPath(lapiz, camino);
        }

        private static GraphicsPath CrearCamino(Rectangle r, int radio)
        {
            int d = Math.Max(1, radio * 2);
            var p = new GraphicsPath();
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }
    }
}
