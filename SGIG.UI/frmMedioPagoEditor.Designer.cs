namespace SGIG.UI;

partial class frmMedioPagoEditor
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
        lblDescripcion = new Label();
        txtDescripcion = new TextBox();
        btnGuardar = new Button();
        btnCancelar = new Button();
        SuspendLayout();
        // 
        // lblDescripcion
        // 
        lblDescripcion.AutoSize = true;
        lblDescripcion.Location = new Point(20, 20);
        lblDescripcion.Name = "lblDescripcion";
        lblDescripcion.Size = new Size(72, 15);
        lblDescripcion.TabIndex = 0;
        lblDescripcion.Text = "Descripción:";
        // 
        // txtDescripcion
        // 
        txtDescripcion.Location = new Point(20, 40);
        txtDescripcion.MaxLength = 50;
        txtDescripcion.Name = "txtDescripcion";
        txtDescripcion.Size = new Size(244, 23);
        txtDescripcion.TabIndex = 1;
        // 
        // btnGuardar
        // 
        btnGuardar.Location = new Point(94, 80);
        btnGuardar.Name = "btnGuardar";
        btnGuardar.Size = new Size(80, 28);
        btnGuardar.TabIndex = 2;
        btnGuardar.Text = "Guardar";
        btnGuardar.UseVisualStyleBackColor = true;
        btnGuardar.Click += btnGuardar_Click;
        // 
        // btnCancelar
        // 
        btnCancelar.Location = new Point(184, 80);
        btnCancelar.Name = "btnCancelar";
        btnCancelar.Size = new Size(80, 28);
        btnCancelar.TabIndex = 3;
        btnCancelar.Text = "Cancelar";
        btnCancelar.UseVisualStyleBackColor = true;
        btnCancelar.Click += btnCancelar_Click;
        // 
        // frmMedioPagoEditor
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(284, 125);
        Controls.Add(btnCancelar);
        Controls.Add(btnGuardar);
        Controls.Add(txtDescripcion);
        Controls.Add(lblDescripcion);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "frmMedioPagoEditor";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Medio de Pago";
        Load += frmMedioPagoEditor_Load;
        ResumeLayout(false);
        PerformLayout();
    }

    private Label lblDescripcion;
    private TextBox txtDescripcion;
    private Button btnGuardar;
    private Button btnCancelar;
}