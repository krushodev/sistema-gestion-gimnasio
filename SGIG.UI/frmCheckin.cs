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
    //   txtDocumento (foco automático, dispara con Enter), lblAyuda (texto guía)
    //   btnBuscarPorNombre, btnLimpiar
    //   pnlResultado (verde/rojo), lblResultado, lblNombreSocio
    // ───────────────────────────────────────────────────────────────────────────
    public partial class frmCheckin : Form
    {
        private readonly ServicioCheckin _servicioCheckin = new();

        // Ícono grande del resultado (check / cruz), creado por código sobre pnlResultado.
        private Label lblIconoResultado = null!;

        public frmCheckin()
        {
            InitializeComponent();
            txtDocumento.KeyPress += Grillas.SoloDigitos;
            ConfigurarResultado();
        }

        /// <summary>
        /// Estado neutro inicial del panel de resultado (blanco con borde fino) y el ícono
        /// grande que acompaña al texto, legible a distancia desde el mostrador.
        /// </summary>
        private void ConfigurarResultado()
        {
            lblIconoResultado = new Label
            {
                AutoSize = false,
                Font = Tema.FuenteIconos(26F),
                Size = new Size(48, 40),
                Location = new Point(16, 0),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft
            };
            pnlResultado.Controls.Add(lblIconoResultado);

            pnlResultado.Paint += (s, e) =>
            {
                using var lapiz = new Pen(Tema.Borde);
                e.Graphics.DrawRectangle(lapiz, 0, 0, pnlResultado.Width - 1, pnlResultado.Height - 1);
            };

            MostrarEstadoNeutro();
        }

        private void MostrarEstadoNeutro()
        {
            pnlResultado.BackColor = Tema.Superficie;
            lblIconoResultado.Text = string.Empty;
            lblResultado.ForeColor = Tema.TextoSecundario;
            lblResultado.Text = "Esperando lectura...";
            lblNombreSocio.ForeColor = Tema.TextoPrimario;
            lblNombreSocio.Text = string.Empty;
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

        private void BtnBuscarPorNombre_Click(object? sender, EventArgs e)
        {
            using var selector = new frmBuscarSocio();
            if (selector.ShowDialog(this) == DialogResult.OK && selector.SocioElegido is not null)
            {
                ProcesarCheckin(selector.SocioElegido.Documento);
            }
        }

        private void BtnLimpiar_Click(object? sender, EventArgs e)
        {
            txtDocumento.Clear();
            MostrarEstadoNeutro();
            txtDocumento.Focus();
        }

        private void ProcesarCheckin() => ProcesarCheckin(txtDocumento.Text.Trim());

        private void ProcesarCheckin(string documento)
        {
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
            Color colorTexto = concedido ? Tema.ExitoTexto : Tema.PeligroTexto;
            pnlResultado.BackColor = concedido ? Tema.ExitoFondo : Tema.PeligroFondo;

            lblIconoResultado.ForeColor = colorTexto;
            lblIconoResultado.Text = concedido ? Iconos.Ok : Iconos.Cancelar;

            lblResultado.ForeColor = colorTexto;
            lblResultado.Text = concedido ? "ACCESO CONCEDIDO" : "ACCESO RECHAZADO";

            lblNombreSocio.ForeColor = colorTexto;
            lblNombreSocio.Text = $"{nombreSocio}\n{mensaje}";
        }
    }
}
