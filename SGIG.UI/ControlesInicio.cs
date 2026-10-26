// Controles armados sólo por código (nunca se arrastran al diseñador).
#pragma warning disable WFO1000
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using SGIG.Entidades;

namespace SGIG.UI
{
    /// <summary>
    /// Aviso del bloque "Para atender hoy" (Fase 13): pastilla de color con ícono y texto,
    /// dibujada con GDI+. El color sale del nivel (rojo peligro, naranja aviso, azul info,
    /// verde ok); si recibe una acción es clickeable y muestra una flecha a la derecha.
    /// </summary>
    internal sealed class ChipAviso : Control
    {
        private const int Alto = 36;
        private const int MargenInterno = 14;
        private const int AnchoIcono = 20;
        private const int AnchoFlecha = 18;

        private readonly Color _fondo;
        private readonly Color _colorTexto;
        private readonly string _icono;
        private readonly bool _clickeable;
        private readonly Font _fuenteTexto = Tema.Fuente(9.5F, FontStyle.Bold);
        private readonly Font _fuenteIcono = Tema.FuenteIconos(11F);
        private bool _resaltado;

        public ChipAviso(AvisoDTO aviso, Action? alHacerClick)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);

            (_fondo, _colorTexto, _icono) = aviso.Nivel switch
            {
                "Peligro" => (Tema.PeligroFondo, Tema.PeligroTexto, Iconos.Aviso),
                "Aviso" => (Tema.AvisoFondo, Tema.AvisoTexto, Iconos.Historial),
                "Ok" => (Tema.ExitoFondo, Tema.ExitoTexto, Iconos.Ok),
                _ => (Tema.AcentoFondo, Tema.AcentoHover, Iconos.Socios)
            };
            Text = aviso.Texto;
            _clickeable = alHacerClick != null;

            int anchoTexto = TextRenderer.MeasureText(Text, _fuenteTexto).Width;
            Size = new Size(MargenInterno + AnchoIcono + 6 + anchoTexto + (_clickeable ? AnchoFlecha : 0) + MargenInterno, Alto);
            Margin = new Padding(0, 0, 10, 10);

            if (!_clickeable) return;
            Cursor = Cursors.Hand;
            Click += (s, e) => alHacerClick!();
            MouseEnter += (s, e) => { _resaltado = true; Invalidate(); };
            MouseLeave += (s, e) => { _resaltado = false; Invalidate(); };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(Parent?.BackColor ?? Tema.Fondo);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using (var camino = Redondeado(rect, 8))
            using (var relleno = new SolidBrush(_fondo))
            {
                g.FillPath(relleno, camino);
                if (_resaltado)
                {
                    using var borde = new Pen(_colorTexto);
                    g.DrawPath(borde, camino);
                }
            }

            const TextFormatFlags centrado = TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix;
            TextRenderer.DrawText(g, _icono, _fuenteIcono,
                new Rectangle(MargenInterno, 0, AnchoIcono, Height), _colorTexto, centrado | TextFormatFlags.HorizontalCenter);
            TextRenderer.DrawText(g, Text, _fuenteTexto,
                new Rectangle(MargenInterno + AnchoIcono + 6, 0, Width, Height), _colorTexto, centrado);
            if (_clickeable)
            {
                TextRenderer.DrawText(g, "›", _fuenteTexto,
                    new Rectangle(Width - MargenInterno - AnchoFlecha + 6, 0, AnchoFlecha, Height), _colorTexto, centrado);
            }
        }

        private static GraphicsPath Redondeado(Rectangle r, int radio)
        {
            int d = radio * 2;
            var p = new GraphicsPath();
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _fuenteTexto.Dispose();
                _fuenteIcono.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// Panel que acomoda sus hijos de izquierda a derecha, pasando a la fila siguiente
    /// cuando no entran, y ajusta su alto al contenido. Posiciona a mano por el mismo
    /// motivo que <see cref="PanelTarjetas"/> (el FlowLayoutPanel falla si se carga oculto).
    /// </summary>
    internal sealed class PanelFlujo : Panel
    {
        public PanelFlujo()
        {
            Dock = DockStyle.Top;
            BackColor = Tema.Fondo;
            Height = 46;
        }

        public void Reacomodar()
        {
            int x = 0, y = 0, altoFila = 0;
            foreach (Control c in Controls)
            {
                if (x > 0 && x + c.Width > ClientSize.Width)
                {
                    x = 0;
                    y += altoFila;
                    altoFila = 0;
                }
                c.Location = new Point(x, y);
                x += c.Width + c.Margin.Right;
                altoFila = Math.Max(altoFila, c.Height + c.Margin.Bottom);
            }
            int alto = y + altoFila;
            if (Controls.Count > 0 && Height != alto) Height = alto;
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            Reacomodar();
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            Reacomodar();
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);
            Reacomodar();
        }
    }

    /// <summary>
    /// Lista del Inicio dentro de una tarjeta blanca con título. Si no hay filas, en vez
    /// de la grilla en blanco muestra un ícono gris y un texto (estado vacío).
    /// </summary>
    internal sealed class BloqueLista : RoundedPanel
    {
        private readonly Panel _pnlVacio;
        private readonly Font _fuenteTitulo = Tema.Fuente(11F, FontStyle.Bold);
        private readonly Font _fuenteIconoVacio = Tema.FuenteIconos(26F);
        private readonly Font _fuenteTextoVacio = Tema.Fuente(10F);

        public DataGridView Grilla { get; }

        public BloqueLista(string titulo, string textoVacio, string iconoVacio)
        {
            Dock = DockStyle.Fill;
            Padding = new Padding(12, 48, 12, 12);
            BackColor = Tema.Fondo;

            Grilla = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };
            Tema.EstilizarGrilla(Grilla);
            // Es una lista de consulta: sin la primera fila marcada en azul al cargar.
            Grilla.DataBindingComplete += (s, e) => Grilla.ClearSelection();

            _pnlVacio = new Panel { Dock = DockStyle.Fill, BackColor = Tema.Superficie, Visible = false };
            var lblIcono = new Label
            {
                Text = iconoVacio,
                Font = _fuenteIconoVacio,
                ForeColor = Tema.SidebarTexto,
                AutoSize = true
            };
            var lblTexto = new Label
            {
                Text = textoVacio,
                Font = _fuenteTextoVacio,
                ForeColor = Tema.TextoSecundario,
                AutoSize = true
            };
            _pnlVacio.Controls.AddRange(new Control[] { lblIcono, lblTexto });
            void Centrar()
            {
                int alto = lblIcono.Height + 8 + lblTexto.Height;
                int top = Math.Max(0, (_pnlVacio.ClientSize.Height - alto) / 2);
                lblIcono.Location = new Point((_pnlVacio.ClientSize.Width - lblIcono.Width) / 2, top);
                lblTexto.Location = new Point((_pnlVacio.ClientSize.Width - lblTexto.Width) / 2, top + lblIcono.Height + 8);
            }
            _pnlVacio.Resize += (s, e) => Centrar();
            _pnlVacio.VisibleChanged += (s, e) => Centrar();

            Controls.Add(Grilla);
            Controls.Add(_pnlVacio);
            Controls.Add(new Label
            {
                Text = titulo,
                Font = _fuenteTitulo,
                ForeColor = Tema.TextoPrimario,
                AutoSize = true,
                Location = new Point(16, 16),
                BackColor = Color.Transparent
            });
        }

        /// <summary>Carga las filas; si no hay ninguna, muestra el estado vacío.</summary>
        public void Mostrar(IList filas)
        {
            Grilla.DataSource = filas;
            bool vacio = filas.Count == 0;
            Grilla.Visible = !vacio;
            _pnlVacio.Visible = vacio;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _fuenteTitulo.Dispose();
                _fuenteIconoVacio.Dispose();
                _fuenteTextoVacio.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
