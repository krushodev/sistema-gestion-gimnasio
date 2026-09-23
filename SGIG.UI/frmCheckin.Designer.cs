namespace SGIG.UI
{
    partial class frmCheckin
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
            this.lblDocumento = new System.Windows.Forms.Label();
            this.txtDocumento = new System.Windows.Forms.TextBox();
            this.lblAyuda = new System.Windows.Forms.Label();
            this.btnBuscarPorNombre = new System.Windows.Forms.Button();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.pnlResultado = new System.Windows.Forms.Panel();
            this.lblResultado = new System.Windows.Forms.Label();
            this.lblNombreSocio = new System.Windows.Forms.Label();
            this.pnlResultado.SuspendLayout();
            this.SuspendLayout();
            //
            // lblDocumento
            //
            this.lblDocumento.AutoSize = true;
            this.lblDocumento.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblDocumento.Location = new System.Drawing.Point(24, 24);
            this.lblDocumento.Name = "lblDocumento";
            this.lblDocumento.Size = new System.Drawing.Size(97, 21);
            this.lblDocumento.TabIndex = 0;
            this.lblDocumento.Text = "Documento:";
            //
            // txtDocumento
            //
            this.txtDocumento.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right));
            this.txtDocumento.Font = new System.Drawing.Font("Segoe UI", 20F);
            this.txtDocumento.Location = new System.Drawing.Point(24, 56);
            this.txtDocumento.MaxLength = 20;
            this.txtDocumento.Name = "txtDocumento";
            this.txtDocumento.Size = new System.Drawing.Size(552, 45);
            this.txtDocumento.TabIndex = 1;
            this.txtDocumento.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtDocumento_KeyDown);
            //
            // lblAyuda
            //
            this.lblAyuda.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right));
            this.lblAyuda.ForeColor = System.Drawing.Color.Gray;
            this.lblAyuda.Location = new System.Drawing.Point(24, 104);
            this.lblAyuda.Name = "lblAyuda";
            this.lblAyuda.Size = new System.Drawing.Size(552, 18);
            this.lblAyuda.TabIndex = 2;
            this.lblAyuda.Text = "Escaneá o escribí el documento y presioná Enter para registrar el acceso.";
            //
            // btnBuscarPorNombre
            //
            this.btnBuscarPorNombre.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBuscarPorNombre.Location = new System.Drawing.Point(304, 130);
            this.btnBuscarPorNombre.Name = "btnBuscarPorNombre";
            this.btnBuscarPorNombre.Size = new System.Drawing.Size(170, 30);
            this.btnBuscarPorNombre.TabIndex = 3;
            this.btnBuscarPorNombre.Text = "🔍 Buscar por nombre";
            this.btnBuscarPorNombre.UseVisualStyleBackColor = true;
            this.btnBuscarPorNombre.Click += new System.EventHandler(this.BtnBuscarPorNombre_Click);
            //
            // btnLimpiar
            //
            this.btnLimpiar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLimpiar.Location = new System.Drawing.Point(486, 130);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(90, 30);
            this.btnLimpiar.TabIndex = 4;
            this.btnLimpiar.Text = "Limpiar";
            this.btnLimpiar.UseVisualStyleBackColor = true;
            this.btnLimpiar.Click += new System.EventHandler(this.BtnLimpiar_Click);
            //
            // pnlResultado
            //
            this.pnlResultado.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlResultado.BackColor = System.Drawing.SystemColors.Control;
            this.pnlResultado.Controls.Add(this.lblNombreSocio);
            this.pnlResultado.Controls.Add(this.lblResultado);
            this.pnlResultado.Location = new System.Drawing.Point(24, 168);
            this.pnlResultado.Name = "pnlResultado";
            this.pnlResultado.Size = new System.Drawing.Size(552, 260);
            this.pnlResultado.TabIndex = 5;
            //
            // lblResultado
            //
            this.lblResultado.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right));
            this.lblResultado.Font = new System.Drawing.Font("Segoe UI", 28F, System.Drawing.FontStyle.Bold);
            this.lblResultado.Location = new System.Drawing.Point(16, 40);
            this.lblResultado.Name = "lblResultado";
            this.lblResultado.Size = new System.Drawing.Size(520, 90);
            this.lblResultado.TabIndex = 0;
            this.lblResultado.Text = "Esperando lectura...";
            this.lblResultado.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // lblNombreSocio
            //
            this.lblNombreSocio.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right));
            this.lblNombreSocio.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.lblNombreSocio.Location = new System.Drawing.Point(16, 150);
            this.lblNombreSocio.Name = "lblNombreSocio";
            this.lblNombreSocio.Size = new System.Drawing.Size(520, 90);
            this.lblNombreSocio.TabIndex = 1;
            this.lblNombreSocio.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // frmCheckin
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 458);
            this.Controls.Add(this.lblDocumento);
            this.Controls.Add(this.txtDocumento);
            this.Controls.Add(this.lblAyuda);
            this.Controls.Add(this.btnBuscarPorNombre);
            this.Controls.Add(this.btnLimpiar);
            this.Controls.Add(this.pnlResultado);
            this.MinimumSize = new System.Drawing.Size(616, 497);
            this.Name = "frmCheckin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Control de Acceso";
            this.Load += new System.EventHandler(this.frmCheckin_Load);
            this.pnlResultado.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblDocumento;
        private System.Windows.Forms.TextBox txtDocumento;
        private System.Windows.Forms.Label lblAyuda;
        private System.Windows.Forms.Button btnBuscarPorNombre;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.Panel pnlResultado;
        private System.Windows.Forms.Label lblResultado;
        private System.Windows.Forms.Label lblNombreSocio;
    }
}
