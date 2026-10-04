using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using SGIG.Negocio;

namespace SGIG.UI
{
    public partial class frmBackup : Form
    {
        private readonly ServicioBackup _servicioBackup;

        public frmBackup()
        {
            InitializeComponent();
            Tema.EstilizarFormulario(this);
            Tema.EstilizarControles(this);
            _servicioBackup = new ServicioBackup();
        }

        private async void btnGenerarBackup_Click(object sender, EventArgs e)
        {
            using var sfd = new SaveFileDialog
            {
                Filter = "Archivo de copia de seguridad SQL (*.bak)|*.bak",
                FileName = $"SGIG_Backup_{DateTime.Now:yyyyMMdd_HHmm}.bak",
                Title = "Guardar Copia de Seguridad"
            };

            if (sfd.ShowDialog() != DialogResult.OK) return;

            string rutaSeleccionada = sfd.FileName;

            BloquearUI("Generando copia de seguridad en SQL Server...");

            try
            {
                await Task.Run(() => _servicioBackup.GenerarCopiaSeguridad(rutaSeleccionada));

                lblEstado.Text = "Copia de seguridad generada con éxito.";
                prgProgreso.Style = ProgressBarStyle.Blocks;
                prgProgreso.Value = 100;

                MessageBox.Show(
                    $"Copia de seguridad generada con éxito en:\n\n{rutaSeleccionada}",
                    "Copia de Seguridad Completada",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblEstado.Text = "Error al generar la copia de seguridad.";
                prgProgreso.Style = ProgressBarStyle.Blocks;
                prgProgreso.Value = 0;

                MessageBox.Show(
                    $"Ocurrió un error al realizar el respaldo:\n\n{ex.Message}",
                    "Error de Backup",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                DesbloquearUI();
            }
        }

        private async void btnRestaurar_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Filter = "Archivo de copia de seguridad SQL (*.bak)|*.bak",
                Title = "Seleccionar Archivo de Respaldo"
            };

            if (ofd.ShowDialog() != DialogResult.OK) return;

            var confirmacion = MessageBox.Show(
                $"¿Confirmás que deseás restaurar la base con el archivo seleccionado?\n\n{ofd.FileName}\n\nLos cambios no guardados se sobreescribirán y se cerrarán conexiones activas.",
                "Confirmación de Restauración",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmacion != DialogResult.Yes) return;

            string rutaSeleccionada = ofd.FileName;

            BloquearUI("Restaurando base de datos desde el backup...");

            try
            {
                await Task.Run(() => _servicioBackup.RestaurarCopiaSeguridad(rutaSeleccionada));

                lblEstado.Text = "Base de datos restaurada con éxito.";
                prgProgreso.Style = ProgressBarStyle.Blocks;
                prgProgreso.Value = 100;

                MessageBox.Show(
                    "La base de datos se ha restaurado correctamente.\nSe recomienda reiniciar las vistas o reconectar la sesión.",
                    "Restauración Exitosa",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblEstado.Text = "Error al restaurar la base de datos.";
                prgProgreso.Style = ProgressBarStyle.Blocks;
                prgProgreso.Value = 0;

                MessageBox.Show(
                    $"Ocurrió un error al restaurar la base de datos:\n\n{ex.Message}",
                    "Error de Restore",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                DesbloquearUI();
            }
        }

        private void BloquearUI(string mensajeEstado)
        {
            btnGenerarBackup.Enabled = false;
            btnRestaurar.Enabled = false;
            lblEstado.Text = mensajeEstado;
            prgProgreso.Style = ProgressBarStyle.Marquee;
            prgProgreso.MarqueeAnimationSpeed = 30;
            Cursor = Cursors.WaitCursor;
        }

        private void DesbloquearUI()
        {
            btnGenerarBackup.Enabled = true;
            btnRestaurar.Enabled = true;
            Cursor = Cursors.Default;
        }
    }
}