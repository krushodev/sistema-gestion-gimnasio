namespace SGIG.UI;

partial class frmPlanes
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
        chkMostrarInactivos = new CheckBox();
        dgvPlanes = new DataGridView();
        pnlBotones = new Panel();
        btnNuevo = new Button();
        btnEditar = new Button();
        btnDarDeBaja = new Button();
        btnCerrar = new Button();
        pnlSuperior.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvPlanes).BeginInit();
        pnlBotones.SuspendLayout();
        SuspendLayout();
        // 
        // pnlSuperior
        // 
        pnlSuperior.Controls.Add(chkMostrarInactivos);
        pnlSuperior.Dock = DockStyle.Top;
        pnlSuperior.Location = new Point(0, 0);
        pnlSuperior.Name = "pnlSuperior";
        pnlSuperior.Size = new Size(680, 45);
        pnlSuperior.TabIndex = 0;
        // 
        // chkMostrarInactivos
        // 
        chkMostrarInactivos.AutoSize = true;
        chkMostrarInactivos.Location = new Point(16, 14);
        chkMostrarInactivos.Name = "chkMostrarInactivos";
        chkMostrarInactivos.Size = new Size(160, 19);
        chkMostrarInactivos.TabIndex = 0;
        chkMostrarInactivos.Text = "Mostrar planes inactivos";
        chkMostrarInactivos.UseVisualStyleBackColor = true;
        chkMostrarInactivos.CheckedChanged += chkMostrarInactivos_CheckedChanged;
        // 
        // dgvPlanes
        // 
        dgvPlanes.AllowUserToAddRows = false;
        dgvPlanes.AllowUserToDeleteRows = false;
        dgvPlanes.AllowUserToResizeRows = false;
        dgvPlanes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvPlanes.Dock = DockStyle.Fill;
        dgvPlanes.Location = new Point(0, 45);
        dgvPlanes.MultiSelect = false;
        dgvPlanes.Name = "dgvPlanes";
        dgvPlanes.ReadOnly = true;
        dgvPlanes.RowHeadersVisible = false;
        dgvPlanes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvPlanes.Size = new Size(680, 320);
        dgvPlanes.TabIndex = 1;
        dgvPlanes.SelectionChanged += dgvPlanes_SelectionChanged;
        // 
        // pnlBotones
        // 
        pnlBotones.Controls.Add(btnCerrar);
        pnlBotones.Controls.Add(btnDarDeBaja);
        pnlBotones.Controls.Add(btnEditar);
        pnlBotones.Controls.Add(btnNuevo);
        pnlBotones.Dock = DockStyle.Bottom;
        pnlBotones.Location = new Point(0, 365);
        pnlBotones.Name = "pnlBotones";
        pnlBotones.Size = new Size(680, 55);
        pnlBotones.TabIndex = 2;
        // 
        // btnNuevo
        // 
        btnNuevo.Location = new Point(16, 12);
        btnNuevo.Name = "btnNuevo";
        btnNuevo.Size = new Size(100, 32);
        btnNuevo.TabIndex = 0;
        btnNuevo.Text = "Nuevo Plan";
        btnNuevo.UseVisualStyleBackColor = true;
        btnNuevo.Click += btnNuevo_Click;
        // 
        // btnEditar
        // 
        btnEditar.Location = new Point(125, 12);
        btnEditar.Name = "btnEditar";
        btnEditar.Size = new Size(95, 32);
        btnEditar.TabIndex = 1;
        btnEditar.Text = "Editar";
        btnEditar.UseVisualStyleBackColor = true;
        btnEditar.Click += btnEditar_Click;
        // 
        // btnDarDeBaja
        // 
        btnDarDeBaja.Location = new Point(230, 12);
        btnDarDeBaja.Name = "btnDarDeBaja";
        btnDarDeBaja.Size = new Size(110, 32);
        btnDarDeBaja.TabIndex = 2;
        btnDarDeBaja.Text = "Dar de Baja";
        btnDarDeBaja.UseVisualStyleBackColor = true;
        btnDarDeBaja.Click += btnDarDeBaja_Click;
        // 
        // btnCerrar
        // 
        btnCerrar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnCerrar.Location = new Point(570, 12);
        btnCerrar.Name = "btnCerrar";
        btnCerrar.Size = new Size(95, 32);
        btnCerrar.TabIndex = 3;
        btnCerrar.Text = "Cerrar";
        btnCerrar.UseVisualStyleBackColor = true;
        btnCerrar.Click += btnCerrar_Click;
        // 
        // frmPlanes
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(680, 420);
        Controls.Add(dgvPlanes);
        Controls.Add(pnlBotones);
        Controls.Add(pnlSuperior);
        MinimumSize = new Size(600, 350);
        Name = "frmPlanes";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Administración de Planes";
        Load += frmPlanes_Load;
        pnlSuperior.ResumeLayout(false);
        pnlSuperior.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvPlanes).EndInit();
        pnlBotones.ResumeLayout(false);
        ResumeLayout(false);
    }

    private Panel pnlSuperior;
    private CheckBox chkMostrarInactivos;
    private DataGridView dgvPlanes;
    private Panel pnlBotones;
    private Button btnNuevo;
    private Button btnEditar;
    private Button btnDarDeBaja;
    private Button btnCerrar;
}