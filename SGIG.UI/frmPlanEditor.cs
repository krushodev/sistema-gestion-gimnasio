using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.UI;

public partial class frmPlanEditor : Form
{
    private readonly ServicioPlan _servicioPlan = new();
    private readonly Plan _plan;

    private Label lblNombre = null!;
    private TextBox txtNombre = null!;
    private Label lblPrecio = null!;
    private TextBox txtPrecio = null!;
    private Label lblPeriodicidad = null!;
    private ComboBox cboTipoPeriodicidad = null!;
    private CheckBox chkActivo = null!;
    private Button btnGuardar = null!;
    private Button btnCancelar = null!;

    public frmPlanEditor(Plan? plan = null)
    {
        ConstruirComponentes();
        _plan = plan ?? new Plan();
        ConfigurarFormulario();
    }

    private void ConstruirComponentes()
    {
        lblNombre = new Label { AutoSize = true, Location = new Point(25, 20), Text = "Nombre:" };
        txtNombre = new TextBox { Location = new Point(25, 38), MaxLength = 100, Size = new Size(250, 23) };

        lblPrecio = new Label { AutoSize = true, Location = new Point(25, 75), Text = "Precio ($):" };
        txtPrecio = new TextBox { Location = new Point(25, 93), MaxLength = 12, Size = new Size(250, 23) };
        txtPrecio.KeyPress += TxtPrecio_KeyPress;

        lblPeriodicidad = new Label { AutoSize = true, Location = new Point(25, 130), Text = "Periodicidad:" };
        cboTipoPeriodicidad = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Location = new Point(25, 148),
            Size = new Size(250, 23)
        };

        chkActivo = new CheckBox
        {
            AutoSize = true,
            Location = new Point(25, 188),
            Size = new Size(60, 19),
            Text = "Activo",
            UseVisualStyleBackColor = true
        };

        btnGuardar = new Button
        {
            Location = new Point(100, 225),
            Size = new Size(85, 30),
            Text = "Guardar",
            UseVisualStyleBackColor = true
        };
        btnGuardar.Click += BtnGuardar_Click;

        btnCancelar = new Button
        {
            Location = new Point(190, 225),
            Size = new Size(85, 30),
            Text = "Cancelar",
            UseVisualStyleBackColor = true
        };
        btnCancelar.Click += BtnCancelar_Click;

        ClientSize = new Size(304, 275);
        Controls.AddRange(new Control[]
        {
            lblNombre, txtNombre, lblPrecio, txtPrecio,
            lblPeriodicidad, cboTipoPeriodicidad, chkActivo,
            btnGuardar, btnCancelar
        });

        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
    }

    private void ConfigurarFormulario()
    {
        cboTipoPeriodicidad.Items.Clear();
        cboTipoPeriodicidad.Items.AddRange(new object[] { "Diario", "Semanal", "Mensual", "Anual" });

        if (_plan.IdPlan == 0)
        {
            Text = "Nuevo Plan";
            cboTipoPeriodicidad.SelectedItem = "Mensual";
            chkActivo.Checked = true;
            chkActivo.Enabled = false;
        }
        else
        {
            Text = "Editar Plan";
            txtNombre.Text = _plan.Nombre;
            txtPrecio.Text = _plan.Precio.ToString("0.00", CultureInfo.InvariantCulture);
            cboTipoPeriodicidad.SelectedItem = _plan.TipoPeriodicidad;
            chkActivo.Checked = _plan.Activo;
            chkActivo.Enabled = true;
        }
    }

    private void TxtPrecio_KeyPress(object? sender, KeyPressEventArgs e)
    {
        if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != ',')
        {
            e.Handled = true;
            return;
        }

        if ((e.KeyChar == '.' || e.KeyChar == ',') && (txtPrecio.Text.Contains('.') || txtPrecio.Text.Contains(',')))
        {
            e.Handled = true;
        }
    }

    private void BtnGuardar_Click(object? sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Debe ingresar un nombre para el plan.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return;
            }

            string precioTexto = txtPrecio.Text.Replace(',', '.');
            if (!decimal.TryParse(precioTexto, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal precio) || precio <= 0)
            {
                MessageBox.Show("Debe ingresar un precio válido mayor a cero.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrecio.Focus();
                return;
            }

            if (cboTipoPeriodicidad.SelectedItem is null)
            {
                MessageBox.Show("Debe seleccionar una periodicidad.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboTipoPeriodicidad.Focus();
                return;
            }

            _plan.Nombre = txtNombre.Text.Trim();
            _plan.Precio = precio;
            _plan.TipoPeriodicidad = cboTipoPeriodicidad.SelectedItem.ToString()!;
            _plan.Activo = chkActivo.Checked;

            _servicioPlan.Guardar(_plan);

            MessageBox.Show("Plan guardado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (NegocioException ex)
        {
            MessageBox.Show(ex.Message, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al guardar el plan: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnCancelar_Click(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }
}