using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.UI
{
    /// <summary>
    /// Listado de socios (RF#05, RF#06, RF#07, RNF#03). Accesible para Administrador
    /// y Recepcionista. El alta y la edición se hacen en el diálogo modal
    /// <see cref="frmSocioEditor"/>; esta pantalla lista a todos (activos y dados de
    /// baja, distinguidos por estado) y permite reactivar sin pasar por el editor.
    /// </summary>
    //
    // ── CONTROLES (ver frmSocios.Designer.cs) ────────────────────────────────
    //   txtBuscar (filtro rápido), dgvSocios, btnNuevo, btnEditar, btnDarDeBaja
    //   (btnDarDeBaja alterna su texto y acción según el estado de la fila
    //   seleccionada: "Dar de baja" para un socio activo, "Dar de alta" para uno
    //   dado de baja — no hace falta agregar un botón nuevo)
    //   colPerfil (columna de botón "Ver perfil", agregada por código en
    //   ConfigurarGrilla) → abre ucPerfilSocio dentro de esta misma pantalla (Fase 12)
    // ───────────────────────────────────────────────────────────────────────────
    public partial class frmSocios : Form
    {
        private const string ColumnaPerfil = "colPerfil";
        // Una sola instancia: CellPainting corre por cada celda en cada repintado.
        private static readonly Font FuenteBotonPerfil = Tema.Fuente(9F, FontStyle.Bold);

        private readonly ServicioSocio _servicioSocio = new();
        private readonly Usuario _usuarioLogueado;
        private List<Socio> _socios = new();
        private ucPerfilSocio? _perfilAbierto;

        /// <param name="usuarioLogueado">Define qué parte del perfil del socio se muestra (Fase 12).</param>
        public frmSocios(Usuario usuarioLogueado)
        {
            _usuarioLogueado = usuarioLogueado;
            InitializeComponent();
            dgvSocios.CellFormatting += DgvSocios_CellFormatting;
            dgvSocios.DataBindingComplete += DgvSocios_DataBindingComplete;
            dgvSocios.SelectionChanged += (s, e) => ActualizarBotonBaja();
            dgvSocios.CellPainting += DgvSocios_CellPainting;
            dgvSocios.CellClick += DgvSocios_CellClick;
            dgvSocios.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0 && dgvSocios.Rows[e.RowIndex].DataBoundItem is Socio socio) AbrirPerfil(socio);
            };
            dgvSocios.CellMouseEnter += (s, e) =>
                dgvSocios.Cursor = e.RowIndex >= 0 && EsColumnaPerfil(e.ColumnIndex) ? Cursors.Hand : Cursors.Default;
        }

        private void frmSocios_Load(object sender, EventArgs e)
        {
            try
            {
                ConfigurarGrilla();
                CargarGrilla();
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        /// <summary>
        /// Columnas explícitas: la grilla muestra sólo lo que le sirve a recepción,
        /// sin exponer ids internos. Se incluye el estado (Activo/Dado de baja) para
        /// que un socio dado de baja siga siendo visible y se pueda reactivar.
        /// </summary>
        private void ConfigurarGrilla()
        {
            Grillas.Configurar(dgvSocios,
                (nameof(Socio.Apellido), "Apellido", 100),
                (nameof(Socio.Nombre), "Nombre", 100),
                (nameof(Socio.Documento), "Documento", 80),
                (nameof(Socio.Telefono), "Teléfono", 90),
                (nameof(Socio.Email), "Correo electrónico", 150),
                (nameof(Socio.FechaVencimientoCuota), "Vencim. cuota", 90),
                (nameof(Socio.Activo), "Estado", 70));

            // Botón por fila para ir al perfil (Fase 12). Se dibuja a mano en CellPainting
            // para que tenga el estilo de Tema y no el botón gris de Windows.
            dgvSocios.Columns.Add(new DataGridViewButtonColumn
            {
                Name = ColumnaPerfil,
                HeaderText = "Perfil",
                Text = "Ver perfil",
                UseColumnTextForButtonValue = true,
                FillWeight = 70,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
        }

        // ── Perfil del socio (Fase 12) ───────────────────────────────────────

        private bool EsColumnaPerfil(int indiceColumna) =>
            indiceColumna >= 0 && dgvSocios.Columns[indiceColumna].Name == ColumnaPerfil;

        private void DgvSocios_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || !EsColumnaPerfil(e.ColumnIndex)) return;
            if (dgvSocios.Rows[e.RowIndex].DataBoundItem is Socio socio) AbrirPerfil(socio);
        }

        /// <summary>Dibuja "Ver perfil" como un botón con borde en el color de acento.</summary>
        private void DgvSocios_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || !EsColumnaPerfil(e.ColumnIndex) || e.Graphics is null) return;

            e.PaintBackground(e.CellBounds, true);

            var boton = new Rectangle(e.CellBounds.X + 8, e.CellBounds.Y + 6,
                Math.Min(110, e.CellBounds.Width - 16), e.CellBounds.Height - 12);
            if (boton.Width > 20 && boton.Height > 10)
            {
                using var lapiz = new Pen(Tema.Acento);
                e.Graphics.DrawRectangle(lapiz, boton);
                TextRenderer.DrawText(e.Graphics, "Ver perfil", FuenteBotonPerfil, boton, Tema.Acento,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            }
            e.Handled = true;
        }

        /// <summary>
        /// Muestra el perfil tapando la grilla, dentro de esta misma pantalla (RNF#02: nada
        /// de ventanas sueltas). Al volver se recarga la grilla por si se editaron datos.
        /// </summary>
        private void AbrirPerfil(Socio socio)
        {
            if (_perfilAbierto != null) return;

            // Se ocultan los controles del listado (no sólo se tapan): si no, el Tab del
            // teclado seguiría llegando a la grilla y a los botones que quedan debajo.
            foreach (Control c in Controls) c.Visible = false;

            _perfilAbierto = new ucPerfilSocio(socio.IdPersona, _usuarioLogueado.IdRol);
            _perfilAbierto.Volver += (s, e) => CerrarPerfil();
            Controls.Add(_perfilAbierto);
            _perfilAbierto.BringToFront();
        }

        private void CerrarPerfil()
        {
            if (_perfilAbierto == null) return;

            var perfil = _perfilAbierto;
            _perfilAbierto = null;
            Controls.Remove(perfil);
            perfil.Dispose();
            foreach (Control c in Controls) c.Visible = true;

            try
            {
                CargarGrilla();
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        private void CargarGrilla()
        {
            _socios = _servicioSocio.ObtenerTodos().ToList();
            AplicarFiltro();
            ActualizarBotonBaja();
        }

        /// <summary>Filtro rápido en memoria por apellido, nombre o documento.</summary>
        private void AplicarFiltro()
        {
            var texto = txtBuscar.Text.Trim();

            var filtrados = string.IsNullOrEmpty(texto)
                ? _socios
                : _socios.Where(s =>
                        s.Apellido.Contains(texto, StringComparison.OrdinalIgnoreCase) ||
                        s.Nombre.Contains(texto, StringComparison.OrdinalIgnoreCase) ||
                        s.Documento.Contains(texto, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            dgvSocios.DataSource = filtrados.ToList();
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e) => AplicarFiltro();

        // ── ABM ──────────────────────────────────────────────────────────────

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            using var editor = new frmSocioEditor(null);
            if (editor.ShowDialog(this) == DialogResult.OK)
            {
                CargarGrilla();
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            var socio = SocioSeleccionado();
            if (socio is null) return;

            using var editor = new frmSocioEditor(socio);
            if (editor.ShowDialog(this) == DialogResult.OK)
            {
                CargarGrilla();
            }
        }

        /// <summary>
        /// Alterna entre dar de baja y reactivar según el estado del socio
        /// seleccionado (ver <see cref="ActualizarBotonBaja"/>).
        /// </summary>
        private void btnDarDeBaja_Click(object sender, EventArgs e)
        {
            var socio = SocioSeleccionado();
            if (socio is null) return;

            if (socio.Activo)
            {
                var respuesta = MessageBox.Show(
                    $"¿Confirmás dar de baja al socio {socio.Apellido}, {socio.Nombre}?",
                    "Confirmar baja", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (respuesta != DialogResult.Yes) return;

                try
                {
                    _servicioSocio.DarDeBaja(socio.IdPersona);
                    CargarGrilla();
                }
                catch (Exception ex)
                {
                    MostrarError(ex);
                }
            }
            else
            {
                var respuesta = MessageBox.Show(
                    $"¿Confirmás dar de alta nuevamente al socio {socio.Apellido}, {socio.Nombre}?",
                    "Confirmar alta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (respuesta != DialogResult.Yes) return;

                try
                {
                    _servicioSocio.Reactivar(socio.IdPersona);
                    CargarGrilla();
                }
                catch (Exception ex)
                {
                    MostrarError(ex);
                }
            }
        }

        // ── Helpers de UI ────────────────────────────────────────────────────

        private Socio? SocioSeleccionado()
        {
            if (dgvSocios.CurrentRow?.DataBoundItem is Socio socio)
            {
                return socio;
            }

            MessageBox.Show("Seleccioná un socio de la grilla.", "SGIG",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return null;
        }

        /// <summary>Cambia el texto de btnDarDeBaja según el estado de la fila seleccionada.</summary>
        private void ActualizarBotonBaja()
        {
            var socio = dgvSocios.CurrentRow?.DataBoundItem as Socio;
            btnDarDeBaja.Text = socio is { Activo: false } ? "Dar de &alta" : "Dar de &baja";
        }

        /// <summary>Convierte la columna booleana Activo en un texto legible.</summary>
        private void DgvSocios_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvSocios.Columns[e.ColumnIndex].Name != "col" + nameof(Socio.Activo)) return;

            if (e.Value is bool activo)
            {
                e.Value = activo ? "Activo" : "Dado de baja";
                e.FormattingApplied = true;
            }
        }

        /// <summary>Atenúa visualmente las filas de socios dados de baja.</summary>
        private void DgvSocios_DataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow fila in dgvSocios.Rows)
            {
                if (fila.DataBoundItem is Socio { Activo: false })
                {
                    fila.DefaultCellStyle.ForeColor = Tema.SlateTexto;
                    fila.DefaultCellStyle.Font = new Font(dgvSocios.Font, FontStyle.Italic);
                }
            }
        }

        /// <summary>
        /// Los errores de negocio se muestran como advertencia (el usuario puede
        /// corregirlos); los de acceso a datos, como error.
        /// </summary>
        private static void MostrarError(Exception ex)
        {
            var esNegocio = ex is NegocioException;

            MessageBox.Show(ex.Message, "SGIG", MessageBoxButtons.OK,
                esNegocio ? MessageBoxIcon.Warning : MessageBoxIcon.Error);
        }
    }
}
