using System.Collections.Generic;
using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.UI
{
    /// <summary>
    /// Alta/edición de un socio (RF#05, RF#06, RF#07), en un diálogo modal separado
    /// de la grilla de <see cref="frmSocios"/>. Reutiliza <see cref="ucDatosPersona"/>
    /// para los campos de Persona, con autocompletado si el documento ya existe.
    /// </summary>
    //
    // ── CONTROLES (ver frmSocioEditor.Designer.cs) ───────────────────────────
    //   ucDatosPersona (campos de Persona + buscar/reutilizar)
    //   txtAptoMedico, cboPlan,
    //   lblFechaVencimientoCuota (solo lectura), chkActivo (solo lectura)
    //   btnGuardar, btnCancelar
    // ─────────────────────────────────────────────────────────────────────────
    public partial class frmSocioEditor : Form
    {
        private readonly ServicioSocio _servicioSocio = new();
        private readonly ServicioCatalogo _servicioCatalogo = new();
        private readonly ServicioPlan _servicioPlan = new();
        private readonly Socio? _socioExistente;
        private readonly Button _btnLimpiar;

        public frmSocioEditor(Socio? socioExistente)
        {
            InitializeComponent();

            _btnLimpiar = new Button
            {
                Text = "Limpiar datos",
                Location = new Point(16, 266),
                Size = new Size(120, 28),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            _btnLimpiar.Click += (s, e) =>
            {
                ucDatosPersona.Reiniciar();
                txtAptoMedico.Clear();
                cboPlan.SelectedValue = 0;
            };
            Controls.Add(_btnLimpiar);

            Tema.EstilizarFormulario(this);
            Tema.EstilizarControles(this);
            _socioExistente = socioExistente;
            Text = socioExistente is null ? "Nuevo socio" : "Editar socio";

            // Sólo tiene sentido en un alta nueva: es lo que deshace el autocompletado
            // bloqueado por ucDatosPersona.btnBuscar_Click cuando el documento ya
            // pertenecía a otra Persona (RF#06).
            _btnLimpiar.Visible = socioExistente is null;
        }

        private void frmSocioEditor_Load(object sender, EventArgs e)
        {
            try
            {
                ucDatosPersona.CargarCatalogos(
                    _servicioCatalogo.ObtenerTiposDocumento(),
                    _servicioCatalogo.ObtenerProvincias(),
                    _servicioCatalogo.ObtenerLocalidades());

                var planes = new List<Plan> { new() { IdPlan = 0, Nombre = "(sin plan)" } };
                planes.AddRange(_servicioPlan.Listar());
                cboPlan.DataSource = planes;
                cboPlan.DisplayMember = nameof(Plan.Nombre);
                cboPlan.ValueMember = nameof(Plan.IdPlan);
                cboPlan.Enabled = true;

                if (_socioExistente is null)
                {
                    ucDatosPersona.PrepararParaAlta();
                    txtAptoMedico.Clear();
                    cboPlan.SelectedValue = 0;
                    lblFechaVencimientoCuota.Text = "Sin cuota registrada";
                    chkActivo.Checked = true;
                }
                else
                {
                    ucDatosPersona.CargarParaEdicion(_socioExistente);
                    txtAptoMedico.Text = _socioExistente.AptoMedico ?? string.Empty;
                    cboPlan.SelectedValue = _socioExistente.IdPlan ?? 0;
                    lblFechaVencimientoCuota.Text =
                        _socioExistente.FechaVencimientoCuota?.ToString("dd/MM/yyyy") ?? "Sin cuota registrada";
                    chkActivo.Checked = _socioExistente.Activo;
                }
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            var socio = new Socio
            {
                IdPersona = _socioExistente?.IdPersona ?? ucDatosPersona.IdPersonaActual ?? 0,
                Documento = ucDatosPersona.Documento,
                IdTipoDocumento = ucDatosPersona.IdTipoDocumento,
                Nombre = ucDatosPersona.Nombre,
                Apellido = ucDatosPersona.Apellido,
                Email = ucDatosPersona.Email,
                Telefono = ucDatosPersona.Telefono,
                IdLocalidad = ucDatosPersona.IdLocalidad,
                FechaNacimiento = ucDatosPersona.FechaNacimiento,
                AptoMedico = string.IsNullOrWhiteSpace(txtAptoMedico.Text) ? null : txtAptoMedico.Text.Trim(),
                IdPlan = cboPlan.SelectedValue is int idPlan && idPlan > 0 ? idPlan : null
            };

            try
            {
                Cursor = Cursors.WaitCursor;

                if (_socioExistente is null)
                {
                    _servicioSocio.Alta(socio);
                }
                else
                {
                    _servicioSocio.Modificar(socio);
                }

                DialogResult = DialogResult.OK;
                Close();
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

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
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
