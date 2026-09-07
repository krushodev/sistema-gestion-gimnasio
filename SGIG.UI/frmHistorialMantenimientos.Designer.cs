namespace SGIG.UI
{
    partial class frmHistorialMantenimientos
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
            this.lblMaquina = new System.Windows.Forms.Label();
            this.cboMaquina = new System.Windows.Forms.ComboBox();
            this.dgvHistorialMantenimientos = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorialMantenimientos)).BeginInit();
            this.SuspendLayout();
            //
            // lblMaquina
            //
            this.lblMaquina.AutoSize = true;
            this.lblMaquina.Location = new System.Drawing.Point(16, 19);
            this.lblMaquina.Name = "lblMaquina";
            this.lblMaquina.Size = new System.Drawing.Size(58, 15);
            this.lblMaquina.TabIndex = 0;
            this.lblMaquina.Text = "Máquina:";
            //
            // cboMaquina
            //
            this.cboMaquina.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMaquina.Location = new System.Drawing.Point(100, 16);
            this.cboMaquina.Name = "cboMaquina";
            this.cboMaquina.Size = new System.Drawing.Size(300, 23);
            this.cboMaquina.TabIndex = 1;
            this.cboMaquina.SelectedIndexChanged += new System.EventHandler(this.cboMaquina_SelectedIndexChanged);
            //
            // dgvHistorialMantenimientos
            //
            this.dgvHistorialMantenimientos.AllowUserToAddRows = false;
            this.dgvHistorialMantenimientos.AllowUserToDeleteRows = false;
            this.dgvHistorialMantenimientos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvHistorialMantenimientos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHistorialMantenimientos.Location = new System.Drawing.Point(16, 52);
            this.dgvHistorialMantenimientos.MultiSelect = false;
            this.dgvHistorialMantenimientos.Name = "dgvHistorialMantenimientos";
            this.dgvHistorialMantenimientos.ReadOnly = true;
            this.dgvHistorialMantenimientos.RowHeadersVisible = false;
            this.dgvHistorialMantenimientos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvHistorialMantenimientos.Size = new System.Drawing.Size(760, 320);
            this.dgvHistorialMantenimientos.TabIndex = 2;
            //
            // frmHistorialMantenimientos
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 392);
            this.Controls.Add(this.lblMaquina);
            this.Controls.Add(this.cboMaquina);
            this.Controls.Add(this.dgvHistorialMantenimientos);
            this.MinimumSize = new System.Drawing.Size(808, 431);
            this.Name = "frmHistorialMantenimientos";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Historial de Mantenimientos";
            this.Load += new System.EventHandler(this.frmHistorialMantenimientos_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorialMantenimientos)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblMaquina;
        private System.Windows.Forms.ComboBox cboMaquina;
        private System.Windows.Forms.DataGridView dgvHistorialMantenimientos;
    }
}
