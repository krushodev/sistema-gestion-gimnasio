namespace SGIG.UI
{
    partial class frmMantenimiento
    {
        /// <summary>Required designer variable.</summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>Clean up any resources being used.</summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.grpNuevo = new System.Windows.Forms.GroupBox();
            this.lblMaquina = new System.Windows.Forms.Label();
            this.cboMaquina = new System.Windows.Forms.ComboBox();
            this.lblFechaInicio = new System.Windows.Forms.Label();
            this.dtpFechaInicio = new System.Windows.Forms.DateTimePicker();
            this.lblDetalleTecnico = new System.Windows.Forms.Label();
            this.txtDetalleTecnico = new System.Windows.Forms.TextBox();
            this.btnRegistrar = new System.Windows.Forms.Button();
            this.dgvMantenimientosActivos = new System.Windows.Forms.DataGridView();
            this.grpFinalizar = new System.Windows.Forms.GroupBox();
            this.lblFechaFin = new System.Windows.Forms.Label();
            this.dtpFechaFin = new System.Windows.Forms.DateTimePicker();
            this.btnFinalizar = new System.Windows.Forms.Button();
            this.grpNuevo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMantenimientosActivos)).BeginInit();
            this.grpFinalizar.SuspendLayout();
            this.SuspendLayout();
            //
            // grpNuevo
            //
            this.grpNuevo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpNuevo.Controls.Add(this.lblMaquina);
            this.grpNuevo.Controls.Add(this.cboMaquina);
            this.grpNuevo.Controls.Add(this.lblFechaInicio);
            this.grpNuevo.Controls.Add(this.dtpFechaInicio);
            this.grpNuevo.Controls.Add(this.lblDetalleTecnico);
            this.grpNuevo.Controls.Add(this.txtDetalleTecnico);
            this.grpNuevo.Controls.Add(this.btnRegistrar);
            this.grpNuevo.Location = new System.Drawing.Point(16, 16);
            this.grpNuevo.Name = "grpNuevo";
            this.grpNuevo.Size = new System.Drawing.Size(760, 136);
            this.grpNuevo.TabIndex = 0;
            this.grpNuevo.TabStop = false;
            this.grpNuevo.Text = "Registrar mantenimiento";
            //
            // lblMaquina
            //
            this.lblMaquina.AutoSize = true;
            this.lblMaquina.Location = new System.Drawing.Point(16, 32);
            this.lblMaquina.Name = "lblMaquina";
            this.lblMaquina.Size = new System.Drawing.Size(58, 15);
            this.lblMaquina.Text = "Máquina:";
            //
            // cboMaquina
            //
            this.cboMaquina.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMaquina.Location = new System.Drawing.Point(100, 29);
            this.cboMaquina.Name = "cboMaquina";
            this.cboMaquina.Size = new System.Drawing.Size(240, 23);
            this.cboMaquina.TabIndex = 0;
            //
            // lblFechaInicio
            //
            this.lblFechaInicio.AutoSize = true;
            this.lblFechaInicio.Location = new System.Drawing.Point(360, 32);
            this.lblFechaInicio.Name = "lblFechaInicio";
            this.lblFechaInicio.Size = new System.Drawing.Size(75, 15);
            this.lblFechaInicio.Text = "Fecha inicio:";
            //
            // dtpFechaInicio
            //
            this.dtpFechaInicio.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaInicio.Location = new System.Drawing.Point(444, 29);
            this.dtpFechaInicio.Name = "dtpFechaInicio";
            this.dtpFechaInicio.Size = new System.Drawing.Size(150, 23);
            this.dtpFechaInicio.TabIndex = 1;
            //
            // lblDetalleTecnico
            //
            this.lblDetalleTecnico.AutoSize = true;
            this.lblDetalleTecnico.Location = new System.Drawing.Point(16, 64);
            this.lblDetalleTecnico.Name = "lblDetalleTecnico";
            this.lblDetalleTecnico.Size = new System.Drawing.Size(78, 15);
            this.lblDetalleTecnico.Text = "Detalle técnico:";
            //
            // txtDetalleTecnico
            //
            this.txtDetalleTecnico.Location = new System.Drawing.Point(100, 61);
            this.txtDetalleTecnico.MaxLength = 500;
            this.txtDetalleTecnico.Name = "txtDetalleTecnico";
            this.txtDetalleTecnico.Size = new System.Drawing.Size(494, 23);
            this.txtDetalleTecnico.TabIndex = 2;
            //
            // btnRegistrar
            //
            this.btnRegistrar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRegistrar.Location = new System.Drawing.Point(590, 93);
            this.btnRegistrar.Name = "btnRegistrar";
            this.btnRegistrar.Size = new System.Drawing.Size(154, 28);
            this.btnRegistrar.TabIndex = 3;
            this.btnRegistrar.Text = "&Registrar";
            this.btnRegistrar.UseVisualStyleBackColor = true;
            this.btnRegistrar.Click += new System.EventHandler(this.btnRegistrar_Click);
            //
            // dgvMantenimientosActivos
            //
            this.dgvMantenimientosActivos.AllowUserToAddRows = false;
            this.dgvMantenimientosActivos.AllowUserToDeleteRows = false;
            this.dgvMantenimientosActivos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvMantenimientosActivos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMantenimientosActivos.Location = new System.Drawing.Point(16, 160);
            this.dgvMantenimientosActivos.MultiSelect = false;
            this.dgvMantenimientosActivos.Name = "dgvMantenimientosActivos";
            this.dgvMantenimientosActivos.ReadOnly = true;
            this.dgvMantenimientosActivos.RowHeadersVisible = false;
            this.dgvMantenimientosActivos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMantenimientosActivos.Size = new System.Drawing.Size(760, 192);
            this.dgvMantenimientosActivos.TabIndex = 1;
            //
            // grpFinalizar
            //
            this.grpFinalizar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpFinalizar.Controls.Add(this.lblFechaFin);
            this.grpFinalizar.Controls.Add(this.dtpFechaFin);
            this.grpFinalizar.Controls.Add(this.btnFinalizar);
            this.grpFinalizar.Location = new System.Drawing.Point(16, 360);
            this.grpFinalizar.Name = "grpFinalizar";
            this.grpFinalizar.Size = new System.Drawing.Size(760, 64);
            this.grpFinalizar.TabIndex = 2;
            this.grpFinalizar.TabStop = false;
            this.grpFinalizar.Text = "Finalizar mantenimiento seleccionado";
            //
            // lblFechaFin
            //
            this.lblFechaFin.AutoSize = true;
            this.lblFechaFin.Location = new System.Drawing.Point(16, 28);
            this.lblFechaFin.Name = "lblFechaFin";
            this.lblFechaFin.Size = new System.Drawing.Size(63, 15);
            this.lblFechaFin.Text = "Fecha fin:";
            //
            // dtpFechaFin
            //
            this.dtpFechaFin.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaFin.Location = new System.Drawing.Point(100, 25);
            this.dtpFechaFin.Name = "dtpFechaFin";
            this.dtpFechaFin.Size = new System.Drawing.Size(150, 23);
            this.dtpFechaFin.TabIndex = 0;
            //
            // btnFinalizar
            //
            this.btnFinalizar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFinalizar.Location = new System.Drawing.Point(590, 21);
            this.btnFinalizar.Name = "btnFinalizar";
            this.btnFinalizar.Size = new System.Drawing.Size(154, 28);
            this.btnFinalizar.TabIndex = 1;
            this.btnFinalizar.Text = "&Finalizar";
            this.btnFinalizar.UseVisualStyleBackColor = true;
            this.btnFinalizar.Click += new System.EventHandler(this.btnFinalizar_Click);
            //
            // frmMantenimiento
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 440);
            this.Controls.Add(this.grpNuevo);
            this.Controls.Add(this.dgvMantenimientosActivos);
            this.Controls.Add(this.grpFinalizar);
            this.MinimumSize = new System.Drawing.Size(808, 479);
            this.Name = "frmMantenimiento";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Mantenimiento";
            this.Load += new System.EventHandler(this.frmMantenimiento_Load);
            this.grpNuevo.ResumeLayout(false);
            this.grpNuevo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMantenimientosActivos)).EndInit();
            this.grpFinalizar.ResumeLayout(false);
            this.grpFinalizar.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox grpNuevo;
        private System.Windows.Forms.Label lblMaquina;
        private System.Windows.Forms.ComboBox cboMaquina;
        private System.Windows.Forms.Label lblFechaInicio;
        private System.Windows.Forms.DateTimePicker dtpFechaInicio;
        private System.Windows.Forms.Label lblDetalleTecnico;
        private System.Windows.Forms.TextBox txtDetalleTecnico;
        private System.Windows.Forms.Button btnRegistrar;
        private System.Windows.Forms.DataGridView dgvMantenimientosActivos;
        private System.Windows.Forms.GroupBox grpFinalizar;
        private System.Windows.Forms.Label lblFechaFin;
        private System.Windows.Forms.DateTimePicker dtpFechaFin;
        private System.Windows.Forms.Button btnFinalizar;
    }
}
