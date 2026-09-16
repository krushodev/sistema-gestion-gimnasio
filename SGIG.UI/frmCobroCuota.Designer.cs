namespace SGIG.UI;

partial class frmCobroCuota
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        pnlSuperior = new Panel();
        grpSocio = new GroupBox();
        lblVencimientoActualValor = new Label();
        lblVencimientoActual = new Label();
        lblNombreSocioValor = new Label();
        lblNombreSocio = new Label();
        btnBuscarSocio = new Button();
        txtDocumento = new TextBox();
        lblDocumento = new Label();
        grpCobro = new GroupBox();
        btnRegistrarPago = new Button();
        lblNuevoVencimientoValor = new Label();
        lblNuevoVencimiento = new Label();
        txtMonto = new TextBox();
        lblMonto = new Label();
        cboMedioPago = new ComboBox();
        lblMedioPago = new Label();
        cboPlanes = new ComboBox();
        lblPlan = new Label();
        grpHistorial = new GroupBox();
        dgvHistorialPagos = new DataGridView();
        pnlSuperior.SuspendLayout();
        grpSocio.SuspendLayout();
        grpCobro.SuspendLayout();
        grpHistorial.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvHistorialPagos).BeginInit();
        SuspendLayout();
        // 
        // pnlSuperior
        // 
        pnlSuperior.Controls.Add(grpCobro);
        pnlSuperior.Controls.Add(grpSocio);
        pnlSuperior.Dock = DockStyle.Top;
        pnlSuperior.Location = new Point(0, 0);
        pnlSuperior.Name = "pnlSuperior";
        pnlSuperior.Padding = new Padding(12);
        pnlSuperior.Size = new Size(820, 230);
        pnlSuperior.TabIndex = 0;
        // 
        // grpSocio
        // 
        grpSocio.Controls.Add(lblVencimientoActualValor);
        grpSocio.Controls.Add(lblVencimientoActual);
        grpSocio.Controls.Add(lblNombreSocioValor);
        grpSocio.Controls.Add(lblNombreSocio);
        grpSocio.Controls.Add(btnBuscarSocio);
        grpSocio.Controls.Add(txtDocumento);
        grpSocio.Controls.Add(lblDocumento);
        grpSocio.Location = new Point(12, 12);
        grpSocio.Name = "grpSocio";
        grpSocio.Size = new Size(380, 205);
        grpSocio.TabIndex = 0;
        grpSocio.TabStop = false;
        grpSocio.Text = "Datos del Socio";
        // 
        // lblDocumento
        // 
        lblDocumento.AutoSize = true;
        lblDocumento.Location = new Point(16, 28);
        lblDocumento.Name = "lblDocumento";
        lblDocumento.Size = new Size(95, 15);
        lblDocumento.TabIndex = 0;
        lblDocumento.Text = "Nro. Documento:";
        // 
        // txtDocumento
        // 
        txtDocumento.Location = new Point(16, 48);
        txtDocumento.MaxLength = 12;
        txtDocumento.Name = "txtDocumento";
        txtDocumento.Size = new Size(180, 23);
        txtDocumento.TabIndex = 1;
        txtDocumento.KeyPress += txtDocumento_KeyPress;
        // 
        // btnBuscarSocio
        // 
        btnBuscarSocio.Location = new Point(205, 47);
        btnBuscarSocio.Name = "btnBuscarSocio";
        btnBuscarSocio.Size = new Size(85, 25);
        btnBuscarSocio.TabIndex = 2;
        btnBuscarSocio.Text = "Buscar";
        btnBuscarSocio.UseVisualStyleBackColor = true;
        btnBuscarSocio.Click += btnBuscarSocio_Click;
        // 
        // lblNombreSocio
        // 
        lblNombreSocio.AutoSize = true;
        lblNombreSocio.Location = new Point(16, 90);
        lblNombreSocio.Name = "lblNombreSocio";
        lblNombreSocio.Size = new Size(106, 15);
        lblNombreSocio.TabIndex = 3;
        lblNombreSocio.Text = "Apellido y Nombre:";
        // 
        // lblNombreSocioValor
        // 
        lblNombreSocioValor.AutoSize = true;
        lblNombreSocioValor.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
        lblNombreSocioValor.Location = new Point(16, 110);
        lblNombreSocioValor.Name = "lblNombreSocioValor";
        lblNombreSocioValor.Size = new Size(13, 17);
        lblNombreSocioValor.TabIndex = 4;
        lblNombreSocioValor.Text = "-";
        // 
        // lblVencimientoActual
        // 
        lblVencimientoActual.AutoSize = true;
        lblVencimientoActual.Location = new Point(16, 145);
        lblVencimientoActual.Name = "lblVencimientoActual";
        lblVencimientoActual.Size = new Size(112, 15);
        lblVencimientoActual.TabIndex = 5;
        lblVencimientoActual.Text = "Vencimiento Actual:";
        // 
        // lblVencimientoActualValor
        // 
        lblVencimientoActualValor.AutoSize = true;
        lblVencimientoActualValor.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
        lblVencimientoActualValor.Location = new Point(16, 165);
        lblVencimientoActualValor.Name = "lblVencimientoActualValor";
        lblVencimientoActualValor.Size = new Size(13, 17);
        lblVencimientoActualValor.TabIndex = 6;
        lblVencimientoActualValor.Text = "-";
        // 
        // grpCobro
        // 
        grpCobro.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        grpCobro.Controls.Add(btnRegistrarPago);
        grpCobro.Controls.Add(lblNuevoVencimientoValor);
        grpCobro.Controls.Add(lblNuevoVencimiento);
        grpCobro.Controls.Add(txtMonto);
        grpCobro.Controls.Add(lblMonto);
        grpCobro.Controls.Add(cboMedioPago);
        grpCobro.Controls.Add(lblMedioPago);
        grpCobro.Controls.Add(cboPlanes);
        grpCobro.Controls.Add(lblPlan);
        grpCobro.Location = new Point(405, 12);
        grpCobro.Name = "grpCobro";
        grpCobro.Size = new Size(403, 205);
        grpCobro.TabIndex = 1;
        grpCobro.TabStop = false;
        grpCobro.Text = "Detalle del Cobro";
        // 
        // lblPlan
        // 
        lblPlan.AutoSize = true;
        lblPlan.Location = new Point(16, 25);
        lblPlan.Name = "lblPlan";
        lblPlan.Size = new Size(108, 15);
        lblPlan.TabIndex = 0;
        lblPlan.Text = "Plan de Membresía:";
        // 
        // cboPlanes
        // 
        cboPlanes.DropDownStyle = ComboBoxStyle.DropDownList;
        cboPlanes.FormattingEnabled = true;
        cboPlanes.Location = new Point(16, 45);
        cboPlanes.Name = "cboPlanes";
        cboPlanes.Size = new Size(200, 23);
        cboPlanes.TabIndex = 1;
        cboPlanes.SelectedIndexChanged += cboPlanes_SelectedIndexChanged;
        // 
        // lblMedioPago
        // 
        lblMedioPago.AutoSize = true;
        lblMedioPago.Location = new Point(225, 25);
        lblMedioPago.Name = "lblMedioPago";
        lblMedioPago.Size = new Size(89, 15);
        lblMedioPago.TabIndex = 2;
        lblMedioPago.Text = "Medio de Pago:";
        // 
        // cboMedioPago
        // 
        cboMedioPago.DropDownStyle = ComboBoxStyle.DropDownList;
        cboMedioPago.FormattingEnabled = true;
        cboMedioPago.Location = new Point(225, 45);
        cboMedioPago.Name = "cboMedioPago";
        cboMedioPago.Size = new Size(160, 23);
        cboMedioPago.TabIndex = 3;
        // 
        // lblMonto
        // 
        lblMonto.AutoSize = true;
        lblMonto.Location = new Point(16, 85);
        lblMonto.Name = "lblMonto";
        lblMonto.Size = new Size(61, 15);
        lblMonto.TabIndex = 4;
        lblMonto.Text = "Monto ($):";
        // 
        // txtMonto
        // 
        txtMonto.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
        txtMonto.Location = new Point(16, 105);
        txtMonto.Name = "txtMonto";
        txtMonto.ReadOnly = true;
        txtMonto.Size = new Size(120, 25);
        txtMonto.TabIndex = 5;
        // 
        // lblNuevoVencimiento
        // 
        lblNuevoVencimiento.AutoSize = true;
        lblNuevoVencimiento.Location = new Point(155, 85);
        lblNuevoVencimiento.Name = "lblNuevoVencimiento";
        lblNuevoVencimiento.Size = new Size(113, 15);
        lblNuevoVencimiento.TabIndex = 6;
        lblNuevoVencimiento.Text = "Nuevo Vencimiento:";
        // 
        // lblNuevoVencimientoValor
        // 
        lblNuevoVencimientoValor.AutoSize = true;
        lblNuevoVencimientoValor.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblNuevoVencimientoValor.ForeColor = Color.DarkGreen;
        lblNuevoVencimientoValor.Location = new Point(155, 108);
        lblNuevoVencimientoValor.Name = "lblNuevoVencimientoValor";
        lblNuevoVencimientoValor.Size = new Size(15, 19);
        lblNuevoVencimientoValor.TabIndex = 7;
        lblNuevoVencimientoValor.Text = "-";
        // 
        // btnRegistrarPago
        // 
        btnRegistrarPago.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnRegistrarPago.BackColor = Color.FromArgb(16, 185, 129);
        btnRegistrarPago.Cursor = Cursors.Hand;
        btnRegistrarPago.FlatStyle = FlatStyle.Flat;
        btnRegistrarPago.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
        btnRegistrarPago.ForeColor = Color.White;
        btnRegistrarPago.Location = new Point(225, 150);
        btnRegistrarPago.Name = "btnRegistrarPago";
        btnRegistrarPago.Size = new Size(160, 40);
        btnRegistrarPago.TabIndex = 8;
        btnRegistrarPago.Text = "Registrar Pago";
        btnRegistrarPago.UseVisualStyleBackColor = false;
        btnRegistrarPago.Click += btnRegistrarPago_Click;
        // 
        // grpHistorial
        // 
        grpHistorial.Controls.Add(dgvHistorialPagos);
        grpHistorial.Dock = DockStyle.Fill;
        grpHistorial.Location = new Point(0, 230);
        grpHistorial.Name = "grpHistorial";
        grpHistorial.Padding = new Padding(12);
        grpHistorial.Size = new Size(820, 260);
        grpHistorial.TabIndex = 1;
        grpHistorial.TabStop = false;
        grpHistorial.Text = "Historial de Pagos del Socio";
        // 
        // dgvHistorialPagos
        // 
        dgvHistorialPagos.AllowUserToAddRows = false;
        dgvHistorialPagos.AllowUserToDeleteRows = false;
        dgvHistorialPagos.AllowUserToResizeRows = false;
        dgvHistorialPagos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvHistorialPagos.Dock = DockStyle.Fill;
        dgvHistorialPagos.Location = new Point(12, 28);
        dgvHistorialPagos.MultiSelect = false;
        dgvHistorialPagos.Name = "dgvHistorialPagos";
        dgvHistorialPagos.ReadOnly = true;
        dgvHistorialPagos.RowHeadersVisible = false;
        dgvHistorialPagos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvHistorialPagos.Size = new Size(796, 220);
        dgvHistorialPagos.TabIndex = 0;
        // 
        // frmCobroCuota
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(820, 490);
        Controls.Add(grpHistorial);
        Controls.Add(pnlSuperior);
        MinimumSize = new Size(750, 450);
        Name = "frmCobroCuota";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Registro de Cobro de Cuota";
        Load += frmCobroCuota_Load;
        pnlSuperior.ResumeLayout(false);
        grpSocio.ResumeLayout(false);
        grpSocio.PerformLayout();
        grpCobro.ResumeLayout(false);
        grpCobro.PerformLayout();
        grpHistorial.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dgvHistorialPagos).EndInit();
        ResumeLayout(false);
    }

    private Panel pnlSuperior;
    private GroupBox grpSocio;
    private Label lblDocumento;
    private TextBox txtDocumento;
    private Button btnBuscarSocio;
    private Label lblNombreSocio;
    private Label lblNombreSocioValor;
    private Label lblVencimientoActual;
    private Label lblVencimientoActualValor;
    private GroupBox grpCobro;
    private Label lblPlan;
    private ComboBox cboPlanes;
    private Label lblMedioPago;
    private ComboBox cboMedioPago;
    private Label lblMonto;
    private TextBox txtMonto;
    private Label lblNuevoVencimiento;
    private Label lblNuevoVencimientoValor;
    private Button btnRegistrarPago;
    private GroupBox grpHistorial;
    private DataGridView dgvHistorialPagos;
}