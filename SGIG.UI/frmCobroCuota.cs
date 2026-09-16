using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.UI;

public partial class frmCobroCuota : Form
{
    private readonly ServicioSocio _servicioSocio = new();
    private readonly ServicioPlan _servicioPlan = new();
    private readonly ServicioTesoreria _servicioTesoreria = new();
    private readonly ServicioCatalogo _servicioCatalogo = new();

    private Socio? _socioActual;

    public frmCobroCuota()
    {
        InitializeComponent();
        ConfigurarGrilla();
    }

    private void frmCobroCuota_Load(object sender, EventArgs e)
    {
        CargarCombos();
        HabilitarSeccionCobro(false);
    }

    private void ConfigurarGrilla()
    {
        dgvHistorialPagos.AutoGenerateColumns = false;
        dgvHistorialPagos.Columns.Clear();

        dgvHistorialPagos.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Pago.IdPago),
            HeaderText = "N° Pago",
            Width = 80
        });

        dgvHistorialPagos.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Pago.FechaPago),
            HeaderText = "Fecha de Pago",
            Width = 140,
            DefaultCellStyle = { Format = "g" }
        });

        dgvHistorialPagos.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Pago.PlanNombre),
            HeaderText = "Plan",
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        });

        dgvHistorialPagos.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Pago.MedioPagoDesc),
            HeaderText = "Medio de Pago",
            Width = 150
        });

        dgvHistorialPagos.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Pago.Monto),
            HeaderText = "Monto ($)",
            Width = 110,
            DefaultCellStyle = { Format = "N2", Alignment = DataGridViewContentAlignment.MiddleRight }
        });
    }

    private void CargarCombos()
    {
        try
        {
            var planes = _servicioPlan.Listar(soloActivos: true).ToList();
            cboPlanes.DataSource = planes;
            cboPlanes.DisplayMember = nameof(Plan.Nombre);
            cboPlanes.ValueMember = nameof(Plan.IdPlan);

            var mediosPago = _servicioCatalogo.ObtenerMediosPago().ToList();
            cboMedioPago.DataSource = mediosPago;
            cboMedioPago.DisplayMember = nameof(MedioPago.Descripcion);
            cboMedioPago.ValueMember = nameof(MedioPago.IdMedioPago);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al cargar catálogos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void HabilitarSeccionCobro(bool habilitar)
    {
        cboPlanes.Enabled = habilitar;
        cboMedioPago.Enabled = habilitar;
        btnRegistrarPago.Enabled = habilitar;

        if (!habilitar)
        {
            lblNombreSocioValor.Text = "-";
            lblVencimientoActualValor.Text = "-";
            txtMonto.Clear();
            lblNuevoVencimientoValor.Text = "-";
            dgvHistorialPagos.DataSource = null;
        }
    }

    private void txtDocumento_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
        {
            e.Handled = true;
        }

        if (e.KeyChar == (char)Keys.Enter)
        {
            e.Handled = true;
            btnBuscarSocio.PerformClick();
        }
    }

    private void btnBuscarSocio_Click(object sender, EventArgs e)
    {
        string documento = txtDocumento.Text.Trim();
        if (string.IsNullOrWhiteSpace(documento))
        {
            MessageBox.Show("Ingrese un número de documento.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtDocumento.Focus();
            return;
        }

        try
        {
            _socioActual = _servicioSocio.ObtenerPorDocumento(documento);

            if (_socioActual is null)
            {
                HabilitarSeccionCobro(false);
                MessageBox.Show("No se encontró ningún socio con el documento indicado.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!_socioActual.Activo)
            {
                HabilitarSeccionCobro(false);
                MessageBox.Show("El socio seleccionado se encuentra dado de baja lógica.", "Socio Inactivo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            lblNombreSocioValor.Text = $"{_socioActual.Apellido}, {_socioActual.Nombre}";
            lblVencimientoActualValor.Text = _socioActual.FechaVencimientoCuota.HasValue
                ? _socioActual.FechaVencimientoCuota.Value.ToString("dd/MM/yyyy")
                : "Sin cuotas previas";

            if (_socioActual.IdPlan.HasValue)
            {
                cboPlanes.SelectedValue = _socioActual.IdPlan.Value;
            }

            HabilitarSeccionCobro(true);
            CalcularNuevoVencimiento();
            CargarHistorialPagos();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al buscar el socio: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void cboPlanes_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (cboPlanes.SelectedItem is Plan plan)
        {
            txtMonto.Text = plan.Precio.ToString("N2");
            CalcularNuevoVencimiento();
        }
    }

    // RF#12: Cálculo de nuevo vencimiento proyectado por calendario
    private void CalcularNuevoVencimiento()
    {
        if (_socioActual is null || cboPlanes.SelectedItem is not Plan plan)
            return;

        DateTime emision = DateTime.Today;
        DateTime baseCalculo = (_socioActual.FechaVencimientoCuota.HasValue && _socioActual.FechaVencimientoCuota.Value > emision)
            ? _socioActual.FechaVencimientoCuota.Value
            : emision;

        DateTime proyectado = plan.TipoPeriodicidad switch
        {
            "Diario" => baseCalculo.AddDays(1),
            "Semanal" => baseCalculo.AddDays(7),
            "Mensual" => baseCalculo.AddMonths(1),
            "Anual" => baseCalculo.AddYears(1),
            _ => baseCalculo.AddMonths(1)
        };

        lblNuevoVencimientoValor.Text = proyectado.ToString("dd/MM/yyyy");
    }

    private void CargarHistorialPagos()
    {
        if (_socioActual is null) return;

        try
        {
            var historial = _servicioTesoreria.ObtenerHistorialSocio(_socioActual.IdPersona).ToList();
            dgvHistorialPagos.DataSource = historial;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al cargar el historial de pagos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnRegistrarPago_Click(object sender, EventArgs e)
    {
        if (_socioActual is null)
        {
            MessageBox.Show("Debe buscar y seleccionar un socio primero.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (cboPlanes.SelectedItem is not Plan plan)
        {
            MessageBox.Show("Seleccione un plan de membresía.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (cboMedioPago.SelectedValue is not int idMedioPago)
        {
            MessageBox.Show("Seleccione un medio de pago.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var confirmacion = MessageBox.Show(
            $"¿Confirma el cobro de ${plan.Precio:N2} correspondiente al plan \"{plan.Nombre}\" para el socio {_socioActual.Nombre} {_socioActual.Apellido}?",
            "Confirmación de Cobro",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question
        );

        if (confirmacion != DialogResult.Yes)
            return;

        try
        {
            _servicioTesoreria.RegistrarCobro(
                _socioActual.IdPersona,
                plan.IdPlan,
                idMedioPago,
                _socioActual.FechaVencimientoCuota
            );

            MessageBox.Show("El pago y la facturación fueron registrados con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

            _socioActual = _servicioSocio.ObtenerPorDocumento(_socioActual.Documento);
            if (_socioActual is not null)
            {
                lblVencimientoActualValor.Text = _socioActual.FechaVencimientoCuota?.ToString("dd/MM/yyyy") ?? "-";
                CalcularNuevoVencimiento();
                CargarHistorialPagos();
            }
        }
        catch (NegocioException ex)
        {
            MessageBox.Show(ex.Message, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ocurrió un error al registrar el pago: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}