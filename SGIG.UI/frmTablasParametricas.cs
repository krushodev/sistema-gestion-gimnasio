using SGIG.Datos;
using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.UI
{
    /// <summary>
    /// ABM de las tablas paramétricas del sistema (RF#04): Rol, Provincia,
    /// Localidad, TipoDocumento y MedioPago. Sólo accesible para el rol
    /// Administrador. La baja de todos estos catálogos es lógica, nunca física.
    /// </summary>
    //
    // ── CONTROLES (ver frmTablasParametricas.Designer.cs) ────────────────────
    //   tabCatalogos (TabControl) con tabRol, tabProvincia, tabLocalidad,
    //   tabTipoDocumento y tabMedioPago. Cada pestaña tiene su dgv, sus campos
    //   y btnAgregar / btnEditar / btnDarDeBaja / btnCancelar.
    // ─────────────────────────────────────────────────────────────────────────
    public partial class frmTablasParametricas : Form
    {
        private readonly ServicioCatalogo _servicio = new();

        /// <summary>Id en edición por pestaña; 0 significa "alta nueva".</summary>
        private int _idRol;
        private int _idProvincia;
        private int _idLocalidad;
        private int _idTipoDocumento;
        private int _idMedioPago;

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

        private void CargarRoles()
        {
            dgvRol.DataSource = _servicio.ObtenerRoles().ToList();
            LimpiarRol();
        }

        private void btnAgregarRol_Click(object sender, EventArgs e)
        {
            try
            {
                var rol = new Rol
                {
                    IdRol = _idRol,
                    NombreRol = txtRol.Text.Trim(),
                    Descripcion = string.IsNullOrWhiteSpace(txtDescripcionRol.Text)
                        ? null
                        : txtDescripcionRol.Text.Trim()
                };

                if (_idRol == 0)
                {
                    _servicio.AltaRol(rol);
                }
                else
                {
                    _servicio.ModificarRol(rol);
                }

                CargarRoles();
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        private void btnEditarRol_Click(object sender, EventArgs e)
        {
            if (dgvRol.CurrentRow?.DataBoundItem is not Rol rol)
            {
                AvisarSeleccion();
                return;
            }

            _idRol = rol.IdRol;
            txtRol.Text = rol.NombreRol;
            txtDescripcionRol.Text = rol.Descripcion ?? string.Empty;
            btnAgregarRol.Text = "Guardar";
            txtRol.Focus();
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

        private void btnCancelarRol_Click(object sender, EventArgs e) => LimpiarRol();

        private void LimpiarRol()
        {
            _idRol = 0;
            txtRol.Clear();
            txtDescripcionRol.Clear();
            btnAgregarRol.Text = "Agregar";
        }

        // ── Provincia ────────────────────────────────────────────────────────

        private void CargarProvincias()
        {
            var provincias = _servicio.ObtenerProvincias().ToList();
            dgvProvincia.DataSource = provincias;

            // El combo de la pestaña Localidad depende de esta misma lista.
            cboProvinciaDeLocalidad.DisplayMember = nameof(Provincia.Nombre);
            cboProvinciaDeLocalidad.ValueMember = nameof(Provincia.IdProvincia);
            cboProvinciaDeLocalidad.DataSource = provincias.ToList();

            LimpiarProvincia();
        }

        private void btnAgregarProvincia_Click(object sender, EventArgs e)
        {
            try
            {
                var provincia = new Provincia { IdProvincia = _idProvincia, Nombre = txtProvincia.Text.Trim() };

                if (_idProvincia == 0)
                {
                    _servicio.AltaProvincia(provincia);
                }
                else
                {
                    _servicio.ModificarProvincia(provincia);
                }

                CargarProvincias();
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        private void btnEditarProvincia_Click(object sender, EventArgs e)
        {
            if (dgvProvincia.CurrentRow?.DataBoundItem is not Provincia provincia)
            {
                AvisarSeleccion();
                return;
            }

            _idProvincia = provincia.IdProvincia;
            txtProvincia.Text = provincia.Nombre;
            btnAgregarProvincia.Text = "Guardar";
            txtProvincia.Focus();
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

        private void btnCancelarProvincia_Click(object sender, EventArgs e) => LimpiarProvincia();

        private void LimpiarProvincia()
        {
            _idProvincia = 0;
            txtProvincia.Clear();
            btnAgregarProvincia.Text = "Agregar";
        }

        // ── Localidad ────────────────────────────────────────────────────────

        private void CargarLocalidades()
        {
            dgvLocalidad.DataSource = _servicio.ObtenerLocalidades().ToList();
            LimpiarLocalidad();
        }

        private void btnAgregarLocalidad_Click(object sender, EventArgs e)
        {
            try
            {
                var localidad = new Localidad
                {
                    IdLocalidad = _idLocalidad,
                    Nombre = txtLocalidad.Text.Trim(),
                    IdProvincia = (int)(cboProvinciaDeLocalidad.SelectedValue ?? 0)
                };

                if (_idLocalidad == 0)
                {
                    _servicio.AltaLocalidad(localidad);
                }
                else
                {
                    _servicio.ModificarLocalidad(localidad);
                }

                CargarLocalidades();
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        private void btnEditarLocalidad_Click(object sender, EventArgs e)
        {
            if (dgvLocalidad.CurrentRow?.DataBoundItem is not Localidad localidad)
            {
                AvisarSeleccion();
                return;
            }

            _idLocalidad = localidad.IdLocalidad;
            txtLocalidad.Text = localidad.Nombre;
            cboProvinciaDeLocalidad.SelectedValue = localidad.IdProvincia;
            btnAgregarLocalidad.Text = "Guardar";
            txtLocalidad.Focus();
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

        private void btnCancelarLocalidad_Click(object sender, EventArgs e) => LimpiarLocalidad();

        private void LimpiarLocalidad()
        {
            _idLocalidad = 0;
            txtLocalidad.Clear();
            btnAgregarLocalidad.Text = "Agregar";
        }

        // ── TipoDocumento ────────────────────────────────────────────────────

        private void CargarTiposDocumento()
        {
            dgvTipoDocumento.DataSource = _servicio.ObtenerTiposDocumento().ToList();
            LimpiarTipoDocumento();
        }

        private void btnAgregarTipoDocumento_Click(object sender, EventArgs e)
        {
            try
            {
                var tipo = new TipoDocumento
                {
                    IdTipoDocumento = _idTipoDocumento,
                    Descripcion = txtTipoDocumento.Text.Trim()
                };

                if (_idTipoDocumento == 0)
                {
                    _servicio.AltaTipoDocumento(tipo);
                }
                else
                {
                    _servicio.ModificarTipoDocumento(tipo);
                }

                CargarTiposDocumento();
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        private void btnEditarTipoDocumento_Click(object sender, EventArgs e)
        {
            if (dgvTipoDocumento.CurrentRow?.DataBoundItem is not TipoDocumento tipo)
            {
                AvisarSeleccion();
                return;
            }

            _idTipoDocumento = tipo.IdTipoDocumento;
            txtTipoDocumento.Text = tipo.Descripcion;
            btnAgregarTipoDocumento.Text = "Guardar";
            txtTipoDocumento.Focus();
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

        private void btnCancelarTipoDocumento_Click(object sender, EventArgs e) => LimpiarTipoDocumento();

        private void LimpiarTipoDocumento()
        {
            _idTipoDocumento = 0;
            txtTipoDocumento.Clear();
            btnAgregarTipoDocumento.Text = "Agregar";
        }

        // ── MedioPago ────────────────────────────────────────────────────────

        private void CargarMediosPago()
        {
            dgvMedioPago.DataSource = _servicio.ObtenerMediosPago().ToList();
            LimpiarMedioPago();
        }

        private void btnAgregarMedioPago_Click(object sender, EventArgs e)
        {
            try
            {
                var medio = new MedioPago
                {
                    IdMedioPago = _idMedioPago,
                    Descripcion = txtMedioPago.Text.Trim()
                };

                if (_idMedioPago == 0)
                {
                    _servicio.AltaMedioPago(medio);
                }
                else
                {
                    _servicio.ModificarMedioPago(medio);
                }

                CargarMediosPago();
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        private void btnEditarMedioPago_Click(object sender, EventArgs e)
        {
            if (dgvMedioPago.CurrentRow?.DataBoundItem is not MedioPago medio)
            {
                AvisarSeleccion();
                return;
            }

            _idMedioPago = medio.IdMedioPago;
            txtMedioPago.Text = medio.Descripcion;
            btnAgregarMedioPago.Text = "Guardar";
            txtMedioPago.Focus();
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

        private void btnCancelarMedioPago_Click(object sender, EventArgs e) => LimpiarMedioPago();

        private void LimpiarMedioPago()
        {
            _idMedioPago = 0;
            txtMedioPago.Clear();
            btnAgregarMedioPago.Text = "Agregar";
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
