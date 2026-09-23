using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.UI
{
    /// <summary>
    /// Historial de accesos de Check-in, para que Recepción pueda ver quién entró y
    /// cuántos días le quedan (o hace cuántos venció) su cuota, sin tener que abrir
    /// la ficha de cada socio. Complementa a <see cref="frmCheckin"/> (que sólo
    /// registra el intento del momento) con una vista de seguimiento.
    /// </summary>
    //
    // ── CONTROLES (ver frmHistorialAccesos.Designer.cs) ──────────────────────
    //   lblDesde, dtpDesde, lblHasta, dtpHasta, lblDocumento, txtDocumento,
    //   btnBuscar, dgvHistorial
    // ───────────────────────────────────────────────────────────────────────────
    public partial class frmHistorialAccesos : Form
    {
        private readonly ServicioCheckin _servicioCheckin = new();

        public frmHistorialAccesos()
        {
            InitializeComponent();
            txtDocumento.KeyPress += Grillas.SoloDigitos;
        }

        private void frmHistorialAccesos_Load(object sender, EventArgs e)
        {
            Grillas.Configurar(dgvHistorial,
                (nameof(Checkin.Documento), "Documento", 90),
                (nameof(Checkin.NombreCompleto), "Nombre y apellido", 160),
                (nameof(Checkin.FechaHora), "Fecha y hora", 120),
                (nameof(Checkin.Resultado), "Resultado", 90),
                (nameof(Checkin.DescripcionVencimiento), "Cuota", 140));

            dtpHasta.Value = DateTime.Today;
            dtpDesde.Value = DateTime.Today.AddDays(-30);

            BuscarHistorial();
        }

        private void btnBuscar_Click(object sender, EventArgs e) => BuscarHistorial();

        private void BuscarHistorial()
        {
            try
            {
                var historial = _servicioCheckin.ObtenerHistorial(
                    dtpDesde.Value.Date, dtpHasta.Value.Date, txtDocumento.Text);

                dgvHistorial.DataSource = historial.ToList();
            }
            catch (Exception ex)
            {
                var esNegocio = ex is NegocioException;
                MessageBox.Show(ex.Message, "SGIG", MessageBoxButtons.OK,
                    esNegocio ? MessageBoxIcon.Warning : MessageBoxIcon.Error);
            }
        }
    }
}
