# sistema-gestion-gimnasio

Sistema de Gestión Integral para Gimnasios (SGIG) — C# / WinForms sobre .NET moderno, SQL Server + Dapper. Ver `CLAUDE.md` para la arquitectura y convenciones del proyecto, y `docs/Plan_Trabajo_SGIG.md` para el estado de avance por fase.

## Requisitos

- Visual Studio 2022+ (con carga de trabajo ".NET desktop development", para el diseñador de WinForms).
- .NET SDK 8 o 10 (el mismo que usa `SGIG.slnx`).
- SQL Server o SQL Server LocalDB (viene con Visual Studio). La cadena de conexión por defecto en `SGIG.UI/App.config` apunta a `(localdb)\MSSQLLocalDB`.

## Puesta en marcha (primera vez)

1. **Cloná el repo** y abrí `SGIG.slnx` en Visual Studio.
2. **Restaurá los paquetes NuGet** (Visual Studio lo hace solo al abrir la solución; si no, clic derecho en la solución → *Restaurar paquetes NuGet*).
3. **Creá la base de datos y cargá los seeders** corriendo `docs/SGIG_CreateDB.sql` contra tu instancia de SQL Server. Dos formas equivalentes:
   - **SSMS / Azure Data Studio / extensión SQL Server de VS Code:** abrir el archivo y ejecutar (F5).
   - **Línea de comandos con `sqlcmd`** (instalado junto con SQL Server / LocalDB; probar `sqlcmd -?` para confirmar que está en el PATH):
     ```powershell
     sqlcmd -S "(localdb)\MSSQLLocalDB" -i "docs\SGIG_CreateDB.sql"
     ```
   - El script crea la base `SGIG` si no existe, las 14 tablas del DER, siembra los catálogos (Provincia, Localidad, TipoDocumento, MedioPago, Rol, Plan, Maquina) y carga datos de prueba para poder probar la app de entrada: 1 usuario administrador, 2 Recepcionistas, 2 Técnicos, y 10 Socios (5 con cuota al día, 5 con cuota vencida, cada uno con su Facturación y Pago) para poder ver el flujo de Check-in concedido/rechazado sin cargar nada a mano.
   - **Usuario admin sembrado:** `admin` / `admin1234` (rol Administrador).
   - **Personal sembrado (rol Recepcionista/Técnico):** `aibarra`, `cmedina`, `lferreyra`, `dparedes` — todos con contraseña `Gimnasio2026!`.
   - Cambiar estas contraseñas desde `frmUsuarios`/`frmConfiguracion` antes de un uso real.
4. **Verificá que `SGIG.UI/App.config` apunte a tu instancia.** Por defecto:
   ```xml
   <add name="SGIG"
        connectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SGIG;Integrated Security=True"
        providerName="Microsoft.Data.SqlClient" />
   ```
   Si usás una instancia con nombre distinto (ej. SQL Server Express, `.\SQLEXPRESS`) o autenticación SQL en vez de Windows, editá `Data Source` / `Integrated Security` acá.
5. **Corré la app.** Con `SGIG.UI` como proyecto de inicio, ejecutá (F5). Iniciá sesión con `admin` / `admin1234`.

### Verificación rápida de que los seeders cargaron bien

```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -d SGIG -Q "SELECT (SELECT COUNT(*) FROM sys.tables) AS Tablas, (SELECT COUNT(*) FROM dbo.Usuario) AS Usuarios, (SELECT COUNT(*) FROM dbo.Socio) AS Socios, (SELECT COUNT(*) FROM dbo.[Plan]) AS Planes, (SELECT COUNT(*) FROM dbo.Maquina) AS Maquinas, (SELECT COUNT(*) FROM dbo.Localidad) AS Localidades" -W
```
Debería devolver: `Tablas = 14`, `Usuarios = 5` (admin + 2 Recepcionistas + 2 Técnicos), `Socios = 10`, `Planes = 4`, `Maquinas = 49`, `Localidades = 103`. Este mismo ciclo (reset + create) se probó contra una instancia LocalDB real el 23/09/2026: compiló sin errores (`dotnet build SGIG.slnx`) y los 10 Socios quedaron 5 con cuota al día / 5 vencida según la fecha del sistema, listos para probar el Check-in sin cargar nada a mano.

## Resetear la base desde cero

El script `SGIG_CreateDB.sql` **no es idempotente**: si la base `SGIG` ya existe con las tablas creadas, volver a correrlo falla (`There is already an object named '...'`). Para simular exactamente el escenario de "bajar el proyecto por primera vez" (por ejemplo, para validar que el paso 3 de arriba funciona sin pasos ocultos):

1. Corré **`docs/SGIG_ResetDB.sql`** — dropea la base `SGIG` por completo (todas las tablas y datos).
2. Volvé a correr **`docs/SGIG_CreateDB.sql`** — recrea el esquema y vuelve a cargar los seeders (catálogos + usuarios + socios de prueba, ver arriba).

Por línea de comandos con `sqlcmd` (ajustar `-S` a tu instancia):

```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -i "docs\SGIG_ResetDB.sql"
sqlcmd -S "(localdb)\MSSQLLocalDB" -i "docs\SGIG_CreateDB.sql"
```

Usar solo en entornos de desarrollo/pruebas — el reset elimina todos los datos sin confirmación adicional.

## Estructura del proyecto

Ver la sección "Arquitectura obligatoria: 4 capas" en `CLAUDE.md`: `SGIG.Entidades` → `SGIG.Datos` → `SGIG.Negocio` → `SGIG.UI`, dependencias en un solo sentido.
