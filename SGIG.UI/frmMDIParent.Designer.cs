namespace SGIG.UI
{
    partial class frmMDIParent
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
            this.mnuPrincipal = new System.Windows.Forms.MenuStrip();
            this.mnuSeguridad = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuUsuarios = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTablasParametricas = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPersonas = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSocios = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTesoreria = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPlanes = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPagos = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHistorialPagos = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuControlAcceso = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCheckin = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuActivos = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMaquinas = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMantenimiento = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHistorialMantenimientos = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuGastos = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuGastosAbm = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuReporteBalance = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuBackup = new System.Windows.Forms.ToolStripMenuItem();
            this.stsEstado = new System.Windows.Forms.StatusStrip();
            this.lblUsuarioLogueado = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnCerrarSesion = new System.Windows.Forms.ToolStripStatusLabel();
            this.mnuPrincipal.SuspendLayout();
            this.stsEstado.SuspendLayout();
            this.SuspendLayout();
            // 
            // mnuPrincipal
            // 
            this.mnuPrincipal.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuSeguridad,
            this.mnuPersonas,
            this.mnuTesoreria,
            this.mnuControlAcceso,
            this.mnuActivos,
            this.mnuGastos});
            this.mnuPrincipal.Location = new System.Drawing.Point(0, 0);
            this.mnuPrincipal.Name = "mnuPrincipal";
            this.mnuPrincipal.Size = new System.Drawing.Size(984, 24);
            this.mnuPrincipal.TabIndex = 0;
            // 
            // mnuSeguridad
            // 
            this.mnuSeguridad.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuUsuarios,
            this.mnuTablasParametricas});
            this.mnuSeguridad.Name = "mnuSeguridad";
            this.mnuSeguridad.Text = "&Seguridad";
            // 
            // mnuUsuarios
            // 
            this.mnuUsuarios.Name = "mnuUsuarios";
            this.mnuUsuarios.Text = "&Usuarios";
            // 
            // mnuTablasParametricas
            // 
            this.mnuTablasParametricas.Name = "mnuTablasParametricas";
            this.mnuTablasParametricas.Text = "Tablas &paramétricas";
            // 
            // mnuPersonas
            // 
            this.mnuPersonas.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuSocios});
            this.mnuPersonas.Name = "mnuPersonas";
            this.mnuPersonas.Text = "&Personas";
            // 
            // mnuSocios
            // 
            this.mnuSocios.Name = "mnuSocios";
            this.mnuSocios.Text = "&Socios";
            // 
            // mnuTesoreria
            // 
            this.mnuTesoreria.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuPlanes,
            this.mnuPagos,
            this.mnuHistorialPagos});
            this.mnuTesoreria.Name = "mnuTesoreria";
            this.mnuTesoreria.Text = "&Tesorería";
            // 
            // mnuPlanes
            // 
            this.mnuPlanes.Name = "mnuPlanes";
            this.mnuPlanes.Text = "P&lanes";
            // 
            // mnuPagos
            // 
            this.mnuPagos.Name = "mnuPagos";
            this.mnuPagos.Text = "Registrar &pago";
            // 
            // mnuHistorialPagos
            // 
            this.mnuHistorialPagos.Name = "mnuHistorialPagos";
            this.mnuHistorialPagos.Text = "&Historial de pagos";
            // 
            // mnuControlAcceso
            // 
            this.mnuControlAcceso.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuCheckin});
            this.mnuControlAcceso.Name = "mnuControlAcceso";
            this.mnuControlAcceso.Text = "Control de &acceso";
            // 
            // mnuCheckin
            // 
            this.mnuCheckin.Name = "mnuCheckin";
            this.mnuCheckin.Text = "&Check-in";
            // 
            // mnuActivos
            // 
            this.mnuActivos.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuMaquinas,
            this.mnuMantenimiento,
            this.mnuHistorialMantenimientos});
            this.mnuActivos.Name = "mnuActivos";
            this.mnuActivos.Text = "Ac&tivos";
            // 
            // mnuMaquinas
            // 
            this.mnuMaquinas.Name = "mnuMaquinas";
            this.mnuMaquinas.Text = "&Máquinas";
            // 
            // mnuMantenimiento
            // 
            this.mnuMantenimiento.Name = "mnuMantenimiento";
            this.mnuMantenimiento.Text = "Registrar &mantenimiento";
            // 
            // mnuHistorialMantenimientos
            // 
            this.mnuHistorialMantenimientos.Name = "mnuHistorialMantenimientos";
            this.mnuHistorialMantenimientos.Text = "&Historial de mantenimientos";
            // 
            // mnuGastos
            // 
            this.mnuGastos.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuGastosAbm,
            this.mnuReporteBalance,
            this.mnuBackup});
            this.mnuGastos.Name = "mnuGastos";
            this.mnuGastos.Text = "&Gastos";
            // 
            // mnuGastosAbm
            // 
            this.mnuGastosAbm.Name = "mnuGastosAbm";
            this.mnuGastosAbm.Text = "&Gastos";
            // 
            // mnuReporteBalance
            // 
            this.mnuReporteBalance.Name = "mnuReporteBalance";
            this.mnuReporteBalance.Text = "Reporte de &balance";
            // 
            // mnuBackup
            // 
            this.mnuBackup.Name = "mnuBackup";
            this.mnuBackup.Text = "&Backup";
            // 
            // stsEstado
            // 
            this.stsEstado.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblUsuarioLogueado,
            this.btnCerrarSesion});
            this.stsEstado.Location = new System.Drawing.Point(0, 639);
            this.stsEstado.Name = "stsEstado";
            this.stsEstado.Size = new System.Drawing.Size(984, 22);
            this.stsEstado.TabIndex = 1;
            // 
            // lblUsuarioLogueado
            // 
            this.lblUsuarioLogueado.Name = "lblUsuarioLogueado";
            this.lblUsuarioLogueado.Size = new System.Drawing.Size(0, 17);
            // 
            // btnCerrarSesion
            // 
            this.btnCerrarSesion.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnCerrarSesion.IsLink = true;
            this.btnCerrarSesion.Name = "btnCerrarSesion";
            this.btnCerrarSesion.Size = new System.Drawing.Size(84, 17);
            this.btnCerrarSesion.Text = "Cerrar sesión";
            this.btnCerrarSesion.Click += new System.EventHandler(this.btnCerrarSesion_Click);
            // 
            // frmMDIParent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 661);
            this.Controls.Add(this.stsEstado);
            this.Controls.Add(this.mnuPrincipal);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.mnuPrincipal;
            this.Name = "frmMDIParent";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SGIG — Sistema de Gestión Integral para Gimnasios";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmMDIParent_Load);
            this.mnuPrincipal.ResumeLayout(false);
            this.mnuPrincipal.PerformLayout();
            this.stsEstado.ResumeLayout(false);
            this.stsEstado.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.MenuStrip mnuPrincipal;
        private System.Windows.Forms.ToolStripMenuItem mnuSeguridad;
        private System.Windows.Forms.ToolStripMenuItem mnuUsuarios;
        private System.Windows.Forms.ToolStripMenuItem mnuTablasParametricas;
        private System.Windows.Forms.ToolStripMenuItem mnuPersonas;
        private System.Windows.Forms.ToolStripMenuItem mnuSocios;
        private System.Windows.Forms.ToolStripMenuItem mnuTesoreria;
        private System.Windows.Forms.ToolStripMenuItem mnuPlanes;
        private System.Windows.Forms.ToolStripMenuItem mnuPagos;
        private System.Windows.Forms.ToolStripMenuItem mnuHistorialPagos;
        private System.Windows.Forms.ToolStripMenuItem mnuControlAcceso;
        private System.Windows.Forms.ToolStripMenuItem mnuCheckin;
        private System.Windows.Forms.ToolStripMenuItem mnuActivos;
        private System.Windows.Forms.ToolStripMenuItem mnuMaquinas;
        private System.Windows.Forms.ToolStripMenuItem mnuMantenimiento;
        private System.Windows.Forms.ToolStripMenuItem mnuHistorialMantenimientos;
        private System.Windows.Forms.ToolStripMenuItem mnuGastos;
        private System.Windows.Forms.ToolStripMenuItem mnuGastosAbm;
        private System.Windows.Forms.ToolStripMenuItem mnuReporteBalance;
        private System.Windows.Forms.ToolStripMenuItem mnuBackup;
        private System.Windows.Forms.StatusStrip stsEstado;
        private System.Windows.Forms.ToolStripStatusLabel lblUsuarioLogueado;
        private System.Windows.Forms.ToolStripStatusLabel btnCerrarSesion;
    }
}
