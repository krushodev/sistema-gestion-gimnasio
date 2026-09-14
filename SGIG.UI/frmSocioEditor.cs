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
    //   txtAptoMedico, cboPlan (deshabilitado, placeholder Fase 4),
    //   lblFechaVencimientoCuota (solo lectura), chkActivo (solo lectura)
    //   btnGuardar, btnCancelar
    // ─────────────────────────────────────────────────────────────────────────
    public partial class frmSocioEditor : Form
    {
        private readonly ServicioSocio _servicioSocio = new();
        private readonly ServicioCatalogo _servicioCatalogo = new();
        private readonly Socio? _socioExistente;

        public frmSocioEditor(Socio? socioExistente)
        {
            InitializeComponent();
            _socioExistente = socioExistente;
            Text = socioExistente is null ? "Nuevo socio" : "Editar socio";
        }

        private void frmSocioEditor_Load(object sender, EventArgs e)
        {
            try
            {
                ucDatosPersona.CargarCatalogos(
                    _servicioCatalogo.ObtenerTiposDocumento(),
                    _servicioCatalogo.ObtenerProvincias(),
                    _servicioCatalogo.ObtenerLocalidades());

                // El módulo de Planes todavía no existe (Fase 4, Tesorería): el combo
                // se deja deshabilitado con un texto explicativo hasta que se pueda cargar.
                cboPlan.Items.Add("(disponible cuando se implemente Fase 4 - Tesorería)");
                cboPlan.SelectedIndex = 0;

                if (_socioExistente is null)
                {
                    ucDatosPersona.PrepararParaAlta();
                    txtAptoMedico.Clear();
                    lblFechaVencimientoCuota.Text = "Sin cuota registrada";
                    chkActivo.Checked = true;
                }
                else
                {
                    ucDatosPersona.CargarParaEdicion(_socioExistente);
                    txtAptoMedico.Text = _socioExistente.AptoMedico ?? string.Empty;
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
                AptoMedico = string.IsNullOrWhiteSpace(txtAptoMedico.Text) ? null : txtAptoMedico.Text.Trim()
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
