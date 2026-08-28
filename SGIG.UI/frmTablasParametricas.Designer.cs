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
            this.lblRol = new System.Windows.Forms.Label();
            this.txtRol = new System.Windows.Forms.TextBox();
            this.btnAgregarRol = new System.Windows.Forms.Button();
            this.btnEditarRol = new System.Windows.Forms.Button();
            this.btnDarDeBajaRol = new System.Windows.Forms.Button();
            this.btnCancelarRol = new System.Windows.Forms.Button();
            this.tabProvincia = new System.Windows.Forms.TabPage();
            this.dgvProvincia = new System.Windows.Forms.DataGridView();
            this.lblProvincia = new System.Windows.Forms.Label();
            this.txtProvincia = new System.Windows.Forms.TextBox();
            this.btnAgregarProvincia = new System.Windows.Forms.Button();
            this.btnEditarProvincia = new System.Windows.Forms.Button();
            this.btnDarDeBajaProvincia = new System.Windows.Forms.Button();
            this.btnCancelarProvincia = new System.Windows.Forms.Button();
            this.tabLocalidad = new System.Windows.Forms.TabPage();
            this.dgvLocalidad = new System.Windows.Forms.DataGridView();
            this.lblLocalidad = new System.Windows.Forms.Label();
            this.txtLocalidad = new System.Windows.Forms.TextBox();
            this.btnAgregarLocalidad = new System.Windows.Forms.Button();
            this.btnEditarLocalidad = new System.Windows.Forms.Button();
            this.btnDarDeBajaLocalidad = new System.Windows.Forms.Button();
            this.btnCancelarLocalidad = new System.Windows.Forms.Button();
            this.tabTipoDocumento = new System.Windows.Forms.TabPage();
            this.dgvTipoDocumento = new System.Windows.Forms.DataGridView();
            this.lblTipoDocumento = new System.Windows.Forms.Label();
            this.txtTipoDocumento = new System.Windows.Forms.TextBox();
            this.btnAgregarTipoDocumento = new System.Windows.Forms.Button();
            this.btnEditarTipoDocumento = new System.Windows.Forms.Button();
            this.btnDarDeBajaTipoDocumento = new System.Windows.Forms.Button();
            this.btnCancelarTipoDocumento = new System.Windows.Forms.Button();
            this.tabMedioPago = new System.Windows.Forms.TabPage();
            this.dgvMedioPago = new System.Windows.Forms.DataGridView();
            this.lblMedioPago = new System.Windows.Forms.Label();
            this.txtMedioPago = new System.Windows.Forms.TextBox();
            this.btnAgregarMedioPago = new System.Windows.Forms.Button();
            this.btnEditarMedioPago = new System.Windows.Forms.Button();
            this.btnDarDeBajaMedioPago = new System.Windows.Forms.Button();
            this.btnCancelarMedioPago = new System.Windows.Forms.Button();
            this.lblDescripcionRol = new System.Windows.Forms.Label();
            this.txtDescripcionRol = new System.Windows.Forms.TextBox();
            this.lblProvinciaDeLocalidad = new System.Windows.Forms.Label();
            this.cboProvinciaDeLocalidad = new System.Windows.Forms.ComboBox();
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
            this.tabRol.Controls.Add(this.lblRol);
            this.tabRol.Controls.Add(this.txtRol);
            this.tabRol.Controls.Add(this.lblDescripcionRol);
            this.tabRol.Controls.Add(this.txtDescripcionRol);
            this.tabRol.Controls.Add(this.btnAgregarRol);
            this.tabRol.Controls.Add(this.btnEditarRol);
            this.tabRol.Controls.Add(this.btnDarDeBajaRol);
            this.tabRol.Controls.Add(this.btnCancelarRol);
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
            this.dgvRol.Size = new System.Drawing.Size(584, 236);
            this.dgvRol.TabIndex = 0;
            // 
            // lblRol
            // 
            this.lblRol.AutoSize = true;
            this.lblRol.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblRol.Location = new System.Drawing.Point(16, 271);
            this.lblRol.Name = "lblRol";
            this.lblRol.Size = new System.Drawing.Size(52, 15);
            this.lblRol.Text = "Nombre:";
            // 
            // txtRol
            // 
            this.txtRol.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRol.Location = new System.Drawing.Point(104, 268);
            this.txtRol.MaxLength = 50;
            this.txtRol.Name = "txtRol";
            this.txtRol.Size = new System.Drawing.Size(496, 23);
            this.txtRol.TabIndex = 1;
            // 
            // lblDescripcionRol
            // 
            this.lblDescripcionRol.AutoSize = true;
            this.lblDescripcionRol.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblDescripcionRol.Location = new System.Drawing.Point(16, 303);
            this.lblDescripcionRol.Name = "lblDescripcionRol";
            this.lblDescripcionRol.Size = new System.Drawing.Size(76, 15);
            this.lblDescripcionRol.Text = "Descripción:";
            // 
            // txtDescripcionRol
            // 
            this.txtDescripcionRol.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescripcionRol.Location = new System.Drawing.Point(104, 300);
            this.txtDescripcionRol.MaxLength = 200;
            this.txtDescripcionRol.Name = "txtDescripcionRol";
            this.txtDescripcionRol.Size = new System.Drawing.Size(496, 23);
            this.txtDescripcionRol.TabIndex = 2;
            // 
            // btnAgregarRol
            // 
            this.btnAgregarRol.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAgregarRol.Location = new System.Drawing.Point(104, 340);
            this.btnAgregarRol.Name = "btnAgregarRol";
            this.btnAgregarRol.Size = new System.Drawing.Size(86, 28);
            this.btnAgregarRol.TabIndex = 3;
            this.btnAgregarRol.Text = "Agregar";
            this.btnAgregarRol.UseVisualStyleBackColor = true;
            this.btnAgregarRol.Click += new System.EventHandler(this.btnAgregarRol_Click);
            // 
            // btnEditarRol
            // 
            this.btnEditarRol.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEditarRol.Location = new System.Drawing.Point(196, 340);
            this.btnEditarRol.Name = "btnEditarRol";
            this.btnEditarRol.Size = new System.Drawing.Size(86, 28);
            this.btnEditarRol.TabIndex = 4;
            this.btnEditarRol.Text = "Editar";
            this.btnEditarRol.UseVisualStyleBackColor = true;
            this.btnEditarRol.Click += new System.EventHandler(this.btnEditarRol_Click);
            // 
            // btnDarDeBajaRol
            // 
            this.btnDarDeBajaRol.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDarDeBajaRol.Location = new System.Drawing.Point(288, 340);
            this.btnDarDeBajaRol.Name = "btnDarDeBajaRol";
            this.btnDarDeBajaRol.Size = new System.Drawing.Size(96, 28);
            this.btnDarDeBajaRol.TabIndex = 5;
            this.btnDarDeBajaRol.Text = "Dar de baja";
            this.btnDarDeBajaRol.UseVisualStyleBackColor = true;
            this.btnDarDeBajaRol.Click += new System.EventHandler(this.btnDarDeBajaRol_Click);
            // 
            // btnCancelarRol
            // 
            this.btnCancelarRol.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancelarRol.Location = new System.Drawing.Point(390, 340);
            this.btnCancelarRol.Name = "btnCancelarRol";
            this.btnCancelarRol.Size = new System.Drawing.Size(86, 28);
            this.btnCancelarRol.TabIndex = 6;
            this.btnCancelarRol.Text = "Cancelar";
            this.btnCancelarRol.UseVisualStyleBackColor = true;
            this.btnCancelarRol.Click += new System.EventHandler(this.btnCancelarRol_Click);
            // 
            // tabProvincia
            // 
            this.tabProvincia.Controls.Add(this.dgvProvincia);
            this.tabProvincia.Controls.Add(this.lblProvincia);
            this.tabProvincia.Controls.Add(this.txtProvincia);
            this.tabProvincia.Controls.Add(this.btnAgregarProvincia);
            this.tabProvincia.Controls.Add(this.btnEditarProvincia);
            this.tabProvincia.Controls.Add(this.btnDarDeBajaProvincia);
            this.tabProvincia.Controls.Add(this.btnCancelarProvincia);
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
            this.dgvProvincia.Size = new System.Drawing.Size(584, 236);
            this.dgvProvincia.TabIndex = 0;
            // 
            // lblProvincia
            // 
            this.lblProvincia.AutoSize = true;
            this.lblProvincia.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblProvincia.Location = new System.Drawing.Point(16, 271);
            this.lblProvincia.Name = "lblProvincia";
            this.lblProvincia.Size = new System.Drawing.Size(52, 15);
            this.lblProvincia.Text = "Nombre:";
            // 
            // txtProvincia
            // 
            this.txtProvincia.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtProvincia.Location = new System.Drawing.Point(104, 268);
            this.txtProvincia.MaxLength = 100;
            this.txtProvincia.Name = "txtProvincia";
            this.txtProvincia.Size = new System.Drawing.Size(496, 23);
            this.txtProvincia.TabIndex = 1;
            // 
            // btnAgregarProvincia
            // 
            this.btnAgregarProvincia.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAgregarProvincia.Location = new System.Drawing.Point(104, 308);
            this.btnAgregarProvincia.Name = "btnAgregarProvincia";
            this.btnAgregarProvincia.Size = new System.Drawing.Size(86, 28);
            this.btnAgregarProvincia.TabIndex = 3;
            this.btnAgregarProvincia.Text = "Agregar";
            this.btnAgregarProvincia.UseVisualStyleBackColor = true;
            this.btnAgregarProvincia.Click += new System.EventHandler(this.btnAgregarProvincia_Click);
            // 
            // btnEditarProvincia
            // 
            this.btnEditarProvincia.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEditarProvincia.Location = new System.Drawing.Point(196, 308);
            this.btnEditarProvincia.Name = "btnEditarProvincia";
            this.btnEditarProvincia.Size = new System.Drawing.Size(86, 28);
            this.btnEditarProvincia.TabIndex = 4;
            this.btnEditarProvincia.Text = "Editar";
            this.btnEditarProvincia.UseVisualStyleBackColor = true;
            this.btnEditarProvincia.Click += new System.EventHandler(this.btnEditarProvincia_Click);
            // 
            // btnDarDeBajaProvincia
            // 
            this.btnDarDeBajaProvincia.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDarDeBajaProvincia.Location = new System.Drawing.Point(288, 308);
            this.btnDarDeBajaProvincia.Name = "btnDarDeBajaProvincia";
            this.btnDarDeBajaProvincia.Size = new System.Drawing.Size(96, 28);
            this.btnDarDeBajaProvincia.TabIndex = 5;
            this.btnDarDeBajaProvincia.Text = "Dar de baja";
            this.btnDarDeBajaProvincia.UseVisualStyleBackColor = true;
            this.btnDarDeBajaProvincia.Click += new System.EventHandler(this.btnDarDeBajaProvincia_Click);
            // 
            // btnCancelarProvincia
            // 
            this.btnCancelarProvincia.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancelarProvincia.Location = new System.Drawing.Point(390, 308);
            this.btnCancelarProvincia.Name = "btnCancelarProvincia";
            this.btnCancelarProvincia.Size = new System.Drawing.Size(86, 28);
            this.btnCancelarProvincia.TabIndex = 6;
            this.btnCancelarProvincia.Text = "Cancelar";
            this.btnCancelarProvincia.UseVisualStyleBackColor = true;
            this.btnCancelarProvincia.Click += new System.EventHandler(this.btnCancelarProvincia_Click);
            // 
            // tabLocalidad
            // 
            this.tabLocalidad.Controls.Add(this.dgvLocalidad);
            this.tabLocalidad.Controls.Add(this.lblLocalidad);
            this.tabLocalidad.Controls.Add(this.txtLocalidad);
            this.tabLocalidad.Controls.Add(this.lblProvinciaDeLocalidad);
            this.tabLocalidad.Controls.Add(this.cboProvinciaDeLocalidad);
            this.tabLocalidad.Controls.Add(this.btnAgregarLocalidad);
            this.tabLocalidad.Controls.Add(this.btnEditarLocalidad);
            this.tabLocalidad.Controls.Add(this.btnDarDeBajaLocalidad);
            this.tabLocalidad.Controls.Add(this.btnCancelarLocalidad);
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
            this.dgvLocalidad.Size = new System.Drawing.Size(584, 236);
            this.dgvLocalidad.TabIndex = 0;
            // 
            // lblLocalidad
            // 
            this.lblLocalidad.AutoSize = true;
            this.lblLocalidad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblLocalidad.Location = new System.Drawing.Point(16, 271);
            this.lblLocalidad.Name = "lblLocalidad";
            this.lblLocalidad.Size = new System.Drawing.Size(52, 15);
            this.lblLocalidad.Text = "Nombre:";
            // 
            // txtLocalidad
            // 
            this.txtLocalidad.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLocalidad.Location = new System.Drawing.Point(104, 268);
            this.txtLocalidad.MaxLength = 100;
            this.txtLocalidad.Name = "txtLocalidad";
            this.txtLocalidad.Size = new System.Drawing.Size(496, 23);
            this.txtLocalidad.TabIndex = 1;
            // 
            // lblProvinciaDeLocalidad
            // 
            this.lblProvinciaDeLocalidad.AutoSize = true;
            this.lblProvinciaDeLocalidad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblProvinciaDeLocalidad.Location = new System.Drawing.Point(16, 303);
            this.lblProvinciaDeLocalidad.Name = "lblProvinciaDeLocalidad";
            this.lblProvinciaDeLocalidad.Size = new System.Drawing.Size(62, 15);
            this.lblProvinciaDeLocalidad.Text = "Provincia:";
            // 
            // cboProvinciaDeLocalidad
            // 
            this.cboProvinciaDeLocalidad.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboProvinciaDeLocalidad.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProvinciaDeLocalidad.Location = new System.Drawing.Point(104, 300);
            this.cboProvinciaDeLocalidad.Name = "cboProvinciaDeLocalidad";
            this.cboProvinciaDeLocalidad.Size = new System.Drawing.Size(496, 23);
            this.cboProvinciaDeLocalidad.TabIndex = 2;
            // 
            // btnAgregarLocalidad
            // 
            this.btnAgregarLocalidad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAgregarLocalidad.Location = new System.Drawing.Point(104, 340);
            this.btnAgregarLocalidad.Name = "btnAgregarLocalidad";
            this.btnAgregarLocalidad.Size = new System.Drawing.Size(86, 28);
            this.btnAgregarLocalidad.TabIndex = 3;
            this.btnAgregarLocalidad.Text = "Agregar";
            this.btnAgregarLocalidad.UseVisualStyleBackColor = true;
            this.btnAgregarLocalidad.Click += new System.EventHandler(this.btnAgregarLocalidad_Click);
            // 
            // btnEditarLocalidad
            // 
            this.btnEditarLocalidad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEditarLocalidad.Location = new System.Drawing.Point(196, 340);
            this.btnEditarLocalidad.Name = "btnEditarLocalidad";
            this.btnEditarLocalidad.Size = new System.Drawing.Size(86, 28);
            this.btnEditarLocalidad.TabIndex = 4;
            this.btnEditarLocalidad.Text = "Editar";
            this.btnEditarLocalidad.UseVisualStyleBackColor = true;
            this.btnEditarLocalidad.Click += new System.EventHandler(this.btnEditarLocalidad_Click);
            // 
            // btnDarDeBajaLocalidad
            // 
            this.btnDarDeBajaLocalidad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDarDeBajaLocalidad.Location = new System.Drawing.Point(288, 340);
            this.btnDarDeBajaLocalidad.Name = "btnDarDeBajaLocalidad";
            this.btnDarDeBajaLocalidad.Size = new System.Drawing.Size(96, 28);
            this.btnDarDeBajaLocalidad.TabIndex = 5;
            this.btnDarDeBajaLocalidad.Text = "Dar de baja";
            this.btnDarDeBajaLocalidad.UseVisualStyleBackColor = true;
            this.btnDarDeBajaLocalidad.Click += new System.EventHandler(this.btnDarDeBajaLocalidad_Click);
            // 
            // btnCancelarLocalidad
            // 
            this.btnCancelarLocalidad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancelarLocalidad.Location = new System.Drawing.Point(390, 340);
            this.btnCancelarLocalidad.Name = "btnCancelarLocalidad";
            this.btnCancelarLocalidad.Size = new System.Drawing.Size(86, 28);
            this.btnCancelarLocalidad.TabIndex = 6;
            this.btnCancelarLocalidad.Text = "Cancelar";
            this.btnCancelarLocalidad.UseVisualStyleBackColor = true;
            this.btnCancelarLocalidad.Click += new System.EventHandler(this.btnCancelarLocalidad_Click);
            // 
            // tabTipoDocumento
            // 
            this.tabTipoDocumento.Controls.Add(this.dgvTipoDocumento);
            this.tabTipoDocumento.Controls.Add(this.lblTipoDocumento);
            this.tabTipoDocumento.Controls.Add(this.txtTipoDocumento);
            this.tabTipoDocumento.Controls.Add(this.btnAgregarTipoDocumento);
            this.tabTipoDocumento.Controls.Add(this.btnEditarTipoDocumento);
            this.tabTipoDocumento.Controls.Add(this.btnDarDeBajaTipoDocumento);
            this.tabTipoDocumento.Controls.Add(this.btnCancelarTipoDocumento);
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
            this.dgvTipoDocumento.Size = new System.Drawing.Size(584, 236);
            this.dgvTipoDocumento.TabIndex = 0;
            // 
            // lblTipoDocumento
            // 
            this.lblTipoDocumento.AutoSize = true;
            this.lblTipoDocumento.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTipoDocumento.Location = new System.Drawing.Point(16, 271);
            this.lblTipoDocumento.Name = "lblTipoDocumento";
            this.lblTipoDocumento.Size = new System.Drawing.Size(76, 15);
            this.lblTipoDocumento.Text = "Descripción:";
            // 
            // txtTipoDocumento
            // 
            this.txtTipoDocumento.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTipoDocumento.Location = new System.Drawing.Point(104, 268);
            this.txtTipoDocumento.MaxLength = 100;
            this.txtTipoDocumento.Name = "txtTipoDocumento";
            this.txtTipoDocumento.Size = new System.Drawing.Size(496, 23);
            this.txtTipoDocumento.TabIndex = 1;
            // 
            // btnAgregarTipoDocumento
            // 
            this.btnAgregarTipoDocumento.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAgregarTipoDocumento.Location = new System.Drawing.Point(104, 308);
            this.btnAgregarTipoDocumento.Name = "btnAgregarTipoDocumento";
            this.btnAgregarTipoDocumento.Size = new System.Drawing.Size(86, 28);
            this.btnAgregarTipoDocumento.TabIndex = 3;
            this.btnAgregarTipoDocumento.Text = "Agregar";
            this.btnAgregarTipoDocumento.UseVisualStyleBackColor = true;
            this.btnAgregarTipoDocumento.Click += new System.EventHandler(this.btnAgregarTipoDocumento_Click);
            // 
            // btnEditarTipoDocumento
            // 
            this.btnEditarTipoDocumento.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEditarTipoDocumento.Location = new System.Drawing.Point(196, 308);
            this.btnEditarTipoDocumento.Name = "btnEditarTipoDocumento";
            this.btnEditarTipoDocumento.Size = new System.Drawing.Size(86, 28);
            this.btnEditarTipoDocumento.TabIndex = 4;
            this.btnEditarTipoDocumento.Text = "Editar";
            this.btnEditarTipoDocumento.UseVisualStyleBackColor = true;
            this.btnEditarTipoDocumento.Click += new System.EventHandler(this.btnEditarTipoDocumento_Click);
            // 
            // btnDarDeBajaTipoDocumento
            // 
            this.btnDarDeBajaTipoDocumento.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDarDeBajaTipoDocumento.Location = new System.Drawing.Point(288, 308);
            this.btnDarDeBajaTipoDocumento.Name = "btnDarDeBajaTipoDocumento";
            this.btnDarDeBajaTipoDocumento.Size = new System.Drawing.Size(96, 28);
            this.btnDarDeBajaTipoDocumento.TabIndex = 5;
            this.btnDarDeBajaTipoDocumento.Text = "Dar de baja";
            this.btnDarDeBajaTipoDocumento.UseVisualStyleBackColor = true;
            this.btnDarDeBajaTipoDocumento.Click += new System.EventHandler(this.btnDarDeBajaTipoDocumento_Click);
            // 
            // btnCancelarTipoDocumento
            // 
            this.btnCancelarTipoDocumento.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancelarTipoDocumento.Location = new System.Drawing.Point(390, 308);
            this.btnCancelarTipoDocumento.Name = "btnCancelarTipoDocumento";
            this.btnCancelarTipoDocumento.Size = new System.Drawing.Size(86, 28);
            this.btnCancelarTipoDocumento.TabIndex = 6;
            this.btnCancelarTipoDocumento.Text = "Cancelar";
            this.btnCancelarTipoDocumento.UseVisualStyleBackColor = true;
            this.btnCancelarTipoDocumento.Click += new System.EventHandler(this.btnCancelarTipoDocumento_Click);
            // 
            // tabMedioPago
            // 
            this.tabMedioPago.Controls.Add(this.dgvMedioPago);
            this.tabMedioPago.Controls.Add(this.lblMedioPago);
            this.tabMedioPago.Controls.Add(this.txtMedioPago);
            this.tabMedioPago.Controls.Add(this.btnAgregarMedioPago);
            this.tabMedioPago.Controls.Add(this.btnEditarMedioPago);
            this.tabMedioPago.Controls.Add(this.btnDarDeBajaMedioPago);
            this.tabMedioPago.Controls.Add(this.btnCancelarMedioPago);
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
            this.dgvMedioPago.Size = new System.Drawing.Size(584, 236);
            this.dgvMedioPago.TabIndex = 0;
            // 
            // lblMedioPago
            // 
            this.lblMedioPago.AutoSize = true;
            this.lblMedioPago.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblMedioPago.Location = new System.Drawing.Point(16, 271);
            this.lblMedioPago.Name = "lblMedioPago";
            this.lblMedioPago.Size = new System.Drawing.Size(76, 15);
            this.lblMedioPago.Text = "Descripción:";
            // 
            // txtMedioPago
            // 
            this.txtMedioPago.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMedioPago.Location = new System.Drawing.Point(104, 268);
            this.txtMedioPago.MaxLength = 100;
            this.txtMedioPago.Name = "txtMedioPago";
            this.txtMedioPago.Size = new System.Drawing.Size(496, 23);
            this.txtMedioPago.TabIndex = 1;
            // 
            // btnAgregarMedioPago
            // 
            this.btnAgregarMedioPago.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAgregarMedioPago.Location = new System.Drawing.Point(104, 308);
            this.btnAgregarMedioPago.Name = "btnAgregarMedioPago";
            this.btnAgregarMedioPago.Size = new System.Drawing.Size(86, 28);
            this.btnAgregarMedioPago.TabIndex = 3;
            this.btnAgregarMedioPago.Text = "Agregar";
            this.btnAgregarMedioPago.UseVisualStyleBackColor = true;
            this.btnAgregarMedioPago.Click += new System.EventHandler(this.btnAgregarMedioPago_Click);
            // 
            // btnEditarMedioPago
            // 
            this.btnEditarMedioPago.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEditarMedioPago.Location = new System.Drawing.Point(196, 308);
            this.btnEditarMedioPago.Name = "btnEditarMedioPago";
            this.btnEditarMedioPago.Size = new System.Drawing.Size(86, 28);
            this.btnEditarMedioPago.TabIndex = 4;
            this.btnEditarMedioPago.Text = "Editar";
            this.btnEditarMedioPago.UseVisualStyleBackColor = true;
            this.btnEditarMedioPago.Click += new System.EventHandler(this.btnEditarMedioPago_Click);
            // 
            // btnDarDeBajaMedioPago
            // 
            this.btnDarDeBajaMedioPago.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDarDeBajaMedioPago.Location = new System.Drawing.Point(288, 308);
            this.btnDarDeBajaMedioPago.Name = "btnDarDeBajaMedioPago";
            this.btnDarDeBajaMedioPago.Size = new System.Drawing.Size(96, 28);
            this.btnDarDeBajaMedioPago.TabIndex = 5;
            this.btnDarDeBajaMedioPago.Text = "Dar de baja";
            this.btnDarDeBajaMedioPago.UseVisualStyleBackColor = true;
            this.btnDarDeBajaMedioPago.Click += new System.EventHandler(this.btnDarDeBajaMedioPago_Click);
            // 
            // btnCancelarMedioPago
            // 
            this.btnCancelarMedioPago.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancelarMedioPago.Location = new System.Drawing.Point(390, 308);
            this.btnCancelarMedioPago.Name = "btnCancelarMedioPago";
            this.btnCancelarMedioPago.Size = new System.Drawing.Size(86, 28);
            this.btnCancelarMedioPago.TabIndex = 6;
            this.btnCancelarMedioPago.Text = "Cancelar";
            this.btnCancelarMedioPago.UseVisualStyleBackColor = true;
            this.btnCancelarMedioPago.Click += new System.EventHandler(this.btnCancelarMedioPago_Click);
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
            this.tabRol.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRol)).EndInit();
            this.tabProvincia.ResumeLayout(false);
            this.tabProvincia.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProvincia)).EndInit();
            this.tabLocalidad.ResumeLayout(false);
            this.tabLocalidad.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLocalidad)).EndInit();
            this.tabTipoDocumento.ResumeLayout(false);
            this.tabTipoDocumento.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTipoDocumento)).EndInit();
            this.tabMedioPago.ResumeLayout(false);
            this.tabMedioPago.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMedioPago)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TabControl tabCatalogos;
        private System.Windows.Forms.TabPage tabRol;
        private System.Windows.Forms.DataGridView dgvRol;
        private System.Windows.Forms.Label lblRol;
        private System.Windows.Forms.TextBox txtRol;
        private System.Windows.Forms.Button btnAgregarRol;
        private System.Windows.Forms.Button btnEditarRol;
        private System.Windows.Forms.Button btnDarDeBajaRol;
        private System.Windows.Forms.Button btnCancelarRol;
        private System.Windows.Forms.TabPage tabProvincia;
        private System.Windows.Forms.DataGridView dgvProvincia;
        private System.Windows.Forms.Label lblProvincia;
        private System.Windows.Forms.TextBox txtProvincia;
        private System.Windows.Forms.Button btnAgregarProvincia;
        private System.Windows.Forms.Button btnEditarProvincia;
        private System.Windows.Forms.Button btnDarDeBajaProvincia;
        private System.Windows.Forms.Button btnCancelarProvincia;
        private System.Windows.Forms.TabPage tabLocalidad;
        private System.Windows.Forms.DataGridView dgvLocalidad;
        private System.Windows.Forms.Label lblLocalidad;
        private System.Windows.Forms.TextBox txtLocalidad;
        private System.Windows.Forms.Button btnAgregarLocalidad;
        private System.Windows.Forms.Button btnEditarLocalidad;
        private System.Windows.Forms.Button btnDarDeBajaLocalidad;
        private System.Windows.Forms.Button btnCancelarLocalidad;
        private System.Windows.Forms.TabPage tabTipoDocumento;
        private System.Windows.Forms.DataGridView dgvTipoDocumento;
        private System.Windows.Forms.Label lblTipoDocumento;
        private System.Windows.Forms.TextBox txtTipoDocumento;
        private System.Windows.Forms.Button btnAgregarTipoDocumento;
        private System.Windows.Forms.Button btnEditarTipoDocumento;
        private System.Windows.Forms.Button btnDarDeBajaTipoDocumento;
        private System.Windows.Forms.Button btnCancelarTipoDocumento;
        private System.Windows.Forms.TabPage tabMedioPago;
        private System.Windows.Forms.DataGridView dgvMedioPago;
        private System.Windows.Forms.Label lblMedioPago;
        private System.Windows.Forms.TextBox txtMedioPago;
        private System.Windows.Forms.Button btnAgregarMedioPago;
        private System.Windows.Forms.Button btnEditarMedioPago;
        private System.Windows.Forms.Button btnDarDeBajaMedioPago;
        private System.Windows.Forms.Button btnCancelarMedioPago;
        private System.Windows.Forms.Label lblDescripcionRol;
        private System.Windows.Forms.TextBox txtDescripcionRol;
        private System.Windows.Forms.Label lblProvinciaDeLocalidad;
        private System.Windows.Forms.ComboBox cboProvinciaDeLocalidad;
    }
}
