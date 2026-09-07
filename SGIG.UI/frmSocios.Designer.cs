namespace SGIG.UI
{
    partial class frmSocios
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
            this.dgvSocios = new System.Windows.Forms.DataGridView();
            this.txtBuscarDocumento = new System.Windows.Forms.TextBox();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.btnNuevo = new System.Windows.Forms.Button();
            this.btnEditar = new System.Windows.Forms.Button();
            this.btnDarDeBaja = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.grpDatos = new System.Windows.Forms.GroupBox();
            this.dtpFechaNacimiento = new System.Windows.Forms.DateTimePicker();
            this.txtDocumento = new System.Windows.Forms.TextBox();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.txtApellido = new System.Windows.Forms.TextBox();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.txtTelefono = new System.Windows.Forms.TextBox();
            this.txtAptoMedico = new System.Windows.Forms.TextBox();
            this.cboTipoDocumento = new System.Windows.Forms.ComboBox();
            this.cboLocalidad = new System.Windows.Forms.ComboBox();
            this.cboPlan = new System.Windows.Forms.ComboBox();
            this.chkActivo = new System.Windows.Forms.CheckBox();
            this.lblFechaVencimientoCuota = new System.Windows.Forms.Label();
            this.lblBuscarDocumento = new System.Windows.Forms.Label();
            this.lblDocumento = new System.Windows.Forms.Label();
            this.lblTipoDocumento = new System.Windows.Forms.Label();
            this.lblNombre = new System.Windows.Forms.Label();
            this.lblApellido = new System.Windows.Forms.Label();
            this.lblEmail = new System.Windows.Forms.Label();
            this.lblTelefono = new System.Windows.Forms.Label();
            this.lblLocalidad = new System.Windows.Forms.Label();
            this.lblFechaNacimiento = new System.Windows.Forms.Label();
            this.lblAptoMedico = new System.Windows.Forms.Label();
            this.lblPlan = new System.Windows.Forms.Label();
            this.lblCaptionVencimiento = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSocios)).BeginInit();
            this.grpDatos.SuspendLayout();
            this.SuspendLayout();
            //
            // lblBuscarDocumento
            //
            this.lblBuscarDocumento.AutoSize = true;
            this.lblBuscarDocumento.Location = new System.Drawing.Point(16, 19);
            this.lblBuscarDocumento.Name = "lblBuscarDocumento";
            this.lblBuscarDocumento.Size = new System.Drawing.Size(72, 15);
            this.lblBuscarDocumento.TabIndex = 0;
            this.lblBuscarDocumento.Text = "Documento:";
            //
            // txtBuscarDocumento
            //
            this.txtBuscarDocumento.Location = new System.Drawing.Point(110, 16);
            this.txtBuscarDocumento.MaxLength = 20;
            this.txtBuscarDocumento.Name = "txtBuscarDocumento";
            this.txtBuscarDocumento.Size = new System.Drawing.Size(180, 23);
            this.txtBuscarDocumento.TabIndex = 1;
            //
            // btnBuscar
            //
            this.btnBuscar.Location = new System.Drawing.Point(300, 15);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(140, 25);
            this.btnBuscar.TabIndex = 2;
            this.btnBuscar.Text = "&Buscar / reutilizar";
            this.btnBuscar.UseVisualStyleBackColor = true;
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            //
            // dgvSocios
            //
            this.dgvSocios.AllowUserToAddRows = false;
            this.dgvSocios.AllowUserToDeleteRows = false;
            this.dgvSocios.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvSocios.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSocios.Location = new System.Drawing.Point(16, 48);
            this.dgvSocios.MultiSelect = false;
            this.dgvSocios.Name = "dgvSocios";
            this.dgvSocios.ReadOnly = true;
            this.dgvSocios.RowHeadersVisible = false;
            this.dgvSocios.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSocios.Size = new System.Drawing.Size(760, 200);
            this.dgvSocios.TabIndex = 3;
            //
            // btnDarDeBaja
            //
            this.btnDarDeBaja.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDarDeBaja.Location = new System.Drawing.Point(482, 256);
            this.btnDarDeBaja.Name = "btnDarDeBaja";
            this.btnDarDeBaja.Size = new System.Drawing.Size(110, 28);
            this.btnDarDeBaja.TabIndex = 4;
            this.btnDarDeBaja.Text = "Dar de &baja";
            this.btnDarDeBaja.UseVisualStyleBackColor = true;
            this.btnDarDeBaja.Click += new System.EventHandler(this.btnDarDeBaja_Click);
            //
            // btnNuevo
            //
            this.btnNuevo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNuevo.Location = new System.Drawing.Point(600, 256);
            this.btnNuevo.Name = "btnNuevo";
            this.btnNuevo.Size = new System.Drawing.Size(86, 28);
            this.btnNuevo.TabIndex = 5;
            this.btnNuevo.Text = "&Nuevo";
            this.btnNuevo.UseVisualStyleBackColor = true;
            this.btnNuevo.Click += new System.EventHandler(this.btnNuevo_Click);
            //
            // btnEditar
            //
            this.btnEditar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEditar.Location = new System.Drawing.Point(690, 256);
            this.btnEditar.Name = "btnEditar";
            this.btnEditar.Size = new System.Drawing.Size(86, 28);
            this.btnEditar.TabIndex = 6;
            this.btnEditar.Text = "&Editar";
            this.btnEditar.UseVisualStyleBackColor = true;
            this.btnEditar.Click += new System.EventHandler(this.btnEditar_Click);
            //
            // grpDatos
            //
            this.grpDatos.Controls.Add(this.lblDocumento);
            this.grpDatos.Controls.Add(this.lblTipoDocumento);
            this.grpDatos.Controls.Add(this.lblNombre);
            this.grpDatos.Controls.Add(this.lblApellido);
            this.grpDatos.Controls.Add(this.lblEmail);
            this.grpDatos.Controls.Add(this.lblTelefono);
            this.grpDatos.Controls.Add(this.lblLocalidad);
            this.grpDatos.Controls.Add(this.lblFechaNacimiento);
            this.grpDatos.Controls.Add(this.lblAptoMedico);
            this.grpDatos.Controls.Add(this.lblPlan);
            this.grpDatos.Controls.Add(this.lblCaptionVencimiento);
            this.grpDatos.Controls.Add(this.lblFechaVencimientoCuota);
            this.grpDatos.Controls.Add(this.txtDocumento);
            this.grpDatos.Controls.Add(this.txtNombre);
            this.grpDatos.Controls.Add(this.txtApellido);
            this.grpDatos.Controls.Add(this.txtEmail);
            this.grpDatos.Controls.Add(this.txtTelefono);
            this.grpDatos.Controls.Add(this.txtAptoMedico);
            this.grpDatos.Controls.Add(this.cboTipoDocumento);
            this.grpDatos.Controls.Add(this.cboLocalidad);
            this.grpDatos.Controls.Add(this.cboPlan);
            this.grpDatos.Controls.Add(this.dtpFechaNacimiento);
            this.grpDatos.Controls.Add(this.chkActivo);
            this.grpDatos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpDatos.Location = new System.Drawing.Point(16, 296);
            this.grpDatos.Name = "grpDatos";
            this.grpDatos.Size = new System.Drawing.Size(760, 268);
            this.grpDatos.TabIndex = 7;
            this.grpDatos.TabStop = false;
            this.grpDatos.Text = "Datos del socio";
            //
            // lblDocumento
            //
            this.lblDocumento.AutoSize = true;
            this.lblDocumento.Location = new System.Drawing.Point(16, 32);
            this.lblDocumento.Name = "lblDocumento";
            this.lblDocumento.Size = new System.Drawing.Size(72, 15);
            this.lblDocumento.Text = "Documento:";
            //
            // lblTipoDocumento
            //
            this.lblTipoDocumento.AutoSize = true;
            this.lblTipoDocumento.Location = new System.Drawing.Point(300, 32);
            this.lblTipoDocumento.Name = "lblTipoDocumento";
            this.lblTipoDocumento.Size = new System.Drawing.Size(64, 15);
            this.lblTipoDocumento.Text = "Tipo doc.:";
            //
            // lblNombre
            //
            this.lblNombre.AutoSize = true;
            this.lblNombre.Location = new System.Drawing.Point(16, 64);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Size = new System.Drawing.Size(52, 15);
            this.lblNombre.Text = "Nombre:";
            //
            // lblApellido
            //
            this.lblApellido.AutoSize = true;
            this.lblApellido.Location = new System.Drawing.Point(300, 64);
            this.lblApellido.Name = "lblApellido";
            this.lblApellido.Size = new System.Drawing.Size(55, 15);
            this.lblApellido.Text = "Apellido:";
            //
            // lblEmail
            //
            this.lblEmail.AutoSize = true;
            this.lblEmail.Location = new System.Drawing.Point(16, 96);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(39, 15);
            this.lblEmail.Text = "Email:";
            //
            // lblTelefono
            //
            this.lblTelefono.AutoSize = true;
            this.lblTelefono.Location = new System.Drawing.Point(300, 96);
            this.lblTelefono.Name = "lblTelefono";
            this.lblTelefono.Size = new System.Drawing.Size(57, 15);
            this.lblTelefono.Text = "Teléfono:";
            //
            // lblLocalidad
            //
            this.lblLocalidad.AutoSize = true;
            this.lblLocalidad.Location = new System.Drawing.Point(16, 128);
            this.lblLocalidad.Name = "lblLocalidad";
            this.lblLocalidad.Size = new System.Drawing.Size(62, 15);
            this.lblLocalidad.Text = "Localidad:";
            //
            // lblFechaNacimiento
            //
            this.lblFechaNacimiento.AutoSize = true;
            this.lblFechaNacimiento.Location = new System.Drawing.Point(16, 168);
            this.lblFechaNacimiento.Name = "lblFechaNacimiento";
            this.lblFechaNacimiento.Size = new System.Drawing.Size(105, 15);
            this.lblFechaNacimiento.Text = "Fecha nacimiento:";
            //
            // lblAptoMedico
            //
            this.lblAptoMedico.AutoSize = true;
            this.lblAptoMedico.Location = new System.Drawing.Point(300, 168);
            this.lblAptoMedico.Name = "lblAptoMedico";
            this.lblAptoMedico.Size = new System.Drawing.Size(76, 15);
            this.lblAptoMedico.Text = "Apto médico:";
            //
            // lblPlan
            //
            this.lblPlan.AutoSize = true;
            this.lblPlan.Location = new System.Drawing.Point(16, 200);
            this.lblPlan.Name = "lblPlan";
            this.lblPlan.Size = new System.Drawing.Size(90, 15);
            this.lblPlan.Text = "Plan preferido:";
            //
            // lblCaptionVencimiento
            //
            this.lblCaptionVencimiento.AutoSize = true;
            this.lblCaptionVencimiento.Location = new System.Drawing.Point(300, 200);
            this.lblCaptionVencimiento.Name = "lblCaptionVencimiento";
            this.lblCaptionVencimiento.Size = new System.Drawing.Size(96, 15);
            this.lblCaptionVencimiento.Text = "Vencim. cuota:";
            //
            // lblFechaVencimientoCuota
            //
            this.lblFechaVencimientoCuota.AutoSize = true;
            this.lblFechaVencimientoCuota.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblFechaVencimientoCuota.Location = new System.Drawing.Point(392, 200);
            this.lblFechaVencimientoCuota.Name = "lblFechaVencimientoCuota";
            this.lblFechaVencimientoCuota.Size = new System.Drawing.Size(130, 15);
            this.lblFechaVencimientoCuota.Text = "Sin cuota registrada";
            //
            // txtDocumento
            //
            this.txtDocumento.Location = new System.Drawing.Point(100, 29);
            this.txtDocumento.MaxLength = 20;
            this.txtDocumento.Name = "txtDocumento";
            this.txtDocumento.Size = new System.Drawing.Size(180, 23);
            this.txtDocumento.TabIndex = 0;
            //
            // txtNombre
            //
            this.txtNombre.Location = new System.Drawing.Point(100, 61);
            this.txtNombre.MaxLength = 100;
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(180, 23);
            this.txtNombre.TabIndex = 1;
            //
            // txtApellido
            //
            this.txtApellido.Location = new System.Drawing.Point(392, 61);
            this.txtApellido.MaxLength = 100;
            this.txtApellido.Name = "txtApellido";
            this.txtApellido.Size = new System.Drawing.Size(180, 23);
            this.txtApellido.TabIndex = 2;
            //
            // txtEmail
            //
            this.txtEmail.Location = new System.Drawing.Point(100, 93);
            this.txtEmail.MaxLength = 150;
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(180, 23);
            this.txtEmail.TabIndex = 3;
            //
            // txtTelefono
            //
            this.txtTelefono.Location = new System.Drawing.Point(392, 93);
            this.txtTelefono.MaxLength = 30;
            this.txtTelefono.Name = "txtTelefono";
            this.txtTelefono.Size = new System.Drawing.Size(180, 23);
            this.txtTelefono.TabIndex = 4;
            //
            // txtAptoMedico
            //
            this.txtAptoMedico.Location = new System.Drawing.Point(392, 165);
            this.txtAptoMedico.MaxLength = 200;
            this.txtAptoMedico.Name = "txtAptoMedico";
            this.txtAptoMedico.Size = new System.Drawing.Size(180, 23);
            this.txtAptoMedico.TabIndex = 6;
            //
            // cboTipoDocumento
            //
            this.cboTipoDocumento.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTipoDocumento.Location = new System.Drawing.Point(392, 29);
            this.cboTipoDocumento.Name = "cboTipoDocumento";
            this.cboTipoDocumento.Size = new System.Drawing.Size(180, 23);
            this.cboTipoDocumento.TabIndex = 8;
            //
            // cboLocalidad
            //
            this.cboLocalidad.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLocalidad.Location = new System.Drawing.Point(100, 125);
            this.cboLocalidad.Name = "cboLocalidad";
            this.cboLocalidad.Size = new System.Drawing.Size(472, 23);
            this.cboLocalidad.TabIndex = 9;
            //
            // cboPlan
            //
            this.cboPlan.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPlan.Enabled = false;
            this.cboPlan.Location = new System.Drawing.Point(100, 197);
            this.cboPlan.Name = "cboPlan";
            this.cboPlan.Size = new System.Drawing.Size(180, 23);
            this.cboPlan.TabIndex = 10;
            //
            // dtpFechaNacimiento
            //
            this.dtpFechaNacimiento.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaNacimiento.Location = new System.Drawing.Point(100, 165);
            this.dtpFechaNacimiento.Name = "dtpFechaNacimiento";
            this.dtpFechaNacimiento.Size = new System.Drawing.Size(180, 23);
            this.dtpFechaNacimiento.TabIndex = 5;
            //
            // chkActivo
            //
            this.chkActivo.AutoSize = true;
            this.chkActivo.Checked = true;
            this.chkActivo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkActivo.Enabled = false;
            this.chkActivo.Location = new System.Drawing.Point(100, 231);
            this.chkActivo.Name = "chkActivo";
            this.chkActivo.Size = new System.Drawing.Size(62, 19);
            this.chkActivo.TabIndex = 11;
            this.chkActivo.Text = "Activo";
            this.chkActivo.UseVisualStyleBackColor = true;
            //
            // btnGuardar
            //
            this.btnGuardar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGuardar.Location = new System.Drawing.Point(600, 576);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(86, 28);
            this.btnGuardar.TabIndex = 8;
            this.btnGuardar.Text = "&Guardar";
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            //
            // btnCancelar
            //
            this.btnCancelar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelar.Location = new System.Drawing.Point(690, 576);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(86, 28);
            this.btnCancelar.TabIndex = 9;
            this.btnCancelar.Text = "&Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            //
            // frmSocios
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 616);
            this.Controls.Add(this.lblBuscarDocumento);
            this.Controls.Add(this.txtBuscarDocumento);
            this.Controls.Add(this.btnBuscar);
            this.Controls.Add(this.dgvSocios);
            this.Controls.Add(this.btnDarDeBaja);
            this.Controls.Add(this.btnNuevo);
            this.Controls.Add(this.btnEditar);
            this.Controls.Add(this.grpDatos);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.btnCancelar);
            this.MinimumSize = new System.Drawing.Size(808, 655);
            this.Name = "frmSocios";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Socios";
            this.Load += new System.EventHandler(this.frmSocios_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSocios)).EndInit();
            this.grpDatos.ResumeLayout(false);
            this.grpDatos.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.DataGridView dgvSocios;
        private System.Windows.Forms.TextBox txtBuscarDocumento;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.Button btnNuevo;
        private System.Windows.Forms.Button btnEditar;
        private System.Windows.Forms.Button btnDarDeBaja;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.GroupBox grpDatos;
        private System.Windows.Forms.DateTimePicker dtpFechaNacimiento;
        private System.Windows.Forms.TextBox txtDocumento;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.TextBox txtApellido;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.TextBox txtTelefono;
        private System.Windows.Forms.TextBox txtAptoMedico;
        private System.Windows.Forms.ComboBox cboTipoDocumento;
        private System.Windows.Forms.ComboBox cboLocalidad;
        private System.Windows.Forms.ComboBox cboPlan;
        private System.Windows.Forms.CheckBox chkActivo;
        private System.Windows.Forms.Label lblFechaVencimientoCuota;
        private System.Windows.Forms.Label lblBuscarDocumento;
        private System.Windows.Forms.Label lblDocumento;
        private System.Windows.Forms.Label lblTipoDocumento;
        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.Label lblApellido;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.Label lblTelefono;
        private System.Windows.Forms.Label lblLocalidad;
        private System.Windows.Forms.Label lblFechaNacimiento;
        private System.Windows.Forms.Label lblAptoMedico;
        private System.Windows.Forms.Label lblPlan;
        private System.Windows.Forms.Label lblCaptionVencimiento;
    }
}
