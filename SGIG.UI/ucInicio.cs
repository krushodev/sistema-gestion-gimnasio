// Controles armados sólo por código (nunca se arrastran al diseñador).
#pragma warning disable WFO1000
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.UI
{
    /// <summary>
    /// Pantalla de inicio "qué tengo que hacer hoy" (Fase 13): saludo, tarjetas de resumen,
    /// atajos, el bloque "Para atender hoy" (avisos de <see cref="ServicioAvisos"/>) y dos
    /// listas lado a lado, cada una en una tarjeta con estado vacío.
    /// Administrador → últimos cobros y cuotas por vencer. Recepcionista → últimos accesos y
    /// vencimientos de la semana. Técnico → mantenimientos en curso y máquinas más antiguas.
    /// Los gráficos tienen su propia sección (<see cref="frmGraficos"/>).
    /// </summary>
    public class ucInicio : UserControl
    {
        private const int DiasPorVencer = ServicioAvisos.DiasDeMargen;

        // Una sola instancia: CellFormatting corre por cada celda en cada repintado.
        private static readonly Font FuenteDestacada = Tema.Fuente(10F, FontStyle.Bold);

        private readonly Usuario _usuario;
        private readonly Action<string> _navegar;

        private readonly PanelTarjetas _flpTarjetas;
        private readonly PanelFlujo _pnlAvisos = new();
        private readonly BloqueLista _bloqueIzquierdo;
        private readonly BloqueLista _bloqueDerecho;
        private int _versionCarga;

        /// <summary>Datos de una tarjeta, calculados en segundo plano (los controles se crean en el hilo de la UI).</summary>
        private sealed record DatosTarjeta(
            string Icono, string Titulo, string Valor, string Detalle, Color ColorValor, string Destino);

        /// <summary>Todo lo que se consulta en segundo plano en cada recarga.</summary>
        private sealed record DatosInicio(
            List<DatosTarjeta> Tarjetas, List<AvisoDTO> Avisos, IList Izquierda, IList Derecha);

        // Filas de las listas (sólo para mostrar: la grilla enlaza sus propiedades).
        private sealed record FilaVencimiento(string Socio, string Documento, DateTime Vence, string Cuando);
        private sealed record FilaMantenimiento(string Maquina, string Tecnico, DateTime Inicio, int DiasFuera);
        private sealed record FilaMaquina(string Maquina, string Marca, DateTime Compra, string Antiguedad, string Estado);

        public ucInicio(Usuario usuario, Action<string> navegar)
        {
            _usuario = usuario;
            _navegar = navegar;

            Dock = DockStyle.Fill;
            BackColor = Tema.Fondo;

            _flpTarjetas = new PanelTarjetas();
            (_bloqueIzquierdo, _bloqueDerecho) = CrearListas();

            // WinForms apila los controles Dock=Top al revés del orden en que se agregan (el
            // último agregado queda más arriba) y el Fill va primero: por eso se agrega de
            // abajo hacia arriba: listas (Fill), avisos, título de avisos, atajos, tarjetas, encabezado.
            var listas = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Tema.Fondo,
                Padding = new Padding(0, 6, 16, 0)
            };
            listas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            listas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            listas.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            _bloqueIzquierdo.Margin = new Padding(0, 0, 8, 0);
            _bloqueDerecho.Margin = new Padding(8, 0, 0, 0);
            listas.Controls.Add(_bloqueIzquierdo, 0, 0);
            listas.Controls.Add(_bloqueDerecho, 1, 0);
            Controls.Add(listas);

            Controls.Add(_pnlAvisos);
            Controls.Add(new Label
            {
                Text = "Para atender hoy",
                Dock = DockStyle.Top,
                Height = 40,
                Font = Tema.Fuente(12F, FontStyle.Bold),
                ForeColor = Tema.TextoPrimario,
                TextAlign = ContentAlignment.MiddleLeft
            });

            Controls.Add(ConstruirAtajos());
            Controls.Add(_flpTarjetas);
            Controls.Add(ConstruirEncabezado());

            listas.BringToFront();
        }

        private Control ConstruirEncabezado()
        {
            int hora = DateTime.Now.Hour;
            string saludo = hora < 12 ? "Buen día" : hora < 20 ? "Buenas tardes" : "Buenas noches";
            string fecha = DateTime.Now.ToString("dddd d 'de' MMMM", new CultureInfo("es-AR"));
            fecha = char.ToUpper(fecha[0], CultureInfo.CurrentCulture) + fecha[1..];

            var panel = new Panel { Dock = DockStyle.Top, Height = 90, BackColor = Tema.Fondo };
            panel.Controls.Add(new Label
            {
                Text = $"{saludo}, {_usuario.Nombre}",
                Font = Tema.Fuente(22F, FontStyle.Bold),
                ForeColor = Tema.TextoPrimario,
                AutoSize = true,
                Location = new Point(0, 4)
            });
            panel.Controls.Add(new Label
            {
                Text = fecha,
                Font = Tema.FuenteSubtitulo,
                ForeColor = Tema.TextoSecundario,
                AutoSize = true,
                Location = new Point(2, 56)
            });
            return panel;
        }

        private Control ConstruirAtajos()
        {
            (string Texto, string Clave, bool Primario)[] atajos = _usuario.IdRol switch
            {
                Roles.Recepcionista => new[]
                {
                    ("Registrar acceso", "checkin", true),
                    ("Cobrar cuota", "cobro", false),
                    ("Ver gráficos", "graficos", false),
                },
                Roles.Tecnico => new[]
                {
                    ("Registrar falla", "mantenimiento", true),
                    ("Ver gráficos", "graficos", false),
                },
                _ => new[]
                {
                    ("Ver gráficos", "graficos", true),
                }
            };

            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 56,
                BackColor = Tema.Fondo,
                Padding = new Padding(0, 12, 0, 0)
            };

            foreach (var (texto, clave, primario) in atajos)
            {
                // El nombre sigue la convención de Tema.EstilizarBoton: "btnCerrar*" lo hace
                // secundario; el resto, primario.
                var btn = new Button
                {
                    Name = primario ? "btnAtajoPrincipal" : "btnAtajoCerrar",
                    Text = texto,
                    Size = new Size(170, 40),
                    Margin = new Padding(0, 0, 12, 0)
                };
                Tema.EstilizarBoton(btn);
                btn.Click += (s, e) => _navegar(clave);
                panel.Controls.Add(btn);
            }
            return panel;
        }

        // ── Listas ───────────────────────────────────────────────────────────

        /// <summary>Las dos listas del rol, con sus columnas y su texto de estado vacío.</summary>
        private (BloqueLista Izquierda, BloqueLista Derecha) CrearListas()
        {
            switch (_usuario.IdRol)
            {
                case Roles.Administrador:
                {
                    var cobros = new BloqueLista("Últimos cobros", "Todavía no se registró ningún cobro", Iconos.Cobro);
                    Grillas.Configurar(cobros.Grilla,
                        (nameof(Pago.FechaPago), "Fecha", 70),
                        (nameof(Pago.NombreSocio), "Socio", 150),
                        (nameof(Pago.PlanNombre), "Plan", 100),
                        (nameof(Pago.Monto), "Monto", 80));
                    Formato(cobros, nameof(Pago.FechaPago), "dd/MM/yyyy");
                    Formato(cobros, nameof(Pago.Monto), "$ #,##0");
                    return (cobros, CrearListaVencimientos($"Cuotas por vencer ({DiasPorVencer} días)"));
                }

                case Roles.Recepcionista:
                {
                    var accesos = new BloqueLista("Últimos accesos", "No hubo accesos en los últimos 7 días", Iconos.Acceso);
                    Grillas.Configurar(accesos.Grilla,
                        (nameof(Checkin.FechaHora), "Fecha y hora", 100),
                        (nameof(Checkin.NombreCompleto), "Socio", 150),
                        (nameof(Checkin.Resultado), "Resultado", 90),
                        (nameof(Checkin.DescripcionVencimiento), "Cuota", 120));
                    Formato(accesos, nameof(Checkin.FechaHora), "dd/MM  HH:mm");
                    accesos.Grilla.CellFormatting += (s, e) =>
                    {
                        if (e.RowIndex < 0 || accesos.Grilla.Columns[e.ColumnIndex].DataPropertyName != nameof(Checkin.Resultado))
                            return;
                        bool concedido = Convert.ToString(e.Value) == "Concedido";
                        e.CellStyle!.ForeColor = concedido ? Tema.Exito : Tema.Peligro;
                        e.CellStyle.Font = FuenteDestacada;
                    };
                    return (accesos, CrearListaVencimientos("Vencen esta semana"));
                }

                default: // Técnico
                {
                    var enCurso = new BloqueLista("Mantenimientos en curso", "No hay máquinas en reparación", Iconos.Ok);
                    Grillas.Configurar(enCurso.Grilla,
                        (nameof(FilaMantenimiento.Maquina), "Máquina", 140),
                        (nameof(FilaMantenimiento.Tecnico), "Técnico", 110),
                        (nameof(FilaMantenimiento.Inicio), "Inicio", 80),
                        (nameof(FilaMantenimiento.DiasFuera), "Días fuera", 70));
                    Formato(enCurso, nameof(FilaMantenimiento.Inicio), "dd/MM/yyyy");
                    // Una reparación de más de 7 días se destaca en naranja (mismo margen que el aviso).
                    enCurso.Grilla.CellFormatting += (s, e) =>
                    {
                        if (e.RowIndex < 0 || enCurso.Grilla.Columns[e.ColumnIndex].DataPropertyName != nameof(FilaMantenimiento.DiasFuera))
                            return;
                        if (e.Value is int dias && dias > ServicioAvisos.DiasDeMargen)
                        {
                            e.CellStyle!.ForeColor = Tema.Aviso;
                            e.CellStyle.Font = FuenteDestacada;
                        }
                    };

                    var antiguas = new BloqueLista("Máquinas más antiguas", "No hay máquinas con fecha de compra", Iconos.Maquinas);
                    Grillas.Configurar(antiguas.Grilla,
                        (nameof(FilaMaquina.Maquina), "Máquina", 140),
                        (nameof(FilaMaquina.Marca), "Marca", 90),
                        (nameof(FilaMaquina.Compra), "Compra", 80),
                        (nameof(FilaMaquina.Antiguedad), "Antigüedad", 80),
                        (nameof(FilaMaquina.Estado), "Estado", 90));
                    Formato(antiguas, nameof(FilaMaquina.Compra), "dd/MM/yyyy");
                    return (enCurso, antiguas);
                }
            }
        }

        private static BloqueLista CrearListaVencimientos(string titulo)
        {
            var vencimientos = new BloqueLista(titulo, "Ninguna cuota vence en los próximos días", Iconos.Ok);
            Grillas.Configurar(vencimientos.Grilla,
                (nameof(FilaVencimiento.Socio), "Socio", 150),
                (nameof(FilaVencimiento.Documento), "Documento", 80),
                (nameof(FilaVencimiento.Vence), "Vence", 80),
                (nameof(FilaVencimiento.Cuando), "Cuándo", 70));
            Formato(vencimientos, nameof(FilaVencimiento.Vence), "dd/MM/yyyy");
            return vencimientos;
        }

        private static void Formato(BloqueLista bloque, string propiedad, string formato) =>
            bloque.Grilla.Columns["col" + propiedad]!.DefaultCellStyle.Format = formato;

        // ── Carga ────────────────────────────────────────────────────────────

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _ = RecargarAsync();
        }

        /// <summary>Vuelve a consultar los datos (se llama cada vez que se muestra el inicio).</summary>
        public async Task RecargarAsync()
        {
            int version = ++_versionCarga;
            try
            {
                var datos = await Task.Run(CargarDatos);
                if (version != _versionCarga || IsDisposed) return;

                _flpTarjetas.SuspendLayout();
                Vaciar(_flpTarjetas);
                foreach (var t in datos.Tarjetas)
                    _flpTarjetas.Controls.Add(new TarjetaMetrica(
                        t.Icono, t.Titulo, t.Valor, t.Detalle, t.ColorValor, () => _navegar(t.Destino)));
                _flpTarjetas.ResumeLayout();

                _pnlAvisos.SuspendLayout();
                Vaciar(_pnlAvisos);
                foreach (var aviso in datos.Avisos)
                {
                    string? destino = DestinoDe(aviso);
                    _pnlAvisos.Controls.Add(new ChipAviso(aviso, destino is null ? null : () => _navegar(destino)));
                }
                _pnlAvisos.ResumeLayout();

                _bloqueIzquierdo.Mostrar(datos.Izquierda);
                _bloqueDerecho.Mostrar(datos.Derecha);
            }
            catch (Exception ex)
            {
                if (IsDisposed || version != _versionCarga) return;
                // Sin MessageBox: el inicio no debería interrumpir. Se avisa en el lugar de las tarjetas.
                Vaciar(_flpTarjetas);
                Vaciar(_pnlAvisos);
                _flpTarjetas.Controls.Add(new Label
                {
                    Text = "No se pudo cargar el resumen: " + ex.Message,
                    ForeColor = Tema.Peligro,
                    Font = Tema.FuenteLabel,
                    AutoSize = true,
                    MaximumSize = new Size(700, 0)
                });
            }
        }

        private static void Vaciar(Control contenedor)
        {
            foreach (var c in contenedor.Controls.Cast<Control>().ToArray()) c.Dispose();
        }

        /// <summary>A qué pantalla lleva cada aviso (null = no es clickeable).</summary>
        private static string? DestinoDe(AvisoDTO aviso) => aviso.Categoria switch
        {
            "Cuotas" or "Cumpleanios" => "socios",
            "Maquinas" => "maquinas",
            _ => null
        };

        private DatosInicio CargarDatos()
        {
            var hoy = DateTime.Today;
            var tarjetas = new List<DatosTarjeta>();
            var avisos = new ServicioAvisos().ObtenerAvisos(_usuario.IdRol, hoy);

            switch (_usuario.IdRol)
            {
                case Roles.Administrador:
                {
                    var m = new ServicioReporte().ObtenerMetricas();
                    tarjetas.Add(new DatosTarjeta(Iconos.Cobro, "Ingresos del mes", $"$ {m.IngresosMes:N0}",
                        "Total facturado este mes", Tema.TextoPrimario, "reportes"));
                    tarjetas.Add(new DatosTarjeta(Iconos.Socios, "Socios activos", m.SociosActivos.ToString(),
                        $"{m.SociosAlDia} con cuota al día", Tema.TextoPrimario, "socios"));
                    tarjetas.Add(new DatosTarjeta(Iconos.Acceso, "Check-ins promedio", $"{m.CheckinsPromedioDia:F1} / día",
                        "Últimos 30 días", Tema.TextoPrimario, "reportes"));

                    var cobros = new ServicioTesoreria().ObtenerUltimosPagos(8).ToList();
                    return new DatosInicio(tarjetas, avisos, cobros, Vencimientos(hoy));
                }

                case Roles.Recepcionista:
                {
                    var m = new ServicioReporte().ObtenerMetricas();
                    var recientes = new ServicioCheckin()
                        .ObtenerHistorial(hoy.AddDays(-7), hoy).ToList();
                    var deHoy = recientes.Where(c => c.FechaHora.Date == hoy).ToList();
                    int concedidos = deHoy.Count(c => c.Resultado == "Concedido");
                    int rechazados = deHoy.Count - concedidos;

                    tarjetas.Add(new DatosTarjeta(Iconos.Acceso, "Accesos de hoy", deHoy.Count.ToString(),
                        $"{concedidos} concedidos · {rechazados} rechazados", Tema.TextoPrimario,
                        "historialAccesos"));
                    tarjetas.Add(new DatosTarjeta(Iconos.Socios, "Socios con cuota al día", m.SociosAlDia.ToString(),
                        $"de {m.SociosActivos} activos", Tema.TextoPrimario, "socios"));
                    return new DatosInicio(tarjetas, avisos, recientes.Take(8).ToList(), Vencimientos(hoy));
                }

                default: // Técnico
                {
                    var maquinas = new ServicioMaquina().ObtenerTodas().ToList();
                    var enCurso = new ServicioMantenimiento().ObtenerActivos().ToList();
                    int enReparacion = maquinas.Count(q => q.Estado != "Operativa");

                    tarjetas.Add(new DatosTarjeta(Iconos.Mantenimiento, "En reparación", enReparacion.ToString(),
                        "Máquinas fuera de servicio", enReparacion > 0 ? Tema.Aviso : Tema.TextoPrimario,
                        "maquinas"));
                    tarjetas.Add(new DatosTarjeta(Iconos.Ok, "Operativas", (maquinas.Count - enReparacion).ToString(),
                        $"de {maquinas.Count} máquinas", Tema.TextoPrimario, "maquinas"));
                    tarjetas.Add(new DatosTarjeta(Iconos.Historial, "Mantenimientos en curso", enCurso.Count.ToString(),
                        "Sin fecha de fin", Tema.TextoPrimario, "mantenimiento"));

                    var filasEnCurso = enCurso
                        .OrderBy(x => x.FechaInicio)
                        .Select(x => new FilaMantenimiento(x.NombreMaquina ?? "—", x.NombreTecnico ?? "—",
                            x.FechaInicio, (hoy - x.FechaInicio.Date).Days))
                        .ToList();
                    var antiguas = maquinas
                        .Where(q => q.FechaCompra.HasValue)
                        .OrderBy(q => q.FechaCompra)
                        .Take(8)
                        .Select(q => new FilaMaquina(q.Nombre, q.Marca ?? "—", q.FechaCompra!.Value,
                            Antiguedad(q.FechaCompra.Value, hoy), q.Estado))
                        .ToList();
                    return new DatosInicio(tarjetas, avisos, filasEnCurso, antiguas);
                }
            }
        }

        private static List<FilaVencimiento> Vencimientos(DateTime hoy) =>
            new ServicioSocio().ObtenerPorVencer(hoy, DiasPorVencer)
                .Select(s =>
                {
                    int dias = (s.FechaVencimientoCuota!.Value.Date - hoy).Days;
                    string cuando = dias switch { 0 => "Hoy", 1 => "Mañana", _ => $"En {dias} días" };
                    return new FilaVencimiento($"{s.Apellido}, {s.Nombre}", s.Documento, s.FechaVencimientoCuota.Value, cuando);
                })
                .ToList();

        /// <summary>"Menos de 1 año", "1 año", "8 años".</summary>
        private static string Antiguedad(DateTime compra, DateTime hoy)
        {
            int anios = hoy.Year - compra.Year;
            if (compra.Date > hoy.AddYears(-anios)) anios--;
            return anios switch { < 1 => "Menos de 1 año", 1 => "1 año", _ => $"{anios} años" };
        }
    }

    /// <summary>
    /// Fila de tarjetas que se reacomoda sola (varias por fila según el ancho, con el alto
    /// ajustado). Posiciona a mano en vez de usar un FlowLayoutPanel: con ese, si las tarjetas
    /// se agregaban mientras el panel estaba oculto, quedaban todas apiladas en (0,0).
    /// </summary>
    internal sealed class PanelTarjetas : Panel
    {
        public PanelTarjetas()
        {
            Dock = DockStyle.Top;
            Height = 140;
            Padding = new Padding(0, 4, 0, 0);
            BackColor = Tema.Fondo;
        }

        public void Reacomodar()
        {
            if (Controls.Count == 0) return;

            var primera = Controls[0];
            int anchoCelda = primera.Width + primera.Margin.Horizontal;
            int altoCelda = primera.Height + primera.Margin.Vertical;
            int porFila = Math.Max(1, ClientSize.Width / Math.Max(1, anchoCelda));

            for (int i = 0; i < Controls.Count; i++)
            {
                Controls[i].Location = new Point(
                    (i % porFila) * anchoCelda,
                    Padding.Top + (i / porFila) * altoCelda);
            }

            int filas = (Controls.Count + porFila - 1) / porFila;
            int alto = Padding.Vertical + filas * altoCelda;
            if (Height != alto) Height = alto;
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
    /// Tarjeta de métrica: ícono de línea, título, valor grande y detalle. Si recibe una
    /// acción, toda la tarjeta es clickeable.
    /// </summary>
    internal sealed class TarjetaMetrica : RoundedPanel
    {
        private readonly Label _lblValor;
        private readonly Label _lblDetalle;
        private readonly Font _fuenteIcono = Tema.FuenteIconos(12F);
        private readonly Font _fuenteTitulo = Tema.Fuente(9.5F);
        private readonly Font _fuenteValor = Tema.Fuente(20F, FontStyle.Bold);
        private readonly Font _fuenteDetalle = Tema.Fuente(9F);

        public TarjetaMetrica(string icono, string titulo, string valor, string detalle,
            Color colorValor, Action? alHacerClick = null)
        {
            Size = new Size(260, 128);
            Margin = new Padding(0, 0, 16, 16);
            BackColor = Tema.Fondo;

            var lblIcono = new Label
            {
                Text = icono,
                Font = _fuenteIcono,
                ForeColor = Tema.TextoSecundario,
                AutoSize = false,
                Size = new Size(24, 24),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(16, 14),
                BackColor = Color.Transparent
            };
            var lblTitulo = new Label
            {
                Text = titulo,
                Font = _fuenteTitulo,
                ForeColor = Tema.TextoSecundario,
                AutoEllipsis = true,
                Location = new Point(46, 16),
                Size = new Size(200, 20),
                BackColor = Color.Transparent
            };
            var lblValor = new Label
            {
                Text = valor,
                Font = _fuenteValor,
                ForeColor = colorValor,
                AutoEllipsis = true,
                Location = new Point(14, 42),
                Size = new Size(232, 46),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };
            var lblDetalle = new Label
            {
                Text = detalle,
                Font = _fuenteDetalle,
                ForeColor = Tema.TextoSecundario,
                AutoEllipsis = true,
                Location = new Point(16, 96),
                Size = new Size(230, 20),
                BackColor = Color.Transparent
            };
            Controls.AddRange(new Control[] { lblIcono, lblTitulo, lblValor, lblDetalle });
            _lblValor = lblValor;
            _lblDetalle = lblDetalle;

            if (alHacerClick == null) return;

            Cursor = Cursors.Hand;
            foreach (Control c in new Control[] { this, lblIcono, lblTitulo, lblValor, lblDetalle })
            {
                c.Cursor = Cursors.Hand;
                c.Click += (s, e) => alHacerClick();
                c.MouseEnter += (s, e) => Resaltar(true);
                c.MouseLeave += (s, e) =>
                    Resaltar(ClientRectangle.Contains(PointToClient(Cursor.Position)));
            }
        }

        /// <summary>Cambia el valor y el detalle una vez que llegan los datos.</summary>
        public void Actualizar(string valor, string detalle)
        {
            _lblValor.Text = valor;
            _lblDetalle.Text = detalle;
        }

        private void Resaltar(bool encendido)
        {
            ColorBorde = encendido ? Tema.Acento : Tema.Borde;
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _fuenteIcono.Dispose();
                _fuenteTitulo.Dispose();
                _fuenteValor.Dispose();
                _fuenteDetalle.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
