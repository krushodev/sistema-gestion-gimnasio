using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.UI;

/// <summary>
/// Selector modal de socio por nombre/apellido/documento, para pantallas de
/// búsqueda de un solo registro (Cobro de Cuotas, Check-in) que no tienen una
/// grilla propia como frmSocios. Construido enteramente en código (sin
/// .Designer.cs, ver docs/patrones/estilo-visual.md) porque el agente no puede
/// usar el diseñador visual de WinForms.
/// </summary>
public class frmBuscarSocio : Form
{
    private readonly ServicioSocio _servicioSocio = new();
    private readonly TextBox _txtBusqueda;
    private readonly DataGridView _dgvResultados;
    private readonly Button _btnSeleccionar;
    private readonly Button _btnCancelar;

    /// <summary>Socio elegido; sólo tiene valor cuando el diálogo cierra con DialogResult.OK.</summary>
    public Socio? SocioElegido { get; private set; }

    public frmBuscarSocio()
    {
        Text = "Buscar socio por nombre";
        ClientSize = new Size(520, 400);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;

        var lblBusqueda = new Label
        {
            Text = "Nombre, apellido o documento:",
            Location = new Point(16, 14),
            AutoSize = true
        };

        _txtBusqueda = new TextBox
        {
            Location = new Point(16, 34),
            Size = new Size(488, 25)
        };
        _txtBusqueda.TextChanged += TxtBusqueda_TextChanged;

        _dgvResultados = new DataGridView
        {
            Location = new Point(16, 68),
            Size = new Size(488, 260),
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            MultiSelect = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoGenerateColumns = false,
            RowHeadersVisible = false
        };
        _dgvResultados.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Socio.Apellido),
            HeaderText = "Apellido",
            Width = 160
        });
        _dgvResultados.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Socio.Nombre),
            HeaderText = "Nombre",
            Width = 160
        });
        _dgvResultados.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(Socio.Documento),
            HeaderText = "Documento",
            Width = 120
        });
        _dgvResultados.CellDoubleClick += (s, e) => ConfirmarSeleccion();

        _btnSeleccionar = new Button
        {
            Text = "Seleccionar",
            Location = new Point(312, 336),
            Size = new Size(90, 28),
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right
        };
        _btnSeleccionar.Click += (s, e) => ConfirmarSeleccion();

        _btnCancelar = new Button
        {
            Text = "Cancelar",
            Location = new Point(414, 336),
            Size = new Size(90, 28),
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right
        };
        _btnCancelar.Click += (s, e) =>
        {
            DialogResult = DialogResult.Cancel;
            Close();
        };

        Controls.AddRange(new Control[] { lblBusqueda, _txtBusqueda, _dgvResultados, _btnSeleccionar, _btnCancelar });
        AcceptButton = _btnSeleccionar;
        CancelButton = _btnCancelar;

        Tema.EstilizarFormulario(this);
        Tema.EstilizarControles(this);
    }

    private void TxtBusqueda_TextChanged(object? sender, EventArgs e)
    {
        try
        {
            _dgvResultados.DataSource = _servicioSocio.BuscarPorTexto(_txtBusqueda.Text).ToList();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al buscar socios: {ex.Message}", "SGIG",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ConfirmarSeleccion()
    {
        if (_dgvResultados.CurrentRow?.DataBoundItem is not Socio socio)
        {
            MessageBox.Show("Seleccioná un socio de la lista.", "SGIG",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        SocioElegido = socio;
        DialogResult = DialogResult.OK;
        Close();
    }
}
