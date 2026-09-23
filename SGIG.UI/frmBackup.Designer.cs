namespace SGIG.UI
{
    partial class frmBackup
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

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblSubtitulo = new System.Windows.Forms.Label();
            this.grpBackup = new System.Windows.Forms.GroupBox();
            this.lblDescBackup = new System.Windows.Forms.Label();
            this.btnGenerarBackup = new System.Windows.Forms.Button();
            this.grpRestaurar = new System.Windows.Forms.GroupBox();
            this.lblDescRestaurar = new System.Windows.Forms.Label();
            this.btnRestaurar = new System.Windows.Forms.Button();
            this.lblEstado = new System.Windows.Forms.Label();
            this.prgProgreso = new System.Windows.Forms.ProgressBar();
            this.grpBackup.SuspendLayout();
            this.grpRestaurar.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI Semibold", 15.75F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(32)))), ((int)(((byte)(44)))));
            this.lblTitulo.Location = new System.Drawing.Point(30, 24);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(374, 30);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Copia de Seguridad de la Base de Datos";
            // 
            // lblSubtitulo
            // 
            this.lblSubtitulo.AutoSize = true;
            this.lblSubtitulo.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblSubtitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(85)))), ((int)(((byte)(104)))));
            this.lblSubtitulo.Location = new System.Drawing.Point(32, 58);
            this.lblSubtitulo.Name = "lblSubtitulo";
            this.lblSubtitulo.Size = new System.Drawing.Size(522, 17);
            this.lblSubtitulo.TabIndex = 1;
            this.lblSubtitulo.Text = "Generá respaldos completos de SGIG_DB o restaurá el estado del sistema ante contingencias.";
            // 
            // grpBackup
            // 
            this.grpBackup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpBackup.Controls.Add(this.lblDescBackup);
            this.grpBackup.Controls.Add(this.btnGenerarBackup);
            this.grpBackup.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.grpBackup.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(55)))), ((int)(((byte)(72)))));
            this.grpBackup.Location = new System.Drawing.Point(35, 100);
            this.grpBackup.Name = "grpBackup";
            this.grpBackup.Size = new System.Drawing.Size(650, 115);
            this.grpBackup.TabIndex = 2;
            this.grpBackup.TabStop = false;
            this.grpBackup.Text = " Generar Respaldo (.bak) ";
            // 
            // lblDescBackup
            // 
            this.lblDescBackup.AutoSize = true;
            this.lblDescBackup.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblDescBackup.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(85)))), ((int)(((byte)(104)))));
            this.lblDescBackup.Location = new System.Drawing.Point(20, 30);
            this.lblDescBackup.Name = "lblDescBackup";
            this.lblDescBackup.Size = new System.Drawing.Size(512, 15);
            this.lblDescBackup.TabIndex = 0;
            this.lblDescBackup.Text = "Crea un archivo completo con el esquema, usuarios, socios, cobros y movimientos de auditoría.";
            // 
            // btnGenerarBackup
            // 
            this.btnGenerarBackup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(130)))), ((int)(((byte)(206)))));
            this.btnGenerarBackup.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGenerarBackup.FlatAppearance.BorderSize = 0;
            this.btnGenerarBackup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenerarBackup.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnGenerarBackup.ForeColor = System.Drawing.Color.White;
            this.btnGenerarBackup.Location = new System.Drawing.Point(23, 60);
            this.btnGenerarBackup.Name = "btnGenerarBackup";
            this.btnGenerarBackup.Size = new System.Drawing.Size(260, 36);
            this.btnGenerarBackup.TabIndex = 1;
            this.btnGenerarBackup.Text = "💾 Crear Copia de Seguridad Ahora";
            this.btnGenerarBackup.UseVisualStyleBackColor = false;
            this.btnGenerarBackup.Click += new System.EventHandler(this.btnGenerarBackup_Click);
            // 
            // grpRestaurar
            // 
            this.grpRestaurar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpRestaurar.Controls.Add(this.lblDescRestaurar);
            this.grpRestaurar.Controls.Add(this.btnRestaurar);
            this.grpRestaurar.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.grpRestaurar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(55)))), ((int)(((byte)(72)))));
            this.grpRestaurar.Location = new System.Drawing.Point(35, 235);
            this.grpRestaurar.Name = "grpRestaurar";
            this.grpRestaurar.Size = new System.Drawing.Size(650, 115);
            this.grpRestaurar.TabIndex = 3;
            this.grpRestaurar.TabStop = false;
            this.grpRestaurar.Text = " Restaurar Base de Datos ";
            // 
            // lblDescRestaurar
            // 
            this.lblDescRestaurar.AutoSize = true;
            this.lblDescRestaurar.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblDescRestaurar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(85)))), ((int)(((byte)(104)))));
            this.lblDescRestaurar.Location = new System.Drawing.Point(20, 30);
            this.lblDescRestaurar.Name = "lblDescRestaurar";
            this.lblDescRestaurar.Size = new System.Drawing.Size(469, 15);
            this.lblDescRestaurar.TabIndex = 0;
            this.lblDescRestaurar.Text = "Seleccioná un archivo .bak previamente generado para restablecer los datos del sistema.";
            // 
            // btnRestaurar
            // 
            this.btnRestaurar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(55)))), ((int)(((byte)(72)))));
            this.btnRestaurar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRestaurar.FlatAppearance.BorderSize = 0;
            this.btnRestaurar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRestaurar.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnRestaurar.ForeColor = System.Drawing.Color.White;
            this.btnRestaurar.Location = new System.Drawing.Point(23, 60);
            this.btnRestaurar.Name = "btnRestaurar";
            this.btnRestaurar.Size = new System.Drawing.Size(260, 36);
            this.btnRestaurar.TabIndex = 1;
            this.btnRestaurar.Text = "📂 Seleccionar archivo y restaurar...";
            this.btnRestaurar.UseVisualStyleBackColor = false;
            this.btnRestaurar.Click += new System.EventHandler(this.btnRestaurar_Click);
            // 
            // lblEstado
            // 
            this.lblEstado.AutoSize = true;
            this.lblEstado.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblEstado.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(85)))), ((int)(((byte)(104)))));
            this.lblEstado.Location = new System.Drawing.Point(35, 370);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Size = new System.Drawing.Size(104, 15);
            this.lblEstado.TabIndex = 4;
            this.lblEstado.Text = "Estado: En espera.";
            // 
            // prgProgreso
            // 
            this.prgProgreso.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.prgProgreso.Location = new System.Drawing.Point(35, 392);
            this.prgProgreso.Name = "prgProgreso";
            this.prgProgreso.Size = new System.Drawing.Size(650, 16);
            this.prgProgreso.TabIndex = 5;
            // 
            // frmBackup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.ClientSize = new System.Drawing.Size(720, 440);
            this.Controls.Add(this.prgProgreso);
            this.Controls.Add(this.lblEstado);
            this.Controls.Add(this.grpRestaurar);
            this.Controls.Add(this.grpBackup);
            this.Controls.Add(this.lblSubtitulo);
            this.Controls.Add(this.lblTitulo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmBackup";
            this.Text = "Copia de Seguridad";
            this.grpBackup.ResumeLayout(false);
            this.grpBackup.PerformLayout();
            this.grpRestaurar.ResumeLayout(false);
            this.grpRestaurar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblSubtitulo;
        private System.Windows.Forms.GroupBox grpBackup;
        private System.Windows.Forms.Label lblDescBackup;
        private System.Windows.Forms.Button btnGenerarBackup;
        private System.Windows.Forms.GroupBox grpRestaurar;
        private System.Windows.Forms.Label lblDescRestaurar;
        private System.Windows.Forms.Button btnRestaurar;
        private System.Windows.Forms.Label lblEstado;
        private System.Windows.Forms.ProgressBar prgProgreso;
    }
}