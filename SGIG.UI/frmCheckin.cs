using System.Drawing;
using SGIG.Negocio;

namespace SGIG.UI
{
    /// <summary>
    /// Control de Acceso (RF#15, RF#17, RNF#01). Un solo campo, sin botones
    /// intermedios: se escribe o escanea el documento y se confirma con Enter.
    /// El resultado se muestra en grande, con feedback visual verde/rojo.
    /// </summary>
    //
    // ── CONTROLES (ver frmCheckin.Designer.cs) ───────────────────────────────
    //   txtDocumento (foco automático, dispara con Enter)
    //   pnlResultado (verde/rojo), lblResultado, lblNombreSocio
    // ───────────────────────────────────────────────────────────────────────────
    public partial class frmCheckin : Form
    {
        private static readonly Color ColorConcedido = Color.FromArgb(220, 252, 231);
        private static readonly Color ColorRechazado = Color.FromArgb(254, 226, 226);
        private static readonly Color ColorTextoConcedido = Color.FromArgb(21, 128, 61);
        private static readonly Color ColorTextoRechazado = Color.FromArgb(185, 28, 28);

        private readonly ServicioCheckin _servicioCheckin = new();

        public frmCheckin()
        {
            InitializeComponent();
            txtDocumento.KeyPress += Grillas.SoloDigitos;
        }

        private void frmCheckin_Load(object sender, EventArgs e)
        {
            txtDocumento.Focus();
        }

        private void txtDocumento_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            e.SuppressKeyPress = true;
            ProcesarCheckin();
        }

        private void ProcesarCheckin()
        {
            var documento = txtDocumento.Text.Trim();
            txtDocumento.Clear();

            if (string.IsNullOrWhiteSpace(documento))
            {
                txtDocumento.Focus();
                return;
            }

            try
            {
                var resultado = _servicioCheckin.RegistrarIntento(documento);
                MostrarResultado(resultado.Concedido, resultado.NombreCompleto, resultado.Mensaje);
            }
            catch (NegocioException ex)
            {
                MostrarResultado(concedido: false, nombreSocio: "—", mensaje: ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SGIG", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                txtDocumento.Focus();
            }
        }

        private void MostrarResultado(bool concedido, string nombreSocio, string mensaje)
        {
            pnlResultado.BackColor = concedido ? ColorConcedido : ColorRechazado;

            lblResultado.ForeColor = concedido ? ColorTextoConcedido : ColorTextoRechazado;
            lblResultado.Text = concedido ? "ACCESO CONCEDIDO" : "ACCESO RECHAZADO";

            lblNombreSocio.ForeColor = concedido ? ColorTextoConcedido : ColorTextoRechazado;
            lblNombreSocio.Text = $"{nombreSocio}\n{mensaje}";
        }
    }
}
