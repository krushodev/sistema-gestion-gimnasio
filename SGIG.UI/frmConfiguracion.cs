using SGIG.Entidades;
using SGIG.Negocio;

namespace SGIG.UI;

/// <summary>
/// Configuración de la propia cuenta: cambio de contraseña y edición de datos de
/// contacto. Abierta desde el botón "⚙ Configuración" de la barra superior de
/// frmMDIParent, para cualquiera de los 3 roles sobre su propia cuenta. Construido
/// enteramente en código (sin .Designer.cs, ver docs/patrones/estilo-visual.md)
/// porque el agente no puede usar el diseñador visual de WinForms.
/// </summary>
public class frmConfiguracion : Form
{
    private readonly ServicioUsuario _servicioUsuario = new();
    private readonly ServicioCatalogo _servicioCatalogo = new();
    private readonly Usuario _usuario;

    private readonly TextBox _txtActual;
    private readonly TextBox _txtNueva;
    private readonly TextBox _txtConfirmar;
    private readonly ucDatosPersona _ucDatosPersona;

    public frmConfiguracion(Usuario usuario)
    {
        _usuario = usuario;

        Text = "Configuración de mi cuenta";
        ClientSize = new Size(600, 560);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;

        // ── Sección: cambiar contraseña ──────────────────────────────────────
        var grpContrasenia = new GroupBox
        {
            Text = "Cambiar contraseña",
            Location = new Point(16, 16),
            Size = new Size(568, 168)
        };

        var lblActual = new Label { Text = "Contraseña actual:", Location = new Point(16, 30), AutoSize = true };
        _txtActual = new TextBox { Location = new Point(160, 27), Size = new Size(200, 23), PasswordChar = '●' };

        var lblNueva = new Label { Text = "Contraseña nueva:", Location = new Point(16, 66), AutoSize = true };
        _txtNueva = new TextBox { Location = new Point(160, 63), Size = new Size(200, 23), PasswordChar = '●' };

        var lblConfirmar = new Label { Text = "Confirmar nueva:", Location = new Point(16, 102), AutoSize = true };
        _txtConfirmar = new TextBox { Location = new Point(160, 99), Size = new Size(200, 23), PasswordChar = '●' };

        var btnCambiarContrasenia = new Button
        {
            Text = "Cambiar contraseña",
            Location = new Point(16, 132),
            Size = new Size(160, 28)
        };
        btnCambiarContrasenia.Click += BtnCambiarContrasenia_Click;

        grpContrasenia.Controls.AddRange(new Control[]
        {
            lblActual, _txtActual, lblNueva, _txtNueva, lblConfirmar, _txtConfirmar, btnCambiarContrasenia
        });

        // ── Sección: mis datos ────────────────────────────────────────────────
        // ucDatosPersona mide 560x200 (ver ucDatosPersona.Designer.cs); el
        // GroupBox y el botón de abajo se dimensionan para que entre entero.
        var grpDatos = new GroupBox
        {
            Text = "Mis datos",
            Location = new Point(16, 200),
            Size = new Size(568, 264)
        };

        _ucDatosPersona = new ucDatosPersona
        {
            Location = new Point(4, 20),
            MostrarFechaNacimiento = false
        };

        var btnGuardarDatos = new Button
        {
            Text = "Guardar mis datos",
            Location = new Point(4, 228),
            Size = new Size(160, 28)
        };
        btnGuardarDatos.Click += BtnGuardarDatos_Click;

        grpDatos.Controls.Add(_ucDatosPersona);
        grpDatos.Controls.Add(btnGuardarDatos);

        var btnCerrar = new Button
        {
            Text = "Cerrar",
            Location = new Point(492, 480),
            Size = new Size(92, 28),
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right
        };
        btnCerrar.Click += (s, e) => Close();

        Controls.AddRange(new Control[] { grpContrasenia, grpDatos, btnCerrar });
        AcceptButton = null;
        CancelButton = btnCerrar;

        Load += FrmConfiguracion_Load;

        Tema.AgregarToggleContrasenia(_txtActual);
        Tema.AgregarToggleContrasenia(_txtNueva);
        Tema.AgregarToggleContrasenia(_txtConfirmar);
        Tema.EstilizarFormulario(this);
        Tema.EstilizarControles(this);
    }

    private void FrmConfiguracion_Load(object? sender, EventArgs e)
    {
        try
        {
            _ucDatosPersona.CargarCatalogos(
                _servicioCatalogo.ObtenerTiposDocumento(),
                _servicioCatalogo.ObtenerProvincias(),
                _servicioCatalogo.ObtenerLocalidades());

            _ucDatosPersona.CargarParaEdicion(_usuario);
            _ucDatosPersona.BloquearIdentidad();
        }
        catch (Exception ex)
        {
            MostrarError(ex);
        }
    }

    private void BtnCambiarContrasenia_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_txtActual.Text))
        {
            MessageBox.Show("Ingresá tu contraseña actual.", "SGIG",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(_txtNueva.Text) || _txtNueva.Text.Length < 4)
        {
            MessageBox.Show("La nueva contraseña debe tener al menos 4 caracteres.", "SGIG",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (_txtNueva.Text != _txtConfirmar.Text)
        {
            MessageBox.Show("La confirmación no coincide con la contraseña nueva.", "SGIG",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            _servicioUsuario.CambiarContrasenia(_usuario.IdPersona, _txtActual.Text, _txtNueva.Text);
            MessageBox.Show("Contraseña actualizada con éxito.", "SGIG",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            _txtActual.Clear();
            _txtNueva.Clear();
            _txtConfirmar.Clear();
        }
        catch (Exception ex)
        {
            MostrarError(ex);
        }
    }

    private void BtnGuardarDatos_Click(object? sender, EventArgs e)
    {
        if (!_ucDatosPersona.Validar())
        {
            return;
        }

        try
        {
            _servicioUsuario.ActualizarDatosPropios(
                _usuario.IdPersona,
                _ucDatosPersona.Nombre,
                _ucDatosPersona.Apellido,
                _ucDatosPersona.Email,
                _ucDatosPersona.Telefono,
                _ucDatosPersona.IdLocalidad);

            // Refleja el cambio en el objeto Usuario de la sesión (lo usa la barra
            // superior de frmMDIParent para mostrar el nombre).
            _usuario.Nombre = _ucDatosPersona.Nombre;
            _usuario.Apellido = _ucDatosPersona.Apellido;
            _usuario.Email = _ucDatosPersona.Email;
            _usuario.Telefono = _ucDatosPersona.Telefono;
            _usuario.IdLocalidad = _ucDatosPersona.IdLocalidad;

            MessageBox.Show("Datos actualizados con éxito.", "SGIG",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MostrarError(ex);
        }
    }

    private static void MostrarError(Exception ex)
    {
        var esNegocio = ex is NegocioException;
        MessageBox.Show(ex.Message, "SGIG", MessageBoxButtons.OK,
            esNegocio ? MessageBoxIcon.Warning : MessageBoxIcon.Error);
    }
}
