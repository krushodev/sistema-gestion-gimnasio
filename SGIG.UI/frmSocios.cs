using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.UI
{
    /// <summary>
    /// ABM de socios (RF#05, RF#06, RF#07, RNF#03). Accesible para Administrador y
    /// Recepcionista. Permite reutilizar una Persona ya cargada (por ejemplo, un
    /// Usuario) al dar de alta un socio nuevo, edición y baja lógica con confirmación.
    /// </summary>
    //
    // ── CONTROLES (ver frmSocios.Designer.cs) ────────────────────────────────
    //   txtBuscarDocumento, btnBuscar (RF#06), dgvSocios, btnNuevo, btnEditar,
    //   btnDarDeBaja
    //   Datos de Persona: txtDocumento, cboTipoDocumento, txtNombre, txtApellido,
    //                     txtEmail, txtTelefono, cboLocalidad
    //   Datos de Socio: dtpFechaNacimiento, txtAptoMedico, cboPlan (deshabilitado
    //                   hasta que exista el módulo de Planes, Fase 4),
    //                   lblFechaVencimientoCuota (solo lectura), chkActivo (solo lectura)
    //   btnGuardar, btnCancelar
    // ───────────────────────────────────────────────────────────────────────────
    public partial class frmSocios : Form
    {
        private readonly ServicioSocio _servicioSocio = new();
        private readonly ServicioCatalogo _servicioCatalogo = new();

        /// <summary>id_persona en edición; null cuando se está dando un alta.</summary>
        private int? _idEnEdicion;

        /// <summary>
        /// id_persona de una Persona existente encontrada por <see cref="btnBuscar_Click"/>
        /// que todavía no es socio; se reutiliza al guardar el alta (RF#06).
        /// </summary>
        private int? _idPersonaReutilizada;

        public frmSocios()
        {
            InitializeComponent();

            // RNF#04: el campo documento no acepta letras.
            txtDocumento.KeyPress += Grillas.SoloDigitos;
            txtBuscarDocumento.KeyPress += Grillas.SoloDigitos;
        }

        private void frmSocios_Load(object sender, EventArgs e)
        {
            try
            {
                ConfigurarGrilla();
                CargarCombos();
                CargarGrilla();
                HabilitarPanel(false);
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        /// <summary>
        /// Columnas explícitas: la grilla muestra sólo lo que le sirve a recepción,
        /// sin exponer ids internos.
        /// </summary>
        private void ConfigurarGrilla()
        {
            Grillas.Configurar(dgvSocios,
                (nameof(Socio.Apellido), "Apellido", 100),
                (nameof(Socio.Nombre), "Nombre", 100),
                (nameof(Socio.Documento), "Documento", 80),
                (nameof(Socio.Telefono), "Teléfono", 90),
                (nameof(Socio.Email), "Email", 130),
                (nameof(Socio.FechaVencimientoCuota), "Vencim. cuota", 90));
        }

        // ── Carga de datos ───────────────────────────────────────────────────

        private void CargarCombos()
        {
            cboTipoDocumento.DisplayMember = nameof(TipoDocumento.Descripcion);
            cboTipoDocumento.ValueMember = nameof(TipoDocumento.IdTipoDocumento);
            cboTipoDocumento.DataSource = _servicioCatalogo.ObtenerTiposDocumento().ToList();

            // La localidad es opcional: se agrega una fila vacía al principio.
            var localidades = _servicioCatalogo.ObtenerLocalidades().ToList();
            localidades.Insert(0, new Localidad { IdLocalidad = 0, Nombre = "(sin especificar)" });
            cboLocalidad.DisplayMember = nameof(Localidad.Nombre);
            cboLocalidad.ValueMember = nameof(Localidad.IdLocalidad);
            cboLocalidad.DataSource = localidades;

            // El módulo de Planes todavía no existe (Fase 4, Tesorería): el combo se
            // deja deshabilitado con un texto explicativo hasta que se pueda cargar.
            cboPlan.Items.Add("(disponible cuando se implemente Fase 4 - Tesorería)");
            cboPlan.SelectedIndex = 0;
        }

        private void CargarGrilla()
        {
            dgvSocios.DataSource = _servicioSocio.ObtenerActivos().ToList();
        }

        // ── ABM ──────────────────────────────────────────────────────────────

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            var documento = txtBuscarDocumento.Text.Trim();

            if (string.IsNullOrWhiteSpace(documento))
            {
                MessageBox.Show("Ingresá un documento para buscar.", "SGIG",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                var persona = _servicioSocio.BuscarPersonaPorDocumento(documento);

                LimpiarPanel();
                _idEnEdicion = null;
                _idPersonaReutilizada = null;

                if (persona is null)
                {
                    txtDocumento.Text = documento;
                    MessageBox.Show(
                        "No existe ninguna persona con ese documento. Completá los datos para darla de alta como socio nuevo.",
                        "SGIG", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    _idPersonaReutilizada = persona.IdPersona;

                    txtDocumento.Text = persona.Documento;
                    cboTipoDocumento.SelectedValue = persona.IdTipoDocumento;
                    txtNombre.Text = persona.Nombre;
                    txtApellido.Text = persona.Apellido;
                    txtEmail.Text = persona.Email ?? string.Empty;
                    txtTelefono.Text = persona.Telefono ?? string.Empty;
                    cboLocalidad.SelectedValue = persona.IdLocalidad ?? 0;
                    dtpFechaNacimiento.Value = persona.FechaNacimiento ?? DateTime.Today;

                    MessageBox.Show(
                        "Ya existe una persona registrada con ese documento. Se reutilizan sus datos: completá el resto y guardá para darla de alta como socio.",
                        "SGIG", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                HabilitarPanel(true);
                txtNombre.Focus();
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            _idEnEdicion = null;
            _idPersonaReutilizada = null;
            LimpiarPanel();
            HabilitarPanel(true);
            txtDocumento.Focus();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            var socio = SocioSeleccionado();
            if (socio is null) return;

            _idEnEdicion = socio.IdPersona;
            _idPersonaReutilizada = null;

            txtDocumento.Text = socio.Documento;
            cboTipoDocumento.SelectedValue = socio.IdTipoDocumento;
            txtNombre.Text = socio.Nombre;
            txtApellido.Text = socio.Apellido;
            txtEmail.Text = socio.Email ?? string.Empty;
            txtTelefono.Text = socio.Telefono ?? string.Empty;
            cboLocalidad.SelectedValue = socio.IdLocalidad ?? 0;
            dtpFechaNacimiento.Value = socio.FechaNacimiento ?? DateTime.Today;
            txtAptoMedico.Text = socio.AptoMedico ?? string.Empty;
            lblFechaVencimientoCuota.Text = socio.FechaVencimientoCuota?.ToString("dd/MM/yyyy") ?? "Sin cuota registrada";
            chkActivo.Checked = socio.Activo;

            HabilitarPanel(true);
            txtDocumento.Focus();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            var idLocalidad = (int)(cboLocalidad.SelectedValue ?? 0);

            var socio = new Socio
            {
                IdPersona = _idEnEdicion ?? _idPersonaReutilizada ?? 0,
                Documento = txtDocumento.Text.Trim(),
                IdTipoDocumento = (int)(cboTipoDocumento.SelectedValue ?? 0),
                Nombre = txtNombre.Text.Trim(),
                Apellido = txtApellido.Text.Trim(),
                Email = TextoOpcional(txtEmail),
                Telefono = TextoOpcional(txtTelefono),
                IdLocalidad = idLocalidad > 0 ? idLocalidad : null,
                FechaNacimiento = dtpFechaNacimiento.Value.Date,
                AptoMedico = TextoOpcional(txtAptoMedico)
            };

            try
            {
                Cursor = Cursors.WaitCursor;

                if (_idEnEdicion is null)
                {
                    _servicioSocio.Alta(socio);
                }
                else
                {
                    _servicioSocio.Modificar(socio);
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
            var socio = SocioSeleccionado();
            if (socio is null) return;

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

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            HabilitarPanel(false);
            LimpiarPanel();
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

        private static string? TextoOpcional(TextBox caja) =>
            string.IsNullOrWhiteSpace(caja.Text) ? null : caja.Text.Trim();

        /// <summary>Habilita el panel de edición y deshabilita la grilla, y viceversa.</summary>
        private void HabilitarPanel(bool editando)
        {
            grpDatos.Enabled = editando;
            btnGuardar.Enabled = editando;
            btnCancelar.Enabled = editando;

            dgvSocios.Enabled = !editando;
            txtBuscarDocumento.Enabled = !editando;
            btnBuscar.Enabled = !editando;
            btnNuevo.Enabled = !editando;
            btnEditar.Enabled = !editando;
            btnDarDeBaja.Enabled = !editando;

            // cboPlan y chkActivo son de solo lectura incluso con el panel habilitado.
            cboPlan.Enabled = false;
            chkActivo.Enabled = false;
        }

        private void LimpiarPanel()
        {
            foreach (var caja in new[] { txtDocumento, txtNombre, txtApellido, txtEmail, txtTelefono, txtAptoMedico })
            {
                caja.Clear();
            }

            if (cboTipoDocumento.Items.Count > 0) cboTipoDocumento.SelectedIndex = 0;
            if (cboLocalidad.Items.Count > 0) cboLocalidad.SelectedIndex = 0;
            dtpFechaNacimiento.Value = DateTime.Today.AddYears(-18);
            lblFechaVencimientoCuota.Text = "Sin cuota registrada";
            chkActivo.Checked = true;
            txtBuscarDocumento.Clear();
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
