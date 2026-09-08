namespace SGIG.UI
{
    partial class frmSocioEditor
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

        private void InitializeComponent()
        {
            this.ucDatosPersona = new SGIG.UI.ucDatosPersona();
            this.lblAptoMedico = new System.Windows.Forms.Label();
            this.txtAptoMedico = new System.Windows.Forms.TextBox();
            this.lblPlan = new System.Windows.Forms.Label();
            this.cboPlan = new System.Windows.Forms.ComboBox();
            this.lblCaptionVencimiento = new System.Windows.Forms.Label();
            this.lblFechaVencimientoCuota = new System.Windows.Forms.Label();
            this.chkActivo = new System.Windows.Forms.CheckBox();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // ucDatosPersona
            //
            this.ucDatosPersona.Location = new System.Drawing.Point(16, 16);
            this.ucDatosPersona.Name = "ucDatosPersona";
            this.ucDatosPersona.Size = new System.Drawing.Size(560, 168);
            this.ucDatosPersona.TabIndex = 0;
            //
            // lblAptoMedico
            //
            this.lblAptoMedico.AutoSize = true;
            this.lblAptoMedico.Location = new System.Drawing.Point(16, 197);
            this.lblAptoMedico.Name = "lblAptoMedico";
            this.lblAptoMedico.Size = new System.Drawing.Size(76, 15);
            this.lblAptoMedico.Text = "Apto médico:";
            //
            // txtAptoMedico
            //
            this.txtAptoMedico.Location = new System.Drawing.Point(100, 194);
            this.txtAptoMedico.MaxLength = 200;
            this.txtAptoMedico.Name = "txtAptoMedico";
            this.txtAptoMedico.Size = new System.Drawing.Size(180, 23);
            this.txtAptoMedico.TabIndex = 1;
            //
            // lblPlan
            //
            this.lblPlan.AutoSize = true;
            this.lblPlan.Location = new System.Drawing.Point(300, 197);
            this.lblPlan.Name = "lblPlan";
            this.lblPlan.Size = new System.Drawing.Size(90, 15);
            this.lblPlan.Text = "Plan preferido:";
            //
            // cboPlan
            //
            this.cboPlan.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPlan.Enabled = false;
            this.cboPlan.Location = new System.Drawing.Point(392, 194);
            this.cboPlan.Name = "cboPlan";
            this.cboPlan.Size = new System.Drawing.Size(184, 23);
            this.cboPlan.TabIndex = 2;
            //
            // lblCaptionVencimiento
            //
            this.lblCaptionVencimiento.AutoSize = true;
            this.lblCaptionVencimiento.Location = new System.Drawing.Point(16, 229);
            this.lblCaptionVencimiento.Name = "lblCaptionVencimiento";
            this.lblCaptionVencimiento.Size = new System.Drawing.Size(96, 15);
            this.lblCaptionVencimiento.Text = "Vencim. cuota:";
            //
            // lblFechaVencimientoCuota
            //
            this.lblFechaVencimientoCuota.AutoSize = true;
            this.lblFechaVencimientoCuota.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblFechaVencimientoCuota.Location = new System.Drawing.Point(120, 229);
            this.lblFechaVencimientoCuota.Name = "lblFechaVencimientoCuota";
            this.lblFechaVencimientoCuota.Size = new System.Drawing.Size(130, 15);
            this.lblFechaVencimientoCuota.Text = "Sin cuota registrada";
            //
            // chkActivo
            //
            this.chkActivo.AutoSize = true;
            this.chkActivo.Checked = true;
            this.chkActivo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkActivo.Enabled = false;
            this.chkActivo.Location = new System.Drawing.Point(392, 228);
            this.chkActivo.Name = "chkActivo";
            this.chkActivo.Size = new System.Drawing.Size(62, 19);
            this.chkActivo.TabIndex = 3;
            this.chkActivo.Text = "Activo";
            this.chkActivo.UseVisualStyleBackColor = true;
            //
            // btnGuardar
            //
            this.btnGuardar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGuardar.Location = new System.Drawing.Point(400, 266);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(86, 28);
            this.btnGuardar.TabIndex = 4;
            this.btnGuardar.Text = "&Guardar";
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            //
            // btnCancelar
            //
            this.btnCancelar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelar.Location = new System.Drawing.Point(490, 266);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(86, 28);
            this.btnCancelar.TabIndex = 5;
            this.btnCancelar.Text = "&Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            //
            // frmSocioEditor
            //
            this.AcceptButton = this.btnGuardar;
            this.CancelButton = this.btnCancelar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(592, 310);
            this.Controls.Add(this.ucDatosPersona);
            this.Controls.Add(this.lblAptoMedico);
            this.Controls.Add(this.txtAptoMedico);
            this.Controls.Add(this.lblPlan);
            this.Controls.Add(this.cboPlan);
            this.Controls.Add(this.lblCaptionVencimiento);
            this.Controls.Add(this.lblFechaVencimientoCuota);
            this.Controls.Add(this.chkActivo);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.btnCancelar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSocioEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Socio";
            this.Load += new System.EventHandler(this.frmSocioEditor_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private SGIG.UI.ucDatosPersona ucDatosPersona;
        private System.Windows.Forms.Label lblAptoMedico;
        private System.Windows.Forms.TextBox txtAptoMedico;
        private System.Windows.Forms.Label lblPlan;
        private System.Windows.Forms.ComboBox cboPlan;
        private System.Windows.Forms.Label lblCaptionVencimiento;
        private System.Windows.Forms.Label lblFechaVencimientoCuota;
        private System.Windows.Forms.CheckBox chkActivo;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCancelar;
    }
}
