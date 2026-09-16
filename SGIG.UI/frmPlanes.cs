using System;
using System.Drawing;
using System.Windows.Forms;
using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.UI;

/// <summary>
/// Consulta de planes de membresía (RF#10). Plan es un catálogo sembrado con la
/// aplicación (ver docs/SGIG_CreateDB.sql): esta pantalla es de solo lectura, sin
/// alta/edición/baja desde la UI.
/// </summary>
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
        btnNuevo.Visible = false;
        btnEditar.Visible = false;
        btnDarDeBaja.Visible = false;

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

    // Nuevo/Editar/DarDeBaja quedan inertes (RF#10 pasó a ser consulta de un
    // catálogo sembrado, ver docs/SGIG_CreateDB.sql): los métodos siguen acá
    // porque frmPlanes.Designer.cs los engancha por evento, pero los botones
    // están ocultos (ver frmPlanes_Load) y nunca se disparan desde la UI.
    private void btnNuevo_Click(object sender, EventArgs e)
    {
    }

    private void btnEditar_Click(object sender, EventArgs e)
    {
    }

    private void btnDarDeBaja_Click(object sender, EventArgs e)
    {
    }

    private void dgvPlanes_SelectionChanged(object? sender, EventArgs e)
    {
    }

    private void btnCerrar_Click(object? sender, EventArgs e)
    {
        Close();
    }
}