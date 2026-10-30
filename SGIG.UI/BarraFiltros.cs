// Controles armados sólo por código (nunca se arrastran al diseñador).
#pragma warning disable WFO1000
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SGIG.UI
{
    /// <summary>
    /// Barra de filtros de los listados (Fase 14: Socios, Usuarios y Máquinas). Se arma por
    /// código encima de la grilla del diseñador, sin tocar el .Designer.cs: una fila de
    /// rótulos y combos que pasa a dos filas si no entra, y la grilla se corre para abajo
    /// para dejarle lugar (conservando su margen inferior, donde están los botones).
    /// </summary>
    internal static class BarraFiltros
    {
        /// <param name="grilla">Grilla del formulario, anclada arriba/abajo/izquierda/derecha.</param>
        /// <param name="margenInferior">Espacio libre bajo la grilla; null = el que tiene en el diseñador.</param>
        /// <param name="controles">Rótulos y campos, en orden de izquierda a derecha.</param>
        public static FlowLayoutPanel Crear(Control grilla, int? margenInferior, params Control[] controles)
        {
            var form = grilla.Parent ?? throw new InvalidOperationException("La grilla tiene que estar dentro del formulario.");
            int margen = margenInferior ?? form.ClientSize.Height - grilla.Bottom;

            var barra = new FlowLayoutPanel
            {
                Name = "flpFiltros",
                Location = new Point(grilla.Left, 8),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = true,
                BackColor = Color.Transparent
            };
            foreach (var c in controles)
            {
                c.Parent?.Controls.Remove(c); // txtBuscar/lblBuscar vienen del diseñador
                barra.Controls.Add(c);
            }
            form.Controls.Add(barra);

            void Reubicar()
            {
                barra.MaximumSize = new Size(Math.Max(200, form.ClientSize.Width - barra.Left * 2), 0);
                int arriba = barra.Bottom + 8;
                grilla.SetBounds(grilla.Left, arriba, grilla.Width, Math.Max(60, form.ClientSize.Height - margen - arriba));
            }
            barra.SizeChanged += (s, e) => Reubicar();
            form.Resize += (s, e) => Reubicar();
            Reubicar();
            return barra;
        }

        public static Label Rotulo(string texto, int margenIzquierdo = 16) => new()
        {
            Text = texto,
            AutoSize = true,
            Font = Tema.Fuente(9F, FontStyle.Bold),
            ForeColor = Tema.TextoSecundario,
            Margin = new Padding(margenIzquierdo, 9, 6, 0)
        };

        public static ComboBox Combo(string nombre, int ancho, params string[] opciones)
        {
            var combo = new ComboBox
            {
                Name = nombre,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = ancho,
                Margin = new Padding(0, 4, 0, 0)
            };
            combo.Items.AddRange(opciones);
            if (opciones.Length > 0) combo.SelectedIndex = 0;
            return combo;
        }

        /// <summary>"Mostrando 5 de 12 socios", al final de la barra.</summary>
        public static Label Resultado(string nombre) => new()
        {
            Name = nombre,
            AutoSize = true,
            Font = Tema.Fuente(9F),
            ForeColor = Tema.TextoSecundario,
            Margin = new Padding(16, 9, 0, 0)
        };

        /// <summary>Margen de la caja de búsqueda del diseñador al moverla a la barra.</summary>
        public static Control ComoCampo(Control campo)
        {
            campo.Margin = new Padding(0, 5, 0, 0);
            return campo;
        }
    }
}
