# Patrón: formulario ABM (grilla + editor modal + baja lógica)

> **Actualizado 08/09/2026:** el patrón pasó de panel de edición embebido a editor modal
> (`ShowDialog`). Ver la nota en `Plan_Trabajo_SGIG.md` (Fases 2 y 3) para el porqué del cambio.
> El ejemplo histórico de panel embebido queda documentado en el historial de git de este archivo
> si hace falta consultarlo.

Estructura estándar de toda pantalla de alta/baja/modificación (`frmUsuarios`, `frmSocios`,
`frmTablasParametricas` y las que sigan, como `frmPlanes` en Fase 4). La pantalla de **listado**
se abre **dentro de `frmMDIParent`**, nunca como ventana suelta (RNF#02); el **editor** de
alta/edición es un formulario aparte que se abre como diálogo modal sobre ella.

**Dos formularios por módulo:**

1. **Listado** (`frmXxx`): `txtBuscar` (filtro rápido en memoria) + `dgvXxx` con los registros
   activos, y los botones `btnNuevo` / `btnEditar` / `btnDarDeBaja`. No tiene campos de edición
   propios — sólo abre el editor y, si vuelve con `DialogResult.OK`, recarga la grilla.
2. **Editor** (`frmXxxEditor`): los campos del alta/edición, `btnGuardar` / `btnCancelar`.
   Constructor `frmXxxEditor(Xxx? existente)` — `null` es alta, un valor es edición. Al guardar
   con éxito hace `DialogResult = DialogResult.OK; Close();`; al cancelar, `DialogResult.Cancel`.

Cuando varios módulos comparten los mismos campos de persona (Socio y Usuario, ambos sobre
`dbo.Persona`), esos campos van en un `UserControl` reutilizable (`ucDatosPersona`) embebido en
cada editor — ver `ucDatosPersona.cs` en `SGIG.UI`.

**Reglas:**

- El formulario **no conoce SQL ni `SGIG.Datos`**: sólo llama a un servicio de `SGIG.Negocio`.
- Toda llamada al servicio va en `try…catch`; el error se muestra con `MessageBox`, nunca se traga.
- La baja es **lógica** y exige confirmación Sí/No (RNF#03). La baja se sigue disparando desde el
  listado (no hace falta abrir el editor para dar de baja).
- Controles en notación húngara — ver [notacion-hungara.md](notacion-hungara.md).

```csharp
// Listado: abrir el editor y recargar sólo si confirmó
private void btnNuevo_Click(object sender, EventArgs e)
{
    using var editor = new frmSocioEditor(null);
    if (editor.ShowDialog(this) == DialogResult.OK)
    {
        CargarGrilla();
    }
}

private void btnEditar_Click(object sender, EventArgs e)
{
    var socio = SocioSeleccionado();
    if (socio is null) return;

    using var editor = new frmSocioEditor(socio);
    if (editor.ShowDialog(this) == DialogResult.OK)
    {
        CargarGrilla();
    }
}

// La baja sigue en el listado, no en el editor
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
```

**Nota para agentes:** cada formulario son **dos archivos** y los escribe el agente: la clase
(`frmSocios.cs`, `frmSocioEditor.cs`) con el comportamiento, y su `.Designer.cs` con los
controles, siguiendo la estructura estándar de Visual Studio (`components`, `Dispose(bool)`,
región `Windows Form Designer generated code`, `SuspendLayout`/`ResumeLayout`, tipos totalmente
calificados) para que el diseñador visual lo pueda seguir editando. Los `.resx` no se inventan:
sólo hacen falta si el formulario usa recursos. Ver la sección "Qué puede y qué no puede hacer un
agente" de `CLAUDE.md`. Siempre compilar con `dotnet build SGIG.slnx` antes de dar el formulario
por terminado.
