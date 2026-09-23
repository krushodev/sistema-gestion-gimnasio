using System;
using System.Drawing;
using System.Windows.Forms;

namespace SGIG.UI
{
    public partial class frmBackup : Form
    {
        public frmBackup()
        {
            InitializeComponent();
            Tema.EstilizarFormulario(this);
            Tema.EstilizarControles(this);
        }

        private void btnGenerarBackup_Click(object sender, EventArgs e)
        {
            using var sfd = new SaveFileDialog
            {
                Filter = "Archivo de copia de seguridad SQL (*.bak)|*.bak",
                FileName = $"SGIG_Backup_{DateTime.Now:yyyyMMdd_HHmm}.bak",
                Title = "Guardar Copia de Seguridad"
            };

            if (sfd.ShowDialog() != DialogResult.OK) return;

            SimularOperacion("Generando copia de seguridad de la base de datos...", () =>
            {
                MessageBox.Show(
                    $"Copia de seguridad generada con éxito en:\n\n{sfd.FileName}",
                    "Copia de Seguridad Completada", MessageBoxButtons.OK, MessageBoxIcon.Information);
            });
        }

        private void btnRestaurar_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Filter = "Archivo de copia de seguridad SQL (*.bak)|*.bak",
                Title = "Seleccionar Archivo de Respaldo"
            };

            if (ofd.ShowDialog() != DialogResult.OK) return;

            var confirmacion = MessageBox.Show(
                $"¿Confirmás que deseás restaurar la base con el archivo seleccionado?\n\n{ofd.FileName}\n\nLos cambios no guardados se sobreescribirán.",
                "Confirmación de Restauración", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirmacion != DialogResult.Yes) return;

            SimularOperacion("Restaurando estructura y registros desde el backup...", () =>
            {
                MessageBox.Show(
                    "La base de datos se ha restaurado correctamente.",
                    "Restauración Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
            });
        }

        private void SimularOperacion(string mensajeEstado, Action onCompletado)
        {
            btnGenerarBackup.Enabled = false;
            btnRestaurar.Enabled = false;
            lblEstado.Text = mensajeEstado;
            prgProgreso.Value = 0;

            var timer = new System.Windows.Forms.Timer { Interval = 35 };
            timer.Tick += (s, e) =>
            {
                if (prgProgreso.Value < 100)
                {
                    prgProgreso.Value += 5;
                }
                else
                {
                    timer.Stop();
                    timer.Dispose();

                    btnGenerarBackup.Enabled = true;
                    btnRestaurar.Enabled = true;
                    lblEstado.Text = "Operación finalizada.";
                    onCompletado();
                }
            };
            timer.Start();
        }
    }
}