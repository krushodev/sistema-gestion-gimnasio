// Controles armados sólo por código (nunca se arrastran al diseñador): no hace falta
// configurar su serialización.
#pragma warning disable WFO1000
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SGIG.UI
{
    /// <summary>
    /// Barra lateral de navegación, construida íntegramente por código (sin .Designer.cs).
    /// Muestra una marca arriba, secciones con ítems en el medio y Configuración /
    /// Cerrar sesión fijados abajo. Se colapsa a sólo íconos (con tooltip) y se expande
    /// de nuevo: el formulario decide cuándo según el ancho de la ventana.
    /// </summary>
    public class ucSidebar : UserControl
    {
        public const int AnchoExpandido = 232;
        public const int AnchoColapsado = 64;

        private readonly ItemSidebar _marca;
        private readonly Panel _pnlNavegacion;
        private readonly Panel _pnlPie;
        private readonly ItemSidebar _itemConfiguracion;
        private readonly ItemSidebar _itemCerrarSesion;
        private readonly ToolTip _tooltip = new();
        private readonly List<Control> _filas = new();
        private readonly List<ItemSidebar> _items = new();
        private bool _colapsado;

        /// <summary>Se dispara con la clave del ítem de navegación elegido.</summary>
        public event Action<string>? ItemSeleccionado;
        public event EventHandler? ConfiguracionSolicitada;
        public event EventHandler? CerrarSesionSolicitado;
        public event EventHandler? ColapsoCambiado;

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Colapsado
        {
            get => _colapsado;
            set
            {
                if (_colapsado == value) return;
                _colapsado = value;
                Width = value ? AnchoColapsado : AnchoExpandido;
                foreach (var item in _items) item.Colapsado = value;
                _marca.Colapsado = value;
                _itemConfiguracion.Colapsado = value;
                _itemCerrarSesion.Colapsado = value;
                foreach (var fila in _filas)
                    if (fila is EncabezadoSeccion enc) enc.Colapsado = value;
                ActualizarTooltips();
                Reubicar();
                ColapsoCambiado?.Invoke(this, EventArgs.Empty);
            }
        }

        public ucSidebar()
        {
            BackColor = Tema.SidebarFondo;
            Width = AnchoExpandido;
            DoubleBuffered = true;

            _marca = new ItemSidebar("__marca", "SGIG", Iconos.Menu) { EsMarca = true, Height = 64 };
            _marca.Click += (s, e) => Colapsado = !Colapsado;
            _tooltip.SetToolTip(_marca, "Mostrar u ocultar el menú");

            _pnlNavegacion = new Panel { Dock = DockStyle.Fill, BackColor = Tema.SidebarFondo };
            _pnlNavegacion.Resize += (s, e) => Reubicar();

            _pnlPie = new Panel { Dock = DockStyle.Bottom, Height = 2 * 44 + 4 + 12, BackColor = Tema.SidebarFondo };
            _itemConfiguracion = new ItemSidebar("__config", "Configuración", Iconos.Configuracion);
            _itemCerrarSesion = new ItemSidebar("__salir", "Cerrar sesión", Iconos.Salir);
            _itemConfiguracion.Click += (s, e) => ConfiguracionSolicitada?.Invoke(this, EventArgs.Empty);
            _itemCerrarSesion.Click += (s, e) => CerrarSesionSolicitado?.Invoke(this, EventArgs.Empty);
            _pnlPie.Controls.Add(_itemConfiguracion);
            _pnlPie.Controls.Add(_itemCerrarSesion);
            _pnlPie.Resize += (s, e) => ReubicarPie();

            _marca.Dock = DockStyle.Top;

            Controls.Add(_pnlNavegacion);
            Controls.Add(_pnlPie);
            Controls.Add(_marca);
            _pnlNavegacion.BringToFront();

            ReubicarPie();
            ActualizarTooltips();
        }

        public void AgregarSeccion(string titulo)
        {
            var encabezado = new EncabezadoSeccion(titulo) { Colapsado = _colapsado };
            _filas.Add(encabezado);
            _pnlNavegacion.Controls.Add(encabezado);
            Reubicar();
        }

        public void AgregarItem(string clave, string texto, string icono)
        {
            var item = new ItemSidebar(clave, texto, icono) { Colapsado = _colapsado };
            item.Click += (s, e) => ItemSeleccionado?.Invoke(clave);
            _items.Add(item);
            _filas.Add(item);
            _pnlNavegacion.Controls.Add(item);
            ActualizarTooltips();
            Reubicar();
        }

        /// <summary>Resalta el ítem con esa clave y apaga el resto.</summary>
        public void Seleccionar(string clave)
        {
            foreach (var item in _items) item.Activo = item.Clave == clave;
        }

        private void ActualizarTooltips()
        {
            foreach (var item in _items)
                _tooltip.SetToolTip(item, _colapsado ? item.Texto : string.Empty);
            _tooltip.SetToolTip(_itemConfiguracion, _colapsado ? _itemConfiguracion.Texto : string.Empty);
            _tooltip.SetToolTip(_itemCerrarSesion, _colapsado ? _itemCerrarSesion.Texto : string.Empty);
        }

        private void Reubicar()
        {
            int y = 8;
            int ancho = _pnlNavegacion.ClientSize.Width;
            foreach (var fila in _filas)
            {
                fila.SetBounds(0, y, ancho, fila.Height);
                y += fila.Height + (fila is ItemSidebar ? 2 : 0);
            }
        }

        private void ReubicarPie()
        {
            int ancho = _pnlPie.ClientSize.Width;
            _itemConfiguracion.SetBounds(0, 6, ancho, 44);
            _itemCerrarSesion.SetBounds(0, 6 + 44 + 2, ancho, 44);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _tooltip.Dispose();
            base.Dispose(disposing);
        }
    }

    /// <summary>Título de grupo de ítems ("GESTIÓN"); colapsado se reduce a una línea fina.</summary>
    internal sealed class EncabezadoSeccion : Control
    {
        private readonly string _titulo;
        private bool _colapsado;
        private readonly Font _fuente = Tema.Fuente(8F, FontStyle.Bold);

        public bool Colapsado
        {
            get => _colapsado;
            set { _colapsado = value; Height = value ? 14 : 30; Invalidate(); }
        }

        public EncabezadoSeccion(string titulo)
        {
            _titulo = titulo.ToUpperInvariant();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            BackColor = Tema.SidebarFondo;
            Height = 30;
            TabStop = false;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Tema.SidebarFondo);
            if (_colapsado)
            {
                using var lapiz = new Pen(Tema.SidebarActivoFondo);
                e.Graphics.DrawLine(lapiz, 16, Height / 2, Width - 16, Height / 2);
                return;
            }
            TextRenderer.DrawText(e.Graphics, _titulo, _fuente,
                new Rectangle(20, 8, Width - 28, Height - 8), Tema.TextoSecundario,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _fuente.Dispose();
            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// Ítem del sidebar: ícono de línea + texto, con estado activo (barra de acento),
    /// hover y foco de teclado. Dibujado a mano para controlar el look exacto.
    /// </summary>
    internal sealed class ItemSidebar : Control
    {
        private readonly Font _fuenteIcono = Tema.FuenteIconos(12F);
        private readonly Font _fuenteTexto = Tema.Fuente(10F);
        private readonly Font _fuenteMarca = Tema.Fuente(12F, FontStyle.Bold);
        private bool _hover;
        private bool _activo;
        private bool _colapsado;

        public string Clave { get; }
        public string Icono { get; }
        public bool EsMarca { get; set; }

        public string Texto { get; }

        public bool Activo
        {
            get => _activo;
            set { if (_activo != value) { _activo = value; Invalidate(); } }
        }

        public bool Colapsado
        {
            get => _colapsado;
            set { _colapsado = value; Invalidate(); }
        }

        public ItemSidebar(string clave, string texto, string icono)
        {
            Clave = clave;
            Texto = texto;
            Icono = icono;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.Selectable, true);
            BackColor = Tema.SidebarFondo;
            Height = 44;
            Cursor = Cursors.Hand;
            TabStop = true;
        }

        protected override void OnMouseEnter(EventArgs e) { _hover = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _hover = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnGotFocus(EventArgs e) { Invalidate(); base.OnGotFocus(e); }
        protected override void OnLostFocus(EventArgs e) { Invalidate(); base.OnLostFocus(e); }

        protected override bool IsInputKey(Keys keyData) =>
            keyData is Keys.Enter or Keys.Space || base.IsInputKey(keyData);

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode is Keys.Enter or Keys.Space)
            {
                OnClick(EventArgs.Empty);
                e.Handled = true;
            }
            base.OnKeyDown(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(Tema.SidebarFondo);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var area = new Rectangle(8, 2, Width - 16, Height - 4);
            bool resaltado = !EsMarca && (_activo || _hover);

            if (resaltado)
            {
                Color fondo = _activo ? Tema.SidebarActivoFondo : Mezclar(Tema.SidebarFondo, Tema.SidebarActivoFondo);
                using var camino = CrearCamino(area, 8);
                using var pincel = new SolidBrush(fondo);
                g.FillPath(pincel, camino);
            }

            if (_activo && !EsMarca)
            {
                using var pincelAcento = new SolidBrush(Tema.Acento);
                using var barra = CrearCamino(new Rectangle(0, Height / 2 - 11, 4, 22), 2);
                g.FillPath(pincelAcento, barra);
            }

            if (Focused && ShowFocusCues)
            {
                using var lapizFoco = new Pen(Tema.Acento, 1.5f);
                using var camino = CrearCamino(new Rectangle(area.X + 1, area.Y + 1, area.Width - 2, area.Height - 2), 8);
                g.DrawPath(lapizFoco, camino);
            }

            Color colorTexto = EsMarca ? Color.White
                : _activo ? Color.White
                : _hover ? Tema.Borde
                : Tema.SidebarTexto;

            Color colorIcono = _activo ? Tema.Acento : colorTexto;
            if (EsMarca) colorIcono = Tema.SidebarTexto;

            // Caja del ícono: 44 px a la izquierda; colapsado, centrada en el ítem.
            var cajaIcono = _colapsado
                ? area
                : new Rectangle(area.X, area.Y, 44, area.Height);
            TextRenderer.DrawText(g, Icono, _fuenteIcono, cajaIcono, colorIcono,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);

            if (!_colapsado)
            {
                var cajaTexto = new Rectangle(area.X + 44, area.Y, area.Width - 44 - 6, area.Height);
                TextRenderer.DrawText(g, Texto, EsMarca ? _fuenteMarca : _fuenteTexto, cajaTexto, colorTexto,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                    TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            }
        }

        private static Color Mezclar(Color a, Color b) => Color.FromArgb(
            (a.R + b.R) / 2, (a.G + b.G) / 2, (a.B + b.B) / 2);

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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _fuenteIcono.Dispose();
                _fuenteTexto.Dispose();
                _fuenteMarca.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    /// <summary>Círculo de color con las iniciales del usuario.</summary>
    internal sealed class AvatarIniciales : Control
    {
        private readonly string _iniciales;
        private readonly Font _fuente = Tema.Fuente(10F, FontStyle.Bold);

        public AvatarIniciales(string iniciales)
        {
            _iniciales = iniciales.ToUpperInvariant();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            Size = new Size(36, 36);
            BackColor = Tema.Superficie;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(BackColor);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var pincel = new SolidBrush(Tema.Acento);
            e.Graphics.FillEllipse(pincel, 0, 0, Width - 1, Height - 1);
            TextRenderer.DrawText(e.Graphics, _iniciales, _fuente, ClientRectangle, Color.White,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _fuente.Dispose();
            base.Dispose(disposing);
        }
    }
}
