using SGIG.Datos;
using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.UI
{
    /// <summary>
    /// ABM de usuarios del sistema (RF#03, RNF#03). Sólo accesible para el rol
    /// Administrador. Alta unificada de Persona + Usuario en una transacción,
    /// edición, y baja lógica con confirmación.
    /// </summary>
    //
    // ── CONTROLES (ver frmUsuarios.Designer.cs) ──────────────────────────────
    //   dgvUsuarios, txtBuscar, btnNuevo, btnEditar, btnDarDeBaja
    //   Datos de Persona: txtDocumento, cboTipoDocumento, txtNombre, txtApellido,
    //                     txtEmail, txtTelefono, cboLocalidad
    //   Datos de Usuario: txtLegajo, dtpFechaIngreso, cboRol, txtNombreUsuario,
    //                     txtContrasenia
    //   btnGuardar, btnCancelar
    // ─────────────────────────────────────────────────────────────────────────
    public partial class frmUsuarios : Form
    {
        private readonly ServicioUsuario _servicioUsuario = new();
        private readonly ServicioCatalogo _servicioCatalogo = new();
        private readonly Usuario _usuarioLogueado;

        private List<Usuario> _usuarios = new();

        /// <summary>id_persona en edición; null cuando se está dando un alta.</summary>
        private int? _idEnEdicion;

        public frmUsuarios(Usuario usuarioLogueado)
        {
            InitializeComponent();
            _usuarioLogueado = usuarioLogueado;
        }

        private void frmUsuarios_Load(object sender, EventArgs e)
        {
            try
            {
                CargarCombos();
                CargarGrilla();
                HabilitarPanel(false);
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        // ── Carga de datos ───────────────────────────────────────────────────

        private void CargarCombos()
        {
            cboRol.DisplayMember = nameof(Rol.NombreRol);
            cboRol.ValueMember = nameof(Rol.IdRol);
            cboRol.DataSource = _servicioUsuario.ObtenerRoles().ToList();

            cboTipoDocumento.DisplayMember = nameof(TipoDocumento.Descripcion);
            cboTipoDocumento.ValueMember = nameof(TipoDocumento.IdTipoDocumento);
            cboTipoDocumento.DataSource = _servicioCatalogo.ObtenerTiposDocumento().ToList();

            // La localidad es opcional: se agrega una fila vacía al principio.
            var localidades = _servicioCatalogo.ObtenerLocalidades().ToList();
            localidades.Insert(0, new Localidad { IdLocalidad = 0, Nombre = "(sin especificar)" });
            cboLocalidad.DisplayMember = nameof(Localidad.Nombre);
            cboLocalidad.ValueMember = nameof(Localidad.IdLocalidad);
            cboLocalidad.DataSource = localidades;
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
            _idEnEdicion = null;
            LimpiarPanel();
            HabilitarPanel(true);
            lblAyudaContrasenia.Text = "Obligatoria.";
            txtDocumento.Focus();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            var usuario = UsuarioSeleccionado();
            if (usuario is null) return;

            _idEnEdicion = usuario.IdPersona;

            txtDocumento.Text = usuario.Documento;
            cboTipoDocumento.SelectedValue = usuario.IdTipoDocumento;
            txtNombre.Text = usuario.Nombre;
            txtApellido.Text = usuario.Apellido;
            txtEmail.Text = usuario.Email ?? string.Empty;
            txtTelefono.Text = usuario.Telefono ?? string.Empty;
            cboLocalidad.SelectedValue = usuario.IdLocalidad ?? 0;

            txtLegajo.Text = usuario.Legajo;
            dtpFechaIngreso.Value = usuario.FechaIngreso ?? DateTime.Today;
            cboRol.SelectedValue = usuario.IdRol;
            txtNombreUsuario.Text = usuario.NombreUsuario;
            txtContrasenia.Clear();

            lblAyudaContrasenia.Text = "Dejar vacía para no cambiarla.";
            HabilitarPanel(true);
            txtDocumento.Focus();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            var idLocalidad = (int)(cboLocalidad.SelectedValue ?? 0);

            var usuario = new Usuario
            {
                IdPersona = _idEnEdicion ?? 0,
                Documento = txtDocumento.Text.Trim(),
                IdTipoDocumento = (int)(cboTipoDocumento.SelectedValue ?? 0),
                Nombre = txtNombre.Text.Trim(),
                Apellido = txtApellido.Text.Trim(),
                Email = TextoOpcional(txtEmail),
                Telefono = TextoOpcional(txtTelefono),
                IdLocalidad = idLocalidad > 0 ? idLocalidad : null,
                Legajo = txtLegajo.Text.Trim(),
                FechaIngreso = dtpFechaIngreso.Value.Date,
                IdRol = (int)(cboRol.SelectedValue ?? 0),
                NombreUsuario = txtNombreUsuario.Text.Trim()
            };

            try
            {
                Cursor = Cursors.WaitCursor;

                if (_idEnEdicion is null)
                {
                    _servicioUsuario.Alta(usuario, txtContrasenia.Text);
                }
                else
                {
                    _servicioUsuario.Modificar(usuario, txtContrasenia.Text);
                }

                CargarGrilla();
                HabilitarPanel(false);
                LimpiarPanel();
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
            finally
            {
                Cursor = Cursors.Default;
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

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            HabilitarPanel(false);
            LimpiarPanel();
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

        private static string? TextoOpcional(TextBox caja) =>
            string.IsNullOrWhiteSpace(caja.Text) ? null : caja.Text.Trim();

        /// <summary>Habilita el panel de edición y deshabilita la grilla, y viceversa.</summary>
        private void HabilitarPanel(bool editando)
        {
            grpDatos.Enabled = editando;
            btnGuardar.Enabled = editando;
            btnCancelar.Enabled = editando;

            dgvUsuarios.Enabled = !editando;
            txtBuscar.Enabled = !editando;
            btnNuevo.Enabled = !editando;
            btnEditar.Enabled = !editando;
            btnDarDeBaja.Enabled = !editando;
        }

        private void LimpiarPanel()
        {
            foreach (var caja in new[] { txtDocumento, txtNombre, txtApellido, txtEmail,
                                         txtTelefono, txtLegajo, txtNombreUsuario, txtContrasenia })
            {
                caja.Clear();
            }

            if (cboTipoDocumento.Items.Count > 0) cboTipoDocumento.SelectedIndex = 0;
            if (cboLocalidad.Items.Count > 0) cboLocalidad.SelectedIndex = 0;
            if (cboRol.Items.Count > 0) cboRol.SelectedIndex = 0;
            dtpFechaIngreso.Value = DateTime.Today;
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
