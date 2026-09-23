using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.UI
{
    /// <summary>
    /// Listado de socios (RF#05, RF#06, RF#07, RNF#03). Accesible para Administrador
    /// y Recepcionista. El alta y la edición se hacen en el diálogo modal
    /// <see cref="frmSocioEditor"/>; esta pantalla sólo lista y filtra.
    /// </summary>
    //
    // ── CONTROLES (ver frmSocios.Designer.cs) ────────────────────────────────
    //   txtBuscar (filtro rápido), dgvSocios, btnNuevo, btnEditar, btnDarDeBaja
    // ───────────────────────────────────────────────────────────────────────────
    public partial class frmSocios : Form
    {
        private readonly ServicioSocio _servicioSocio = new();
        private List<Socio> _socios = new();

        public frmSocios()
        {
            InitializeComponent();
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
        /// sin exponer ids internos.
        /// </summary>
        private void ConfigurarGrilla()
        {
            Grillas.Configurar(dgvSocios,
                (nameof(Socio.Apellido), "Apellido", 100),
                (nameof(Socio.Nombre), "Nombre", 100),
                (nameof(Socio.Documento), "Documento", 80),
                (nameof(Socio.Telefono), "Teléfono", 90),
                (nameof(Socio.Email), "Correo electrónico", 150),
                (nameof(Socio.FechaVencimientoCuota), "Vencim. cuota", 90));
        }

        private void CargarGrilla()
        {
            _socios = _servicioSocio.ObtenerActivos().ToList();
            AplicarFiltro();
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
