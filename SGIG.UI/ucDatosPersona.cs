using System.ComponentModel;
using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.UI
{
    /// <summary>
    /// Campos de dbo.Persona, compartidos entre <see cref="frmSocioEditor"/> y
    /// <see cref="frmUsuarioEditor"/>. Al buscar por documento, si la persona ya
    /// existe (por ejemplo, ya es Socio y ahora se le crea un Usuario, o viceversa)
    /// autocompleta y bloquea sus datos: no se pisan desde acá, sólo se reutilizan.
    /// </summary>
    //
    // ── CONTROLES (ver ucDatosPersona.Designer.cs) ───────────────────────────
    //   txtDocumento, btnBuscar, cboTipoDocumento, txtNombre, txtApellido,
    //   txtEmail, txtTelefono, cboProvincia, cboLocalidad (filtrado según la
    //   provincia elegida en cboProvincia), dtpFechaNacimiento (ocultable con
    //   MostrarFechaNacimiento)
    //
    //   Control nuevo a agregar con el diseñador: lblProvincia (Label, texto
    //   "Provincia:") + cboProvincia (ComboBox, DropDownStyle = DropDownList),
    //   ubicado arriba de cboLocalidad. El evento SelectedIndexChanged de
    //   cboProvincia se suscribe por código en el constructor, no hace falta
    //   tocarlo desde el diseñador.
    // ──────────────────────────────────────────────────────────────────────────
    public partial class ucDatosPersona : UserControl
    {
        private readonly ServicioPersona _servicioPersona = new();

        /// <summary>Todas las localidades activas, para filtrar en memoria por provincia.</summary>
        private List<Localidad> _todasLasLocalidades = new();

        /// <summary>
        /// id_persona resuelto: null si es alta nueva sin buscar todavía, o un valor
        /// &gt;0 si se reutilizó una Persona existente o se está editando una.
        /// </summary>
        public int? IdPersonaActual { get; private set; }

        public ucDatosPersona()
        {
            InitializeComponent();
            txtDocumento.KeyPress += Grillas.SoloDigitos;
            cboProvincia.SelectedIndexChanged += cboProvincia_SelectedIndexChanged;
        }

        /// <summary>Oculta el campo de fecha de nacimiento (no aplica al alta de Usuario).</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool MostrarFechaNacimiento
        {
            get => lblFechaNacimiento.Visible;
            set => lblFechaNacimiento.Visible = dtpFechaNacimiento.Visible = value;
        }

        public void CargarCatalogos(
            IEnumerable<TipoDocumento> tiposDocumento,
            IEnumerable<Provincia> provincias,
            IEnumerable<Localidad> localidades)
        {
            cboTipoDocumento.DisplayMember = nameof(TipoDocumento.Descripcion);
            cboTipoDocumento.ValueMember = nameof(TipoDocumento.IdTipoDocumento);
            cboTipoDocumento.DataSource = tiposDocumento.ToList();

            // Se guarda la lista completa para filtrar cboLocalidad en memoria
            // cada vez que cambia la provincia elegida, sin volver a consultar la base.
            _todasLasLocalidades = localidades.ToList();

            // La provincia es opcional: se agrega una fila vacía al principio.
            var listaProvincias = provincias.ToList();
            listaProvincias.Insert(0, new Provincia { IdProvincia = 0, Nombre = "(sin especificar)" });
            cboProvincia.DisplayMember = nameof(Provincia.Nombre);
            cboProvincia.ValueMember = nameof(Provincia.IdProvincia);
            cboProvincia.DataSource = listaProvincias;
        }

        /// <summary>
        /// Recarga cboLocalidad con sólo las localidades de la provincia elegida en
        /// cboProvincia (o vacío, si no se eligió ninguna). Se dispara sola al
        /// cambiar cboProvincia, y también se llama a mano después de fijar
        /// cboProvincia por código (búsqueda por documento, edición, limpiar).
        /// </summary>
        private void cboProvincia_SelectedIndexChanged(object? sender, EventArgs e) =>
            ActualizarLocalidadesSegunProvincia();

        private void ActualizarLocalidadesSegunProvincia()
        {
            var idProvincia = (int)(cboProvincia.SelectedValue ?? 0);

            var localidadesFiltradas = idProvincia > 0
                ? _todasLasLocalidades.Where(l => l.IdProvincia == idProvincia).ToList()
                : new List<Localidad>();

            // La localidad es opcional: se agrega una fila vacía al principio.
            localidadesFiltradas.Insert(0, new Localidad { IdLocalidad = 0, Nombre = "(sin especificar)" });
            cboLocalidad.DisplayMember = nameof(Localidad.Nombre);
            cboLocalidad.ValueMember = nameof(Localidad.IdLocalidad);
            cboLocalidad.DataSource = localidadesFiltradas;
        }

        /// <summary>
        /// Selecciona en cboProvincia la provincia dueña de <paramref name="idLocalidad"/>
        /// (lo que dispara el filtrado de cboLocalidad) y recién ahí selecciona la
        /// localidad. Usado al buscar por documento y al editar un registro existente.
        /// </summary>
        private void SeleccionarLocalidad(int? idLocalidad)
        {
            var localidad = idLocalidad.HasValue
                ? _todasLasLocalidades.FirstOrDefault(l => l.IdLocalidad == idLocalidad.Value)
                : null;

            cboProvincia.SelectedValue = localidad?.IdProvincia ?? 0;
            cboLocalidad.SelectedValue = idLocalidad ?? 0;
        }

        /// <summary>Deja el control listo para un alta nueva: vacío, editable, sin persona resuelta.</summary>
        public void PrepararParaAlta()
        {
            IdPersonaActual = null;
            Limpiar();
            HabilitarCampos(true);
            btnBuscar.Enabled = true;
            txtDocumento.Focus();
        }

        /// <summary>
        /// Alias de <see cref="PrepararParaAlta"/> pensado para un botón "Limpiar"/"Nuevo"
        /// que el formulario contenedor ofrece después de que <see cref="btnBuscar_Click"/>
        /// bloqueó los campos con los datos de una Persona reutilizada.
        /// </summary>
        public void Reiniciar() => PrepararParaAlta();

        /// <summary>
        /// Precarga los datos de una Persona ya existente para editarla (no pasa por
        /// el flujo de "buscar y bloquear": el registro ya se sabe existente).
        /// </summary>
        public void CargarParaEdicion(Persona persona)
        {
            IdPersonaActual = persona.IdPersona;

            txtDocumento.Text = persona.Documento;
            cboTipoDocumento.SelectedValue = persona.IdTipoDocumento;
            txtNombre.Text = persona.Nombre;
            txtApellido.Text = persona.Apellido;
            txtEmail.Text = persona.Email ?? string.Empty;
            txtTelefono.Text = persona.Telefono ?? string.Empty;
            SeleccionarLocalidad(persona.IdLocalidad);
            dtpFechaNacimiento.Value = persona.FechaNacimiento ?? DateTime.Today.AddYears(-18);

            HabilitarCampos(true);
            btnBuscar.Enabled = false;
        }

        /// <summary>
        /// Bloquea documento y tipo de documento sin afectar el resto de los campos
        /// (que siguen editables). Usado por frmConfiguracion: un usuario puede
        /// editar sus propios datos de contacto, pero no su documento de identidad.
        /// </summary>
        public void BloquearIdentidad()
        {
            txtDocumento.Enabled = false;
            cboTipoDocumento.Enabled = false;
        }

        public void Limpiar()
        {
            foreach (var caja in new[] { txtDocumento, txtNombre, txtApellido, txtEmail, txtTelefono })
            {
                caja.Clear();
            }

            if (cboTipoDocumento.Items.Count > 0) cboTipoDocumento.SelectedIndex = 0;
            if (cboProvincia.Items.Count > 0) cboProvincia.SelectedIndex = 0;
            ActualizarLocalidadesSegunProvincia();
            dtpFechaNacimiento.Value = DateTime.Today.AddYears(-18);
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            var documento = txtDocumento.Text.Trim();

            if (string.IsNullOrWhiteSpace(documento))
            {
                MessageBox.Show("Ingresá un documento para buscar.", "SGIG",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                var persona = _servicioPersona.BuscarPorDocumento(documento);

                if (persona is null)
                {
                    IdPersonaActual = null;
                    HabilitarCampos(true);
                    MessageBox.Show(
                        "No existe ninguna persona con ese documento. Completá los datos para darla de alta.",
                        "SGIG", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtNombre.Focus();
                    return;
                }

                IdPersonaActual = persona.IdPersona;

                cboTipoDocumento.SelectedValue = persona.IdTipoDocumento;
                txtNombre.Text = persona.Nombre;
                txtApellido.Text = persona.Apellido;
                txtEmail.Text = persona.Email ?? string.Empty;
                txtTelefono.Text = persona.Telefono ?? string.Empty;
                SeleccionarLocalidad(persona.IdLocalidad);
                dtpFechaNacimiento.Value = persona.FechaNacimiento ?? DateTime.Today.AddYears(-18);

                // Se bloquean los datos de Persona: no se pisan desde acá, sólo se reutilizan.
                HabilitarCampos(false);

                MessageBox.Show(
                    "Ya existe una persona registrada con ese documento. Se reutilizan sus datos.",
                    "SGIG", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                var esNegocio = ex is NegocioException;
                MessageBox.Show(ex.Message, "SGIG", MessageBoxButtons.OK,
                    esNegocio ? MessageBoxIcon.Warning : MessageBoxIcon.Error);
            }
        }

        private void HabilitarCampos(bool habilitados)
        {
            cboTipoDocumento.Enabled = habilitados;
            txtNombre.Enabled = habilitados;
            txtApellido.Enabled = habilitados;
            txtEmail.Enabled = habilitados;
            txtTelefono.Enabled = habilitados;
            cboProvincia.Enabled = habilitados;
            cboLocalidad.Enabled = habilitados;
            dtpFechaNacimiento.Enabled = habilitados;
        }

        // ── Datos armados para persistir ─────────────────────────────────────

        public string Documento => txtDocumento.Text.Trim();
        public int IdTipoDocumento => (int)(cboTipoDocumento.SelectedValue ?? 0);
        public string Nombre => txtNombre.Text.Trim();
        public string Apellido => txtApellido.Text.Trim();
        public string? Email => string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim();
        public string? Telefono => string.IsNullOrWhiteSpace(txtTelefono.Text) ? null : txtTelefono.Text.Trim();
        public int? IdLocalidad
        {
            get
            {
                var idLocalidad = (int)(cboLocalidad.SelectedValue ?? 0);
                return idLocalidad > 0 ? idLocalidad : null;
            }
        }
        public DateTime FechaNacimiento => dtpFechaNacimiento.Value.Date;
    }
}
