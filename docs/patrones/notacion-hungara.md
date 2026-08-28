# Patrón: notación húngara para controles (RNF#05)

Convención obligatoria de la cátedra (`Notación.pdf`) para **nombres de controles de formulario**.
Es una convención de nombres de controles: no aplica a variables, propiedades de entidades ni
métodos, que van en PascalCase/camelCase de C#.

| Prefijo | Control | Ejemplo del proyecto |
|---|---|---|
| `frm` | Form | `frmLogin`, `frmMDIParent`, `frmSocios` |
| `txt` | TextBox | `txtDocumento`, `txtNombreUsuario` |
| `btn` / `cmd` | Button | `btnGuardar`, `btnDarDeBaja` |
| `cbo` | ComboBox | `cboRol`, `cboLocalidad`, `cboTipoDocumento` |
| `chk` | CheckBox | `chkActivo` |
| `lst` | ListBox | `lstPlanes` |
| `dgv` | DataGridView | `dgvUsuarios`, `dgvSocios` |
| `lbl` | Label | `lblUsuario`, `lblMensajeError` |
| `mnu` | MenuStrip / ítem de menú | `mnuSeguridad`, `mnuUsuarios` |
| `dtp` | DateTimePicker | `dtpFechaIngreso` |
| `tab` | TabControl / TabPage | `tabCatalogos`, `tabProvincia` |
| `grp` | GroupBox / Panel de edición | `grpDatosPersona` |

Reglas de uso:

- El prefijo va en minúscula y el resto en PascalCase: `btnGuardar`, no `BtnGuardar` ni `btn_guardar`.
- El nombre describe **qué dato o acción** representa, no dónde está: `txtDocumento`, no `txtCampo1`.
- Los manejadores de eventos heredan el nombre del control: `btnGuardar_Click`, `dgvSocios_SelectionChanged`.
- Los formularios en español y con el prefijo `frm`, aunque el repo de referencia mezcle inglés.

Ver también: [formulario-abm.md](formulario-abm.md).
