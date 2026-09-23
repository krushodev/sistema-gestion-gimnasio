using System;
using System.Drawing;
using System.Windows.Forms;

namespace SGIG.UI
{
    public static class Tema
    {
        // ── Paleta de colores ────────────────────────────────────────────────
        public static readonly Color SlateOscuro = Color.FromArgb(26, 32, 44);
        public static readonly Color SlateMedio = Color.FromArgb(45, 55, 72);
        public static readonly Color SlateTexto = Color.FromArgb(74, 85, 104);
        public static readonly Color BordeClaro = Color.FromArgb(226, 232, 240);
        public static readonly Color FondoClaro = Color.FromArgb(247, 250, 252);
        public static readonly Color Primario = Color.FromArgb(49, 130, 206);
        public static readonly Color Peligro = Color.FromArgb(229, 62, 62);
        public static readonly Color Exito = Color.FromArgb(56, 161, 105);

        // ── Fuentes estándar ────────────────────────────────────────────────
        public static readonly Font FuenteTitulo = new Font("Segoe UI Semibold", 15.75F, FontStyle.Bold);
        public static readonly Font FuenteSubtitulo = new Font("Segoe UI", 9.75F, FontStyle.Regular);
        public static readonly Font FuenteLabel = new Font("Segoe UI", 9F, FontStyle.Regular);
        public static readonly Font FuenteTexto = new Font("Segoe UI", 10F, FontStyle.Regular);
        public static readonly Font FuenteBoton = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);

        // ── Métodos de estilización ─────────────────────────────────────────
        public static void EstilizarFormulario(Form form)
        {
            form.BackColor = FondoClaro;
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
                if (c is Button btn && btn.BackColor != Peligro && btn.BackColor != Exito)
                {
                    btn.BackColor = Primario;
                    btn.ForeColor = Color.White;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                    btn.Font = FuenteBoton;
                    btn.Cursor = Cursors.Hand;
                }
                else if (c is TextBox txt)
                {
                    txt.BorderStyle = BorderStyle.FixedSingle;
                    txt.Font = FuenteTexto;
                }
                else if (c.HasChildren)
                {
                    EstilizarControles(c.Controls);
                }
            }
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

            // Carácter configurado en tu designer ('●')
            char caracterOculto = txt.PasswordChar != '\0' ? txt.PasswordChar : '●';

            Button btnToggle = new()
            {
                Name = btnNombre,
                Text = "👁",
                Font = new Font("Segoe UI", 9f, FontStyle.Regular),
                Cursor = Cursors.Hand,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(26, txt.Height - 4),
                Location = new Point(txt.Right - 28, txt.Top + 2),
                TabStop = false,
                BackColor = Color.White
            };

            btnToggle.FlatAppearance.BorderSize = 0;
            btnToggle.FlatAppearance.MouseOverBackColor = Color.FromArgb(230, 230, 230);

            txt.LocationChanged += (s, e) =>
            {
                btnToggle.Location = new Point(txt.Right - 28, txt.Top + 2);
            };

            btnToggle.Click += (s, e) =>
            {
                if (txt.PasswordChar != '\0')
                {
                    // Muestra el texto plano
                    txt.PasswordChar = '\0';
                    btnToggle.Text = "🙈";
                }
                else
                {
                    // Oculta con el círculo '●' original
                    txt.PasswordChar = caracterOculto;
                    btnToggle.Text = "👁";
                }

                txt.Focus();
                txt.SelectionStart = txt.TextLength;
            };

            parent.Controls.Add(btnToggle);
            btnToggle.BringToFront();
        }
    }
}