# Catálogos seed-only (sin ABM en la UI)

> **Decisión (16/09/2026):** Provincia, Localidad, TipoDocumento, MedioPago, Rol, Plan y Maquina
> dejaron de tener pantallas de alta/edición/baja. Vienen cargados con la aplicación via el seed
> de `docs/SGIG_CreateDB.sql` (sección 9) y sólo se consultan desde la UI. `frmTablasParametricas`
> (que administraba Rol/Provincia/Localidad/TipoDocumento/MedioPago) se eliminó por completo, y
> `frmPlanes`/`frmMaquinas` se recortaron a solo lectura (sin `btnNuevo`/`btnGuardar`/`btnEliminar`
> visibles). Ver la nota correspondiente en `Plan_Trabajo_SGIG.md` (Fases 2.6, 4.4 y 6.4).

**Por qué:** son catálogos chicos y estables para un gimnasio real (provincias/localidades del
país, tipos de documento, medios de pago habituales, los 3 roles del sistema, el cuadro de planes
y el parque de máquinas) — no aportaba valor mantener un ABM completo (con sus validaciones,
bloqueos de baja por FK, etc.) para datos que en la práctica casi no cambian y que, cuando
cambian, es una tarea puntual de base de datos, no una operación diaria de Recepción/Administración.

**Qué significa en la capa de datos/negocio:** `RepositorioCatalogo`, `RepositorioRol`,
`RepositorioPlan` y `RepositorioMaquina` (y sus `Servicio*` correspondientes) sólo exponen métodos
`Obtener*`/`Listar` — nunca `Alta`/`Modificar`/`Eliminar`/`BajaLogica` para estas 7 entidades. Si
en el futuro alguna necesita volver a tener ABM, el repositorio/servicio es el lugar donde
agregarlo (siguiendo `repository-dapper.md`), y recién ahí tendría sentido una pantalla nueva.

**Qué significa en la UI:**

- `frmPlanes` y `frmMaquinas` conservan su pantalla (grilla de consulta) porque siguen siendo
  útiles para que Recepción vea tarifas vigentes o Técnico vea el estado del parque de máquinas.
  Sus botones de ABM (`btnNuevo`, `btnGuardar`/`btnEditar`, `btnEliminar`/`btnDarDeBaja`) siguen
  declarados en el `.Designer.cs` — **nunca se edita ese archivo** — pero se ocultan
  (`Visible = false`) en el `Load` de la pantalla, y sus métodos `_Click` quedan con el cuerpo
  vacío (el evento sigue enganchado desde el Designer, pero no hace nada).
- El resto de los catálogos (Provincia, Localidad, TipoDocumento, MedioPago, Rol) no tienen
  ninguna pantalla dedicada: se consumen únicamente como combos de otras pantallas (alta de
  Persona/Socio/Usuario, cobro de cuota, etc.), vía los métodos `Obtener*` que sí sobreviven.

**Si hace falta agregar/editar un dato de catálogo:** se hace directo en
`docs/SGIG_CreateDB.sql` (agregando la fila al `INSERT` correspondiente de la sección 9) y
recreando la base local, no desde la aplicación.
