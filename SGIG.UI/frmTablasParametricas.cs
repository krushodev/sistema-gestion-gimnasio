using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.UI
{
    /// <summary>
    /// Listado de las tablas paramétricas del sistema (RF#04): Rol, Provincia,
    /// Localidad, TipoDocumento y MedioPago. Sólo accesible para el rol
    /// Administrador. El alta y la edición de cada catálogo se hacen en su propio
    /// diálogo modal (frmRolEditor, frmProvinciaEditor, frmLocalidadEditor,
    /// frmTipoDocumentoEditor, frmMedioPagoEditor); esta pantalla sólo lista.
    /// La baja de todos estos catálogos es lógica, nunca física.
    /// </summary>
    //
    // ── CONTROLES (ver frmTablasParametricas.Designer.cs) ────────────────────
    //   tabCatalogos (TabControl) con tabRol, tabProvincia, tabLocalidad,
    //   tabTipoDocumento y tabMedioPago. Cada pestaña tiene sólo su dgv y sus
    //   botones btnNuevo / btnEditar / btnDarDeBaja.
    // ─────────────────────────────────────────────────────────────────────────
    public partial class frmTablasParametricas : Form
    {
        private readonly ServicioCatalogo _servicio = new();

        public frmTablasParametricas()
        {
            InitializeComponent();
        }

        private void frmTablasParametricas_Load(object sender, EventArgs e)
        {
            try
            {
                ConfigurarGrillas();
                CargarRoles();
                CargarProvincias();
                CargarLocalidades();
                CargarTiposDocumento();
                CargarMediosPago();
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        /// <summary>
        /// Columnas explícitas en las cinco grillas, para no mostrar los ids internos
        /// ni la bandera 'activo' (que siempre vale 1, porque las consultas ya filtran).
        /// </summary>
        private void ConfigurarGrillas()
        {
            Grillas.Configurar(dgvRol,
                (nameof(Rol.NombreRol), "Rol", 100),
                (nameof(Rol.Descripcion), "Descripción", 200));

            Grillas.Configurar(dgvProvincia,
                (nameof(Provincia.Nombre), "Provincia", 100));

            Grillas.Configurar(dgvLocalidad,
                (nameof(Localidad.Nombre), "Localidad", 100),
                (nameof(Localidad.NombreProvincia), "Provincia", 100));

            Grillas.Configurar(dgvTipoDocumento,
                (nameof(TipoDocumento.Descripcion), "Tipo de documento", 100));

            Grillas.Configurar(dgvMedioPago,
                (nameof(MedioPago.Descripcion), "Medio de pago", 100));
        }

        // ── Rol ──────────────────────────────────────────────────────────────

        private void CargarRoles() => dgvRol.DataSource = _servicio.ObtenerRoles().ToList();

        private void btnNuevoRol_Click(object sender, EventArgs e)
        {
            using var editor = new frmRolEditor(null);
            if (editor.ShowDialog(this) == DialogResult.OK) CargarRoles();
        }

        private void btnEditarRol_Click(object sender, EventArgs e)
        {
            if (dgvRol.CurrentRow?.DataBoundItem is not Rol rol)
            {
                AvisarSeleccion();
                return;
            }

            using var editor = new frmRolEditor(rol);
            if (editor.ShowDialog(this) == DialogResult.OK) CargarRoles();
        }

        private void btnDarDeBajaRol_Click(object sender, EventArgs e)
        {
            if (dgvRol.CurrentRow?.DataBoundItem is not Rol rol)
            {
                AvisarSeleccion();
                return;
            }

            if (!Confirmar("el rol", rol.NombreRol)) return;

            try
            {
                _servicio.BajaLogicaRol(rol.IdRol);
                CargarRoles();
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        // ── Provincia ────────────────────────────────────────────────────────

        private void CargarProvincias() => dgvProvincia.DataSource = _servicio.ObtenerProvincias().ToList();

        private void btnNuevoProvincia_Click(object sender, EventArgs e)
        {
            using var editor = new frmProvinciaEditor(null);
            if (editor.ShowDialog(this) == DialogResult.OK)
            {
                CargarProvincias();
                CargarLocalidades();
            }
        }

        private void btnEditarProvincia_Click(object sender, EventArgs e)
        {
            if (dgvProvincia.CurrentRow?.DataBoundItem is not Provincia provincia)
            {
                AvisarSeleccion();
                return;
            }

            using var editor = new frmProvinciaEditor(provincia);
            if (editor.ShowDialog(this) == DialogResult.OK)
            {
                CargarProvincias();
                CargarLocalidades();
            }
        }

        private void btnDarDeBajaProvincia_Click(object sender, EventArgs e)
        {
            if (dgvProvincia.CurrentRow?.DataBoundItem is not Provincia item)
            {
                AvisarSeleccion();
                return;
            }

            if (!Confirmar("la provincia", item.Nombre)) return;

            try
            {
                _servicio.BajaLogicaProvincia(item.IdProvincia);
                CargarProvincias();
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        // ── Localidad ────────────────────────────────────────────────────────

        private void CargarLocalidades() => dgvLocalidad.DataSource = _servicio.ObtenerLocalidades().ToList();

        private void btnNuevoLocalidad_Click(object sender, EventArgs e)
        {
            using var editor = new frmLocalidadEditor(null);
            if (editor.ShowDialog(this) == DialogResult.OK) CargarLocalidades();
        }

        private void btnEditarLocalidad_Click(object sender, EventArgs e)
        {
            if (dgvLocalidad.CurrentRow?.DataBoundItem is not Localidad localidad)
            {
                AvisarSeleccion();
                return;
            }

            using var editor = new frmLocalidadEditor(localidad);
            if (editor.ShowDialog(this) == DialogResult.OK) CargarLocalidades();
        }

        private void btnDarDeBajaLocalidad_Click(object sender, EventArgs e)
        {
            if (dgvLocalidad.CurrentRow?.DataBoundItem is not Localidad item)
            {
                AvisarSeleccion();
                return;
            }

            if (!Confirmar("la localidad", item.Nombre)) return;

            try
            {
                _servicio.BajaLogicaLocalidad(item.IdLocalidad);
                CargarLocalidades();
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        // ── TipoDocumento ────────────────────────────────────────────────────

        private void CargarTiposDocumento() => dgvTipoDocumento.DataSource = _servicio.ObtenerTiposDocumento().ToList();

        private void btnNuevoTipoDocumento_Click(object sender, EventArgs e)
        {
            using var editor = new frmTipoDocumentoEditor(null);
            if (editor.ShowDialog(this) == DialogResult.OK) CargarTiposDocumento();
        }

        private void btnEditarTipoDocumento_Click(object sender, EventArgs e)
        {
            if (dgvTipoDocumento.CurrentRow?.DataBoundItem is not TipoDocumento tipo)
            {
                AvisarSeleccion();
                return;
            }

            using var editor = new frmTipoDocumentoEditor(tipo);
            if (editor.ShowDialog(this) == DialogResult.OK) CargarTiposDocumento();
        }

        private void btnDarDeBajaTipoDocumento_Click(object sender, EventArgs e)
        {
            if (dgvTipoDocumento.CurrentRow?.DataBoundItem is not TipoDocumento item)
            {
                AvisarSeleccion();
                return;
            }

            if (!Confirmar("el tipo de documento", item.Descripcion)) return;

            try
            {
                _servicio.BajaLogicaTipoDocumento(item.IdTipoDocumento);
                CargarTiposDocumento();
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        // ── MedioPago ────────────────────────────────────────────────────────

        private void CargarMediosPago() => dgvMedioPago.DataSource = _servicio.ObtenerMediosPago().ToList();

        private void btnNuevoMedioPago_Click(object sender, EventArgs e)
        {
            using var editor = new frmMedioPagoEditor(null);
            if (editor.ShowDialog(this) == DialogResult.OK) CargarMediosPago();
        }

        private void btnEditarMedioPago_Click(object sender, EventArgs e)
        {
            if (dgvMedioPago.CurrentRow?.DataBoundItem is not MedioPago medio)
            {
                AvisarSeleccion();
                return;
            }

            using var editor = new frmMedioPagoEditor(medio);
            if (editor.ShowDialog(this) == DialogResult.OK) CargarMediosPago();
        }

        private void btnDarDeBajaMedioPago_Click(object sender, EventArgs e)
        {
            if (dgvMedioPago.CurrentRow?.DataBoundItem is not MedioPago item)
            {
                AvisarSeleccion();
                return;
            }

            if (!Confirmar("el medio de pago", item.Descripcion)) return;

            try
            {
                _servicio.BajaLogicaMedioPago(item.IdMedioPago);
                CargarMediosPago();
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        /// <summary>
        /// Confirmación obligatoria antes de cualquier baja (RNF#03). La baja es
        /// lógica: el registro se marca inactivo, la fila no se borra (RF#04).
        /// </summary>
        private static bool Confirmar(string etiqueta, string descripcion)
        {
            var mensaje = $"¿Confirmás dar de baja {etiqueta} \"{descripcion}\"?"
                + Environment.NewLine + Environment.NewLine
                + "Dejará de aparecer en las grillas y en los combos, pero los registros "
                + "que ya la usan no se ven afectados.";

            return MessageBox.Show(mensaje, "Confirmar baja",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        private static void AvisarSeleccion()
        {
            MessageBox.Show("Seleccioná una fila de la grilla.", "SGIG",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static void MostrarError(Exception ex)
        {
            MessageBox.Show(ex.Message, "SGIG", MessageBoxButtons.OK,
                ex is NegocioException ? MessageBoxIcon.Warning : MessageBoxIcon.Error);
        }
    }
}
