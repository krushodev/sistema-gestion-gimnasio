// Controles armados sólo por código (nunca se arrastran al diseñador).
#pragma warning disable WFO1000
using System;
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
    /// Pantalla de inicio: saludo, tarjetas de resumen y, según el rol, atajos y una lista
    /// de actividad reciente. Sólo reutiliza servicios existentes (ServicioReporte,
    /// ServicioCheckin, ServicioMaquina, ServicioMantenimiento); no agrega consultas.
    /// Administrador → métricas del negocio. Recepcionista → accesos y atajos de mostrador.
    /// Técnico → estado de las máquinas y mantenimientos en curso.
    /// </summary>
    public class ucInicio : UserControl
    {
        private readonly Usuario _usuario;
        private readonly Action<string> _navegar;

        private readonly FlowLayoutPanel _flpTarjetas;
        private readonly Label _lblTituloLista = new();
        private readonly DataGridView _dgvLista = new();
        private int _versionCarga;

        /// <summary>Datos de una tarjeta, calculados en segundo plano (los controles se crean en el hilo de la UI).</summary>
        private sealed record DatosTarjeta(
            string Icono, string Titulo, string Valor, string Detalle, Color ColorValor, string Destino);

        public ucInicio(Usuario usuario, Action<string> navegar)
        {
            _usuario = usuario;
            _navegar = navegar;

            Dock = DockStyle.Fill;
            BackColor = Tema.Fondo;

            // WinForms apila los controles Dock=Top al revés del orden en que se agregan (el
            // último agregado queda más arriba) y el Fill va primero: por eso se agrega de
            // abajo hacia arriba: lista (Fill), título de lista, atajos, tarjetas, encabezado.
            _flpTarjetas = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = true,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Tema.Fondo,
                Padding = new Padding(0, 4, 0, 0)
            };

            if (_usuario.IdRol != Roles.Administrador)
            {
                ConfigurarLista();
                Controls.Add(_dgvLista);

                _lblTituloLista.Dock = DockStyle.Top;
                _lblTituloLista.Height = 40;
                _lblTituloLista.Font = Tema.Fuente(12F, FontStyle.Bold);
                _lblTituloLista.ForeColor = Tema.TextoPrimario;
                _lblTituloLista.TextAlign = ContentAlignment.BottomLeft;
                _lblTituloLista.Text = _usuario.IdRol == Roles.Recepcionista
                    ? "Últimos accesos"
                    : "Mantenimientos en curso";
                Controls.Add(_lblTituloLista);
            }

            var atajos = ConstruirAtajos();
            if (atajos != null) Controls.Add(atajos);

            Controls.Add(_flpTarjetas);
            Controls.Add(ConstruirEncabezado());

            if (_usuario.IdRol != Roles.Administrador) _dgvLista.BringToFront();
        }

        private Control ConstruirEncabezado()
        {
            int hora = DateTime.Now.Hour;
            string saludo = hora < 12 ? "Buen día" : hora < 20 ? "Buenas tardes" : "Buenas noches";
            string fecha = DateTime.Now.ToString("dddd d 'de' MMMM", new CultureInfo("es-AR"));
            fecha = char.ToUpper(fecha[0], CultureInfo.CurrentCulture) + fecha[1..];

            var panel = new Panel { Dock = DockStyle.Top, Height = 84, BackColor = Tema.Fondo };
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
                Location = new Point(2, 48)
            });
            return panel;
        }

        private Control? ConstruirAtajos()
        {
            (string Texto, string Clave, bool Primario)[] atajos = _usuario.IdRol switch
            {
                Roles.Recepcionista => new[]
                {
                    ("Registrar acceso", "checkin", true),
                    ("Cobrar cuota", "cobro", false),
                },
                Roles.Tecnico => new[]
                {
                    ("Registrar falla", "mantenimiento", true),
                },
                _ => Array.Empty<(string, string, bool)>()
            };

            if (atajos.Length == 0) return null;

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

        private void ConfigurarLista()
        {
            _dgvLista.Dock = DockStyle.Fill;
            _dgvLista.ReadOnly = true;
            _dgvLista.AllowUserToAddRows = false;
            _dgvLista.AllowUserToDeleteRows = false;
            _dgvLista.AllowUserToResizeRows = false;
            _dgvLista.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _dgvLista.MultiSelect = false;
            Tema.EstilizarGrilla(_dgvLista);

            if (_usuario.IdRol == Roles.Recepcionista)
            {
                Grillas.Configurar(_dgvLista,
                    (nameof(Checkin.FechaHora), "Fecha y hora", 110),
                    (nameof(Checkin.NombreCompleto), "Socio", 170),
                    (nameof(Checkin.Resultado), "Resultado", 90),
                    (nameof(Checkin.DescripcionVencimiento), "Cuota", 140));
                _dgvLista.Columns["col" + nameof(Checkin.FechaHora)]!.DefaultCellStyle.Format = "dd/MM  HH:mm";
                _dgvLista.CellFormatting += (s, e) =>
                {
                    if (e.RowIndex < 0 || _dgvLista.Columns[e.ColumnIndex].DataPropertyName != nameof(Checkin.Resultado))
                        return;
                    bool concedido = Convert.ToString(e.Value) == "Concedido";
                    e.CellStyle!.ForeColor = concedido ? Tema.Exito : Tema.Peligro;
                    e.CellStyle.Font = Tema.Fuente(10F, FontStyle.Bold);
                };
            }
            else
            {
                Grillas.Configurar(_dgvLista,
                    (nameof(Mantenimiento.NombreMaquina), "Máquina", 150),
                    (nameof(Mantenimiento.NombreTecnico), "Técnico", 130),
                    (nameof(Mantenimiento.FechaInicio), "Inicio", 90),
                    (nameof(Mantenimiento.DetalleTecnico), "Detalle", 200));
                _dgvLista.Columns["col" + nameof(Mantenimiento.FechaInicio)]!.DefaultCellStyle.Format = "dd/MM/yyyy";
            }
        }

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
                var resultado = await Task.Run(CargarDatos);
                if (version != _versionCarga || IsDisposed) return;

                _flpTarjetas.SuspendLayout();
                foreach (Control c in _flpTarjetas.Controls) c.Dispose();
                _flpTarjetas.Controls.Clear();
                foreach (var t in resultado.Tarjetas)
                    _flpTarjetas.Controls.Add(new TarjetaMetrica(
                        t.Icono, t.Titulo, t.Valor, t.Detalle, t.ColorValor, () => _navegar(t.Destino)));
                _flpTarjetas.ResumeLayout();

                if (_usuario.IdRol != Roles.Administrador) _dgvLista.DataSource = resultado.Lista;
            }
            catch (Exception ex)
            {
                if (IsDisposed || version != _versionCarga) return;
                // Sin MessageBox: el inicio no debería interrumpir. Se avisa en el lugar de las tarjetas.
                _flpTarjetas.Controls.Clear();
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

        private (List<DatosTarjeta> Tarjetas, object? Lista) CargarDatos()
        {
            var tarjetas = new List<DatosTarjeta>();

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
                    return (tarjetas, null);
                }

                case Roles.Recepcionista:
                {
                    var m = new ServicioReporte().ObtenerMetricas();
                    var recientes = new ServicioCheckin()
                        .ObtenerHistorial(DateTime.Today.AddDays(-7), DateTime.Today).ToList();
                    var deHoy = recientes.Where(c => c.FechaHora.Date == DateTime.Today).ToList();
                    int concedidos = deHoy.Count(c => c.Resultado == "Concedido");
                    int rechazados = deHoy.Count - concedidos;

                    tarjetas.Add(new DatosTarjeta(Iconos.Acceso, "Accesos de hoy", deHoy.Count.ToString(),
                        $"{concedidos} concedidos · {rechazados} rechazados", Tema.TextoPrimario,
                        "historialAccesos"));
                    tarjetas.Add(new DatosTarjeta(Iconos.Socios, "Socios con cuota al día", m.SociosAlDia.ToString(),
                        $"de {m.SociosActivos} activos", Tema.TextoPrimario, "socios"));
                    return (tarjetas, recientes.Take(8).ToList());
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
                    return (tarjetas, enCurso);
                }
            }
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
