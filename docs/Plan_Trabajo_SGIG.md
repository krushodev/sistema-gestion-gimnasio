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

- [x] `ServicioReporte.cs`: total de ingresos (suma de `Pago.monto`) filtrado por rango de fechas (RF#24) — `GenerarReporte(desde, hasta)` y `ObtenerMetricas()` para las tarjetas del dashboard.

### 7.2 Pantalla `frmReporteIngresos` (RF#24)

- **Rol de acceso:** Administrador. **Se abre desde:** `mnuReporteIngresos`.
- **Controles:** `dtpDesde`/`dtpHasta`, `btnGenerar`, `lblTotalIngresos`, `dgvDetalle` (detalle de pagos del período: fecha, socio, plan, monto, medio de pago).

- [x] Implementado como `frmReportes` (en lugar de `frmReporteIngresos`): filtro por rango de fechas, tarjetas de métricas (ingresos del mes, socios activos, check-ins promedio), grilla de detalle y exportación a CSV. Se abre desde el menú Reportes de `frmMDIParent`.

### 7.3 Pantalla `frmBackup` (RF#25)

- **Rol de acceso:** Administrador. **Se abre desde:** `mnuBackup`.
- **Controles:** `txtRutaArchivo`, `btnSeleccionarRuta`, `btnBackup`, `btnRestore` (con confirmación previa), `lblEstado`.

- [x] Crear `frmBackup` (botones `btnGenerarBackup` y `btnRestaurar`, `lblEstado`; operaciones en segundo plano con la UI bloqueada).
- [x] `ServicioBackup.cs`: `BACKUP DATABASE` (`GenerarCopiaSeguridad`).
- [x] `ServicioBackup.cs`: `RESTORE DATABASE` (`RestaurarCopiaSeguridad`) con confirmación `MessageBox` Sí/No previa.

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

## Fase 9 — Rediseño visual (UI moderna, minimalista y adaptable)

> **Origen (18/10/2026):** pedido del usuario tras cerrar el desarrollo funcional. Objetivo: una app más intuitiva, con paleta definida, sidebar de navegación, íconos minimalistas y layout que se adapte al tamaño de la ventana. **Es un cambio de presentación: no toca entidades, datos, negocio, la ERS ni el modelo de roles.** Extiende la decisión de `docs/patrones/estilo-visual.md` (hoy `Tema.cs` sólo pinta; esta fase lo convierte en un sistema de diseño).

### Diagnóstico del estado actual

- **Navegación:** dashboard de 8 tarjetas (grilla 4×2) + botón "Volver al Inicio" en cada módulo. Para cambiar de módulo hay que volver al inicio: 2 clics de más en cada cambio.
- **Íconos:** emojis a color (`Segoe UI Emoji`, 34 pt en las tarjetas, ~26 usos en `frmMDIParent.cs` más `frmCheckin`, `frmBackup`, `frmConfiguracion`, `Tema.cs`). Son el principal motivo de lo "grotesco": tamaño fijo, colores que no responden a la paleta y aspecto distinto según la versión de Windows.
- **Color:** cada tarjeta tiene su propio color de acento (8 distintos) y los colores están hardcodeados (`Color.FromArgb(...)`) en `frmMDIParent`, `frmLogin` y `Tema`, con valores casi iguales pero no idénticos (ej. `241,245,249` vs `Tema.FondoClaro 247,250,252`).
- **Barra superior:** "Configuración" y "Cerrar sesión" como botones sólidos con texto; el título de sección vive en otra barra distinta (`pnlBarraVolver`).
- **Adaptabilidad:** la grilla de tarjetas escala por porcentajes, pero la barra superior posiciona los botones a mano (`Location` calculada una sola vez, no se recalcula al redimensionar) y no hay ancho mínimo ni comportamiento definido en ventanas chicas.

### 9.1 Decisiones de diseño (a confirmar antes de implementar)

- **Paleta (tokens únicos en `Tema.cs`; ningún `Color.FromArgb` suelto fuera de ahí).** Neutros slate + **un solo** acento; los colores por módulo desaparecen.

  | Token | Valor | Uso |
  |---|---|---|
  | `SidebarFondo` | `#0F172A` | fondo del sidebar |
  | `SidebarTexto` / `SidebarActivoFondo` | `#94A3B8` / `#1E293B` | ítem normal / ítem seleccionado (con barra de acento a la izquierda) |
  | `Fondo` | `#F8FAFC` | fondo del área de contenido |
  | `Superficie` | `#FFFFFF` | tarjetas, grillas, formularios |
  | `Borde` | `#E2E8F0` | separadores y bordes de 1 px (reemplazan sombras) |
  | `TextoPrimario` / `TextoSecundario` | `#0F172A` / `#64748B` | títulos y cuerpo / descripciones |
  | `Acento` / `AcentoHover` | `#2563EB` / `#1D4ED8` | botón principal, ítem activo, foco |
  | `Exito` / `Peligro` / `Aviso` | `#16A34A` / `#DC2626` / `#D97706` | estados (cuota al día / vencida / por vencer; máquina operativa / en mantenimiento) |

  Contraste mínimo AA (4.5:1) para texto sobre `Fondo` y `Superficie`; verificarlo al fijar los valores finales.
- **Tipografía:** `Segoe UI Variable` con *fallback* a `Segoe UI` (Windows 10). Escala fija en `Tema`: Título 20, Subtítulo 12, Cuerpo 10, Detalle 9. Se eliminan las fuentes sueltas de 26/34 pt.
- **Íconos:** glifos de **`Segoe Fluent Icons`** (Windows 11) con *fallback* a `Segoe MDL2 Assets` (Windows 10). Son monocromos, de línea fina, se tiñen con el color del texto (activo/inactivo/hover) y escalan nítidos a cualquier DPI. **Sin paquetes NuGet ni archivos de imagen.** Alternativa descartada por ahora: SVG con librería externa (más control visual, pero suma dependencia y recursos). El mapa de glifos se centraliza en una clase `Iconos` (`Iconos.Socios`, `Iconos.Cobro`, ...) para poder cambiar la fuente en un único lugar.
- **Layout general:** sidebar fijo a la izquierda (232 px) + barra superior liviana (título de la sección actual a la izquierda; usuario/rol a la derecha) + área de contenido con padding uniforme de 24 px.
- **Adaptabilidad:** por debajo de ~1100 px de ancho el sidebar se **colapsa a sólo íconos** (64 px, con *tooltip*); un botón de hamburguesa lo expande/contrae a mano. `MinimumSize` definido en `frmMDIParent`. Dashboard con `FlowLayoutPanel` (tarjetas de ancho mínimo que se reacomodan en 4/3/2/1 columnas) en vez de la grilla fija 4×2.
- **Tarjetas y controles:** esquinas redondeadas (radio 10) con borde de 1 px en vez de la barra de color superior; botones en 3 variantes (primario relleno / secundario con borde / peligro) con estado hover/pressed; foco visible en campos de texto; grillas (`dgv`) con filas altas, sin líneas verticales, cabecera sin relleno y fila alterna muy sutil.
- **Alcance:** *no* se reordenan los controles internos de los ~21 formularios (riesgo de solapamientos que no se pueden verificar sin ver la pantalla — ver `estilo-visual.md`). Los módulos heredan el nuevo look por pintura central (`Tema`) y por el nuevo contenedor; sólo Login, Dashboard y Check-in se rehacen a fondo. El ajuste fino del resto queda para el diseñador de Visual Studio.

### 9.2 Sistema de diseño en `Tema.cs` (base de todo lo demás)

- [x] Reemplazar la paleta actual por los tokens de 9.1 (mantener los nombres viejos como alias `[Obsolete]` durante la migración para no romper pantallas).
- [x] Escala tipográfica y helper de fuente con *fallback* (`Tema.Fuente(tamaño, estilo)`).
- [x] Clase `Iconos` con el mapa de glifos + helper `Tema.FuenteIconos(tamaño)` con *fallback* Fluent → MDL2.
- [x] Extender `Tema.EstilizarControles`: variantes de botón por convención de nombre (ya existe `ColorSegunNombre`), `ComboBox`/`DateTimePicker`/`TextBox` con borde y foco coherentes, y `Tema.EstilizarGrilla(DataGridView)` (unifica y reemplaza lo que hoy hace `Grillas.cs` donde corresponda).
- [x] Control propio `RoundedPanel` (borde redondeado + borde de 1 px) para tarjetas y bloques de formulario. Escrito en código (`OnPaint`), sin `.Designer.cs` involucrado.
- [x] Soporte High-DPI: `ApplicationConfiguration.Initialize()` ya está en `Program.cs`; los formularios nuevos usarán `AutoScaleMode.Dpi`.
- [x] Actualizar `docs/patrones/estilo-visual.md` con el sistema nuevo (tokens, íconos, reglas de uso) y registrar la decisión en `CLAUDE.md` como nota histórica #6.

### 9.3 Contenedor principal: sidebar + barra superior (`frmMDIParent`)

- [x] Control `ucSidebar` (UserControl en código): logo/nombre "SGIG" arriba, ítems de navegación filtrados por rol (misma matriz de permisos de 2.4, **sin cambiarla**), ítem activo resaltado, "Configuración" y "Cerrar sesión" fijados abajo.
- [x] Agrupar los ítems por sección: **Gestión** (Socios, Cobro de cuotas), **Acceso** (Control de acceso, Historial de accesos), **Activos** (Máquinas, Mantenimiento, Historial de mantenimientos), **Administración** (Usuarios, Reportes, Copia de seguridad) — cada rol ve sólo las secciones e ítems que le corresponden.
- [x] Barra superior nueva: título de la sección actual + usuario y rol (avatar con iniciales) a la derecha. **Se elimina `pnlBarraVolver` y el botón "Volver al Inicio"**: navegar es hacer clic en el sidebar.
- [x] `AbrirFormularioEnPanel` pasa a ser `NavegarA(...)`: reemplaza el formulario hijo actual (cerrándolo y liberándolo) en vez de apilarlo, y marca el ítem activo del sidebar.
- [x] Colapso adaptable del sidebar (< 1100 px → sólo íconos con *tooltip*; botón de hamburguesa manual) y `MinimumSize` del formulario.
- [x] Mantener la confirmación `MessageBox` Sí/No al cerrar sesión.

> **Implementado (18/10/2026):** `ucSidebar.cs` (sidebar + `ItemSidebar`/`EncabezadoSeccion`/`AvatarIniciales`) y `frmMDIParent.cs` reescrito por código; el `.Designer.cs` no se tocó (la franja `stsEstado` sigue oculta y `btnCerrarSesion_Click` se conserva). "Planes" quedó en la sección Gestión (sólo Administrador). Mientras no esté la 9.4, "Inicio" muestra sólo un saludo. Pendiente de revisión visual por el usuario: glifos de íconos, colapso a < 1100 px y recorrido por rol.

### 9.4 Dashboard de inicio

- [x] Reemplazar las 8 tarjetas de acceso (redundantes con el sidebar) por un panel de **resumen según el rol**: Administrador → ingresos del mes, socios activos / con cuota al día, check-ins promedio (reutiliza `ServicioReporte.ObtenerMetricas()`); Recepcionista → atajos a Check-in y Cobro de cuota + últimos accesos; Técnico → máquinas en mantenimiento y atajo a registrar falla.
- [x] Tarjetas de métrica con `RoundedPanel`, ícono de línea, valor grande y detalle en `TextoSecundario`; distribución con `FlowLayoutPanel` adaptable (4/3/2/1 columnas).
- [x] Saludo contextual ("Buen día, Nombre") y fecha actual.
- [x] Si algún dato del resumen requiere una consulta nueva en `SGIG.Datos`/`SGIG.Negocio`, documentarla acá antes de escribirla (por ahora se espera reutilizar lo existente).

> **Implementado (18/10/2026):** `ucInicio.cs` (+ `TarjetaMetrica`). No hizo falta ninguna consulta nueva: Administrador usa `ServicioReporte.ObtenerMetricas()`; Recepcionista `ObtenerMetricas()` + `ServicioCheckin.ObtenerHistorial` (últimos 7 días; "accesos de hoy" se calcula de ahí); Técnico `ServicioMaquina.ObtenerTodas()` + `ServicioMantenimiento.ObtenerActivos()`. Las tarjetas son clickeables y llevan a la sección relacionada; los datos se recargan cada vez que se vuelve a Inicio y, si falla la carga, el error se muestra en el lugar de las tarjetas (sin `MessageBox`).

### 9.5 Login (`frmLogin`)

- [x] Layout partido en dos paneles: marca minimalista a la izquierda (fondo `SidebarFondo`) y formulario a la derecha sobre `Superficie`; en ventana angosta se oculta el panel de marca.
- [x] Campos con ícono de línea, mensaje de error inline (en vez de `MessageBox`) y botón principal con estado de carga.
- [x] Reemplazar el emoji del toggle de contraseña (`👁`/`🙈`) en `Tema.AgregarToggleContrasenia` por glifos de `Iconos` (ojo / ojo tachado).
- [x] **Se conserva** el flujo de autenticación tal cual (`ServicioUsuario`, hash SHA256, `Hash.Coincide`): sólo cambia la presentación.

> **Implementado (18/10/2026):** el rediseño vive en `frmLogin.cs` (`ConstruirLayout`/`Reubicar`): reubica por código los controles que ya declara el `.Designer.cs` (mismos nombres), así que el diseñador de Visual Studio seguirá mostrando el layout viejo; es esperable. La ventana pasa a redimensionable (mínimo 480×480) y el panel de marca se oculta por debajo de 680 px de ancho. El toggle de contraseña ya usaba glifos desde la 9.2. Pendiente de revisión visual por el usuario.

### 9.6 Módulos (herencia del nuevo look, de mayor a menor visibilidad)

Para cada pantalla: aplicar `Tema.EstilizarGrilla`, botones por variante, reemplazar emojis por `Iconos`, agrupar filtros/acciones en una barra superior de la pantalla y confirmar que redimensionar la ventana no corta ni solapa controles.

- [x] `frmCheckin` (la pantalla de uso más frecuente en recepción): resultado con estado de color grande (al día / vencido / por vencer), legible a distancia.
- [x] `frmCobroCuota`.
- [x] `frmSocios` y `frmSocioEditor` (+ `ucDatosPersona`).
- [x] `frmUsuarios` y `frmUsuarioEditor`.
- [x] `frmMantenimiento`, `frmHistorialMantenimientos`, `frmMaquinas`, `frmPlanes`.
- [x] `frmHistorialAccesos`, `frmBuscarSocio` (el emoji del botón Buscar se limpia por `Tema`).
- [x] `frmReportes` (tarjetas de métrica coherentes con el dashboard) y `frmBackup`.
- [x] `frmConfiguracion`.

> **Avance (18/10/2026):** además de las pantallas tildadas, `Tema` aplica a **todos** los módulos, de forma central y sin tocar sus diseñadores: botones por variante (con hover), grillas nuevas, `ComboBox` planos, grises de sistema → `TextoSecundario`, y se recorta el emoji inicial de los textos de botón (`SinEmojiInicial`: "💾 Crear copia…" → "Crear copia…"); `btnBuscarPorNombre` pasa a variante secundaria. Detalle de lo hecho a mano: `frmCheckin` (estado neutro + verde/rojo con tokens y un ícono grande de check/cruz sobre el panel de resultado) y `frmReportes` (layout por `Dock` en vez de posiciones fijas, ya no es un diálogo de tamaño fijo, tarjetas = `TarjetaMetrica` igual que el Inicio, barra de filtro con Exportar a la derecha). Todas las pantallas se revisaron después por render (ver 9.7).

### 9.7 Verificación y cierre

- [x] `dotnet build SGIG.slnx` sin errores tras cada subfase (los 3 warnings de nulabilidad de `ServicioBackup`/`frmLogin` son anteriores a la Fase 9).
- [ ] Revisión visual por el usuario en su máquina: 125 % y 150 % de escala de pantalla y monitor chico (1366×768). Ya revisado por render del agente: ventana a 1400 y 1000 px, escala ~114 % (ver nota de render más abajo).
- [ ] Recorrido de cada rol (Administrador / Recepcionista / Técnico) para confirmar que el sidebar muestra sólo lo permitido.
- [ ] Retomar las capturas de pantalla de la Fase 8 **después** de esta fase, para que el Manual de Usuario muestre la UI definitiva.
- [ ] *(Opcional)* modo oscuro con un segundo juego de tokens (`Tema.Aplicar(Modo)`); WinForms en .NET 10 lo soporta de forma experimental, por eso queda fuera del alcance base.

**Orden recomendado:** 9.2 → 9.3 → 9.4 → 9.5 → 9.6 → 9.7. Cada subfase compila y se puede revisar por separado; si el tiempo apremia se puede cortar después de 9.3–9.5 y la app ya se ve rediseñada en lo más visible.

> **Revisión por render (18/10/2026):** como el agente tiene acceso a la base local (usuario `admin` del seed), armó un arnés fuera del repo que dibuja las pantallas a imagen con `Control.DrawToBitmap` (sin abrir ventanas en el escritorio) y las inspeccionó para los 3 roles, a 1400 px y a 1000 px de ancho. Encontró y corrigió: (1) `ucInicio` explotaba con `NullReferenceException` para Recepcionista y Técnico (buscaba columnas por nombre de propiedad en vez de `"col" + propiedad`); (2) orden de apilado `Dock=Top` invertido en Inicio y Reportes (**regla: lo último que se agrega queda más arriba, y el `Fill` va primero/`BringToFront`**); (3) cifras de las tarjetas recortadas; (4) ícono del campo contraseña (ahora candado); (5) etiqueta del sidebar truncada ("Historial de mantenimientos" → "Intervenciones"); (6) botones cuyo texto no entra pasan a una fuente más chica en vez de recortarse; (7) el cuerpo de texto vuelve a `Segoe UI` (la variante *Variable* es más ancha y hacía que los rótulos de los formularios con diseñador pisaran a sus campos).
>
> **Pendiente de verificación humana:** los glifos de los íconos en tu fuente instalada, escala 125 %/150 %, y las pantallas que el render no cubrió (`frmSocioEditor`, `frmUsuarioEditor`, `frmConfiguracion`, `frmBuscarSocio`, `frmPlanes`, `frmMaquinas`, `frmHistorialMantenimientos` se renderizaron sólo en parte). Observaciones menores abiertas: el rótulo "Detalle técnico" de `frmMantenimiento` queda tapado por su campo; las fechas se muestran como `M/d/yyyy` porque la cultura de Windows es en-US (no se fuerza `es-AR`).

> **Segunda ronda de render (18/10/2026)** — se revisaron los 3 roles, los 4 diálogos (`frmSocioEditor`, `frmUsuarioEditor`, `frmConfiguracion`, `frmBuscarSocio`) y el estado concedido/rechazado del Check-in, a 1400 y 1000 px. Correcciones: (1) **cultura `es-AR` forzada** en `Program.cs` (fechas `dd/MM/yyyy`, montos `$ 12.000,00`) y `DateTimePicker` con formato `dd/MM/yyyy` por `Tema`; para que el cobro no dependa de la configuración regional, `frmCobroCuota` ahora escribe el monto con `"0.00"` de la cultura actual y lo lee normalizando `,`→`.` (antes se leía siempre con `InvariantCulture` y habría fallado en una PC con regional es-AR); (2) `Tema.AjustarSolapes` corre el `TextBox` de la misma fila cuando su rótulo, más ancho que lo diseñado, lo tapaba ("Detalle técnico"); (3) `ComboBox` vuelven al estilo estándar (con borde: en `Flat` se veían sin borde); (4) botones: fuente de peso normal como último recurso para que el texto entre, y `frmBuscarSocio` con botones nombrados y anchos que entran; (5) `frmSocioEditor`: el `UserControl` de datos de persona era 36 px más bajo que su propia fila "Fecha nacimiento" (se veía sólo el borde del selector y pisaba "Apto médico"); se agranda y se baja lo de abajo; (6) `frmMaquinas`: la grilla ocupa todo el alto (quedaba en 220 px por el panel de edición oculto); (7) tarjetas del Inicio/Reportes: `PanelTarjetas` con posicionamiento manual — el `FlowLayoutPanel` dejaba las tarjetas apiladas en (0,0) si se recargaban con el panel oculto; (8) `frmMDIParent`: ya no conserva el `MinimumSize` de diseño de cada pantalla (la agrandaba más que el espacio y la cortaba); sólo `frmCobroCuota` (marcada `Rigida`) va en un panel con scroll con su tamaño de diseño; (9) cabecera de grillas sin salto de línea; columnas de `frmUsuarios` reajustadas; ícono del Check-in centrado y más grande.
>
> **Pendiente de verificación humana:** glifos de íconos con tu fuente, escala 125 %/150 % (el render usó la escala de esta máquina, ~114 %), y el flujo real con teclado/mouse (el render no ejecuta clics). Aclaración sobre el arnés: vive fuera del repo (carpeta temporal de la sesión) y usa `Control.DrawToBitmap` con el usuario `admin` del seed; no abre ventanas visibles.

---

## Fase 10 — Pruebas automáticas (agregada 18/10/2026)

> Hasta la Fase 9 el proyecto no tenía tests automáticos. Detalle y convenciones en `docs/patrones/testing.md`.

- [x] Proyecto `SGIG.Tests` (xUnit, `net10.0`) agregado a `SGIG.slnx`, con referencias a Entidades, Datos y Negocio.
- [x] `BaseDeDatosFixture`: crea la base aislada `SGIG_Test` desde `docs/SGIG_CreateDB.sql`, la borra al terminar y aborta si la cadena de conexión no apunta a ella.
- [x] Pruebas unitarias (sin base): validaciones y hash.
- [x] Pruebas de integración: autenticación, usuarios, socios, tesorería, check-in, mantenimiento, reportes, catálogos, backup/restore (87 pruebas en total, todas en verde).
- [x] Verificado que detectan roturas (se rompió a propósito el vencimiento mensual y fallaron las pruebas de cobro; se revirtió).
- [ ] Opcional: pruebas de la capa `SGIG.Datos` por separado (hoy se prueba a través de los servicios) y de reglas que dependan de la fecha del sistema con un reloj inyectable.
- [ ] Opcional: reporte de cobertura y ejecución automática en cada push (CI).

> **Auditoría de consistencia (18/10/2026)** — pasada pantalla por pantalla (login, 6 diálogos incluyendo alta *y* edición de socio/usuario, y las 18 pantallas por rol) con un recorrido automático de **todos los controles**: cortes fuera del contenedor, solapes, texto truncado, fuentes y colores fuera de la paleta, estilo de botones y de `TextBox`; resultado final: **0 hallazgos** (sólo quedan, por diseño, los campos de login con borde propio). Se corrigió: (1) el ojo de mostrar/ocultar contraseña **no se veía** en ningún campo (estaba superpuesto sobre el `TextBox`) y en los diálogos se re-estilizaba como botón azul con fuente de texto; ahora va al costado del campo y `Tema` no lo toca; (2) botones con 7 alturas distintas (25–43 px) → todos de 36 px (`Tema.AlturaBoton`; los de acción grande —login, cobro, backup— quedan aparte); (3) `frmConfiguracion` tenía controles sin nombre (el botón Cerrar salía azul y Guardar no tomaba la variante de éxito) y quedaba justo en alto; (4) `frmBackup` conservaba la paleta y fuentes viejas (Semibold, 9,75/15,75 pt, grises y verdes propios) → normalizado centralmente (`ColorHeredado`, `NormalizarFuente`); `GroupBox` unificados; (5) rótulos que pisaban a su `ComboBox`; (6) botones deshabilitados con estado gris coherente (antes conservaban el color de acción); (7) `ucDatosPersona.BloquearIdentidad` ahora también deshabilita *Buscar*; (8) el botón *Cerrar* de Planes dejaba el área vacía con el ítem del sidebar marcado: toda pantalla que se cierra sola vuelve al **Inicio**; (9) rótulos del Inicio/Reportes/Check-in que se tocaban.
>
> **Escala de fuentes resultante** (una sola, centralizada en `Tema`): cuerpo 9 pt · campos/botones/combos 10 pt · títulos de sección 20 pt (`Segoe UI Variable Display`) · valores de tarjeta 20 pt. Botones: 36 px de alto, variantes primario/éxito/peligro/secundario, deshabilitado gris. Sin colores fuera de la paleta.

---

## Fase 11 — Gráficos en el Inicio según el rol (agregada 25/10/2026)

> **Origen:** pedido del usuario: al iniciar sesión, el Inicio (`ucInicio`) muestra un gráfico representativo del rol, debajo de las tarjetas de métricas. De la propuesta de 3–4 gráficos por rol se eligió **uno por rol**. Es presentación + consultas de solo lectura: no cambia el modelo de datos, los roles ni la ERS.

| Rol | Gráfico | Tipo | Fuente de datos |
|---|---|---|---|
| Administrador | **Ingresos por plan del mes actual** ("¿qué plan deja más plata?") | Barras horizontales, de mayor a menor | `Facturacion.monto_total` por `fecha_emision` (mismo criterio que la tarjeta "Ingresos del mes", así las barras suman lo mismo que la tarjeta) + `Plan.nombre`. Muestra todos los planes activos, aunque estén en 0. |
| Recepcionista | **Accesos de hoy por hora** (hora pico) | Columnas | `Checkin.fecha_hora` del día (concedidos + rechazados, igual que la tarjeta "Accesos de hoy"). Franja fija 06–22 h, ampliada si hay accesos fuera de ella. |
| Técnico | **Mantenimientos por mes** (últimos 6 meses, incluido el actual) | Columnas | `Mantenimiento.fecha_inicio` agrupado por año/mes; los meses sin registros aparecen en 0. |

**Consultas nuevas (documentadas antes de escribirlas, regla de la 9.4):** en `RepositorioReporte` → `ObtenerIngresosPorPlan(desde, hasta)`, `ObtenerAccesosPorHora(dia)`, `ObtenerMantenimientosPorMes(desde, hasta)`; en `ServicioReporte` los métodos homónimos, que validan los parámetros y completan con 0 las horas/meses sin datos. DTOs nuevos en `ReporteDTOs.cs`: `PuntoGraficoDTO` (lo que consume la UI), `AccesosPorHoraDTO`, `MantenimientosPorMesDTO`.

**Dibujo:** control propio `TarjetaGrafico` (`RoundedPanel` + GDI+ en `OnPaint`), sin paquetes NuGet — `System.Windows.Forms.DataVisualization` (el `Chart` clásico) no existe en .NET moderno. Colores sólo de `Tema` (se agrega el token `AcentoSuave`); la barra de mayor valor va en `Acento` y el resto en `AcentoSuave`; tooltip con el valor exacto al pasar el mouse; estado vacío "Sin datos para mostrar".

- [x] DTOs, consultas en `RepositorioReporte` y métodos en `ServicioReporte`.
- [x] Pruebas en `SGIG.Tests/GraficosTests.cs` (los valores coinciden con SQL directo, se completan los huecos, validaciones).
- [x] `TarjetaGrafico` y token `Tema.AcentoSuave`.
- [x] Integración en `ucInicio` (carga en segundo plano junto con las tarjetas; si falla, el gráfico muestra el error en su lugar).

> **Revisión (25/10/2026, sugerencia de la cátedra):** los gráficos pasan del Inicio a una **sección propia** del sidebar, **"Estadísticas › Gráficos"** (`frmGraficos`, armado por código, visible para los 3 roles; cada uno ve los suyos, uno por fila ocupando toda la pantalla). El Inicio vuelve a ser sólo saludo + tarjetas + lista, con un atajo **"Ver gráficos"** para cada rol. El Administrador suma un segundo gráfico: **Ingresos por mes** (columnas, últimos 12 meses incluido el actual, `Facturacion.monto_total` por `fecha_emision`; el último mes coincide con la tarjeta "Ingresos del mes"). Consulta nueva `RepositorioReporte.ObtenerIngresosPorMes`; `MantenimientosPorMesDTO` se generaliza a `TotalPorMesDTO` (sirve para cantidades y montos) y `ServicioReporte` comparte el armado mes por mes (`PorMes`) entre los dos gráficos mensuales. Ícono nuevo `Iconos.Graficos`.

- [x] Sección "Estadísticas › Gráficos" (`frmGraficos`) y atajo "Ver gráficos" en el Inicio.
- [x] Segundo gráfico del Administrador (ingresos por mes) + prueba.
- [ ] Revisión visual por el usuario en su máquina con los 3 roles.

---

## Fase 12 — Perfil del socio (agregada 25/10/2026)

> **Origen:** pedido del usuario: desde la grilla de `frmSocios`, un botón por fila que lleve a un panel de perfil con más información del socio (plan que usa, estado de la cuota, pagos, accesos, datos personales). Es una pantalla de consulta: no cambia el modelo de datos, los roles ni la ERS. Mismos roles que `frmSocios` (Administrador y Recepcionista).

- **Dónde se abre:** columna nueva **"Perfil"** con un botón "Ver perfil" al lado de "Estado" (no reemplaza al estado: un socio dado de baja se sigue distinguiendo); también con doble clic sobre la fila. El perfil se muestra **dentro de la misma pantalla** (`ucPerfilSocio`, panel que tapa la grilla) con "Volver al listado", sin abrir ventanas nuevas (RNF#02) y conservando el filtro de búsqueda.
- **Contenido:** encabezado (iniciales, nombre, documento, estado Activo/Dado de baja, botón "Editar datos" que reutiliza `frmSocioEditor`); 4 tarjetas (`TarjetaMetrica`): **plan actual** (el de la última facturación; si nunca pagó, el plan preferido `Socio.IdPlan`), **cuota** (Al día / Por vencer ≤ 7 días / Vence hoy / Vencida / Sin cuotas, con color), **último pago** y **accesos de los últimos 30 días**; bloque de **datos personales** (tipo y número de documento, edad, email, teléfono, localidad y provincia, apto médico); grillas de **historial de pagos** y **accesos de los últimos 90 días**.
- **Datos:** sin consultas nuevas. `ServicioSocio.ObtenerPerfil(idPersona)` arma un `PerfilSocioDTO` reutilizando `RepositorioSocio.ObtenerPorId`, `ServicioTesoreria.ObtenerHistorialSocio`, `ServicioCheckin.ObtenerHistorial` (filtrado por el `IdPersona` del socio), `ServicioCatalogo` y `ServicioPlan`; calcula ahí el estado de la cuota, la edad y los totales (regla de negocio, no de pantalla).

- **Permisos dentro del perfil (decidido por el usuario, 25/10/2026):** el Administrador ve todo; el **Recepcionista ve el perfil sin la parte económica** (sin la tarjeta "Último pago", sin "Total pagado" y sin el historial de pagos; conserva plan actual, estado de la cuota, datos personales y accesos). El filtro está en la capa de negocio: `ObtenerPerfil(idPersona, idRolSolicitante)` no devuelve pagos ni montos si el rol no es Administrador (`PerfilSocioDTO.IncluyeDatosEconomicos = false`); la pantalla sólo acomoda el layout. `frmSocios` recibe el usuario logueado (`new frmSocios(_usuario)`) para saber el rol.

- [x] `PerfilSocioDTO` y `ServicioSocio.ObtenerPerfil`.
- [x] Pruebas en `SGIG.Tests/PerfilSocioTests.cs`.
- [x] `ucPerfilSocio` y columna "Ver perfil" + doble clic en `frmSocios`.
- [ ] Revisión visual por el usuario en su máquina.

---

## Fase 13 — Inicio "qué tengo que hacer hoy" (agregada 25/10/2026)

> **Origen:** el usuario notó el Inicio "muy blanco y vacío" después de mover los gráficos a su sección. Se rediseña como una pantalla de pendientes del día (los gráficos cubren el "cómo vamos"). Sin cambios en el modelo de datos, los roles ni la ERS.

- **Bloque "Para atender hoy"** (debajo de las tarjetas): avisos de color, clickeables, armados en la capa de negocio por `ServicioAvisos.ObtenerAvisos(idRol, hoy)` → `List<AvisoDTO>` (nivel, texto, categoría):
  - Administrador y Recepcionista: socios activos con la **cuota vencida** (rojo) y cuotas que **vencen en los próximos 7 días** (naranja) → llevan a Socios.
  - Recepcionista: **cumpleaños de hoy** de socios activos (azul; quien nació un 29/02 lo festeja el 28/02 en años no bisiestos).
  - Administrador y Técnico: **máquinas en reparación hace más de 7 días** (mantenimientos sin fecha de fin) → llevan a Máquinas.
  - Sin pendientes: "Todo al día" (verde).
- **Dos listas lado a lado**, cada una en una tarjeta con título:

  | Rol | Izquierda | Derecha |
  |---|---|---|
  | Administrador | Últimos cobros (socio, plan, monto) | Cuotas por vencer (7 días) |
  | Recepcionista | Últimos accesos | Vencen esta semana |
  | Técnico | Mantenimientos en curso (+ días fuera de servicio) | Máquinas más antiguas (por fecha de compra) |
- **Estados vacíos:** si una lista no tiene filas se muestra un ícono gris y un texto ("Todavía no hubo accesos hoy") en vez de la grilla en blanco.
- **Datos:** una sola consulta nueva, `RepositorioTesoreria.ObtenerUltimosPagos(cantidad)` (pagos de todos los socios con el nombre del socio; `Pago` suma la propiedad de grilla `NombreSocio`). El resto se resuelve en negocio sobre lo existente: `ServicioSocio.ObtenerPorVencer(hoy, dias)` y `ServicioSocio.EsCumpleanios` (sobre `ObtenerActivos`), mantenimientos activos y máquinas.

- [x] `AvisoDTO`, `ServicioAvisos`, `ServicioSocio.ObtenerPorVencer`/`EsCumpleanios`, `ServicioTesoreria.ObtenerUltimosPagos`.
- [x] Pruebas en `SGIG.Tests/InicioTests.cs`.
- [x] Rediseño de `ucInicio` (avisos + dos listas en tarjetas + estados vacíos); tokens `AvisoFondo`/`AvisoTexto`/`AcentoFondo` en `Tema`.
- [ ] Revisión visual por el usuario en su máquina con los 3 roles.

---

## Cómo usar este plan con el agente

1. Seguir las fases en el orden en que aparecen; dentro de una fase, las subsecciones también van en orden de dependencia (entidad → datos → negocio → pantalla).
2. No empezar una fase si la anterior no compila.
3. Cuando el usuario diga "seguí con lo que sigue" o "la próxima fase", el agente ubica el primer paso `[ ]` de este archivo y continúa desde ahí.
4. El agente sí compila (`dotnet build SGIG.slnx`) y escribe tanto la clase del formulario como su `.Designer.cs`; lo que no puede es juzgar el resultado *visual* (ver `CLAUDE.md`) — cada pantalla trae su tabla de controles como contrato, y el ajuste fino de layout queda para el diseñador de Visual Studio.
5. Marcar cada paso como hecho (`[x]`) a medida que se entrega. Si el agente no puede editar este archivo en el momento, debe decirle al usuario qué pasos completó para que él los tilde.
