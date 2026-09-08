namespace SGIG.UI
{
    partial class frmTablasParametricas
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
            this.tabCatalogos = new System.Windows.Forms.TabControl();
            this.tabRol = new System.Windows.Forms.TabPage();
            this.dgvRol = new System.Windows.Forms.DataGridView();
            this.btnNuevoRol = new System.Windows.Forms.Button();
            this.btnEditarRol = new System.Windows.Forms.Button();
            this.btnDarDeBajaRol = new System.Windows.Forms.Button();
            this.tabProvincia = new System.Windows.Forms.TabPage();
            this.dgvProvincia = new System.Windows.Forms.DataGridView();
            this.btnNuevoProvincia = new System.Windows.Forms.Button();
            this.btnEditarProvincia = new System.Windows.Forms.Button();
            this.btnDarDeBajaProvincia = new System.Windows.Forms.Button();
            this.tabLocalidad = new System.Windows.Forms.TabPage();
            this.dgvLocalidad = new System.Windows.Forms.DataGridView();
            this.btnNuevoLocalidad = new System.Windows.Forms.Button();
            this.btnEditarLocalidad = new System.Windows.Forms.Button();
            this.btnDarDeBajaLocalidad = new System.Windows.Forms.Button();
            this.tabTipoDocumento = new System.Windows.Forms.TabPage();
            this.dgvTipoDocumento = new System.Windows.Forms.DataGridView();
            this.btnNuevoTipoDocumento = new System.Windows.Forms.Button();
            this.btnEditarTipoDocumento = new System.Windows.Forms.Button();
            this.btnDarDeBajaTipoDocumento = new System.Windows.Forms.Button();
            this.tabMedioPago = new System.Windows.Forms.TabPage();
            this.dgvMedioPago = new System.Windows.Forms.DataGridView();
            this.btnNuevoMedioPago = new System.Windows.Forms.Button();
            this.btnEditarMedioPago = new System.Windows.Forms.Button();
            this.btnDarDeBajaMedioPago = new System.Windows.Forms.Button();
            this.tabCatalogos.SuspendLayout();
            this.tabRol.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRol)).BeginInit();
            this.tabProvincia.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProvincia)).BeginInit();
            this.tabLocalidad.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLocalidad)).BeginInit();
            this.tabTipoDocumento.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTipoDocumento)).BeginInit();
            this.tabMedioPago.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMedioPago)).BeginInit();
            this.SuspendLayout();
            //
            // tabCatalogos
            //
            this.tabCatalogos.Controls.Add(this.tabRol);
            this.tabCatalogos.Controls.Add(this.tabProvincia);
            this.tabCatalogos.Controls.Add(this.tabLocalidad);
            this.tabCatalogos.Controls.Add(this.tabTipoDocumento);
            this.tabCatalogos.Controls.Add(this.tabMedioPago);
            this.tabCatalogos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabCatalogos.Location = new System.Drawing.Point(0, 0);
            this.tabCatalogos.Name = "tabCatalogos";
            this.tabCatalogos.SelectedIndex = 0;
            this.tabCatalogos.Size = new System.Drawing.Size(624, 441);
            this.tabCatalogos.TabIndex = 0;
            //
            // tabRol
            //
            this.tabRol.Controls.Add(this.dgvRol);
            this.tabRol.Controls.Add(this.btnNuevoRol);
            this.tabRol.Controls.Add(this.btnEditarRol);
            this.tabRol.Controls.Add(this.btnDarDeBajaRol);
            this.tabRol.Location = new System.Drawing.Point(4, 24);
            this.tabRol.Name = "tabRol";
            this.tabRol.Padding = new System.Windows.Forms.Padding(3);
            this.tabRol.Size = new System.Drawing.Size(616, 413);
            this.tabRol.TabIndex = 0;
            this.tabRol.Text = "Roles";
            this.tabRol.UseVisualStyleBackColor = true;
            //
            // dgvRol
            //
            this.dgvRol.AllowUserToAddRows = false;
            this.dgvRol.AllowUserToDeleteRows = false;
            this.dgvRol.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvRol.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRol.Location = new System.Drawing.Point(16, 16);
            this.dgvRol.MultiSelect = false;
            this.dgvRol.Name = "dgvRol";
            this.dgvRol.ReadOnly = true;
            this.dgvRol.RowHeadersVisible = false;
            this.dgvRol.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvRol.Size = new System.Drawing.Size(584, 340);
            this.dgvRol.TabIndex = 0;
            //
            // btnNuevoRol
            //
            this.btnNuevoRol.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnNuevoRol.Location = new System.Drawing.Point(16, 368);
            this.btnNuevoRol.Name = "btnNuevoRol";
            this.btnNuevoRol.Size = new System.Drawing.Size(86, 28);
            this.btnNuevoRol.TabIndex = 1;
            this.btnNuevoRol.Text = "&Nuevo";
            this.btnNuevoRol.UseVisualStyleBackColor = true;
            this.btnNuevoRol.Click += new System.EventHandler(this.btnNuevoRol_Click);
            //
            // btnEditarRol
            //
            this.btnEditarRol.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEditarRol.Location = new System.Drawing.Point(108, 368);
            this.btnEditarRol.Name = "btnEditarRol";
            this.btnEditarRol.Size = new System.Drawing.Size(86, 28);
            this.btnEditarRol.TabIndex = 2;
            this.btnEditarRol.Text = "&Editar";
            this.btnEditarRol.UseVisualStyleBackColor = true;
            this.btnEditarRol.Click += new System.EventHandler(this.btnEditarRol_Click);
            //
            // btnDarDeBajaRol
            //
            this.btnDarDeBajaRol.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDarDeBajaRol.Location = new System.Drawing.Point(200, 368);
            this.btnDarDeBajaRol.Name = "btnDarDeBajaRol";
            this.btnDarDeBajaRol.Size = new System.Drawing.Size(110, 28);
            this.btnDarDeBajaRol.TabIndex = 3;
            this.btnDarDeBajaRol.Text = "Dar de &baja";
            this.btnDarDeBajaRol.UseVisualStyleBackColor = true;
            this.btnDarDeBajaRol.Click += new System.EventHandler(this.btnDarDeBajaRol_Click);
            //
            // tabProvincia
            //
            this.tabProvincia.Controls.Add(this.dgvProvincia);
            this.tabProvincia.Controls.Add(this.btnNuevoProvincia);
            this.tabProvincia.Controls.Add(this.btnEditarProvincia);
            this.tabProvincia.Controls.Add(this.btnDarDeBajaProvincia);
            this.tabProvincia.Location = new System.Drawing.Point(4, 24);
            this.tabProvincia.Name = "tabProvincia";
            this.tabProvincia.Padding = new System.Windows.Forms.Padding(3);
            this.tabProvincia.Size = new System.Drawing.Size(616, 413);
            this.tabProvincia.TabIndex = 1;
            this.tabProvincia.Text = "Provincias";
            this.tabProvincia.UseVisualStyleBackColor = true;
            //
            // dgvProvincia
            //
            this.dgvProvincia.AllowUserToAddRows = false;
            this.dgvProvincia.AllowUserToDeleteRows = false;
            this.dgvProvincia.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvProvincia.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProvincia.Location = new System.Drawing.Point(16, 16);
            this.dgvProvincia.MultiSelect = false;
            this.dgvProvincia.Name = "dgvProvincia";
            this.dgvProvincia.ReadOnly = true;
            this.dgvProvincia.RowHeadersVisible = false;
            this.dgvProvincia.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvProvincia.Size = new System.Drawing.Size(584, 340);
            this.dgvProvincia.TabIndex = 0;
            //
            // btnNuevoProvincia
            //
            this.btnNuevoProvincia.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnNuevoProvincia.Location = new System.Drawing.Point(16, 368);
            this.btnNuevoProvincia.Name = "btnNuevoProvincia";
            this.btnNuevoProvincia.Size = new System.Drawing.Size(86, 28);
            this.btnNuevoProvincia.TabIndex = 1;
            this.btnNuevoProvincia.Text = "&Nuevo";
            this.btnNuevoProvincia.UseVisualStyleBackColor = true;
            this.btnNuevoProvincia.Click += new System.EventHandler(this.btnNuevoProvincia_Click);
            //
            // btnEditarProvincia
            //
            this.btnEditarProvincia.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEditarProvincia.Location = new System.Drawing.Point(108, 368);
            this.btnEditarProvincia.Name = "btnEditarProvincia";
            this.btnEditarProvincia.Size = new System.Drawing.Size(86, 28);
            this.btnEditarProvincia.TabIndex = 2;
            this.btnEditarProvincia.Text = "&Editar";
            this.btnEditarProvincia.UseVisualStyleBackColor = true;
            this.btnEditarProvincia.Click += new System.EventHandler(this.btnEditarProvincia_Click);
            //
            // btnDarDeBajaProvincia
            //
            this.btnDarDeBajaProvincia.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDarDeBajaProvincia.Location = new System.Drawing.Point(200, 368);
            this.btnDarDeBajaProvincia.Name = "btnDarDeBajaProvincia";
            this.btnDarDeBajaProvincia.Size = new System.Drawing.Size(110, 28);
            this.btnDarDeBajaProvincia.TabIndex = 3;
            this.btnDarDeBajaProvincia.Text = "Dar de &baja";
            this.btnDarDeBajaProvincia.UseVisualStyleBackColor = true;
            this.btnDarDeBajaProvincia.Click += new System.EventHandler(this.btnDarDeBajaProvincia_Click);
            //
            // tabLocalidad
            //
            this.tabLocalidad.Controls.Add(this.dgvLocalidad);
            this.tabLocalidad.Controls.Add(this.btnNuevoLocalidad);
            this.tabLocalidad.Controls.Add(this.btnEditarLocalidad);
            this.tabLocalidad.Controls.Add(this.btnDarDeBajaLocalidad);
            this.tabLocalidad.Location = new System.Drawing.Point(4, 24);
            this.tabLocalidad.Name = "tabLocalidad";
            this.tabLocalidad.Padding = new System.Windows.Forms.Padding(3);
            this.tabLocalidad.Size = new System.Drawing.Size(616, 413);
            this.tabLocalidad.TabIndex = 2;
            this.tabLocalidad.Text = "Localidades";
            this.tabLocalidad.UseVisualStyleBackColor = true;
            //
            // dgvLocalidad
            //
            this.dgvLocalidad.AllowUserToAddRows = false;
            this.dgvLocalidad.AllowUserToDeleteRows = false;
            this.dgvLocalidad.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvLocalidad.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLocalidad.Location = new System.Drawing.Point(16, 16);
            this.dgvLocalidad.MultiSelect = false;
            this.dgvLocalidad.Name = "dgvLocalidad";
            this.dgvLocalidad.ReadOnly = true;
            this.dgvLocalidad.RowHeadersVisible = false;
            this.dgvLocalidad.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLocalidad.Size = new System.Drawing.Size(584, 340);
            this.dgvLocalidad.TabIndex = 0;
            //
            // btnNuevoLocalidad
            //
            this.btnNuevoLocalidad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnNuevoLocalidad.Location = new System.Drawing.Point(16, 368);
            this.btnNuevoLocalidad.Name = "btnNuevoLocalidad";
            this.btnNuevoLocalidad.Size = new System.Drawing.Size(86, 28);
            this.btnNuevoLocalidad.TabIndex = 1;
            this.btnNuevoLocalidad.Text = "&Nuevo";
            this.btnNuevoLocalidad.UseVisualStyleBackColor = true;
            this.btnNuevoLocalidad.Click += new System.EventHandler(this.btnNuevoLocalidad_Click);
            //
            // btnEditarLocalidad
            //
            this.btnEditarLocalidad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEditarLocalidad.Location = new System.Drawing.Point(108, 368);
            this.btnEditarLocalidad.Name = "btnEditarLocalidad";
            this.btnEditarLocalidad.Size = new System.Drawing.Size(86, 28);
            this.btnEditarLocalidad.TabIndex = 2;
            this.btnEditarLocalidad.Text = "&Editar";
            this.btnEditarLocalidad.UseVisualStyleBackColor = true;
            this.btnEditarLocalidad.Click += new System.EventHandler(this.btnEditarLocalidad_Click);
            //
            // btnDarDeBajaLocalidad
            //
            this.btnDarDeBajaLocalidad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDarDeBajaLocalidad.Location = new System.Drawing.Point(200, 368);
            this.btnDarDeBajaLocalidad.Name = "btnDarDeBajaLocalidad";
            this.btnDarDeBajaLocalidad.Size = new System.Drawing.Size(110, 28);
            this.btnDarDeBajaLocalidad.TabIndex = 3;
            this.btnDarDeBajaLocalidad.Text = "Dar de &baja";
            this.btnDarDeBajaLocalidad.UseVisualStyleBackColor = true;
            this.btnDarDeBajaLocalidad.Click += new System.EventHandler(this.btnDarDeBajaLocalidad_Click);
            //
            // tabTipoDocumento
            //
            this.tabTipoDocumento.Controls.Add(this.dgvTipoDocumento);
            this.tabTipoDocumento.Controls.Add(this.btnNuevoTipoDocumento);
            this.tabTipoDocumento.Controls.Add(this.btnEditarTipoDocumento);
            this.tabTipoDocumento.Controls.Add(this.btnDarDeBajaTipoDocumento);
            this.tabTipoDocumento.Location = new System.Drawing.Point(4, 24);
            this.tabTipoDocumento.Name = "tabTipoDocumento";
            this.tabTipoDocumento.Padding = new System.Windows.Forms.Padding(3);
            this.tabTipoDocumento.Size = new System.Drawing.Size(616, 413);
            this.tabTipoDocumento.TabIndex = 3;
            this.tabTipoDocumento.Text = "Tipos de documento";
            this.tabTipoDocumento.UseVisualStyleBackColor = true;
            //
            // dgvTipoDocumento
            //
            this.dgvTipoDocumento.AllowUserToAddRows = false;
            this.dgvTipoDocumento.AllowUserToDeleteRows = false;
            this.dgvTipoDocumento.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvTipoDocumento.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTipoDocumento.Location = new System.Drawing.Point(16, 16);
            this.dgvTipoDocumento.MultiSelect = false;
            this.dgvTipoDocumento.Name = "dgvTipoDocumento";
            this.dgvTipoDocumento.ReadOnly = true;
            this.dgvTipoDocumento.RowHeadersVisible = false;
            this.dgvTipoDocumento.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTipoDocumento.Size = new System.Drawing.Size(584, 340);
            this.dgvTipoDocumento.TabIndex = 0;
            //
            // btnNuevoTipoDocumento
            //
            this.btnNuevoTipoDocumento.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnNuevoTipoDocumento.Location = new System.Drawing.Point(16, 368);
            this.btnNuevoTipoDocumento.Name = "btnNuevoTipoDocumento";
            this.btnNuevoTipoDocumento.Size = new System.Drawing.Size(86, 28);
            this.btnNuevoTipoDocumento.TabIndex = 1;
            this.btnNuevoTipoDocumento.Text = "&Nuevo";
            this.btnNuevoTipoDocumento.UseVisualStyleBackColor = true;
            this.btnNuevoTipoDocumento.Click += new System.EventHandler(this.btnNuevoTipoDocumento_Click);
            //
            // btnEditarTipoDocumento
            //
            this.btnEditarTipoDocumento.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEditarTipoDocumento.Location = new System.Drawing.Point(108, 368);
            this.btnEditarTipoDocumento.Name = "btnEditarTipoDocumento";
            this.btnEditarTipoDocumento.Size = new System.Drawing.Size(86, 28);
            this.btnEditarTipoDocumento.TabIndex = 2;
            this.btnEditarTipoDocumento.Text = "&Editar";
            this.btnEditarTipoDocumento.UseVisualStyleBackColor = true;
            this.btnEditarTipoDocumento.Click += new System.EventHandler(this.btnEditarTipoDocumento_Click);
            //
            // btnDarDeBajaTipoDocumento
            //
            this.btnDarDeBajaTipoDocumento.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDarDeBajaTipoDocumento.Location = new System.Drawing.Point(200, 368);
            this.btnDarDeBajaTipoDocumento.Name = "btnDarDeBajaTipoDocumento";
            this.btnDarDeBajaTipoDocumento.Size = new System.Drawing.Size(110, 28);
            this.btnDarDeBajaTipoDocumento.TabIndex = 3;
            this.btnDarDeBajaTipoDocumento.Text = "Dar de &baja";
            this.btnDarDeBajaTipoDocumento.UseVisualStyleBackColor = true;
            this.btnDarDeBajaTipoDocumento.Click += new System.EventHandler(this.btnDarDeBajaTipoDocumento_Click);
            //
            // tabMedioPago
            //
            this.tabMedioPago.Controls.Add(this.dgvMedioPago);
            this.tabMedioPago.Controls.Add(this.btnNuevoMedioPago);
            this.tabMedioPago.Controls.Add(this.btnEditarMedioPago);
            this.tabMedioPago.Controls.Add(this.btnDarDeBajaMedioPago);
            this.tabMedioPago.Location = new System.Drawing.Point(4, 24);
            this.tabMedioPago.Name = "tabMedioPago";
            this.tabMedioPago.Padding = new System.Windows.Forms.Padding(3);
            this.tabMedioPago.Size = new System.Drawing.Size(616, 413);
            this.tabMedioPago.TabIndex = 4;
            this.tabMedioPago.Text = "Medios de pago";
            this.tabMedioPago.UseVisualStyleBackColor = true;
            //
            // dgvMedioPago
            //
            this.dgvMedioPago.AllowUserToAddRows = false;
            this.dgvMedioPago.AllowUserToDeleteRows = false;
            this.dgvMedioPago.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvMedioPago.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMedioPago.Location = new System.Drawing.Point(16, 16);
            this.dgvMedioPago.MultiSelect = false;
            this.dgvMedioPago.Name = "dgvMedioPago";
            this.dgvMedioPago.ReadOnly = true;
            this.dgvMedioPago.RowHeadersVisible = false;
            this.dgvMedioPago.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMedioPago.Size = new System.Drawing.Size(584, 340);
            this.dgvMedioPago.TabIndex = 0;
            //
            // btnNuevoMedioPago
            //
            this.btnNuevoMedioPago.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnNuevoMedioPago.Location = new System.Drawing.Point(16, 368);
            this.btnNuevoMedioPago.Name = "btnNuevoMedioPago";
            this.btnNuevoMedioPago.Size = new System.Drawing.Size(86, 28);
            this.btnNuevoMedioPago.TabIndex = 1;
            this.btnNuevoMedioPago.Text = "&Nuevo";
            this.btnNuevoMedioPago.UseVisualStyleBackColor = true;
            this.btnNuevoMedioPago.Click += new System.EventHandler(this.btnNuevoMedioPago_Click);
            //
            // btnEditarMedioPago
            //
            this.btnEditarMedioPago.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEditarMedioPago.Location = new System.Drawing.Point(108, 368);
            this.btnEditarMedioPago.Name = "btnEditarMedioPago";
            this.btnEditarMedioPago.Size = new System.Drawing.Size(86, 28);
            this.btnEditarMedioPago.TabIndex = 2;
            this.btnEditarMedioPago.Text = "&Editar";
            this.btnEditarMedioPago.UseVisualStyleBackColor = true;
            this.btnEditarMedioPago.Click += new System.EventHandler(this.btnEditarMedioPago_Click);
            //
            // btnDarDeBajaMedioPago
            //
            this.btnDarDeBajaMedioPago.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDarDeBajaMedioPago.Location = new System.Drawing.Point(200, 368);
            this.btnDarDeBajaMedioPago.Name = "btnDarDeBajaMedioPago";
            this.btnDarDeBajaMedioPago.Size = new System.Drawing.Size(110, 28);
            this.btnDarDeBajaMedioPago.TabIndex = 3;
            this.btnDarDeBajaMedioPago.Text = "Dar de &baja";
            this.btnDarDeBajaMedioPago.UseVisualStyleBackColor = true;
            this.btnDarDeBajaMedioPago.Click += new System.EventHandler(this.btnDarDeBajaMedioPago_Click);
            //
            // frmTablasParametricas
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 441);
            this.Controls.Add(this.tabCatalogos);
            this.MinimumSize = new System.Drawing.Size(640, 480);
            this.Name = "frmTablasParametricas";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tablas paramétricas";
            this.Load += new System.EventHandler(this.frmTablasParametricas_Load);
            this.tabCatalogos.ResumeLayout(false);
            this.tabRol.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRol)).EndInit();
            this.tabProvincia.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvProvincia)).EndInit();
            this.tabLocalidad.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLocalidad)).EndInit();
            this.tabTipoDocumento.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTipoDocumento)).EndInit();
            this.tabMedioPago.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMedioPago)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TabControl tabCatalogos;
        private System.Windows.Forms.TabPage tabRol;
        private System.Windows.Forms.DataGridView dgvRol;
        private System.Windows.Forms.Button btnNuevoRol;
        private System.Windows.Forms.Button btnEditarRol;
        private System.Windows.Forms.Button btnDarDeBajaRol;
        private System.Windows.Forms.TabPage tabProvincia;
        private System.Windows.Forms.DataGridView dgvProvincia;
        private System.Windows.Forms.Button btnNuevoProvincia;
        private System.Windows.Forms.Button btnEditarProvincia;
        private System.Windows.Forms.Button btnDarDeBajaProvincia;
        private System.Windows.Forms.TabPage tabLocalidad;
        private System.Windows.Forms.DataGridView dgvLocalidad;
        private System.Windows.Forms.Button btnNuevoLocalidad;
        private System.Windows.Forms.Button btnEditarLocalidad;
        private System.Windows.Forms.Button btnDarDeBajaLocalidad;
        private System.Windows.Forms.TabPage tabTipoDocumento;
        private System.Windows.Forms.DataGridView dgvTipoDocumento;
        private System.Windows.Forms.Button btnNuevoTipoDocumento;
        private System.Windows.Forms.Button btnEditarTipoDocumento;
        private System.Windows.Forms.Button btnDarDeBajaTipoDocumento;
        private System.Windows.Forms.TabPage tabMedioPago;
        private System.Windows.Forms.DataGridView dgvMedioPago;
        private System.Windows.Forms.Button btnNuevoMedioPago;
        private System.Windows.Forms.Button btnEditarMedioPago;
        private System.Windows.Forms.Button btnDarDeBajaMedioPago;
    }
}
