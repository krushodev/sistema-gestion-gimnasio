using System;
using System.Drawing;
using System.Windows.Forms;
using SGIG.Entidades;
using SGIG.Negocio;
using System;
using System.Drawing;
using System.Windows.Forms;
using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.UI;

public partial class frmPlanes : Form
{
    private readonly ServicioPlan _servicioPlan = new();

    public frmPlanes()
    {
        InitializeComponent();
        ConfigurarGrilla();
    }

    private void frmPlanes_Load(object sender, EventArgs e)
    {
        CargarPlanes();
    }

    private void ConfigurarGrilla()
    {
        dgvPlanes.AutoGenerateColumns = false;
        dgvPlanes.Columns.Clear();

        dgvPlanes.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Plan.IdPlan),
            HeaderText = "ID",
            Width = 60
        });

        dgvPlanes.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Plan.Nombre),
            HeaderText = "Nombre del Plan",
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        });

        dgvPlanes.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Plan.Precio),
            HeaderText = "Precio ($)",
            Width = 110,
            DefaultCellStyle = { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight }
        });

        dgvPlanes.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Plan.TipoPeriodicidad),
            HeaderText = "Periodicidad",
            Width = 120,
            DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
        });

        dgvPlanes.Columns.Add(new DataGridViewCheckBoxColumn
        {
            DataPropertyName = nameof(Plan.Activo),
            HeaderText = "Activo",
            Width = 70
        });
    }

    private void CargarPlanes()
    {
        try
        {
            bool soloActivos = !chkMostrarInactivos.Checked;
            dgvPlanes.DataSource = _servicioPlan.Listar(soloActivos);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al cargar los planes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void chkMostrarInactivos_CheckedChanged(object sender, EventArgs e)
    {
        CargarPlanes();
    }

    private void btnNuevo_Click(object sender, EventArgs e)
    {
        using var frm = new frmPlanEditor();
        if (frm.ShowDialog(this) == DialogResult.OK)
        {
            CargarPlanes();
        }
    }

    private void btnEditar_Click(object sender, EventArgs e)
    {
        if (dgvPlanes.CurrentRow?.DataBoundItem is not Plan planSeleccionado)
        {
            MessageBox.Show("Seleccione un plan de la lista.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        using var frm = new frmPlanEditor(planSeleccionado);
        if (frm.ShowDialog(this) == DialogResult.OK)
        {
            CargarPlanes();
        }
    }

    private void btnDarDeBaja_Click(object sender, EventArgs e)
    {
        if (dgvPlanes.CurrentRow?.DataBoundItem is not Plan planSeleccionado)
        {
            MessageBox.Show("Seleccione un plan de la lista.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!planSeleccionado.Activo)
        {
            MessageBox.Show("El plan seleccionado ya está inactivo.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var confirmacion = MessageBox.Show(
            $"¿Está seguro de que desea dar de baja el plan \"{planSeleccionado.Nombre}\"?",
            "Confirmar Baja Lógica",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question
        );

        if (confirmacion == DialogResult.Yes)
        {
            try
            {
                _servicioPlan.CambiarEstado(planSeleccionado.IdPlan, activo: false);
                MessageBox.Show("El plan fue dado de baja correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarPlanes();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al dar de baja el plan: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void dgvPlanes_SelectionChanged(object? sender, EventArgs e)
    {
        // Habilita o deshabilita botones según si hay una fila seleccionada
        bool haySeleccion = dgvPlanes.CurrentRow is not null;
        btnEditar.Enabled = haySeleccion;
        btnDarDeBaja.Enabled = haySeleccion;
    }

    private void btnCerrar_Click(object? sender, EventArgs e)
    {
        Close();
    }
}