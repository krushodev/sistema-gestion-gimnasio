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
            // pnlResultado
            //
            this.pnlResultado.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlResultado.BackColor = System.Drawing.SystemColors.Control;
            this.pnlResultado.Controls.Add(this.lblNombreSocio);
            this.pnlResultado.Controls.Add(this.lblResultado);
            this.pnlResultado.Location = new System.Drawing.Point(24, 120);
            this.pnlResultado.Name = "pnlResultado";
            this.pnlResultado.Size = new System.Drawing.Size(552, 260);
            this.pnlResultado.TabIndex = 2;
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
            this.ClientSize = new System.Drawing.Size(600, 410);
            this.Controls.Add(this.lblDocumento);
            this.Controls.Add(this.txtDocumento);
            this.Controls.Add(this.pnlResultado);
            this.MinimumSize = new System.Drawing.Size(616, 449);
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
        private System.Windows.Forms.Panel pnlResultado;
        private System.Windows.Forms.Label lblResultado;
        private System.Windows.Forms.Label lblNombreSocio;
    }
}
