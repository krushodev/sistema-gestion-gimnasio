using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.UI
{
    /// <summary>
    /// Listado de usuarios del sistema (RF#03, RNF#03). Sólo accesible para el rol
    /// Administrador. El alta y la edición se hacen en el diálogo modal
    /// <see cref="frmUsuarioEditor"/>; esta pantalla lista a todos (activos y dados
    /// de baja, distinguidos por estado) y permite reactivar sin pasar por el editor.
    /// </summary>
    //
    // ── CONTROLES (ver frmUsuarios.Designer.cs) ──────────────────────────────
    //   txtBuscar (filtro rápido), dgvUsuarios, btnNuevo, btnEditar, btnDarDeBaja
    //   (btnDarDeBaja alterna su texto y acción según el estado de la fila
    //   seleccionada: "Dar de baja" para un usuario activo, "Dar de alta" para uno
    //   dado de baja — no hace falta agregar un botón nuevo)
    // ─────────────────────────────────────────────────────────────────────────
    public partial class frmUsuarios : Form
    {
        private readonly ServicioUsuario _servicioUsuario = new();
        private readonly Usuario _usuarioLogueado;

        private List<Usuario> _usuarios = new();

        public frmUsuarios(Usuario usuarioLogueado)
        {
            InitializeComponent();
            _usuarioLogueado = usuarioLogueado;
            dgvUsuarios.CellFormatting += DgvUsuarios_CellFormatting;
            dgvUsuarios.DataBindingComplete += DgvUsuarios_DataBindingComplete;
            dgvUsuarios.SelectionChanged += (s, e) => ActualizarBotonBaja();
        }

        private void frmUsuarios_Load(object sender, EventArgs e)
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
        /// Columnas explícitas: la grilla muestra sólo lo que le sirve al administrador.
        /// Sin esto se autogeneraría una columna por propiedad, incluido el hash de la
        /// contraseña y todos los ids internos. Se incluye el estado (Activo/Dado de
        /// baja) para que un usuario dado de baja siga siendo visible y se pueda reactivar.
        /// </summary>
        private void ConfigurarGrilla()
        {
            Grillas.Configurar(dgvUsuarios,
                (nameof(Usuario.Apellido), "Apellido", 100),
                (nameof(Usuario.Nombre), "Nombre", 100),
                (nameof(Usuario.Documento), "Documento", 80),
                (nameof(Usuario.NombreUsuario), "Usuario", 90),
                (nameof(Usuario.NombreRol), "Rol", 90),
                (nameof(Usuario.Legajo), "Legajo", 70),
                (nameof(Usuario.Email), "Correo electrónico", 150),
                (nameof(Usuario.Activo), "Estado", 70));
        }

        private void CargarGrilla()
        {
            _usuarios = _servicioUsuario.ObtenerTodos().ToList();
            AplicarFiltro();
            ActualizarBotonBaja();
        }

        /// <summary>Filtro rápido en memoria por apellido, nombre, documento, legajo o usuario.</summary>
        private void AplicarFiltro()
        {
            var texto = txtBuscar.Text.Trim();

            var filtrados = string.IsNullOrEmpty(texto)
                ? _usuarios
                : _usuarios.Where(u =>
                        u.Apellido.Contains(texto, StringComparison.OrdinalIgnoreCase) ||
                        u.Nombre.Contains(texto, StringComparison.OrdinalIgnoreCase) ||
                        u.Documento.Contains(texto, StringComparison.OrdinalIgnoreCase) ||
                        u.Legajo.Contains(texto, StringComparison.OrdinalIgnoreCase) ||
                        u.NombreUsuario.Contains(texto, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            dgvUsuarios.DataSource = filtrados.ToList();
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e) => AplicarFiltro();

        // ── ABM ──────────────────────────────────────────────────────────────

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            using var editor = new frmUsuarioEditor(null);
            if (editor.ShowDialog(this) == DialogResult.OK)
            {
                CargarGrilla();
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            var usuario = UsuarioSeleccionado();
            if (usuario is null) return;

            using var editor = new frmUsuarioEditor(usuario);
            if (editor.ShowDialog(this) == DialogResult.OK)
            {
                CargarGrilla();
            }
        }

        /// <summary>
        /// Alterna entre dar de baja y reactivar según el estado del usuario
        /// seleccionado (ver <see cref="ActualizarBotonBaja"/>).
        /// </summary>
        private void btnDarDeBaja_Click(object sender, EventArgs e)
        {
            var usuario = UsuarioSeleccionado();
            if (usuario is null) return;

            if (usuario.Activo)
            {
                var respuesta = MessageBox.Show(
                    $"¿Confirmás dar de baja al usuario {usuario.Apellido}, {usuario.Nombre} ({usuario.NombreUsuario})?",
                    "Confirmar baja", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (respuesta != DialogResult.Yes) return;

                try
                {
                    _servicioUsuario.DarDeBaja(usuario.IdPersona, _usuarioLogueado.IdPersona);
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
                    $"¿Confirmás dar de alta nuevamente al usuario {usuario.Apellido}, {usuario.Nombre} ({usuario.NombreUsuario})?",
                    "Confirmar alta", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (respuesta != DialogResult.Yes) return;

                try
                {
                    _servicioUsuario.Reactivar(usuario.IdPersona);
                    CargarGrilla();
                }
                catch (Exception ex)
                {
                    MostrarError(ex);
                }
            }
        }

        // ── Helpers de UI ────────────────────────────────────────────────────

        private Usuario? UsuarioSeleccionado()
        {
            if (dgvUsuarios.CurrentRow?.DataBoundItem is Usuario usuario)
            {
                return usuario;
            }

            MessageBox.Show("Seleccioná un usuario de la grilla.", "SGIG",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return null;
        }

        /// <summary>Cambia el texto de btnDarDeBaja según el estado de la fila seleccionada.</summary>
        private void ActualizarBotonBaja()
        {
            var usuario = dgvUsuarios.CurrentRow?.DataBoundItem as Usuario;
            btnDarDeBaja.Text = usuario is { Activo: false } ? "Dar de &alta" : "Dar de &baja";
        }

        /// <summary>Convierte la columna booleana Activo en un texto legible.</summary>
        private void DgvUsuarios_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvUsuarios.Columns[e.ColumnIndex].Name != "col" + nameof(Usuario.Activo)) return;

            if (e.Value is bool activo)
            {
                e.Value = activo ? "Activo" : "Dado de baja";
                e.FormattingApplied = true;
            }
        }

        /// <summary>Atenúa visualmente las filas de usuarios dados de baja.</summary>
        private void DgvUsuarios_DataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow fila in dgvUsuarios.Rows)
            {
                if (fila.DataBoundItem is Usuario { Activo: false })
                {
                    fila.DefaultCellStyle.ForeColor = Tema.SlateTexto;
                    fila.DefaultCellStyle.Font = new Font(dgvUsuarios.Font, FontStyle.Italic);
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
