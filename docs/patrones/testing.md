# Patrón: pruebas automáticas (`SGIG.Tests`)

> **Agregado (18/10/2026):** hasta esta fecha el proyecto no tenía ningún test automático. `SGIG.Tests`
> (xUnit, `net10.0`) cubre las capas **Entidades/Datos/Negocio**; la UI (WinForms) no se prueba
> automáticamente — se revisa a mano (ver Fase 9 del plan).

## Cómo correrlas

```
dotnet test SGIG.Tests          # o desde Visual Studio: Prueba > Explorador de pruebas
```

Requiere SQL Server LocalDB (el mismo que usa la app). No hace falta preparar nada: cada corrida
**crea** una base nueva, **corre** las pruebas y **la borra**.

## Por qué no tocan los datos reales

- `SGIG.Tests/App.config` apunta a la base **`SGIG_Test`**, nunca a `SGIG`.
- `BaseDeDatosFixture` la crea a partir de `docs/SGIG_CreateDB.sql` (reemplazando el nombre) y la
  elimina al terminar. Si la cadena de conexión no apunta a `SGIG_Test`, aborta **antes** de ejecutar
  cualquier cosa (guarda de seguridad).
- El host de pruebas (`testhost`) no lee el `App.config` del proyecto: `ConfiguracionDePruebas`
  (inicializador de módulo) le indica `APP_CONFIG_FILE` para que `Conexion` use la cadena de pruebas.
- Como la base se arma con el script oficial, **las pruebas también verifican que el script funciona**.

## Qué se prueba

| Archivo | Alcance |
|---|---|
| `Unitarias.cs` | Validaciones (DNI, email, teléfono, documento genérico) y hash SHA256 — sin base de datos |
| `AutenticacionYUsuariosTests.cs` | Login (credenciales malas, inyección SQL, baja/reactivación), alta de usuario, hash en base, cambio de contraseña, no auto-baja, atomicidad |
| `SociosTests.cs` | Alta, duplicados, datos inválidos, modificar, búsqueda, baja lógica (no borra), reactivar, reutilizar Persona (RF#06), atomicidad |
| `TesoreriaYCheckinTests.cs` | Cobro (Facturación + Pago, vencimiento por calendario, cobro adelantado, monto editable, errores, atomicidad) y Check-in (concedido/rechazado, registro RF#17, vencimiento hoy, baja, tiempo < 2 s RNF#01, historial) |
| `ActivosReportesYBackupTests.cs` | Mantenimiento (estado de la máquina atómico), reportes vs. SQL, métricas, catálogos, backup y restore |
| `GraficosTests.cs` | Gráficos del Inicio (Fase 11): ingresos por plan, accesos por hora y mantenimientos por mes vs. SQL, huecos completados en 0, validaciones |

## Convenciones para agregar pruebas

- Toda clase que use la base lleva `[Collection(ColeccionBaseDeDatos.Nombre)]` y recibe
  `BaseDeDatosFixture` en el constructor: comparten **una** base y corren en serie.
- **Cada prueba crea sus propios datos** (`Datos.NuevoSocio()`, `Datos.Doc()` da DNI únicos) y no
  depende de lo que dejó otra: el orden de ejecución no está garantizado, y la prueba de *restore*
  revierte la base al punto del backup.
- No depender de fechas del seed (son fijas, no relativas a hoy): usar `Datos.FijarVencimiento`.
- Una regla de negocio nueva = una prueba nueva. Se comprobó que las pruebas detectan roturas
  (cambiando el vencimiento mensual de 1 a 2 meses fallan exactamente las pruebas de cobro).
