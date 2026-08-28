# Patrón: formulario ABM (grilla + panel de edición + baja lógica)

Estructura estándar de toda pantalla de alta/baja/modificación (`frmUsuarios`, `frmSocios`,
`frmPlanes`, `frmMaquinas`, `frmGastos`). Todas se abren **dentro de `frmMDIParent`**, nunca
como ventana suelta (RNF#02).

**Layout:** arriba `txtBuscar` + `dgvXxx` con los registros activos; abajo un panel de edición
(`grpDatos…`) con los campos; a un costado `btnNuevo` / `btnEditar` / `btnDarDeBaja` y, en el panel,
`btnGuardar` / `btnCancelar`. El panel arranca deshabilitado y se habilita con `btnNuevo`/`btnEditar`.

**Reglas:**

- El formulario **no conoce SQL ni `SGIG.Datos`**: sólo llama a un servicio de `SGIG.Negocio`.
- Toda llamada al servicio va en `try…catch`; el error se muestra con `MessageBox`, nunca se traga.
- La baja es **lógica** y exige confirmación Sí/No (RNF#03).
- Controles en notación húngara — ver [notacion-hungara.md](notacion-hungara.md).

```csharp
private void btnDarDeBaja_Click(object sender, EventArgs e)
{
    if (dgvSocios.CurrentRow?.DataBoundItem is not Socio socio)
    {
        MessageBox.Show("Seleccioná un socio de la grilla.", "SGIG",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
        return;
    }

    var respuesta = MessageBox.Show(
        $"¿Confirmás dar de baja al socio {socio.Nombre}?", "Confirmar baja",
        MessageBoxButtons.YesNo, MessageBoxIcon.Question);

    if (respuesta != DialogResult.Yes) return;

    try
    {
        _servicioSocio.DarDeBaja(socio.IdPersona);
        CargarGrilla();
    }
    catch (Exception ex)
    {
        MessageBox.Show(ex.Message, "SGIG", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}

private void CargarGrilla()
{
    dgvSocios.DataSource = _servicioSocio.ObtenerActivos().ToList();
}
```

**Nota para agentes:** el formulario son **dos archivos** y los escribe el agente: la clase
(`frmSocios.cs`) con el comportamiento, y `frmSocios.Designer.cs` con los controles, siguiendo la
estructura estándar de Visual Studio (`components`, `Dispose(bool)`, región
`Windows Form Designer generated code`, `SuspendLayout`/`ResumeLayout`, tipos totalmente
calificados) para que el diseñador visual lo pueda seguir editando. Los `.resx` no se inventan:
sólo hacen falta si el formulario usa recursos. Ver la sección "Qué puede y qué no puede hacer un
agente" de `CLAUDE.md`. Siempre compilar con `dotnet build SGIG.slnx` antes de dar el formulario
por terminado.
