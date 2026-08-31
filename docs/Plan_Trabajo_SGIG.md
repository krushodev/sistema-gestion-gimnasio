# Plan de trabajo unificado — SGIG

Un solo plan, de punta a punta: arranca en la arquitectura, sigue con cada módulo de backend (entidades → datos → negocio) y, en el mismo lugar donde corresponde, con la pantalla completa de ese módulo (controles y comportamiento ya definidos, no hay que inventarlos después). Es la fuente que un agente Claude debe seguir para saber qué construir y en qué orden. `CLAUDE.md` lo referencia desde su sección "Orden de trabajo".

El documento hermano de este es `Cards_Trello_SGIG.md`: por cada entregable de acá abajo, ese archivo te dice qué tarjeta crear y en qué lista, para que puedas ir cargando Trello a la par que el agente (o vos) avanza con el plan.

Convenciones: `[ ]` = pendiente, `[x]` = hecho. Notación húngara según `Notación.pdf` (RNF#05). Cada referencia `RF#`/`RNF#`/`HU-` apunta a la ERS v3.1.

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
  | `mnuGastos` | MenuStrip item | → `mnuGastosAbm`, `mnuReporteBalance`, `mnuBackup` |
  | `lblUsuarioLogueado` | Label (`StatusStrip`) | Nombre y rol activo |
  | `btnCerrarSesion` | ToolStripButton | Vuelve a `frmLogin` |

- [x] Crear `frmMDIParent.cs` — esqueleto (`IsMdiContainer = true`), con este menú, todas las opciones deshabilitadas por ahora.
- [x] Editar `Program.cs` para que `Main` arranque en `frmLogin`.

## Fase 2 — Seguridad: Rol, Usuario, Login funcional, ABM de Usuarios, Tablas paramétricas

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
  | `frmGastos` | ✅ | ❌ | ❌ |
  | `frmReporteBalance` | ✅ | ❌ | ❌ |
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

### 2.6 Pantalla `frmTablasParametricas` (RF#04)

- **Rol de acceso:** Administrador.
- **Se abre desde:** `mnuTablasParametricas`.
- **Controles:** `tabCatalogos` (TabControl) con pestañas `tabRol`, `tabProvincia`, `tabLocalidad`, `tabTipoDocumento`, `tabMedioPago`, cada una con su `dgv`, sus campos de texto/combo y `btnAgregar`/`btnEditar`/`btnDarDeBaja`/`btnCancelar`. La baja de los cinco catálogos es **lógica** (campo `activo`, RF#04 — ERS v3.2), nunca física.

- [x] Crear `frmTablasParametricas` con las 5 pestañas.
- [x] CRUD de Provincia y Localidad.
- [x] CRUD de TipoDocumento.
- [x] CRUD de MedioPago.
- [x] CRUD de Rol (baja lógica; no se puede dar de baja un rol con usuarios activos).

## Fase 3 — Personas: Socios

### 3.1 Entidad

- [ ] `Socio.cs` en `SGIG.Entidades`.

### 3.2 Acceso a datos

- [ ] `RepositorioPersona.cs`: búsqueda por documento (RF#06).
- [ ] `RepositorioSocio.cs`: alta transaccional (Persona + Socio).
- [ ] `RepositorioSocio.cs`: consulta por documento y listado de activos.
- [ ] `RepositorioSocio.cs`: baja lógica.

### 3.3 Lógica de negocio

- [ ] `ServicioSocio.cs`: validación de documento único (RF#09).
- [ ] `ServicioSocio.cs`: validación regex de email/documento (RF#09).

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
  | `dtpFechaNacimiento`, `txtAptoMedico`, `cboPlan`, `lblFechaVencimientoCuota` (solo lectura), `chkActivo` (solo lectura) | — | Datos de Socio |
  | `btnGuardar` / `btnCancelar` | Button | Confirmar o descartar |

- [ ] Crear `frmSocios` con estos controles.
- [ ] Alta reutilizando persona existente si el documento ya está cargado.
- [ ] Edición.
- [ ] Baja lógica con confirmación.

## Fase 4 — Tesorería: Planes y Pagos

### 4.1 Entidades

- [ ] `Plan.cs`, `Pago.cs` en `SGIG.Entidades`.

### 4.2 Acceso a datos

- [ ] `RepositorioPlan.cs`: CRUD.
- [ ] `RepositorioPago.cs`: alta transaccional (actualiza `Socio.fecha_vencimiento_cuota`).
- [ ] `RepositorioPago.cs`: historial por socio (RF#14).

### 4.3 Lógica de negocio

- [ ] `ServicioPlan.cs`: CRUD del catálogo (RF#10).
- [ ] `ServicioPago.cs`: cálculo del nuevo vencimiento sumando `dias_vigencia` (RF#12).
- [ ] `ServicioPago.cs`: no alterar el monto histórico si el plan cambió de precio después (RF#13).

### 4.4 Pantalla `frmPlanes` (RF#10)

- **Rol de acceso:** Administrador. **Se abre desde:** `mnuPlanes`.
- **Controles:** `dgvPlanes`, `txtNombre`, `txtPrecio`, `txtDiasVigencia`, `chkActivo`, `btnNuevo`/`btnGuardar`/`btnEliminar`.

- [ ] Crear `frmPlanes` con ABM completo.

### 4.5 Pantalla `frmPagos` (RF#11, RF#12, RF#13)

- **Rol de acceso:** Recepcionista. **Se abre desde:** `mnuPagos`.
- **Controles:** `txtBuscarDocumento`/`btnBuscar`, `lblNombreSocio`, `lblVencimientoActual`, `cboPlan`, `cboMedioPago`, `txtMonto`, `dtpFechaPago`, `btnRegistrarPago`, `lblNuevoVencimiento`.

- [ ] Crear `frmPagos` — búsqueda de socio por documento.
- [ ] Registro de cobro con selección de plan y medio de pago, actualizando el vencimiento en la misma transacción del alta.

### 4.6 Pantalla `frmHistorialPagos` (RF#14)

- **Rol de acceso:** Administrador, Recepcionista. **Se abre desde:** `mnuHistorialPagos`.
- **Controles:** `txtBuscarDocumento`/`btnBuscar`, `dtpDesde`/`dtpHasta` (filtro opcional), `dgvHistorialPagos`.

- [ ] Crear `frmHistorialPagos` con consulta por socio.

## Fase 5 — Control de Acceso: Check-in

### 5.1 Entidad

- [ ] `Checkin.cs` en `SGIG.Entidades`.

### 5.2 Acceso a datos

- [ ] `RepositorioCheckin.cs`: inserción.
- [ ] `RepositorioCheckin.cs`: consulta rápida por documento.

### 5.3 Lógica de negocio

- [ ] `ServicioCheckin.cs`: lógica Concedido/Rechazado comparando contra `fecha_vencimiento_cuota` (RF#16).

### 5.4 Pantalla `frmCheckin` (RF#15, RF#17, RNF#01)

- **Rol de acceso:** Recepcionista. **Se abre desde:** `mnuCheckin`.
- **Controles:** `txtDocumento` (foco automático, dispara con Enter), `pnlResultado` (verde/rojo), `lblResultado`, `lblNombreSocio`.

- [ ] Crear `frmCheckin` — campo único, sin botones intermedios.
- [ ] Feedback visual verde/rojo y registro automático del intento.
- [ ] Verificar que la respuesta se resuelve en menos de 2 segundos.

## Fase 6 — Activos: Máquinas y Mantenimientos (rol Técnico)

### 6.1 Entidades

- [ ] `Maquina.cs`, `Mantenimiento.cs` en `SGIG.Entidades`.

### 6.2 Acceso a datos

- [ ] `RepositorioMaquina.cs`: CRUD.
- [ ] `RepositorioMantenimiento.cs`: alta transaccional (mantenimiento + estado de máquina + gasto asociado, RF#20/RF#23).
- [ ] `RepositorioMantenimiento.cs`: historial por máquina (RF#21).

### 6.3 Lógica de negocio

- [ ] `ServicioMaquina.cs`: cambio de estado automático.
- [ ] `ServicioMantenimiento.cs`: técnico a cargo tomado del usuario logueado.

### 6.4 Pantalla `frmMaquinas` (RF#18)

- **Rol de acceso:** Administrador, Técnico. **Se abre desde:** `mnuMaquinas`.
- **Controles:** `dgvMaquinas`, `txtMarca`, `txtNombre`, `dtpFechaCompra`, `cboEstado`, `btnNuevo`/`btnGuardar`/`btnEliminar`.

- [ ] Crear `frmMaquinas` con ABM completo.

### 6.5 Pantalla `frmMantenimiento` (RF#19, RF#20, RF#23)

- **Rol de acceso:** Técnico. **Se abre desde:** `mnuMantenimiento`.
- **Controles:** `dgvMantenimientosActivos`, `cboMaquina`, `dtpFechaInicio`, `txtDetalleTecnico`, `txtMontoGasto`, `txtDescripcionGasto`, `btnRegistrar`, `dtpFechaFin`, `btnFinalizar`.

- [ ] Crear `frmMantenimiento` — alta transaccional (mantenimiento + estado "En Reparación" + gasto).
- [ ] Finalización: cierra el mantenimiento y devuelve la máquina a "Operativa", estado visible en la grilla.

### 6.6 Pantalla `frmHistorialMantenimientos` (RF#21)

- **Rol de acceso:** Administrador, Técnico. **Se abre desde:** `mnuHistorialMantenimientos`.
- **Controles:** `cboMaquina`, `dgvHistorialMantenimientos`.

- [ ] Crear `frmHistorialMantenimientos` con consulta por máquina.

## Fase 7 — Gastos y Reportes

### 7.1 Entidad

- [ ] `Gasto.cs` en `SGIG.Entidades`.

### 7.2 Acceso a datos

- [ ] `RepositorioGasto.cs`: CRUD.

### 7.3 Lógica de negocio

- [ ] `ServicioGasto.cs`: CRUD de gastos (RF#22).
- [ ] `ServicioReporte.cs`: balance (pagos − gastos) filtrado por rango de fechas (RF#24).

### 7.4 Pantalla `frmGastos` (RF#22)

- **Rol de acceso:** Administrador. **Se abre desde:** `mnuGastosAbm`.
- **Controles:** `dgvGastos`, `dtpFecha`, `txtMonto`, `txtDescripcion`, `txtComprobante`, `btnNuevo`/`btnGuardar`/`btnEliminar`.

- [ ] Crear `frmGastos` con ABM completo.

### 7.5 Pantalla `frmReporteBalance` (RF#24)

- **Rol de acceso:** Administrador. **Se abre desde:** `mnuReporteBalance`.
- **Controles:** `dtpDesde`/`dtpHasta`, `btnGenerar`, `lblTotalPagos`/`lblTotalGastos`/`lblBalance`, `dgvDetalle`.

- [ ] Crear `frmReporteBalance` con cálculo y filtro por fechas.

### 7.6 Pantalla `frmBackup` (RF#25)

- **Rol de acceso:** Administrador. **Se abre desde:** `mnuBackup`.
- **Controles:** `txtRutaArchivo`, `btnSeleccionarRuta`, `btnBackup`, `btnRestore` (con confirmación previa), `lblEstado`.

- [ ] Crear `frmBackup`.
- [ ] `ServicioBackup.cs`: `BACKUP DATABASE`.
- [ ] `ServicioBackup.cs`: `RESTORE DATABASE` con confirmación `MessageBox` Sí/No previa.

## Fase 8 — Documentación y Entrega

- [ ] Capturas de pantalla por módulo (Seguridad, Personas, Tesorería, Control de Acceso, Activos, Gastos) — las toma el usuario una vez armado cada formulario.
- [ ] Insertar capturas en el Manual de Usuario (Anexo A de la ERS).
- [ ] Completar integrantes del grupo en la portada de la ERS.
- [ ] Cargar datos de prueba (socios, planes, máquinas) para la demo.
- [ ] Probar flujo completo — rol Administrador.
- [ ] Probar flujo completo — rol Recepcionista.
- [ ] Probar flujo completo — rol Técnico.
- [ ] Preparar el guion de la presentación/demo para la cátedra.

---

## Cómo usar este plan con el agente

1. Seguir las fases en el orden en que aparecen; dentro de una fase, las subsecciones también van en orden de dependencia (entidad → datos → negocio → pantalla).
2. No empezar una fase si la anterior no compila.
3. Cuando el usuario diga "seguí con lo que sigue" o "la próxima fase", el agente ubica el primer paso `[ ]` de este archivo y continúa desde ahí.
4. El agente sí compila (`dotnet build SGIG.slnx`) y escribe tanto la clase del formulario como su `.Designer.cs`; lo que no puede es juzgar el resultado *visual* (ver `CLAUDE.md`) — cada pantalla trae su tabla de controles como contrato, y el ajuste fino de layout queda para el diseñador de Visual Studio.
5. Marcar cada paso como hecho (`[x]`) a medida que se entrega. Si el agente no puede editar este archivo en el momento, debe decirle al usuario qué pasos completó para que él los tilde.