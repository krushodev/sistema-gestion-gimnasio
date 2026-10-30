// Control armado sólo por código (nunca se arrastra al diseñador).
#pragma warning disable WFO1000
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SGIG.Entidades;

namespace SGIG.UI
{
    /// <summary>
    /// Tarjeta con un gráfico de barras dibujado con GDI+ (el <c>Chart</c> de .NET Framework no
    /// existe en .NET moderno y no se agregan paquetes). Columnas verticales o barras
    /// horizontales; la de mayor valor va en <see cref="Tema.Acento"/> y el resto en
    /// <see cref="Tema.AcentoSuave"/>. Al pasar el mouse muestra el valor exacto.
    /// </summary>
    internal sealed class TarjetaGrafico : RoundedPanel
    {
        public enum Orientacion { Columnas, Barras }

        private const int Margen = 20;
        private const int AltoEncabezado = 72; // título, subtítulo y línea de resumen

        private readonly string _titulo;
        private string _subtitulo;
        private string? _resumen;
        private readonly Orientacion _orientacion;
        private readonly Func<decimal, string> _formato;
        private readonly ToolTip _tooltip = new();
        private readonly Font _fuenteTitulo = Tema.Fuente(11F, FontStyle.Bold);
        private readonly Font _fuenteSubtitulo = Tema.Fuente(9F);
        private readonly Font _fuenteEje = Tema.Fuente(8.5F);
        private readonly Font _fuenteValor = Tema.Fuente(8.5F, FontStyle.Bold);

        private List<PuntoGraficoDTO> _puntos = new();
        private string? _mensaje = "Cargando…";
        private readonly List<(Rectangle Zona, PuntoGraficoDTO Punto)> _zonas = new();
        private PuntoGraficoDTO? _puntoBajoMouse;

        public TarjetaGrafico(string titulo, string subtitulo, Orientacion orientacion, Func<decimal, string> formato)
        {
            _titulo = titulo;
            _subtitulo = subtitulo;
            _orientacion = orientacion;
            _formato = formato;
            Dock = DockStyle.Fill;
        }

        /// <summary>Texto bajo el título (por ejemplo, el período que se está mostrando).</summary>
        public string Subtitulo
        {
            get => _subtitulo;
            set { _subtitulo = value; Invalidate(); }
        }

        /// <summary>
        /// Muestra los datos (si todos valen 0, el estado vacío) y, opcionalmente, una línea de
        /// resumen bajo el subtítulo ("Total $ 470.000 · Promedio por mes $ 39.167 · …").
        /// </summary>
        public void Mostrar(IEnumerable<PuntoGraficoDTO> puntos, string? resumen = null)
        {
            _puntos = puntos.ToList();
            _mensaje = _puntos.Any(p => p.Valor > 0) ? null : "Sin datos para mostrar en este período";
            _resumen = _mensaje is null ? resumen : null;
            Invalidate();
        }

        /// <summary>Reemplaza el gráfico por un texto (por ejemplo, un error de carga o "Cargando…").</summary>
        public void MostrarMensaje(string mensaje)
        {
            _puntos = new List<PuntoGraficoDTO>();
            _mensaje = mensaje;
            _resumen = null;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            TextRenderer.DrawText(g, _titulo, _fuenteTitulo, new Point(Margen - 4, 16), Tema.TextoPrimario);
            TextRenderer.DrawText(g, _subtitulo, _fuenteSubtitulo, new Point(Margen - 4, 36), Tema.TextoSecundario);
            if (_resumen != null)
            {
                TextRenderer.DrawText(g, _resumen, _fuenteValor,
                    new Rectangle(Margen - 4, 54, Width - Margen * 2, 18), Tema.TextoPrimario,
                    TextFormatFlags.Left | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            }

            _zonas.Clear();
            var area = new Rectangle(Margen, AltoEncabezado + 12, Width - Margen * 2, Height - AltoEncabezado - 12 - Margen);
            if (area.Width < 60 || area.Height < 40) return;

            if (_mensaje != null)
            {
                TextRenderer.DrawText(g, _mensaje, _fuenteSubtitulo, area, Tema.TextoSecundario,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak);
                return;
            }

            if (_orientacion == Orientacion.Columnas) DibujarColumnas(g, area);
            else DibujarBarras(g, area);
        }

        private void DibujarColumnas(Graphics g, Rectangle area)
        {
            decimal maximo = EscalaMaxima(_puntos.Max(p => p.Valor));
            int altoEje = _fuenteEje.Height + 6;
            int anchoEtiquetaY = TextRenderer.MeasureText(_formato(maximo), _fuenteEje).Width + 8;
            // Arriba se deja lugar para el valor escrito sobre la columna más alta.
            int altoValor = _fuenteValor.Height + 4;
            var grafico = new Rectangle(area.Left + anchoEtiquetaY, area.Top + altoValor,
                area.Width - anchoEtiquetaY, area.Height - altoEje - altoValor);

            // Líneas guía horizontales con su valor a la izquierda.
            using var lapizGuia = new Pen(Tema.Borde);
            const int divisiones = 4; // EscalaMaxima devuelve 4 pasos enteros
            for (int i = 0; i <= divisiones; i++)
            {
                int y = grafico.Bottom - grafico.Height * i / divisiones;
                g.DrawLine(lapizGuia, grafico.Left, y, grafico.Right, y);
                var texto = _formato(maximo * i / divisiones);
                var tam = TextRenderer.MeasureText(texto, _fuenteEje);
                TextRenderer.DrawText(g, texto, _fuenteEje,
                    new Point(grafico.Left - tam.Width - 4, y - tam.Height / 2), Tema.TextoSecundario);
            }

            float anchoCelda = (float)grafico.Width / _puntos.Count;
            int anchoColumna = Math.Max(4, (int)(anchoCelda * 0.6F));
            int anchoEtiqueta = _puntos.Max(p => TextRenderer.MeasureText(p.Etiqueta, _fuenteEje).Width) + 6;
            int cadaCuantas = Math.Max(1, (int)Math.Ceiling(anchoEtiqueta / anchoCelda));
            decimal mayor = _puntos.Max(p => p.Valor);

            for (int i = 0; i < _puntos.Count; i++)
            {
                var punto = _puntos[i];
                int centro = grafico.Left + (int)(anchoCelda * i + anchoCelda / 2);
                int alto = (int)(grafico.Height * (punto.Valor / maximo));
                var columna = new Rectangle(centro - anchoColumna / 2, grafico.Bottom - alto, anchoColumna, alto);

                if (alto > 0)
                {
                    using var relleno = new SolidBrush(ColorDe(punto, mayor));
                    g.FillRectangle(relleno, columna);

                    // El valor arriba de la columna, sólo si entra en el ancho de la celda.
                    var valor = _formato(punto.Valor);
                    var tamValor = TextRenderer.MeasureText(valor, _fuenteValor);
                    if (tamValor.Width <= anchoCelda - 2)
                    {
                        TextRenderer.DrawText(g, valor, _fuenteValor,
                            new Point(centro - tamValor.Width / 2, columna.Top - tamValor.Height - 1), Tema.TextoPrimario);
                    }
                }

                if (i % cadaCuantas == 0)
                {
                    var tam = TextRenderer.MeasureText(punto.Etiqueta, _fuenteEje);
                    TextRenderer.DrawText(g, punto.Etiqueta, _fuenteEje,
                        new Point(centro - tam.Width / 2, grafico.Bottom + 4), Tema.TextoSecundario);
                }

                _zonas.Add((new Rectangle((int)(grafico.Left + anchoCelda * i), grafico.Top,
                    (int)Math.Ceiling(anchoCelda), grafico.Height), punto));
            }
        }

        private void DibujarBarras(Graphics g, Rectangle area)
        {
            decimal mayor = _puntos.Max(p => p.Valor);
            int anchoEtiqueta = Math.Min(area.Width / 3,
                _puntos.Max(p => TextRenderer.MeasureText(p.Etiqueta, _fuenteEje).Width) + 12);
            int anchoValor = _puntos.Max(p => TextRenderer.MeasureText(_formato(p.Valor), _fuenteValor).Width) + 10;
            int anchoBarraMax = Math.Max(10, area.Width - anchoEtiqueta - anchoValor);

            int altoFila = Math.Min(40, area.Height / _puntos.Count);
            int altoBarra = Math.Max(6, (int)(altoFila * 0.6F));

            for (int i = 0; i < _puntos.Count; i++)
            {
                var punto = _puntos[i];
                int top = area.Top + altoFila * i;
                var fila = new Rectangle(area.Left, top, area.Width, altoFila);

                TextRenderer.DrawText(g, punto.Etiqueta, _fuenteEje,
                    new Rectangle(area.Left, top, anchoEtiqueta - 8, altoFila), Tema.TextoPrimario,
                    TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

                int largo = (int)(anchoBarraMax * (punto.Valor / mayor));
                var barra = new Rectangle(area.Left + anchoEtiqueta, top + (altoFila - altoBarra) / 2, largo, altoBarra);
                if (largo > 0)
                {
                    using var relleno = new SolidBrush(ColorDe(punto, mayor));
                    g.FillRectangle(relleno, barra);
                }

                TextRenderer.DrawText(g, _formato(punto.Valor), _fuenteValor,
                    new Rectangle(barra.Right + 6, top, anchoValor, altoFila), Tema.TextoPrimario,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

                _zonas.Add((fila, punto));
            }
        }

        private static Color ColorDe(PuntoGraficoDTO punto, decimal mayor) =>
            punto.Valor == mayor ? Tema.Acento : Tema.AcentoSuave;

        /// <summary>
        /// Tope del eje: 4 divisiones de un paso "redondo" (1, 2, 2,5 o 5 × 10ⁿ), nunca menor
        /// a 1 para que los conteos no muestren guías con decimales (0,25 accesos).
        /// </summary>
        private static decimal EscalaMaxima(decimal valor)
        {
            decimal paso = Math.Max(1, valor / 4);
            decimal potencia = 1;
            while (potencia * 10 <= paso) potencia *= 10;
            foreach (var factor in new[] { 1m, 2m, 2.5m, 5m, 10m })
            {
                if (factor == 2.5m && potencia < 10) continue;
                if (potencia * factor >= paso) return potencia * factor * 4;
            }
            return potencia * 40;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            var punto = _zonas.FirstOrDefault(z => z.Zona.Contains(e.Location)).Punto;
            if (ReferenceEquals(punto, _puntoBajoMouse)) return;

            _puntoBajoMouse = punto;
            if (punto == null) _tooltip.Hide(this);
            else _tooltip.Show($"{punto.Etiqueta}: {_formato(punto.Valor)}", this, e.X + 12, e.Y + 12);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _puntoBajoMouse = null;
            _tooltip.Hide(this);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _tooltip.Dispose();
                _fuenteTitulo.Dispose();
                _fuenteSubtitulo.Dispose();
                _fuenteEje.Dispose();
                _fuenteValor.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
