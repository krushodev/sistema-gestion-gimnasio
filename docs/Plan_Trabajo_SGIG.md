# Plan de trabajo unificado — SGIG

Un solo plan, de punta a punta: arranca en la arquitectura, sigue con cada módulo de backend (entidades → datos → negocio) y, en el mismo lugar donde corresponde, con la pantalla completa de ese módulo (controles y comportamiento ya definidos, no hay que inventarlos después). Es la fuente que un agente Claude debe seguir para saber qué construir y en qué orden. `CLAUDE.md` lo referencia desde su sección "Orden de trabajo".

El documento hermano de este es el **backlog de Trello**: por cada entregable de acá abajo, indica qué tarjeta crear y en qué lista, para que puedas ir cargando el tablero a la par que el agente (o vos) avanza con el plan. Ese backlog **no está versionado en este repo** — vive fuera, del lado del usuario —, así que este archivo es la única fuente de verdad sobre qué construir.

**Cambio de modelo (31/08/2026 — ERS v4.0):** se eliminó la tabla `Gasto` (el Mantenimiento ya no genera un gasto asociado, se simplifica) y se refactorizó Tesorería: `Plan` pasa a tener `tipo_periodicidad` (Diario/Semanal/Mensual/Anual) en vez de `dias_vigencia`, y se agregó `Facturacion` como tabla intermedia entre Socio, Plan y Pago (representa un ciclo de cuota; `Pago` ahora cuelga de una `Facturacion`). El cálculo de vencimiento para Mensual/Anual usa aritmética de calendario (`AddMonths`/`AddYears`), no días fijos. Esto afecta las Fases 4, 6 y 7 de este plan — ver el detalle en cada una.

Convenciones: `[ ]` = pendiente, `[x]` = hecho. Notación húngara según `Notación.pdf` (RNF#05). Cada referencia `RF#`/`RNF#`/`HU-` apunta a la ERS v4.0 (`docs/ERS_SGIG_v4_0.docx`).

---

## Fase 0 — Arquitectura y configuración inicial

- [x] Crear `SGIG.slnx` (solución en blanco) — lo hace el usuario en Visual Studio, el agente no puede.
- [x] Crear los 4 proyectos SDK-style: `SGIG.Entidades`, `SGIG.Datos`, `SGIG.Negocio` (bibliotecas de clases), `SGIG.UI` (Windows Forms App).
- [x] Configurar referencias: `SGIG.Datos → SGIG.Entidades`; `SGIG.Negocio → SGIG.Datos` + `SGIG.Entidades`; `SGIG.UI → SGIG.Negocio` + `SGIG.Entidades`.
- [x] Instalar en `SGIG.Datos` los paquetes NuGet: `Dapper`, `Microsoft.Data.SqlClient`, `System.Configuration.ConfigurationManager`.
- [x] Establecer `SGIG.UI` como proyecto de inicio.
- [x] Crear `App.config` en `SGIG.UI` con la cadena de conexión `"SGIG"`.
- [x] Crear `.gitignore` (`bin/`, `obj/`, `.vs/`, `*.user`).
- [x] Copiar `docs/` (ERS, script SQL, este plan, cards de Trello) y `CLAUDE.md` a la raíz del repo.
- [x] Build vacío de los 4 proyectos + primer commit.
- [x] Crear `Conexion.cs` en `SGIG.Datos`: clase estática con `ObtenerConexionAbierta()` que lee `App.config` vía `ConfigurationManager.ConnectionStrings["SGIG"]`.
- [x] Crear `AccesoDatosException.cs` en `SGIG.Datos`, para envolver `SqlException` en cada repositorio.

## Fase 1 — Infraestructura base de UI: login y contenedor MDI

### 1.1 Pantalla `frmLogin`

- **Rol de acceso:** ninguno (pantalla previa al login).
- **Objetivo:** autenticar al usuario (RF#01).
- **Se abre desde:** `Program.cs` — primer formulario de la app.
- **Controles:**

  | Control | Tipo | Propósito |
  |---|---|---|
  | `lblUsuario` / `txtUsuario` | Label / TextBox | Nombre de usuario |
  | `lblContrasenia` / `txtContrasenia` | Label / TextBox (`PasswordChar='*'`) | Contraseña |
  | `btnIngresar` | Button | Dispara la autenticación |
  | `lblMensajeError` | Label (oculto) | "Usuario o contraseña incorrectos" |

- [x] Crear `frmLogin.cs` — esqueleto con estos controles, sin lógica de autenticación todavía (eso es Fase 2.4).

### 1.2 Pantalla `frmMDIParent`

- **Rol de acceso:** cualquier usuario logueado (el menú varía por rol, ver matriz de permisos en Fase 2.4).
- **Objetivo:** contenedor MDI, punto único de navegación (RNF#02).
- **Se abre desde:** `frmLogin`, tras login exitoso.
- **Controles:**

  | Control | Tipo | Propósito |
  |---|---|---|
  | `mnuSeguridad` | MenuStrip item | → `mnuUsuarios`, `mnuTablasParametricas` |
  | `mnuPersonas` | MenuStrip item | → `mnuSocios` |
  | `mnuTesoreria` | MenuStrip item | → `mnuPlanes`, `mnuPagos`, `mnuHistorialPagos` |
  | `mnuControlAcceso` | MenuStrip item | → `mnuCheckin` |
  | `mnuActivos` | MenuStrip item | → `mnuMaquinas`, `mnuMantenimiento`, `mnuHistorialMantenimientos` |
  | `mnuReportes` | MenuStrip item | → `mnuReporteIngresos`, `mnuBackup` |
  | `lblUsuarioLogueado` | Label (`StatusStrip`) | Nombre y rol activo |
  | `btnCerrarSesion` | ToolStripButton | Vuelve a `frmLogin` |

- [x] Crear `frmMDIParent.cs` — esqueleto (`IsMdiContainer = true`), con este menú, todas las opciones deshabilitadas por ahora.
- [x] Editar `Program.cs` para que `Main` arranque en `frmLogin`.

- [x] Renombrar el menú `mnuGastos` → `mnuReportes` y `mnuReporteBalance` → `mnuReporteIngresos`, y sacar el ítem `mnuGastosAbm` (ya no hay ABM de gastos, ERS v4.0). `mnuReportes` queda con `mnuReporteIngresos` y `mnuBackup`.

## Fase 2 — Seguridad: Rol, Usuario, Login funcional, ABM de Usuarios, Tablas paramétricas

> **Nota (08/09/2026):** refactor de UI y permisos, a pedido del usuario. `frmUsuarios` y
> `frmTablasParametricas` (sus 5 pestañas) pasaron del patrón de panel de edición embebido a
> editor modal (`ShowDialog`) — ver `docs/patrones/formulario-abm.md`. Se agregaron
> `frmUsuarioEditor`, `frmRolEditor`, `frmProvinciaEditor`, `frmLocalidadEditor`,
> `frmTipoDocumentoEditor` y `frmMedioPagoEditor`. `frmMDIParent.CargarTarjetasSegunRol` pasó de
> comparar `_usuario.Rol?.NombreRol` (string) a comparar `_usuario.IdRol` contra las constantes de
> la nueva clase `SGIG.Entidades.Roles` (Administrador=1, Recepcionista=2, Tecnico=3, según el
> orden del seed en `SGIG_CreateDB.sql`) — los 3 roles siguen siendo fijos, no se crean roles
> nuevos desde la UI de negocio; esto sólo evita comparar por string. Además, `ServicioUsuario.Alta`
> ahora reutiliza una Persona existente (por ejemplo, alguien que ya es Socio) en vez de rechazar
> el alta con `CampoDuplicadoException`, igual que ya hacía `ServicioSocio.Alta` — ver el nuevo
> `ServicioPersona` (Fase 3).

### 2.1 Entidades

- [x] `Rol.cs`, `Persona.cs`, `Usuario.cs` en `SGIG.Entidades`.

### 2.2 Acceso a datos

- [x] `RepositorioRol.cs`: alta y listado.
- [x] `RepositorioUsuario.cs`: consulta por nombre de usuario.
- [x] `RepositorioUsuario.cs`: alta transaccional (Persona + Usuario).
- [x] `RepositorioUsuario.cs`: validación de `nombre_usuario`/`legajo` único.
- [x] `RepositorioUsuario.cs`: baja lógica.

### 2.3 Lógica de negocio

- [x] `ServicioAutenticacion.cs`: valida usuario/contraseña contra hash SHA256, devuelve `Usuario` con su `Rol` (RF#01).

### 2.4 Login funcional y matriz de permisos (RF#01, RF#02)

- [x] Conectar `btnIngresar_Click` de `frmLogin` con `ServicioAutenticacion`; si es correcto abre `frmMDIParent` con el `Usuario`, si falla muestra `lblMensajeError`.
- [x] En `frmMDIParent`, habilitar los ítems de menú según esta matriz de permisos:

  | Pantalla | Administrador | Recepcionista | Técnico |
  |---|---|---|---|
  | `frmUsuarios` | ✅ | ❌ | ❌ |
  | `frmTablasParametricas` | ✅ | ❌ | ❌ |
  | `frmSocios` | ✅ | ✅ | ❌ |
  | `frmPlanes` | ✅ | ❌ | ❌ |
  | `frmPagos` | ❌ | ✅ | ❌ |
  | `frmHistorialPagos` | ✅ | ✅ | ❌ |
  | `frmCheckin` | ❌ | ✅ | ❌ |
  | `frmMaquinas` | ✅ | ❌ | ✅ |
  | `frmMantenimiento` | ❌ | ❌ | ✅ |
  | `frmHistorialMantenimientos` | ✅ | ❌ | ✅ |
  | `frmReporteIngresos` | ✅ | ❌ | ❌ |
  | `frmBackup` | ✅ | ❌ | ❌ |

### 2.5 Pantalla `frmUsuarios` (RF#03, RNF#03)

- **Rol de acceso:** Administrador.
- **Se abre desde:** `mnuUsuarios`.
- **Controles:**

  | Control | Tipo | Propósito |
  |---|---|---|
  | `dgvUsuarios` | DataGridView | Listado de usuarios activos |
  | `txtBuscar` | TextBox | Filtro rápido |
  | `btnNuevo` / `btnEditar` / `btnDarDeBaja` | Button | ABM |
  | `txtDocumento`, `cboTipoDocumento`, `txtNombre`, `txtApellido`, `txtEmail`, `txtTelefono`, `cboLocalidad` | — | Datos de Persona |
  | `txtLegajo`, `dtpFechaIngreso`, `cboRol`, `txtNombreUsuario`, `txtContrasenia` | — | Datos de Usuario |
  | `btnGuardar` / `btnCancelar` | Button | Confirmar o descartar |

- [x] Crear `frmUsuarios` con estos controles.
- [x] Alta unificada Persona + Usuario en una sola transacción.
- [x] Edición.
- [x] Baja lógica con confirmación `MessageBox` Sí/No (RNF#03).
- [x] Validación de formato con expresiones regulares: documento sólo numérico y estructura del email (RF#09, RNF#04).
- [x] Columnas explícitas en `dgvUsuarios` (no exponer el hash de contraseña ni los ids internos).

### 2.6 Pantalla `frmTablasParametricas` (RF#04) — RETIRADA

> **Nota (16/09/2026):** `frmTablasParametricas` y sus 5 editores (`frmRolEditor`,
> `frmProvinciaEditor`, `frmLocalidadEditor`, `frmTipoDocumentoEditor`, `frmMedioPagoEditor`) se
> eliminaron del todo. Decisión del usuario: Rol, Provincia, Localidad, TipoDocumento y MedioPago
> pasan a ser catálogos **sembrados con la aplicación** (ver seed en `docs/SGIG_CreateDB.sql`),
> sin ABM en la UI — ver `docs/patrones/catalogos-seed-only.md`. `RepositorioCatalogo`/
> `ServicioCatalogo`/`RepositorioRol` quedaron recortados a solo los métodos `Obtener*` que
> siguen alimentando combos en otras pantallas. La tarjeta "⚙️ Tablas Paramétricas" se sacó de
> `frmMDIParent`. Los roles en sí siguen siendo los 3 fijos de siempre (Administrador,
> Recepcionista, Técnico) — lo que ya era dinámico y se mantiene sin cambios es la asignación de
> uno de esos 3 roles al dar de alta un `Usuario` (`frmUsuarioEditor.cboRol`, ver 2.5).

- [x] ~~Crear `frmTablasParametricas` con las 5 pestañas.~~ Retirada — ver nota arriba.

## Fase 3 — Personas: Socios

> **Nota (07/09/2026, actualizada el mismo día):** `frmSocios` quedó alineado al contrato de controles de abajo — grilla + panel de edición (patrón `docs/patrones/formulario-abm.md`), con `txtBuscarDocumento`/`btnBuscar` para RF#06, y todos los campos de Persona y Socio, incluidos `cboPlan` (deshabilitado con texto explicativo hasta que exista el módulo de Planes, Fase 4) y `lblFechaVencimientoCuota`/`chkActivo` de solo lectura. `Socio.cs` ya tiene `IdPlan` y `FechaVencimientoCuota` (quedan en `NULL` hasta que Tesorería registre el primer pago). De paso se corrigió un bug real en `RepositorioPersona.ObtenerPorDocumento` (apuntaba a una tabla `Personas`/columnas que no existen en el DER) y se resolvió el caso de reactivar un socio dado de baja cuya fila en `dbo.Socio` ya existe (no se puede volver a `INSERT`, hace falta `UPDATE`). `frmSocios` ahora pasa por `ServicioSocio` en vez de llamar a `SGIG.Datos` directo, corrigiendo también una violación de capas.

### 3.1 Entidad

- [x] `Socio.cs` en `SGIG.Entidades`.

### 3.2 Acceso a datos

- [x] `RepositorioPersona.cs`: búsqueda por documento (RF#06).
- [x] `RepositorioSocio.cs`: alta transaccional (Persona + Socio).
- [x] `RepositorioSocio.cs`: consulta por documento y listado de activos.
- [x] `RepositorioSocio.cs`: baja lógica.

### 3.3 Lógica de negocio

- [x] `ServicioSocio.cs`: validación de documento único (RF#09).
- [x] `ServicioSocio.cs`: validación regex de email/documento (RF#09).

### 3.4 Pantalla `frmSocios` (RF#05, RF#06, RF#07, RNF#03)

- **Rol de acceso:** Administrador, Recepcionista.
- **Se abre desde:** `mnuSocios`.
- **Controles:**

  | Control | Tipo | Propósito |
  |---|---|---|
  | `txtBuscarDocumento` / `btnBuscar` | — | Reutilizar persona existente (RF#06) |
  | `dgvSocios` | DataGridView | Listado |
  | `btnNuevo` / `btnEditar` / `btnDarDeBaja` | Button | ABM |
  | `txtDocumento`, `cboTipoDocumento`, `txtNombre`, `txtApellido`, `txtEmail`, `txtTelefono`, `cboLocalidad` | — | Datos de Persona |
  | `dtpFechaNacimiento`, `txtAptoMedico`, `cboPlan`, `lblFechaVencimientoCuota` (solo lectura), `chkActivo` (solo lectura) | — | Datos de Socio (`cboPlan` es el plan preferido, no genera facturación por sí solo) |
  | `btnGuardar` / `btnCancelar` | Button | Confirmar o descartar |

- [x] Crear `frmSocios` con estos controles — ver nota de divergencia arriba, no coincide 1:1 con la tabla.
- [x] Alta reutilizando persona existente si el documento ya está cargado.
- [x] Edición.
- [x] Baja lógica con confirmación.

> **Nota (08/09/2026):** `frmSocios` pasó del panel de edición embebido a editor modal
> (`frmSocioEditor`, `ShowDialog`) — mismo refactor que Fase 2, ver `docs/patrones/formulario-abm.md`.
> El flujo de "buscar/reutilizar Persona por documento" (antes sólo en `frmSocios.btnBuscar_Click`)
> se extrajo a un componente compartido: `ServicioPersona` (`SGIG.Negocio`, wrapper de
> `RepositorioPersona.ObtenerPorDocumento`) y el `UserControl` `ucDatosPersona` (`SGIG.UI`), que
> autocompleta y **bloquea** los campos de Persona cuando el documento ya existe. Ambos editores
> (`frmSocioEditor` y el nuevo `frmUsuarioEditor` de Fase 2) embeben `ucDatosPersona`, así que
> ahora el alta de Usuario también puede reutilizar una Persona que ya es Socio (antes fallaba con
> `CampoDuplicadoException`) — para eso `RepositorioUsuario`/`ServicioUsuario` ganaron los mismos
> métodos de reutilización que ya tenía `RepositorioSocio`/`ServicioSocio` (`ExisteFilaUsuario`,
> `AltaSobrePersonaExistente`, `Reactivar`, `ExisteDocumentoUsuarioActivo`).

## Fase 4 — Tesorería: Planes, Facturación y Pagos

> **Refactor (31/08/2026 — ERS v4.0):** esta fase cambió de fondo respecto de versiones anteriores del plan. `Plan` ya no tiene `dias_vigencia` sino `tipo_periodicidad` (Diario/Semanal/Mensual/Anual). Se agrega `Facturacion` como tabla intermedia: representa un ciclo de cuota de un socio en un plan (fecha de emisión, vencimiento, monto). `Pago` ya no apunta directo a Socio+Plan, sino a una `Facturacion`. El vencimiento de Mensual/Anual se calcula con aritmética de calendario (`AddMonths`/`AddYears`), no días fijos — así se maneja bien la irregularidad de los meses (RF#12).

> **Nota (16/09/2026):** implementada. Acceso a datos y lógica de negocio se consolidaron en `RepositorioTesoreria.cs`/`ServicioTesoreria.cs` en vez de separar `RepositorioFacturacion`/`RepositorioPago` y `ServicioFacturacion`/`ServicioPago` como preveía este plan (Facturación y Pago se dan de alta siempre juntos dentro de la misma transacción, así que no había un caso de uso real que los necesitara aparte). Las pantallas `frmPagos` y `frmHistorialPagos` también se unificaron en una sola, `frmCobroCuota` (búsqueda de socio + cobro + grilla de historial en la misma ventana), con controles `txtDocumento`/`btnBuscarSocio` en vez de `txtBuscarDocumento`/`btnBuscar` y `cboPlanes` en vez de `cboPlan`. Al revisar esta entrega se corrigió además un bug real: `RepositorioTesoreria.ObtenerHistorialPorSocio` apuntaba a una tabla `dbo.Medio_Pago` que no existe (el DER usa `dbo.MedioPago`, sin guion bajo), y `RepositorioPlan.cs` no tenía el `Try…Catch` obligatorio (RNF#06) en ninguno de sus métodos — ambos corregidos. La base de datos local se recreó desde `SGIG_CreateDB.sql` para pasar del esquema viejo (con `Gasto`) al de ERS v4.0.

### 4.1 Entidades

- [x] `Plan.cs` en `SGIG.Entidades` — propiedades: `IdPlan`, `Nombre`, `Precio`, `TipoPeriodicidad` (string: "Diario"/"Semanal"/"Mensual"/"Anual"), `Activo`.
- [x] `Facturacion.cs` — propiedades: `IdFacturacion`, `IdPersona`, `IdPlan`, `FechaEmision`, `FechaVencimiento`, `MontoTotal`, `Estado`.
- [x] `Pago.cs` — propiedades: `IdPago`, `IdFacturacion`, `IdMedioPago`, `FechaPago`, `Monto`.

### 4.2 Acceso a datos

- [x] `RepositorioPlan.cs`: CRUD.
- [x] `RepositorioTesoreria.cs` (en vez de `RepositorioFacturacion.cs`/`RepositorioPago.cs` separados): alta transaccional de Facturación + Pago (ver 4.5), historial de pagos por socio (join contra `Facturacion` para llegar a `id_persona`, RF#14).

### 4.3 Lógica de negocio

- [x] `ServicioPlan.cs`: lectura del catálogo (RF#10) — ver nota 4.4, ya no tiene alta/edición/baja.
- [x] `ServicioTesoreria.cs` (en vez de `ServicioFacturacion.cs`/`ServicioPago.cs` separados): calcula `fecha_vencimiento` a partir de `fecha_emision` (o el vencimiento vigente, si todavía no venció) y `Plan.tipo_periodicidad` — `Diario` → `+1 día`, `Semanal` → `+7 días`, `Mensual` → `AddMonths(1)`, `Anual` → `AddYears(1)` (RF#12). Copia `Plan.precio` a `Facturacion.monto_total` en el momento de emitir (RF#13) y orquesta el alta transaccional actualizando `Socio.fecha_vencimiento_cuota`.

### 4.4 Pantalla `frmPlanes` (RF#10) — pasó a solo lectura

> **Nota (16/09/2026):** decisión del usuario: Plan pasa a ser un catálogo sembrado con la
> aplicación (ver seed en `docs/SGIG_CreateDB.sql`), sin ABM en la UI —
> `docs/patrones/catalogos-seed-only.md`. Se eliminó `frmPlanEditor`; `frmPlanes` conserva la
> grilla de consulta pero oculta `btnNuevo`/`btnEditar`/`btnDarDeBaja`. `RepositorioPlan`/
> `ServicioPlan` quedaron recortados a `ObtenerTodos`/`Listar`/`ObtenerPorId`.

- **Rol de acceso:** Administrador. **Se abre desde:** tarjeta "Planes de Membresía" en `frmMDIParent`.
- **Controles:** `dgvPlanes`, `chkMostrarInactivos`. `btnNuevo`/`btnEditar`/`btnDarDeBaja` siguen en el `.Designer.cs` pero ocultos (nunca se muestran).

- [x] Crear `frmPlanes` — de solo lectura, muestra el catálogo sembrado.

### 4.5 y 4.6 Pantalla `frmCobroCuota` (RF#11, RF#12, RF#13, RF#14 — unifica `frmPagos` y `frmHistorialPagos`)

- **Rol de acceso:** Recepcionista. **Se abre desde:** tarjeta de Tesorería en `frmMDIParent`.
- **Controles:** `txtDocumento`/`btnBuscarSocio`, `lblNombreSocioValor`, `lblVencimientoActualValor`, `cboPlanes`, `cboMedioPago`, `txtMonto` (autocompletado con `Plan.precio`), `btnRegistrarPago`, `lblNuevoVencimientoValor`, `dgvHistorialPagos`.

- [x] Crear `frmCobroCuota` — búsqueda de socio por documento.
- [x] `btnRegistrarPago_Click` dispara **una sola transacción** (`RepositorioTesoreria.RegistrarCobroTransaccional`) que: (1) crea la `Facturacion` (emisión = hoy, vencimiento calculado por `ServicioTesoreria`, monto = precio del plan elegido, estado `'Pagada'`), (2) inserta el `Pago` asociado a esa `Facturacion`, (3) actualiza `Socio.id_plan` y `Socio.fecha_vencimiento_cuota`. Muestra el resultado en `lblNuevoVencimientoValor` y refresca `dgvHistorialPagos` en la misma pantalla.

## Fase 5 — Control de Acceso: Check-in

> **Nota (07/09/2026):** implementada en su totalidad. Como Fase 4 (Tesorería) todavía no existe, `Socio.FechaVencimientoCuota` está en `NULL` para todo socio hasta que se registre el primer pago real — hoy el Check-in rechaza a todo el mundo por "no tiene ninguna cuota registrada", que es el comportamiento correcto dado el estado actual de los datos, no un bug. Para probar el camino "Concedido" antes de que exista Fase 4, hay que cargar manualmente un `fecha_vencimiento_cuota` futuro en algún registro de `dbo.Socio` por SQL directo. La verificación de que la respuesta baja de 2 segundos (RNF#01) queda pendiente porque requiere ejecutar la app con datos reales — el agente no puede correr WinForms (ver límite en `CLAUDE.md`); la consulta de `RepositorioCheckin.BuscarSocioPorDocumento` es una única sentencia indexada, así que en la práctica no debería ser un problema.

### 5.1 Entidad

- [x] `Checkin.cs` en `SGIG.Entidades`.

### 5.2 Acceso a datos

- [x] `RepositorioCheckin.cs`: inserción.
- [x] `RepositorioCheckin.cs`: consulta rápida por documento.
- [x] `RepositorioCheckin.cs`: `ObtenerHistorial(desde, hasta, documento?)` — historial con JOIN a `Persona`/`Socio`, usa `IX_Checkin_Persona_Fecha` (23/09/2026, ver 5.5).

### 5.3 Lógica de negocio

- [x] `ServicioCheckin.cs`: lógica Concedido/Rechazado comparando contra `Socio.fecha_vencimiento_cuota` (el campo caché, no consulta `Facturacion` — es lo que mantiene el Check-in rápido, RNF#01) (RF#16).
- [x] `ServicioCheckin.cs`: `ObtenerHistorial(...)` — valida el rango de fechas y delega en el repositorio (23/09/2026).
- [x] `ResultadoCheckin.DiasRestantesCuota` — calculado sin consultas extra (el dato ya se lee para decidir el acceso); el mensaje de `frmCheckin` ahora dice "Vence en N día(s)" / "Vencida hace N día(s)" en vez de sólo Concedido/Rechazado (23/09/2026).

### 5.4 Pantalla `frmCheckin` (RF#15, RF#17, RNF#01)

- **Rol de acceso:** Recepcionista. **Se abre desde:** el dashboard de `frmMDIParent`.
- **Controles:** `txtDocumento` (foco automático, dispara con Enter), `lblAyuda` (texto guía), `btnBuscarPorNombre`, `btnLimpiar`, `pnlResultado` (verde/rojo), `lblResultado`, `lblNombreSocio`.

- [x] Crear `frmCheckin` — campo único, sin botones intermedios.
- [x] Feedback visual verde/rojo y registro automático del intento.
- [x] Buscador por nombre (`btnBuscarPorNombre` → `frmBuscarSocio`) y botón `btnLimpiar`, reordenados en una fila de acciones propia debajo del campo de documento (23/09/2026, corrige el solape de anchors que tenía el botón agregado por código).
- [ ] Verificar que la respuesta se resuelve en menos de 2 segundos (requiere ejecutar la app — ver nota arriba).

### 5.5 Pantalla `frmHistorialAccesos` (agregado 23/09/2026, fuera del alcance original de RF#15-17)

Pedido explícito: Recepción necesita un registro de qué personas accedieron y cuántos días
les quedan (o hace cuántos vencieron) antes de la próxima cuota, para seguimiento de gestión
— `frmCheckin` sólo mostraba el resultado del intento del momento, sin historial.

- **Rol de acceso:** Recepcionista (mismo criterio que `frmCheckin` — ver `CargarTarjetasSegunRol` en `frmMDIParent.cs`). **Se abre desde:** el dashboard de `frmMDIParent`, tarjeta "Historial de Accesos".
- **Controles:** `dtpDesde`, `dtpHasta` (por defecto últimos 30 días), `txtDocumento` (filtro opcional, parcial), `btnBuscar`, `dgvHistorial` (Documento, Nombre y apellido, Fecha y hora, Resultado, Cuota).

- [x] `Checkin.cs`: campos de sólo lectura resueltos por JOIN (`Documento`, `NombreCompleto`, `DiasRestantesCuota`) y `DescripcionVencimiento` calculado ("Vence en N día(s)" / "Vencida hace N día(s)" / "Sin cuota registrada").
- [x] Crear `frmHistorialAccesos` siguiendo el mismo patrón que `frmHistorialMantenimientos`.
- [x] Tarjeta en el dashboard de `frmMDIParent`, junto a "Control de Acceso".

## Fase 6 — Activos: Máquinas y Mantenimientos (rol Técnico)

> **Simplificación (31/08/2026 — ERS v4.0):** Mantenimiento ya no genera un `Gasto` asociado (la tabla `Gasto` se eliminó del modelo). RF#23 ("debe generar el Gasto asociado") queda dado de baja. La transacción de alta de Mantenimiento se reduce a 2 tablas: `Mantenimiento` + actualización de `Maquina.estado`.

> **Nota (07/09/2026):** implementada en su totalidad. El estado `'En Reparacion'` se escribe sin
> tilde en todo el código porque así está el `CHECK` en `SGIG_CreateDB.sql` (`CK_Maquina_Estado`).
> `frmMantenimiento` sólo ofrece en `cboMaquina` las máquinas hoy "Operativa" (una "En Reparacion"
> ya tiene un mantenimiento activo). En el dashboard de `frmMDIParent`, "Máquinas" e "Historial de
> Mantenimientos" quedaron habilitadas para Administrador y Técnico, y "Mantenimiento" sólo para
> Técnico, según la matriz de la Fase 2.4. **Actualización (16/09/2026):** Maquina pasó a ser
> catálogo seed-only y `frmMaquinas` a solo lectura — ver nota en 6.4. El detalle sobre el
> `DELETE` físico (`dbo.Maquina` no tiene columna `activo`) queda como referencia histórica del
> repositorio, aunque el botón que lo disparaba ya no es visible desde la UI.

### 6.1 Entidades

- [x] `Maquina.cs`, `Mantenimiento.cs` en `SGIG.Entidades` (`Mantenimiento` ya no tiene `IdGasto`).

### 6.2 Acceso a datos

- [x] `RepositorioMaquina.cs`: CRUD.
- [x] `RepositorioMantenimiento.cs`: alta transaccional (inserta el mantenimiento y cambia el estado de la máquina a "En Reparación", RF#20).
- [x] `RepositorioMantenimiento.cs`: historial por máquina (RF#21).

### 6.3 Lógica de negocio

- [x] `ServicioMaquina.cs`: cambio de estado automático.
- [x] `ServicioMantenimiento.cs`: técnico a cargo tomado del usuario logueado.

### 6.4 Pantalla `frmMaquinas` (RF#18) — pasó a solo lectura

> **Nota (16/09/2026):** decisión del usuario: Maquina pasa a ser un catálogo sembrado con la
> aplicación (ver seed en `docs/SGIG_CreateDB.sql`), sin ABM en la UI —
> `docs/patrones/catalogos-seed-only.md`. `frmMaquinas` conserva la grilla de consulta pero oculta
> `grpDatos` (marca/nombre/fecha de compra/estado) y `btnNuevo`/`btnGuardar`/`btnEliminar`.
> `RepositorioMaquina`/`ServicioMaquina` quedaron recortados a `ObtenerTodas`/`ObtenerOperativas`
> (`ObtenerOperativas` la sigue usando `frmMantenimiento` para el combo de máquinas).

- **Rol de acceso:** Administrador, Técnico. **Se abre desde:** tarjeta "Máquinas" en `frmMDIParent`.
- **Controles:** `dgvMaquinas`. `grpDatos` y `btnNuevo`/`btnGuardar`/`btnEliminar` siguen en el `.Designer.cs` pero ocultos (nunca se muestran).

- [x] Crear `frmMaquinas` — de solo lectura, muestra el catálogo sembrado.

### 6.5 Pantalla `frmMantenimiento` (RF#19, RF#20)

- **Rol de acceso:** Técnico. **Se abre desde:** `mnuMantenimiento`.
- **Controles:** `dgvMantenimientosActivos`, `cboMaquina`, `dtpFechaInicio`, `txtDetalleTecnico`, `btnRegistrar`, `dtpFechaFin`, `btnFinalizar`. (Se sacan `txtMontoGasto` y `txtDescripcionGasto` — ya no aplica.)

- [x] Crear `frmMantenimiento` — alta transaccional (mantenimiento + estado "En Reparación").
- [x] Finalización: cierra el mantenimiento y devuelve la máquina a "Operativa", estado visible en la grilla.

### 6.6 Pantalla `frmHistorialMantenimientos` (RF#21)

- **Rol de acceso:** Administrador, Técnico. **Se abre desde:** `mnuHistorialMantenimientos`.
- **Controles:** `cboMaquina`, `dgvHistorialMantenimientos`.

- [x] Crear `frmHistorialMantenimientos` con consulta por máquina.

## Fase 7 — Reportes y Backup

> **Cambio de alcance (31/08/2026 — ERS v4.0):** al eliminar `Gasto`, no hay ABM de gastos ni "balance" (pagos − gastos). RF#22 se da de baja. RF#24 se redefine como un **reporte de ingresos por pagos**, filtrado por rango de fechas, sin gastos.

### 7.1 Lógica de negocio

- [ ] `ServicioReporte.cs`: total de ingresos (suma de `Pago.monto`) filtrado por rango de fechas (RF#24).

### 7.2 Pantalla `frmReporteIngresos` (RF#24)

- **Rol de acceso:** Administrador. **Se abre desde:** `mnuReporteIngresos`.
- **Controles:** `dtpDesde`/`dtpHasta`, `btnGenerar`, `lblTotalIngresos`, `dgvDetalle` (detalle de pagos del período: fecha, socio, plan, monto, medio de pago).

- [ ] Crear `frmReporteIngresos` con cálculo y filtro por fechas.

### 7.3 Pantalla `frmBackup` (RF#25)

- **Rol de acceso:** Administrador. **Se abre desde:** `mnuBackup`.
- **Controles:** `txtRutaArchivo`, `btnSeleccionarRuta`, `btnBackup`, `btnRestore` (con confirmación previa), `lblEstado`.

- [ ] Crear `frmBackup`.
- [ ] `ServicioBackup.cs`: `BACKUP DATABASE`.
- [ ] `ServicioBackup.cs`: `RESTORE DATABASE` con confirmación `MessageBox` Sí/No previa.

## Fase 8 — Documentación y Entrega

- [ ] Capturas de pantalla por módulo (Seguridad, Personas, Tesorería, Control de Acceso, Activos, Reportes) — las toma el usuario una vez armado cada formulario.
- [ ] Insertar capturas en el Manual de Usuario (Anexo A de la ERS).
- [ ] Completar integrantes del grupo en la portada de la ERS.
- [x] Cargar datos de prueba (socios, planes, máquinas) para la demo — `docs/SGIG_CreateDB.sql` seedea 2 Recepcionistas, 2 Técnicos, 10 Socios (5 con cuota al día, 5 vencida) con su Facturación/Pago, y amplió el catálogo de Máquinas (23/09/2026).
- [ ] Probar flujo completo — rol Administrador.
- [ ] Probar flujo completo — rol Recepcionista.
- [ ] Probar flujo completo — rol Técnico.
- [ ] Preparar el guion de la presentación/demo para la cátedra.

---

## Correcciones post-Fase 4 (16/09/2026)

Ronda de correcciones de UX reportadas por el usuario después de probar la app, transversales a
varias fases ya cerradas — no es una fase nueva, se documenta acá para no perderla de vista:

- **`frmSocioEditor`**: `cboPlan` estaba deshabilitado con un placeholder ("disponible cuando se
  implemente Fase 4") que nunca se actualizó al cerrar esa fase — ahora carga `ServicioPlan.Listar()`
  y persiste `Socio.IdPlan`.
- **`ucDatosPersona`**: nuevo método `Reiniciar()` (alias de `PrepararParaAlta()`) y nuevo método
  `BloquearIdentidad()` (deshabilita sólo documento/tipo de documento, para el caso de "editar mis
  datos" sin tocar la identidad). `frmSocioEditor` y `frmUsuarioEditor` suman un botón "Limpiar
  datos" (sólo visible en alta nueva) que deshace el autocompletado bloqueado por búsqueda de
  documento.
- **`frmCobroCuota`**: `txtMonto` pasa a editable (antes `ReadOnly` fijo al precio del plan, sin
  forma de aplicar un descuento puntual). `ServicioTesoreria.RegistrarCobro` ahora recibe el monto
  como parámetro en vez de tomar siempre `Plan.Precio`.
- **Búsqueda por nombre**: nuevo `frmBuscarSocio` (selector modal por nombre/apellido/documento,
  `RepositorioSocio.BuscarActivosPorTexto`/`ServicioSocio.BuscarPorTexto`), agregado como botón
  "Por nombre"/"Buscar por nombre" junto a la búsqueda por documento de `frmCobroCuota` y
  `frmCheckin`. `frmSocios` no se tocó: su filtro de grilla ya buscaba por nombre/apellido/documento.
- **Barra superior**: el usuario logueado, "⚙ Configuración" y "Cerrar sesión" se movieron de
  `stsEstado` (franja inferior, casi invisible) a una barra nueva y persistente arriba a la derecha
  de `frmMDIParent`, visible tanto en el dashboard como con cualquier módulo abierto (a diferencia
  de `pnlHeader`, que se oculta al abrir un módulo). `stsEstado` queda oculto pero sin eliminar.
- **Máquinas**: seed ampliado en `docs/SGIG_CreateDB.sql` (de 12 a ~39 filas) con más variedad —
  cardio, fuerza selectorizada, free weights (barras, discos, mancuernas, kettlebells) y accesorios,
  con marcas reales variadas.
- **Mostrar/ocultar contraseña**: nuevo `Tema.AgregarToggleContrasenia(TextBox)`, aplicado en
  `frmLogin`, `frmUsuarioEditor` y los 3 campos de contraseña de `frmConfiguracion`.
- **Nueva pantalla `frmConfiguracion`**: cambio de contraseña propia (`ServicioUsuario.
  CambiarContrasenia`, valida la actual con `Hash.Coincide` antes de aceptar la nueva) y edición de
  datos de contacto propios (`ServicioUsuario.ActualizarDatosPropios`, no toca documento/rol/legajo).
  Disponible para los 3 roles sobre su propia cuenta, desde el botón "⚙ Configuración" de la barra
  superior.

## Cómo usar este plan con el agente

1. Seguir las fases en el orden en que aparecen; dentro de una fase, las subsecciones también van en orden de dependencia (entidad → datos → negocio → pantalla).
2. No empezar una fase si la anterior no compila.
3. Cuando el usuario diga "seguí con lo que sigue" o "la próxima fase", el agente ubica el primer paso `[ ]` de este archivo y continúa desde ahí.
4. El agente sí compila (`dotnet build SGIG.slnx`) y escribe tanto la clase del formulario como su `.Designer.cs`; lo que no puede es juzgar el resultado *visual* (ver `CLAUDE.md`) — cada pantalla trae su tabla de controles como contrato, y el ajuste fino de layout queda para el diseñador de Visual Studio.
5. Marcar cada paso como hecho (`[x]`) a medida que se entrega. Si el agente no puede editar este archivo en el momento, debe decirle al usuario qué pasos completó para que él los tilde.
