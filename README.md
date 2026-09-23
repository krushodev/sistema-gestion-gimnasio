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
   - El script crea la base `SGIG` si no existe, las 14 tablas del DER, y siembra los catálogos (Provincia, Localidad, TipoDocumento, MedioPago, Rol, Plan, Maquina) más un usuario administrador inicial.
   - **Usuario admin sembrado:** `admin` / `admin1234` (rol Administrador). Cambiarlo desde `frmUsuarios` antes de un uso real.
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
sqlcmd -S "(localdb)\MSSQLLocalDB" -d SGIG -Q "SELECT (SELECT COUNT(*) FROM sys.tables) AS Tablas, (SELECT COUNT(*) FROM dbo.Usuario) AS Usuarios, (SELECT nombre_usuario FROM dbo.Usuario) AS AdminUser, (SELECT COUNT(*) FROM dbo.[Plan]) AS Planes, (SELECT COUNT(*) FROM dbo.Maquina) AS Maquinas, (SELECT COUNT(*) FROM dbo.Localidad) AS Localidades" -W
```
Debería devolver: `Tablas = 14`, `Usuarios = 1` (`admin`), `Planes = 4`, `Maquinas = 41`, `Localidades = 103`.

## Resetear la base desde cero

El script `SGIG_CreateDB.sql` **no es idempotente**: si la base `SGIG` ya existe con las tablas creadas, volver a correrlo falla (`There is already an object named '...'`). Para simular exactamente el escenario de "bajar el proyecto por primera vez" (por ejemplo, para validar que el paso 3 de arriba funciona sin pasos ocultos):

1. Corré **`docs/SGIG_ResetDB.sql`** — dropea la base `SGIG` por completo (todas las tablas y datos).
2. Volvé a correr **`docs/SGIG_CreateDB.sql`** — recrea el esquema y vuelve a cargar los seeders (catálogos + usuario `admin`/`admin1234`).

Por línea de comandos con `sqlcmd` (ajustar `-S` a tu instancia):

```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -i "docs\SGIG_ResetDB.sql"
sqlcmd -S "(localdb)\MSSQLLocalDB" -i "docs\SGIG_CreateDB.sql"
```

Este ciclo (reset + create) ya fue probado contra una instancia LocalDB local: el reset dropea la base sin errores y el create vuelve a dejar las 14 tablas y los seeders (catálogos + admin) en el mismo estado que una instalación nueva.

Usar solo en entornos de desarrollo/pruebas — el reset elimina todos los datos sin confirmación adicional.

## Estructura del proyecto

Ver la sección "Arquitectura obligatoria: 4 capas" en `CLAUDE.md`: `SGIG.Entidades` → `SGIG.Datos` → `SGIG.Negocio` → `SGIG.UI`, dependencias en un solo sentido.
