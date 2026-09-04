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
        private Button btnModificar = null!;
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
            this.Size = new Size(850, 520);
            this.StartPosition = FormStartPosition.CenterParent;

            // Panel superior de acciones
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

            btnModificar = new Button
            {
                Text = "Modificar",
                Location = new Point(135, 10),
                Size = new Size(110, 30)
            };
            btnModificar.Click += BtnModificar_Click;

            btnDarDeBaja = new Button
            {
                Text = "Dar de Baja",
                Location = new Point(255, 10),
                Size = new Size(110, 30)
            };
            btnDarDeBaja.Click += BtnDarDeBaja_Click;

            btnRecargar = new Button
            {
                Text = "Actualizar",
                Location = new Point(375, 10),
                Size = new Size(100, 30)
            };
            btnRecargar.Click += (s, e) => CargarSocios();

            panelSuperior.Controls.Add(btnNuevo);
            panelSuperior.Controls.Add(btnModificar);
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
            dgvSocios.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Email", HeaderText = "Email", Width = 200 });

            // Doble clic en una fila abre el editor
            dgvSocios.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0) BtnModificar_Click(s, e);
            };

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
            AbrirDialogoSocio(null);
        }

        private void BtnModificar_Click(object? sender, EventArgs e)
        {
            if (dgvSocios.CurrentRow?.DataBoundItem is not Socio socioSeleccionado)
            {
                MessageBox.Show("Seleccioná un socio de la lista para modificar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            AbrirDialogoSocio(socioSeleccionado);
        }

        private void AbrirDialogoSocio(Socio? socioExistente)
        {
            bool esEdicion = socioExistente != null;

            using var prompt = new Form
            {
                Width = 380,
                Height = 310,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = esEdicion ? "Modificar Socio" : "Registrar Nuevo Socio",
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var lblDni = new Label { Left = 20, Top = 20, Text = "DNI:" };
            var txtDni = new TextBox { Left = 120, Top = 18, Width = 210, Text = socioExistente?.Documento ?? "" };

            var lblNom = new Label { Left = 20, Top = 55, Text = "Nombre:" };
            var txtNom = new TextBox { Left = 120, Top = 53, Width = 210, Text = socioExistente?.Nombre ?? "" };

            var lblApe = new Label { Left = 20, Top = 90, Text = "Apellido:" };
            var txtApe = new TextBox { Left = 120, Top = 88, Width = 210, Text = socioExistente?.Apellido ?? "" };

            var lblTel = new Label { Left = 20, Top = 125, Text = "Teléfono:" };
            var txtTel = new TextBox { Left = 120, Top = 123, Width = 210, Text = socioExistente?.Telefono ?? "" };

            var lblEmail = new Label { Left = 20, Top = 160, Text = "Email:" };
            var txtEmail = new TextBox { Left = 120, Top = 158, Width = 210, Text = socioExistente?.Email ?? "" };

            var btnOk = new Button { Text = "Guardar", Left = 140, Width = 90, Top = 210, DialogResult = DialogResult.OK };
            var btnCancel = new Button { Text = "Cancelar", Left = 240, Width = 90, Top = 210, DialogResult = DialogResult.Cancel };

            prompt.Controls.AddRange(new Control[] {
                lblDni, txtDni,
                lblNom, txtNom,
                lblApe, txtApe,
                lblTel, txtTel,
                lblEmail, txtEmail,
                btnOk, btnCancel
            });

            prompt.AcceptButton = btnOk;
            prompt.CancelButton = btnCancel;

            if (prompt.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrWhiteSpace(txtDni.Text) || string.IsNullOrWhiteSpace(txtNom.Text) || string.IsNullOrWhiteSpace(txtApe.Text))
                {
                    MessageBox.Show("DNI, Nombre y Apellido son campos obligatorios.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    var socio = socioExistente ?? new Socio();
                    socio.Documento = txtDni.Text.Trim();
                    socio.IdTipoDocumento = socioExistente?.IdTipoDocumento ?? 1;
                    socio.Nombre = txtNom.Text.Trim();
                    socio.Apellido = txtApe.Text.Trim();
                    socio.Telefono = string.IsNullOrWhiteSpace(txtTel.Text) ? null : txtTel.Text.Trim();
                    socio.Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim();

                    _repoSocio.Guardar(socio);
                    MessageBox.Show(esEdicion ? "Socio actualizado con éxito." : "Socio registrado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarSocios();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al guardar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnDarDeBaja_Click(object? sender, EventArgs e)
        {
            if (dgvSocios.CurrentRow?.DataBoundItem is not Socio socio)
            {
                MessageBox.Show("Seleccioná un socio de la lista para dar de baja.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                $"¿Seguro que deseás dar de baja a {socio.Nombre} {socio.Apellido}?",
                "Confirmar Baja",
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
                    MessageBox.Show($"Error al dar de baja: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}