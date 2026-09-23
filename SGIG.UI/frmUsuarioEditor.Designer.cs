namespace SGIG.UI
{
    partial class frmUsuarioEditor
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
            this.lblLegajo = new System.Windows.Forms.Label();
            this.lblLegajoPrefijo = new System.Windows.Forms.Label();
            this.txtLegajo = new System.Windows.Forms.TextBox();
            this.lblFechaIngreso = new System.Windows.Forms.Label();
            this.dtpFechaIngreso = new System.Windows.Forms.DateTimePicker();
            this.lblRol = new System.Windows.Forms.Label();
            this.cboRol = new System.Windows.Forms.ComboBox();
            this.lblNombreUsuario = new System.Windows.Forms.Label();
            this.txtNombreUsuario = new System.Windows.Forms.TextBox();
            this.lblContrasenia = new System.Windows.Forms.Label();
            this.txtContrasenia = new System.Windows.Forms.TextBox();
            this.lblAyudaContrasenia = new System.Windows.Forms.Label();
            this.btnLimpiar = new System.Windows.Forms.Button();
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
            // lblLegajo
            //
            this.lblLegajo.AutoSize = true;
            this.lblLegajo.Location = new System.Drawing.Point(16, 197);
            this.lblLegajo.Name = "lblLegajo";
            this.lblLegajo.Size = new System.Drawing.Size(45, 15);
            this.lblLegajo.Text = "Legajo:";
            //
            // lblLegajoPrefijo
            //
            this.lblLegajoPrefijo.AutoSize = true;
            this.lblLegajoPrefijo.Location = new System.Drawing.Point(100, 197);
            this.lblLegajoPrefijo.Name = "lblLegajoPrefijo";
            this.lblLegajoPrefijo.Size = new System.Drawing.Size(32, 15);
            this.lblLegajoPrefijo.Text = "LEG-";
            //
            // txtLegajo
            //
            this.txtLegajo.Location = new System.Drawing.Point(134, 194);
            this.txtLegajo.MaxLength = 4;
            this.txtLegajo.Name = "txtLegajo";
            this.txtLegajo.Size = new System.Drawing.Size(50, 23);
            this.txtLegajo.TabIndex = 1;
            this.txtLegajo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            //
            // lblFechaIngreso
            //
            this.lblFechaIngreso.AutoSize = true;
            this.lblFechaIngreso.Location = new System.Drawing.Point(300, 197);
            this.lblFechaIngreso.Name = "lblFechaIngreso";
            this.lblFechaIngreso.Size = new System.Drawing.Size(86, 15);
            this.lblFechaIngreso.Text = "Fecha ingreso:";
            //
            // dtpFechaIngreso
            //
            this.dtpFechaIngreso.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaIngreso.Location = new System.Drawing.Point(392, 194);
            this.dtpFechaIngreso.Name = "dtpFechaIngreso";
            this.dtpFechaIngreso.Size = new System.Drawing.Size(184, 23);
            this.dtpFechaIngreso.TabIndex = 2;
            //
            // lblRol
            //
            this.lblRol.AutoSize = true;
            this.lblRol.Location = new System.Drawing.Point(16, 229);
            this.lblRol.Name = "lblRol";
            this.lblRol.Size = new System.Drawing.Size(24, 15);
            this.lblRol.Text = "Rol:";
            //
            // cboRol
            //
            this.cboRol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRol.Location = new System.Drawing.Point(100, 226);
            this.cboRol.Name = "cboRol";
            this.cboRol.Size = new System.Drawing.Size(180, 23);
            this.cboRol.TabIndex = 3;
            //
            // lblNombreUsuario
            //
            this.lblNombreUsuario.AutoSize = true;
            this.lblNombreUsuario.Location = new System.Drawing.Point(300, 229);
            this.lblNombreUsuario.Name = "lblNombreUsuario";
            this.lblNombreUsuario.Size = new System.Drawing.Size(52, 15);
            this.lblNombreUsuario.Text = "Usuario:";
            //
            // txtNombreUsuario
            //
            this.txtNombreUsuario.Location = new System.Drawing.Point(392, 226);
            this.txtNombreUsuario.MaxLength = 50;
            this.txtNombreUsuario.Name = "txtNombreUsuario";
            this.txtNombreUsuario.Size = new System.Drawing.Size(184, 23);
            this.txtNombreUsuario.TabIndex = 4;
            //
            // lblContrasenia
            //
            this.lblContrasenia.AutoSize = true;
            this.lblContrasenia.Location = new System.Drawing.Point(16, 261);
            this.lblContrasenia.Name = "lblContrasenia";
            this.lblContrasenia.Size = new System.Drawing.Size(72, 15);
            this.lblContrasenia.Text = "Contraseña:";
            //
            // txtContrasenia
            //
            this.txtContrasenia.Location = new System.Drawing.Point(100, 258);
            this.txtContrasenia.MaxLength = 50;
            this.txtContrasenia.PasswordChar = '*';
            this.txtContrasenia.Name = "txtContrasenia";
            this.txtContrasenia.Size = new System.Drawing.Size(180, 23);
            this.txtContrasenia.TabIndex = 5;
            //
            // lblAyudaContrasenia
            //
            this.lblAyudaContrasenia.AutoSize = true;
            this.lblAyudaContrasenia.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblAyudaContrasenia.Location = new System.Drawing.Point(300, 261);
            this.lblAyudaContrasenia.Name = "lblAyudaContrasenia";
            this.lblAyudaContrasenia.Size = new System.Drawing.Size(240, 15);
            this.lblAyudaContrasenia.Text = "";
            //
            // btnLimpiar
            //
            this.btnLimpiar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLimpiar.Location = new System.Drawing.Point(16, 310);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(120, 28);
            this.btnLimpiar.TabIndex = 8;
            this.btnLimpiar.Text = "Limpiar datos";
            this.btnLimpiar.UseVisualStyleBackColor = true;
            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);
            //
            // btnGuardar
            //
            this.btnGuardar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGuardar.Location = new System.Drawing.Point(392, 310);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(86, 28);
            this.btnGuardar.TabIndex = 6;
            this.btnGuardar.Text = "&Guardar";
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            //
            // btnCancelar
            //
            this.btnCancelar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelar.Location = new System.Drawing.Point(490, 310);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(86, 28);
            this.btnCancelar.TabIndex = 7;
            this.btnCancelar.Text = "&Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            //
            // frmUsuarioEditor
            //
            this.AcceptButton = this.btnGuardar;
            this.CancelButton = this.btnCancelar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(592, 354);
            this.Controls.Add(this.ucDatosPersona);
            this.Controls.Add(this.lblLegajo);
            this.Controls.Add(this.lblLegajoPrefijo);
            this.Controls.Add(this.txtLegajo);
            this.Controls.Add(this.lblFechaIngreso);
            this.Controls.Add(this.dtpFechaIngreso);
            this.Controls.Add(this.lblRol);
            this.Controls.Add(this.cboRol);
            this.Controls.Add(this.lblNombreUsuario);
            this.Controls.Add(this.txtNombreUsuario);
            this.Controls.Add(this.lblContrasenia);
            this.Controls.Add(this.txtContrasenia);
            this.Controls.Add(this.lblAyudaContrasenia);
            this.Controls.Add(this.btnLimpiar);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.btnCancelar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmUsuarioEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Usuario";
            this.Load += new System.EventHandler(this.frmUsuarioEditor_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private SGIG.UI.ucDatosPersona ucDatosPersona;
        private System.Windows.Forms.Label lblLegajo;
        private System.Windows.Forms.Label lblLegajoPrefijo;
        private System.Windows.Forms.TextBox txtLegajo;
        private System.Windows.Forms.Label lblFechaIngreso;
        private System.Windows.Forms.DateTimePicker dtpFechaIngreso;
        private System.Windows.Forms.Label lblRol;
        private System.Windows.Forms.ComboBox cboRol;
        private System.Windows.Forms.Label lblNombreUsuario;
        private System.Windows.Forms.TextBox txtNombreUsuario;
        private System.Windows.Forms.Label lblContrasenia;
        private System.Windows.Forms.TextBox txtContrasenia;
        private System.Windows.Forms.Label lblAyudaContrasenia;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCancelar;
    }
}
