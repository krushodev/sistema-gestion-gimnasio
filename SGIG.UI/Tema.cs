using System.Drawing;
using System.Windows.Forms;

namespace SGIG.UI;

/// <summary>
/// Paleta y helpers visuales compartidos, extraídos de los colores/fuentes que ya
/// usaban frmLogin y frmMDIParent. Centraliza el estilo para que todas las
/// pantallas (ABM, editores modales) se vean como una sola aplicación, sin tener
/// que reescribir cada .Designer.cs (ver docs/patrones/estilo-visual.md).
/// </summary>
public static class Tema
{
    public static readonly Color SlateOscuro = Color.FromArgb(15, 23, 42);
    public static readonly Color SlateMedio = Color.FromArgb(71, 85, 105);
    public static readonly Color SlateTexto = Color.FromArgb(100, 116, 139);
    public static readonly Color FondoPrincipal = Color.FromArgb(241, 245, 249);
    public static readonly Color FondoClaro = Color.FromArgb(247, 250, 252);
    public static readonly Color Primario = Color.FromArgb(37, 99, 235);
    public static readonly Color Exito = Color.FromArgb(5, 150, 105);
    public static readonly Color Peligro = Color.FromArgb(220, 38, 38);
    public static readonly Color Advertencia = Color.FromArgb(217, 119, 6);

    public static readonly Font FuenteTitulo = new("Segoe UI Semibold", 14F, FontStyle.Bold);
    public static readonly Font FuenteBoton = new("Segoe UI Semibold", 9.5F, FontStyle.Bold);

    /// <summary>Fondo del formulario. No toca Size/Location de ningún control.</summary>
    public static void EstilizarFormulario(Form f)
    {
        f.BackColor = FondoPrincipal;
    }

    /// <summary>
    /// Recorre el árbol de controles y aplica color/estilo "de pintura" (sin tocar
    /// Size, Location, Anchor, Dock ni Font de controles preexistentes) para no
    /// romper layouts calculados por el diseñador de Visual Studio.
    /// </summary>
    public static void EstilizarControles(Control raiz)
    {
        foreach (Control control in raiz.Controls)
        {
            switch (control)
            {
                case Button boton:
                    EstilizarBoton(boton);
                    break;
                case DataGridView grilla:
                    EstilizarGrilla(grilla);
                    break;
            }

            if (control.HasChildren)
            {
                EstilizarControles(control);
            }
        }
    }

    private static void EstilizarBoton(Button boton)
    {
        boton.FlatStyle = FlatStyle.Flat;
        boton.FlatAppearance.BorderSize = 0;
        boton.Cursor = Cursors.Hand;
        boton.ForeColor = Color.White;
        boton.BackColor = ColorSegunNombre(boton.Name);
    }

    private static Color ColorSegunNombre(string nombreControl)
    {
        string nombre = nombreControl.ToLowerInvariant();

        if (nombre.Contains("eliminar") || nombre.Contains("baja") || nombre.Contains("rechaz"))
        {
            return Peligro;
        }

        if (nombre.Contains("cancelar") || nombre.Contains("cerrar") || nombre.Contains("volver"))
        {
            return SlateMedio;
        }

        if (nombre.Contains("registrar") || nombre.Contains("guardar") || nombre.Contains("pago") || nombre.Contains("confirmar"))
        {
            return Exito;
        }

        return Primario;
    }

    /// <summary>
    /// Agrega un botón "👁"/"🙈" pegado al borde derecho de <paramref name="caja"/>
    /// que alterna <see cref="TextBox.UseSystemPasswordChar"/> para mostrar/ocultar
    /// la contraseña que se está tipeando. Se agrega al mismo padre que
    /// <paramref name="caja"/> -- nunca requiere tocar el .Designer.cs del
    /// formulario que la contiene.
    /// </summary>
    public static Button AgregarToggleContrasenia(TextBox caja)
    {
        var boton = new Button
        {
            Text = "👁",
            Font = new Font("Segoe UI", 8F),
            Size = new Size(26, caja.Height),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            TabStop = false,
            Anchor = caja.Anchor
        };
        boton.FlatAppearance.BorderSize = 1;

        void Reposicionar() =>
            boton.Location = new Point(caja.Right + 4, caja.Top);

        Reposicionar();
        caja.LocationChanged += (s, e) => Reposicionar();

        boton.Click += (s, e) =>
        {
            caja.UseSystemPasswordChar = !caja.UseSystemPasswordChar;
            boton.Text = caja.UseSystemPasswordChar ? "👁" : "🙈";
        };

        caja.Parent!.Controls.Add(boton);
        boton.BringToFront();
        return boton;
    }

    private static void EstilizarGrilla(DataGridView grilla)
    {
        grilla.EnableHeadersVisualStyles = false;
        grilla.BorderStyle = BorderStyle.None;
        grilla.BackgroundColor = Color.White;
        grilla.GridColor = Color.FromArgb(226, 232, 240);

        grilla.ColumnHeadersDefaultCellStyle.BackColor = SlateOscuro;
        grilla.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        grilla.ColumnHeadersDefaultCellStyle.SelectionBackColor = SlateOscuro;
        grilla.ColumnHeadersHeight = 34;

        grilla.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
        grilla.DefaultCellStyle.SelectionForeColor = SlateOscuro;
        grilla.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
        grilla.RowTemplate.Height = 30;
    }
}
