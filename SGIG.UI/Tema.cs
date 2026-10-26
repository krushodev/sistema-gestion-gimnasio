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
        public static readonly Color AcentoSuave = Color.FromArgb(147, 197, 253);      // #93C5FD (barras de gráfico)
        public static readonly Color Exito = Color.FromArgb(22, 163, 74);              // #16A34A
        public static readonly Color Peligro = Color.FromArgb(220, 38, 38);            // #DC2626
        public static readonly Color PeligroFondo = Color.FromArgb(254, 242, 242);     // #FEF2F2
        public static readonly Color ExitoFondo = Color.FromArgb(220, 252, 231);       // #DCFCE7
        public static readonly Color ExitoTexto = Color.FromArgb(21, 128, 61);         // #15803D
        public static readonly Color PeligroTexto = Color.FromArgb(185, 28, 28);       // #B91C1C
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
        // El cuerpo de texto usa "Segoe UI" (no la variante "Variable" de Windows 11): los
        // formularios con diseñador se dibujaron con sus métricas, y una fuente más ancha
        // hace que los rótulos pisen a los campos vecinos. Los títulos sí usan Variable.
        private static readonly string FamiliaTexto = "Segoe UI";
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
                else if (c is Label lbl)
                {
                    lbl.ForeColor = ColorHeredado(lbl.ForeColor);
                    NormalizarFuente(lbl);
                }
                else if (c is GroupBox grp)
                {
                    grp.ForeColor = ColorHeredado(grp.ForeColor) == grp.ForeColor && grp.ForeColor == SystemColors.ControlText
                        ? TextoPrimario
                        : ColorHeredado(grp.ForeColor);
                    NormalizarFuente(grp);
                }
                else if (c is TextBox txt)
                {
                    txt.BorderStyle = BorderStyle.FixedSingle;
                    txt.Font = FuenteTexto;
                }
                else if (c is DateTimePicker dtp)
                {
                    dtp.Format = DateTimePickerFormat.Custom;
                    dtp.CustomFormat = "dd/MM/yyyy";
                    dtp.Font = FuenteTexto;
                }
                else if (c is ComboBox cbo)
                {
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

            AjustarSolapes(controls);
        }

        /// <summary>
        /// Los rótulos de los formularios con diseñador están dimensionados para la fuente
        /// con la que se dibujaron; con otra fuente o escala crecen y tapan el campo que
        /// tienen a la derecha ("Detalle técnic|"). Para cada rótulo de ancho automático se
        /// corre el TextBox vecino de su misma fila, conservando su borde derecho.
        /// </summary>
        private static void AjustarSolapes(Control.ControlCollection controls)
        {
            foreach (Control rotulo in controls)
            {
                if (rotulo is not Label { AutoSize: true } etiqueta) continue;

                int derecha = etiqueta.Left + etiqueta.PreferredSize.Width;
                foreach (Control vecino in controls)
                {
                    if (vecino is not (TextBox or ComboBox) || vecino is TextBox { Multiline: true }) continue;
                    var campo = vecino;

                    bool mismaFila = campo.Top < etiqueta.Bottom && campo.Bottom > etiqueta.Top;
                    bool pisado = campo.Left >= etiqueta.Left && campo.Left < derecha + 6;
                    if (!mismaFila || !pisado) continue;

                    int nuevaIzquierda = derecha + 8;
                    int borde = campo.Right;
                    campo.Left = nuevaIzquierda;
                    campo.Width = Math.Max(60, borde - nuevaIzquierda);
                }
            }
        }

        /// <summary>Colores de la paleta vieja (y grises de sistema) que pasan a los tokens actuales.</summary>
        private static Color ColorHeredado(Color color)
        {
            if (color == Color.Gray || color == SystemColors.GrayText) return TextoSecundario;

            return (color.R, color.G, color.B) switch
            {
                (26, 32, 44) or (45, 55, 72) => TextoPrimario,
                (74, 85, 104) => TextoSecundario,
                (0, 100, 0) => ExitoTexto,
                _ => color
            };
        }

        /// <summary>
        /// "Segoe UI Semibold" y los tamaños 9,75/15,75 vienen de diseñadores viejos: se llevan a
        /// "Segoe UI" (negrita si era semibold) y a 10/16 pt, para que haya una sola escala.
        /// </summary>
        private static void NormalizarFuente(Control control)
        {
            var f = control.Font;
            bool semibold = f.Name == "Segoe UI Semibold";
            float tamanio = f.SizeInPoints switch { 9.75F => 10F, 15.75F => 16F, 9.5F when control is GroupBox => 9F, var t => t };
            if (!semibold && tamanio == f.SizeInPoints) return;

            var estilo = semibold && control is Label ? FontStyle.Bold : semibold ? FontStyle.Regular : f.Style;
            control.Font = new Font("Segoe UI", tamanio, estilo);
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
            if (n.Contains("cancelar") || n.Contains("cerrar") || n.Contains("volver") || n.Contains("limpiar")
                || n.Contains("pornombre"))
                return VarianteBoton.Secundario;
            if (n.Contains("guardar") || n.Contains("registrar") || n.Contains("confirmar"))
                return VarianteBoton.Exito;
            return VarianteBoton.Primario;
        }

        public static void EstilizarBoton(Button btn)
        {
            // El ojo de mostrar/ocultar contraseña ya viene estilizado (fuente de íconos): si se lo
            // tratara como un botón de formulario quedaría azul y con un cuadradito en vez del glifo.
            if (btn.Name.StartsWith("btnToggle_", StringComparison.Ordinal)) return;

            var variante = VarianteSegunNombre(btn.Name);
            btn.Text = SinEmojiInicial(btn.Text);

            btn.FlatStyle = FlatStyle.Flat;
            UniformarAltura(btn);
            btn.Font = AjustarFuenteBoton(btn);
            btn.Cursor = Cursors.Hand;
            btn.UseVisualStyleBackColor = false;

            AplicarColores(btn, variante);

            // Habilitado/deshabilitado cambia en tiempo de ejecución (p. ej. "Registrar pago" hasta
            // que se elige un socio): los colores se recalculan para que el gris sea siempre el mismo.
            if (_botonesSuscritos.TryGetValue(btn, out _)) return;
            _botonesSuscritos.Add(btn, new object());
            btn.EnabledChanged += (s, e) => AplicarColores(btn, VarianteSegunNombre(btn.Name));
        }

        private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<Button, object> _botonesSuscritos = new();

        private static void AplicarColores(Button btn, VarianteBoton variante)
        {
            if (!btn.Enabled)
            {
                btn.BackColor = FilaAlterna;
                btn.ForeColor = Color.FromArgb(148, 163, 184); // #94A3B8 (mismo gris que el texto del sidebar)
                btn.FlatAppearance.BorderSize = 1;
                btn.FlatAppearance.BorderColor = Borde;
                return;
            }

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

        /// <summary>
        /// Quita el emoji decorativo con el que arrancan algunos textos de botón de los
        /// diseñadores viejos ("💾 Crear copia" → "Crear copia"); los íconos ahora salen de
        /// <see cref="Iconos"/>. Sólo recorta símbolos/emojis iniciales, nunca letras ni "+".
        /// </summary>
        private static string SinEmojiInicial(string texto)
        {
            int i = 0;
            while (i < texto.Length)
            {
                char c = texto[i];
                bool simbolo = char.IsSurrogate(c) || c == ' ' || c == '\uFE0F'
                    || (c >= '\u2190' && c <= '\u21FF') || (c >= '\u2300' && c <= '\u23FF')
                    || (c >= '\u2600' && c <= '\u27BF') || (c >= '\u2B00' && c <= '\u2BFF');
                if (!simbolo) break;
                i++;
            }
            return i == 0 || i >= texto.Length ? texto : texto[i..];
        }

        /// <summary>Altura estándar de los botones de formulario (los de acción grande —login, cobro, backup— quedan como están).</summary>
        public const int AlturaBoton = 36;

        /// <summary>
        /// Los diseñadores dejaron botones de 25 a 43 px de alto según la pantalla. Se unifican a
        /// <see cref="AlturaBoton"/> sin mover el control: si está anclado abajo conserva su borde
        /// inferior; si no, crece/achica parejo desde el centro.
        /// </summary>
        private static void UniformarAltura(Button btn)
        {
            if (btn.Dock != DockStyle.None || btn.Height == AlturaBoton || btn.Height < 24 || btn.Height > 44) return;

            int diferencia = btn.Height - AlturaBoton;
            bool anclaAbajo = btn.Anchor.HasFlag(AnchorStyles.Bottom) && !btn.Anchor.HasFlag(AnchorStyles.Top);
            btn.Top += anclaAbajo ? diferencia : diferencia / 2;
            btn.Height = AlturaBoton;
        }

        /// <summary>
        /// La fuente estándar del botón; si el texto no entra en el ancho que le dio el
        /// diseñador, baja de a 0,5 pt (hasta 8 pt) en vez de dejarlo recortado.
        /// </summary>
        private static Font AjustarFuenteBoton(Button btn)
        {
            if (string.IsNullOrEmpty(btn.Text) || btn.Width <= 0) return FuenteBoton;

            int disponible = btn.Width - 12;
            foreach (var (estilo, desde, hasta) in new[]
            {
                (FontStyle.Bold, 10F, 8.5F),
                (FontStyle.Regular, 9F, 8F)
            })
            {
                for (float tamanio = desde; tamanio >= hasta; tamanio -= 0.5F)
                {
                    using var prueba = new Font(FuenteBoton.FontFamily, tamanio, estilo);
                    if (TextRenderer.MeasureText(btn.Text, prueba).Width <= disponible)
                        return estilo == FontStyle.Bold && tamanio == 10F
                            ? FuenteBoton
                            : new Font(FuenteBoton.FontFamily, tamanio, estilo);
                }
            }
            return new Font(FuenteBoton.FontFamily, 8F, FontStyle.Regular);
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
            dgv.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;

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
                Size = new Size(26, txt.Height),
                TabStop = false,
                BackColor = Superficie
            };

            // El botón ocupa el extremo derecho del espacio que tenía el campo: se le quitan 30 px
            // al TextBox y el ojo va a su lado. Superponerlo sobre el TextBox no es confiable (son
            // dos controles nativos y el orden de dibujo no está garantizado).
            const int espacio = 30;
            txt.Width -= espacio;
            btnToggle.Location = new Point(txt.Right + 4, txt.Top);

            btnToggle.FlatAppearance.BorderSize = 0;
            btnToggle.FlatAppearance.MouseOverBackColor = FilaAlterna;

            txt.LocationChanged += (s, e) => btnToggle.Location = new Point(txt.Right + 4, txt.Top);

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
        public const string Menu = "\uE700";
        public const string Inicio = "\uE80F";
        public const string Socios = "\uE716";
        public const string Usuarios = "\uE77B";
        public const string Cobro = "\uE8C7";
        public const string Acceso = "\uE8FB";
        public const string Historial = "\uE81C";
        public const string Maquinas = "\uE7F8";
        public const string Mantenimiento = "\uE90F";
        public const string Planes = "\uE8A5";
        public const string Reportes = "\uE9D9";
        public const string Backup = "\uE74E";
        public const string Configuracion = "\uE713";
        public const string Salir = "\uE7E8";
        public const string Buscar = "\uE721";
        public const string Ver = "\uE7B3";
        public const string Ocultar = "\uED1A";
        public const string Aviso = "\uE7BA";
        public const string Ok = "\uE73E";
        public const string Candado = "\uE72E";
        public const string Cancelar = "\uE711";
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
