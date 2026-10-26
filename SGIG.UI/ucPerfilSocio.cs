// Controles armados sólo por código (nunca se arrastran al diseñador).
#pragma warning disable WFO1000
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.UI
{
    /// <summary>
    /// Perfil del socio (Fase 12): se muestra dentro de <see cref="frmSocios"/>, tapando la
    /// grilla, y vuelve al listado con "Volver". Encabezado con estado, tarjetas (plan
    /// actual, cuota, último pago, accesos), datos personales e historiales de pagos y
    /// accesos. Los datos los arma <see cref="ServicioSocio.ObtenerPerfil"/>; el
    /// Recepcionista no recibe la parte económica (sin "Último pago", "Total pagado" ni
    /// historial de pagos).
    /// </summary>
    //
    // ── CONTROLES (creados por código, no hay .Designer.cs) ──────────────────
    //   btnVolver, btnEditarPerfil, lblNombre, lblDocumento, lblEstado,
    //   (tarjetas) TarjetaMetrica ×4, pnlDatos, dgvPagos, dgvAccesos
    // ───────────────────────────────────────────────────────────────────────────
    public class ucPerfilSocio : UserControl
    {
        // Una sola instancia: CellFormatting corre por cada celda en cada repintado.
        private static readonly Font FuenteResultado = Tema.Fuente(10F, FontStyle.Bold);

        private readonly int _idPersona;
        private readonly int _idRolLogueado;
        private readonly ServicioSocio _servicioSocio = new();
        private readonly Panel _pnlContenido = new() { Dock = DockStyle.Fill, BackColor = Tema.Fondo };
        private PerfilSocioDTO? _perfil;

        /// <summary>El usuario pidió volver al listado (o editó datos y conviene recargarlo).</summary>
        public event EventHandler? Volver;

        /// <param name="idRolLogueado">
        /// Rol de quien consulta: el Recepcionista ve el perfil sin la parte económica
        /// (el servicio no se la devuelve; acá sólo se acomoda el layout).
        /// </param>
        public ucPerfilSocio(int idPersona, int idRolLogueado)
        {
            _idPersona = idPersona;
            _idRolLogueado = idRolLogueado;
            Dock = DockStyle.Fill;
            BackColor = Tema.Fondo;
            Padding = new Padding(16);

            Controls.Add(_pnlContenido);
            Controls.Add(ConstruirBarraVolver());
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _ = CargarAsync();
        }

        private Control ConstruirBarraVolver()
        {
            var barra = new Panel { Dock = DockStyle.Top, Height = 48, BackColor = Tema.Fondo };
            var btnVolver = new Button
            {
                Name = "btnVolver",
                Text = "‹  Volver al listado",
                Size = new Size(170, Tema.AlturaBoton),
                Location = new Point(0, 0)
            };
            Tema.EstilizarBoton(btnVolver);
            btnVolver.Click += (s, e) => Volver?.Invoke(this, EventArgs.Empty);
            barra.Controls.Add(btnVolver);
            return barra;
        }

        private async Task CargarAsync()
        {
            MostrarMensaje("Cargando perfil…", Tema.TextoSecundario);
            try
            {
                var perfil = await Task.Run(() => _servicioSocio.ObtenerPerfil(_idPersona, _idRolLogueado));
                if (IsDisposed) return;
                _perfil = perfil;
                Construir(perfil);
            }
            catch (Exception ex)
            {
                if (IsDisposed) return;
                MostrarMensaje("No se pudo cargar el perfil: " + ex.Message, Tema.Peligro);
            }
        }

        private void MostrarMensaje(string texto, Color color)
        {
            LimpiarContenido();
            _pnlContenido.Controls.Add(new Label
            {
                Text = texto,
                ForeColor = color,
                Font = Tema.FuenteTexto,
                AutoSize = true,
                MaximumSize = new Size(700, 0),
                Location = new Point(0, 8)
            });
        }

        private void LimpiarContenido()
        {
            foreach (var c in _pnlContenido.Controls.Cast<Control>().ToArray()) c.Dispose();
        }

        // ── Armado del perfil ────────────────────────────────────────────────
        private void Construir(PerfilSocioDTO perfil)
        {
            _pnlContenido.SuspendLayout();
            LimpiarContenido();

            // Dock=Top se apila al revés del orden de alta: primero el Fill, después de abajo hacia arriba.
            var cuerpo = ConstruirCuerpo(perfil);
            _pnlContenido.Controls.Add(cuerpo);

            var tarjetas = new PanelTarjetas();
            _pnlContenido.Controls.Add(tarjetas);
            foreach (var tarjeta in ConstruirTarjetas(perfil)) tarjetas.Controls.Add(tarjeta);

            _pnlContenido.Controls.Add(ConstruirEncabezado(perfil));
            cuerpo.BringToFront();
            _pnlContenido.ResumeLayout();
        }

        private Control ConstruirEncabezado(PerfilSocioDTO perfil)
        {
            var socio = perfil.Socio;
            var panel = new Panel { Dock = DockStyle.Top, Height = 84, BackColor = Tema.Fondo };

            string iniciales = $"{Inicial(socio.Nombre)}{Inicial(socio.Apellido)}";
            var avatar = new AvatarIniciales(iniciales.Length > 0 ? iniciales : "?")
            {
                Size = new Size(48, 48),
                Location = new Point(0, 10),
                BackColor = Tema.Fondo
            };

            var lblNombre = new Label
            {
                Name = "lblNombre",
                Text = $"{socio.Apellido}, {socio.Nombre}",
                Font = Tema.Fuente(18F, FontStyle.Bold),
                ForeColor = Tema.TextoPrimario,
                AutoSize = true,
                Location = new Point(60, 4)
            };

            var lblDocumento = new Label
            {
                Name = "lblDocumento",
                Text = $"{perfil.TipoDocumento} {socio.Documento}".Trim(),
                Font = Tema.Fuente(10F),
                ForeColor = Tema.TextoSecundario,
                AutoSize = true,
                Location = new Point(62, 44)
            };

            var lblEstado = new Label
            {
                Name = "lblEstado",
                Text = socio.Activo ? "Activo" : "Dado de baja",
                Font = Tema.Fuente(9F, FontStyle.Bold),
                ForeColor = socio.Activo ? Tema.ExitoTexto : Tema.PeligroTexto,
                BackColor = socio.Activo ? Tema.ExitoFondo : Tema.PeligroFondo,
                AutoSize = true,
                Padding = new Padding(8, 3, 8, 3)
            };

            var btnEditarPerfil = new Button
            {
                Name = "btnEditarPerfil",
                Text = "Editar datos",
                Size = new Size(140, Tema.AlturaBoton),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            Tema.EstilizarBoton(btnEditarPerfil);
            btnEditarPerfil.Click += (s, e) => EditarDatos();

            panel.Controls.AddRange(new Control[] { avatar, lblNombre, lblDocumento, lblEstado, btnEditarPerfil });

            // El estado va a continuación del documento; el botón, contra el borde derecho.
            void Ubicar()
            {
                lblEstado.Location = new Point(lblDocumento.Right + 12, lblDocumento.Top - 2);
                btnEditarPerfil.Location = new Point(Math.Max(lblNombre.Right + 16, panel.ClientSize.Width - btnEditarPerfil.Width - 16), 14);
            }
            panel.Layout += (s, e) => Ubicar();
            return panel;
        }

        private static string Inicial(string texto) =>
            string.IsNullOrWhiteSpace(texto) ? string.Empty : texto.Trim()[..1];

        private static List<TarjetaMetrica> ConstruirTarjetas(PerfilSocioDTO perfil)
        {
            var ultimoPago = perfil.Pagos.FirstOrDefault();
            var ultimoAcceso = perfil.Accesos.FirstOrDefault();
            var vencimiento = perfil.Socio.FechaVencimientoCuota;

            string detallePlan = perfil.PlanActual != null
                ? ultimoPago != null ? $"Último cobro: {ultimoPago.FechaPago:dd/MM/yyyy}" : "Plan de la última cuota cobrada"
                : perfil.PlanPreferido != null
                    ? "Plan preferido (todavía sin cobros)"
                    : "Todavía no se le cobró ninguna cuota";

            string detalleCuota = perfil.DiasParaVencer switch
            {
                null => "Sin vencimiento registrado",
                < 0 => $"Venció el {vencimiento:dd/MM/yyyy} (hace {-perfil.DiasParaVencer} días)",
                0 => $"Vence hoy, {vencimiento:dd/MM/yyyy}",
                _ => $"Vence el {vencimiento:dd/MM/yyyy} (en {perfil.DiasParaVencer} días)"
            };

            Color colorCuota = perfil.EstadoCuota switch
            {
                "Al día" => Tema.Exito,
                "Vencida" => Tema.Peligro,
                "Sin cuotas" => Tema.TextoSecundario,
                _ => Tema.Aviso // Por vencer / Vence hoy
            };

            var tarjetas = new List<TarjetaMetrica>
            {
                new(Iconos.Planes, "Plan actual",
                    perfil.PlanActual ?? perfil.PlanPreferido ?? "Sin plan", detallePlan, Tema.TextoPrimario),
                new(Iconos.Historial, "Cuota", perfil.EstadoCuota, detalleCuota, colorCuota),
            };

            if (perfil.IncluyeDatosEconomicos)
            {
                tarjetas.Add(new TarjetaMetrica(Iconos.Cobro, "Último pago",
                    ultimoPago != null ? $"$ {ultimoPago.Monto:N0}" : "—",
                    ultimoPago != null
                        ? $"{ultimoPago.FechaPago:dd/MM/yyyy} · {ultimoPago.MedioPagoDesc}"
                        : "Sin pagos registrados",
                    Tema.TextoPrimario));
            }

            tarjetas.Add(new TarjetaMetrica(Iconos.Acceso, "Accesos (30 días)", perfil.AccesosUltimos30Dias.ToString(),
                ultimoAcceso != null
                    ? $"Último: {ultimoAcceso.FechaHora:dd/MM/yyyy HH:mm}"
                    : "Sin accesos en los últimos 90 días",
                Tema.TextoPrimario));

            return tarjetas;
        }

        private Control ConstruirCuerpo(PerfilSocioDTO perfil)
        {
            var cuerpo = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Tema.Fondo,
                Padding = new Padding(0, 4, 0, 0)
            };
            cuerpo.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 340));
            cuerpo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            cuerpo.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            cuerpo.Controls.Add(ConstruirDatosPersonales(perfil), 0, 0);

            var historiales = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = Tema.Fondo,
                Margin = new Padding(0)
            };
            historiales.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            if (perfil.IncluyeDatosEconomicos)
            {
                historiales.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
                historiales.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
                historiales.Controls.Add(ConstruirPagos(perfil), 0, 0);
                historiales.Controls.Add(ConstruirAccesos(perfil, new Padding(0, 8, 0, 0)), 0, 1);
            }
            else
            {
                // Sin parte económica: los accesos ocupan toda la columna.
                historiales.RowCount = 1;
                historiales.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
                historiales.Controls.Add(ConstruirAccesos(perfil, new Padding(0)), 0, 0);
            }
            cuerpo.Controls.Add(historiales, 1, 0);

            return cuerpo;
        }

        private static Control ConstruirDatosPersonales(PerfilSocioDTO perfil)
        {
            var socio = perfil.Socio;
            var bloque = CrearBloque("Datos personales", new Padding(0, 0, 16, 0));
            bloque.Name = "pnlDatos";
            bloque.AutoScroll = true; // en ventanas bajas las últimas filas no entran

            var datos = new List<(string Rotulo, string Valor)>
            {
                ("Documento", $"{perfil.TipoDocumento} {socio.Documento}".Trim()),
                ("Fecha de nacimiento", socio.FechaNacimiento.HasValue
                    ? $"{socio.FechaNacimiento:dd/MM/yyyy} ({perfil.Edad} años)"
                    : "—"),
                ("Correo electrónico", ValorOGuion(socio.Email)),
                ("Teléfono", ValorOGuion(socio.Telefono)),
                ("Localidad", ValorOGuion(perfil.Localidad)),
                ("Plan preferido", ValorOGuion(perfil.PlanPreferido)),
                ("Apto médico", ValorOGuion(socio.AptoMedico)),
            };
            if (perfil.IncluyeDatosEconomicos)
                datos.Add(("Total pagado", $"$ {perfil.TotalPagado:N0} en {perfil.Pagos.Count} pago(s)"));

            int y = 52;
            foreach (var (rotulo, valor) in datos)
            {
                bloque.Controls.Add(new Label
                {
                    Text = rotulo,
                    Font = Tema.Fuente(9F),
                    ForeColor = Tema.TextoSecundario,
                    AutoSize = false,
                    Location = new Point(16, y),
                    Size = new Size(290, 18),
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                    BackColor = Color.Transparent
                });
                bloque.Controls.Add(new Label
                {
                    Text = valor,
                    Font = Tema.Fuente(10F),
                    ForeColor = Tema.TextoPrimario,
                    AutoSize = false,
                    AutoEllipsis = true,
                    Location = new Point(16, y + 18),
                    Size = new Size(290, 22),
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                    BackColor = Color.Transparent
                });
                y += 48;
            }
            return bloque;
        }

        private static string ValorOGuion(string? valor) => string.IsNullOrWhiteSpace(valor) ? "—" : valor;

        private static Control ConstruirPagos(PerfilSocioDTO perfil)
        {
            var bloque = CrearBloque($"Historial de pagos ({perfil.Pagos.Count})", new Padding(0, 0, 0, 8));
            var dgvPagos = CrearGrilla("dgvPagos");
            Grillas.Configurar(dgvPagos,
                (nameof(Pago.FechaPago), "Fecha", 80),
                (nameof(Pago.PlanNombre), "Plan", 110),
                (nameof(Pago.MedioPagoDesc), "Medio de pago", 100),
                (nameof(Pago.Monto), "Monto", 80),
                (nameof(Pago.FechaVencimiento), "Cubre hasta", 80));
            dgvPagos.Columns["col" + nameof(Pago.FechaPago)]!.DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvPagos.Columns["col" + nameof(Pago.FechaVencimiento)]!.DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvPagos.Columns["col" + nameof(Pago.Monto)]!.DefaultCellStyle.Format = "$ #,##0.00";
            dgvPagos.DataSource = perfil.Pagos;
            AgregarGrilla(bloque, dgvPagos);
            return bloque;
        }

        private static Control ConstruirAccesos(PerfilSocioDTO perfil, Padding margen)
        {
            var bloque = CrearBloque($"Accesos de los últimos 90 días ({perfil.Accesos.Count})", margen);
            var dgvAccesos = CrearGrilla("dgvAccesos");
            Grillas.Configurar(dgvAccesos,
                (nameof(Checkin.FechaHora), "Fecha y hora", 120),
                (nameof(Checkin.Resultado), "Resultado", 100));
            dgvAccesos.Columns["col" + nameof(Checkin.FechaHora)]!.DefaultCellStyle.Format = "dd/MM/yyyy  HH:mm";
            dgvAccesos.CellFormatting += (s, e) =>
            {
                if (e.RowIndex < 0 || dgvAccesos.Columns[e.ColumnIndex].DataPropertyName != nameof(Checkin.Resultado))
                    return;
                bool concedido = Convert.ToString(e.Value) == "Concedido";
                e.CellStyle!.ForeColor = concedido ? Tema.Exito : Tema.Peligro;
                e.CellStyle.Font = FuenteResultado;
            };
            dgvAccesos.DataSource = perfil.Accesos;
            AgregarGrilla(bloque, dgvAccesos);
            return bloque;
        }

        /// <summary>Tarjeta blanca con un título arriba, para los bloques del cuerpo.</summary>
        private static RoundedPanel CrearBloque(string titulo, Padding margen)
        {
            var bloque = new RoundedPanel
            {
                Dock = DockStyle.Fill,
                Margin = margen,
                Padding = new Padding(12, 44, 12, 12),
                BackColor = Tema.Fondo
            };
            bloque.Controls.Add(new Label
            {
                Text = titulo,
                Font = Tema.Fuente(11F, FontStyle.Bold),
                ForeColor = Tema.TextoPrimario,
                AutoSize = true,
                Location = new Point(14, 14),
                BackColor = Color.Transparent
            });
            return bloque;
        }

        private static DataGridView CrearGrilla(string nombre)
        {
            var dgv = new DataGridView
            {
                Name = nombre,
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };
            Tema.EstilizarGrilla(dgv);
            return dgv;
        }

        private static void AgregarGrilla(RoundedPanel bloque, DataGridView grilla)
        {
            bloque.Controls.Add(grilla);
            grilla.BringToFront();
        }

        // ── Acciones ─────────────────────────────────────────────────────────
        private void EditarDatos()
        {
            if (_perfil is null) return;

            using var editor = new frmSocioEditor(_perfil.Socio);
            if (editor.ShowDialog(FindForm()) == DialogResult.OK)
            {
                _ = CargarAsync();
            }
        }
    }
}
