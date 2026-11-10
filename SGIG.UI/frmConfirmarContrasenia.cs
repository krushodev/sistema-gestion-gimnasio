// Diálogo armado sólo por código (sin .Designer.cs).
#pragma warning disable WFO1000
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SGIG.UI
{
    /// <summary>
    /// Pide la contraseña del usuario logueado antes de una operación sensible (backup y
    /// restore). Sólo captura el texto: la validación contra el hash y el control de rol
    /// están en <c>ServicioBackup</c>, no acá.
    /// </summary>
    public class frmConfirmarContrasenia : Form
    {
        private readonly TextBox txtContrasenia = new() { Name = "txtContrasenia", UseSystemPasswordChar = true };
        private readonly Button btnConfirmar = new() { Name = "btnConfirmar", Text = "Confirmar", DialogResult = DialogResult.OK };
        private readonly Button btnCancelar = new() { Name = "btnCancelar", Text = "Cancelar", DialogResult = DialogResult.Cancel };

        private frmConfirmarContrasenia(string titulo)
        {
            Text = titulo;
            ClientSize = new Size(380, 170);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = MinimizeBox = ShowInTaskbar = false;

            var lblMensaje = new Label
            {
                Name = "lblMensaje",
                Text = "Esta operación requiere confirmar tu identidad.\nIngresá tu contraseña de Administrador:",
                Location = new Point(20, 16),
                Size = new Size(340, 40)
            };
            txtContrasenia.Location = new Point(20, 66);
            txtContrasenia.Size = new Size(340, 27);
            btnConfirmar.Location = new Point(144, 116);
            btnConfirmar.Size = new Size(104, 36);
            btnCancelar.Location = new Point(256, 116);
            btnCancelar.Size = new Size(104, 36);

            Controls.AddRange(new Control[] { lblMensaje, txtContrasenia, btnConfirmar, btnCancelar });
            AcceptButton = btnConfirmar;
            CancelButton = btnCancelar;

            Tema.AgregarToggleContrasenia(txtContrasenia);
            Tema.EstilizarFormulario(this);
            Tema.EstilizarControles(this);
            Shown += (s, e) => txtContrasenia.Focus();
        }

        /// <summary>Devuelve la contraseña tipeada, o <c>null</c> si se canceló.</summary>
        public static string? Pedir(IWin32Window padre, string titulo)
        {
            using var dialogo = new frmConfirmarContrasenia(titulo);
            return dialogo.ShowDialog(padre) == DialogResult.OK ? dialogo.txtContrasenia.Text : null;
        }
    }
}
