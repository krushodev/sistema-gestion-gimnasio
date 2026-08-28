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

**Nota para agentes:** el `.Designer.cs` y el `.resx` los genera Visual Studio — nunca crearlos ni
editarlos. En el archivo de la clase se deja un comentario con la lista de controles a agregar con
el diseñador (nombre + tipo) y se escribe el code-behind asumiendo esos nombres.
