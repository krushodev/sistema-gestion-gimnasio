# Patrón: validación en la UI (ErrorProvider + `ValidacionesUI`)

> **Agregado 22/09/2026.** Hasta acá, el formato de los datos (documento, email,
> teléfono) sólo se validaba en `SGIG.Negocio.Validaciones` (RF#09, RNF#04) — el
> usuario recién se enteraba de un error después de tocar la base, con un
> `MessageBox`. Este patrón agrega una validación previa, en la UI, que marca el
> control inválido con `ErrorProvider` y evita el viaje al servicio para los
> errores que ya se ven a simple vista. **No reemplaza** la validación de
> `SGIG.Negocio`: esa sigue siendo la última palabra (RNF#06) — por ejemplo, la
> unicidad de documento/nombre de usuario sólo se puede resolver contra la base.

## Qué agrega `SGIG.UI/ValidacionesUI.cs`

- `SoloLetras` (KeyPress): para `txtNombre`/`txtApellido` — RNF#04 pide que un
  campo no numérico rechace números, igual que `Grillas.SoloDigitos` ya hacía
  para documento.
- `EsSoloLetras(texto)`: para validar el contenido completo al guardar.
- `SoloDecimal` (KeyPress): para campos de monto.
- `Marcar(errorProvider, control, esValido, mensaje)`: setea o limpia el error
  de un control y devuelve el mismo booleano, para poder encadenar varias
  validaciones con `&` (no `&&`, así se evalúan y marcan **todas**, no sólo
  hasta la primera que falla — el usuario ve de una vez todo lo que falta
  corregir).

## Dónde vive la validación de cada formulario

- **`ucDatosPersona`** (compartido por `frmSocioEditor`, `frmUsuarioEditor`,
  `frmConfiguracion`) tiene su propio `public bool Validar()`: documento
  (`Validaciones.EsDocumentoValido`), nombre/apellido (sólo letras,
  obligatorios), tipo de documento seleccionado, email/teléfono (formato válido
  si se cargaron — son opcionales), fecha de nacimiento no futura. El
  formulario contenedor lo llama **antes** de armar el objeto a persistir:
  ```csharp
  private void btnGuardar_Click(object sender, EventArgs e)
  {
      if (!ucDatosPersona.Validar()) return;
      // ... arma la entidad y llama al servicio
  }
  ```
- Los campos propios de cada editor (no de Persona) se validan con su propio
  `ErrorProvider` en el mismo formulario: legajo/rol/nombre de
  usuario/contraseña en `frmUsuarioEditor`, máquina/detalle/fecha en
  `frmMantenimiento`, rango de fechas en filtros (`frmReportes`).
- `frmCobroCuota` valida cada campo con `MessageBox` (documento, plan, medio
  de pago, monto > 0) en vez de `ErrorProvider`, porque históricamente ya
  seguía ese estilo — no hace falta migrarlo, pero un formulario nuevo con
  varios campos sigue el patrón de `ErrorProvider`, no una cadena de
  `MessageBox`.

## Reglas para un formulario nuevo

1. Si tiene campos de Persona, usá `ucDatosPersona` y llamá a `Validar()` antes
   de guardar — no dupliques sus reglas.
2. Cualquier otro campo obligatorio o con formato propio (fecha, combo,
   número) se valida con su propio `ErrorProvider` y `ValidacionesUI.Marcar`,
   no dejando que el único chequeo sea la excepción que tira el servicio.
3. Restricción de tecleo (`KeyPress`) para los campos numéricos/alfabéticos
   más comunes: `Grillas.SoloDigitos`, `ValidacionesUI.SoloLetras`,
   `ValidacionesUI.SoloDecimal`.
4. `_errorProvider.Clear()` en cualquier "Limpiar"/recarga de datos, para no
   dejar marcas de una validación anterior sobre datos ya reemplazados.
