using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SGIG.Datos;
using SGIG.Entidades;

namespace SGIG.UI
{
    public class frmSocios : Form
    {
        private readonly RepositorioSocio _repoSocio = new();
        private DataGridView dgvSocios = null!;
        private Button btnNuevo = null!;
        private Button btnDarDeBaja = null!;
        private Button btnRecargar = null!;

        public frmSocios()
        {
            InicializarComponentes();
            CargarSocios();
        }

        private void InicializarComponentes()
        {
            this.Text = "Gestión de Socios";
            this.Size = new Size(800, 500);
            this.StartPosition = FormStartPosition.CenterParent;

            // Panel superior de botones
            var panelSuperior = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50
            };

            btnNuevo = new Button
            {
                Text = "Nuevo Socio",
                Location = new Point(15, 10),
                Size = new Size(110, 30)
            };
            btnNuevo.Click += BtnNuevo_Click;

            btnDarDeBaja = new Button
            {
                Text = "Dar de Baja",
                Location = new Point(135, 10),
                Size = new Size(110, 30)
            };
            btnDarDeBaja.Click += BtnDarDeBaja_Click;

            btnRecargar = new Button
            {
                Text = "Actualizar",
                Location = new Point(255, 10),
                Size = new Size(100, 30)
            };
            btnRecargar.Click += (s, e) => CargarSocios();

            panelSuperior.Controls.Add(btnNuevo);
            panelSuperior.Controls.Add(btnDarDeBaja);
            panelSuperior.Controls.Add(btnRecargar);

            // Grilla de Socios
            dgvSocios = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false
            };

            dgvSocios.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "IdPersona", HeaderText = "ID", Width = 50 });
            dgvSocios.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Documento", HeaderText = "DNI", Width = 100 });
            dgvSocios.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Nombre", HeaderText = "Nombre", Width = 140 });
            dgvSocios.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Apellido", HeaderText = "Apellido", Width = 140 });
            dgvSocios.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Telefono", HeaderText = "Teléfono", Width = 110 });
            dgvSocios.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Email", HeaderText = "Email", Width = 180 });

            this.Controls.Add(dgvSocios);
            this.Controls.Add(panelSuperior);
        }

        private void CargarSocios()
        {
            try
            {
                var lista = _repoSocio.ListarTodos().ToList();
                dgvSocios.DataSource = lista;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar la lista de socios: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnNuevo_Click(object? sender, EventArgs e)
        {
            // Prompt rápido para cargar datos sin romper formularios
            using var prompt = new Form
            {
                Width = 350,
                Height = 280,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "Registrar Nuevo Socio",
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var lblDni = new Label { Left = 20, Top = 20, Text = "DNI:" };
            var txtDni = new TextBox { Left = 120, Top = 18, Width = 180 };

            var lblNom = new Label { Left = 20, Top = 55, Text = "Nombre:" };
            var txtNom = new TextBox { Left = 120, Top = 53, Width = 180 };

            var lblApe = new Label { Left = 20, Top = 90, Text = "Apellido:" };
            var txtApe = new TextBox { Left = 120, Top = 88, Width = 180 };

            var lblTel = new Label { Left = 20, Top = 125, Text = "Teléfono:" };
            var txtTel = new TextBox { Left = 120, Top = 123, Width = 180 };

            var btnOk = new Button { Text = "Guardar", Left = 120, Width = 80, Top = 175, DialogResult = DialogResult.OK };
            var btnCancel = new Button { Text = "Cancelar", Left = 210, Width = 80, Top = 175, DialogResult = DialogResult.Cancel };

            prompt.Controls.AddRange(new Control[] { lblDni, txtDni, lblNom, txtNom, lblApe, txtApe, lblTel, txtTel, btnOk, btnCancel });
            prompt.AcceptButton = btnOk;
            prompt.CancelButton = btnCancel;

            if (prompt.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrWhiteSpace(txtDni.Text) || string.IsNullOrWhiteSpace(txtNom.Text) || string.IsNullOrWhiteSpace(txtApe.Text))
                {
                    MessageBox.Show("DNI, Nombre y Apellido son obligatorios.", "Aviso");
                    return;
                }

                try
                {
                    var nuevo = new Socio
                    {
                        Documento = txtDni.Text.Trim(),
                        IdTipoDocumento = 1,
                        Nombre = txtNom.Text.Trim(),
                        Apellido = txtApe.Text.Trim(),
                        Telefono = string.IsNullOrWhiteSpace(txtTel.Text) ? null : txtTel.Text.Trim()
                    };

                    _repoSocio.Guardar(nuevo);
                    MessageBox.Show("Socio guardado correctamente.", "Éxito");
                    CargarSocios();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al guardar: {ex.Message}", "Error");
                }
            }
        }

        private void BtnDarDeBaja_Click(object? sender, EventArgs e)
        {
            if (dgvSocios.CurrentRow?.DataBoundItem is not Socio socio)
            {
                MessageBox.Show("Seleccioná un socio de la lista.", "Aviso");
                return;
            }

            var confirm = MessageBox.Show(
                $"¿Seguro que deseás dar de baja a {socio.Nombre} {socio.Apellido}?",
                "Confirmar",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    _repoSocio.BajaLogica(socio.IdPersona);
                    CargarSocios();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al dar de baja: {ex.Message}", "Error");
                }
            }
        }
    }
}