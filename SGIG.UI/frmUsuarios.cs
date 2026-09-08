using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.UI
{
    /// <summary>
    /// Listado de usuarios del sistema (RF#03, RNF#03). Sólo accesible para el rol
    /// Administrador. El alta y la edición se hacen en el diálogo modal
    /// <see cref="frmUsuarioEditor"/>; esta pantalla sólo lista y filtra.
    /// </summary>
    //
    // ── CONTROLES (ver frmUsuarios.Designer.cs) ──────────────────────────────
    //   txtBuscar (filtro rápido), dgvUsuarios, btnNuevo, btnEditar, btnDarDeBaja
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
        /// contraseña y todos los ids internos.
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
                (nameof(Usuario.Email), "Email", 130));
        }

        private void CargarGrilla()
        {
            _usuarios = _servicioUsuario.ObtenerActivos().ToList();
            AplicarFiltro();
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

        private void btnDarDeBaja_Click(object sender, EventArgs e)
        {
            var usuario = UsuarioSeleccionado();
            if (usuario is null) return;

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
