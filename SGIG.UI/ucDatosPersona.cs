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
    //   txtEmail, txtTelefono, cboLocalidad, dtpFechaNacimiento (ocultable con
    //   MostrarFechaNacimiento)
    // ──────────────────────────────────────────────────────────────────────────
    public partial class ucDatosPersona : UserControl
    {
        private readonly ServicioPersona _servicioPersona = new();

        /// <summary>
        /// id_persona resuelto: null si es alta nueva sin buscar todavía, o un valor
        /// &gt;0 si se reutilizó una Persona existente o se está editando una.
        /// </summary>
        public int? IdPersonaActual { get; private set; }

        public ucDatosPersona()
        {
            InitializeComponent();
            txtDocumento.KeyPress += Grillas.SoloDigitos;
        }

        /// <summary>Oculta el campo de fecha de nacimiento (no aplica al alta de Usuario).</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool MostrarFechaNacimiento
        {
            get => lblFechaNacimiento.Visible;
            set => lblFechaNacimiento.Visible = dtpFechaNacimiento.Visible = value;
        }

        public void CargarCatalogos(IEnumerable<TipoDocumento> tiposDocumento, IEnumerable<Localidad> localidades)
        {
            cboTipoDocumento.DisplayMember = nameof(TipoDocumento.Descripcion);
            cboTipoDocumento.ValueMember = nameof(TipoDocumento.IdTipoDocumento);
            cboTipoDocumento.DataSource = tiposDocumento.ToList();

            // La localidad es opcional: se agrega una fila vacía al principio.
            var listaLocalidades = localidades.ToList();
            listaLocalidades.Insert(0, new Localidad { IdLocalidad = 0, Nombre = "(sin especificar)" });
            cboLocalidad.DisplayMember = nameof(Localidad.Nombre);
            cboLocalidad.ValueMember = nameof(Localidad.IdLocalidad);
            cboLocalidad.DataSource = listaLocalidades;
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
            cboLocalidad.SelectedValue = persona.IdLocalidad ?? 0;
            dtpFechaNacimiento.Value = persona.FechaNacimiento ?? DateTime.Today.AddYears(-18);

            HabilitarCampos(true);
            btnBuscar.Enabled = false;
        }

        public void Limpiar()
        {
            foreach (var caja in new[] { txtDocumento, txtNombre, txtApellido, txtEmail, txtTelefono })
            {
                caja.Clear();
            }

            if (cboTipoDocumento.Items.Count > 0) cboTipoDocumento.SelectedIndex = 0;
            if (cboLocalidad.Items.Count > 0) cboLocalidad.SelectedIndex = 0;
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
                cboLocalidad.SelectedValue = persona.IdLocalidad ?? 0;
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
