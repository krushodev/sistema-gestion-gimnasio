# SGIG — Sistema de Gestión Integral para Gimnasios

Instrucciones para cualquier agente Claude que trabaje sobre este repositorio. Proyecto de la materia **Taller de Programación II** (Lic. en Sistemas de Información, FACENA). Léase junto con `/docs/ERS_SGIG_v4_0.docx` (Especificación de Requisitos completa: alcance, historias de usuario, RF, RNF, DER, manual de usuario), `/docs/SGIG_CreateDB.sql` (script de base de datos ya validado) y `/docs/Plan_Trabajo_SGIG.md` (el plan de trabajo unificado, de la arquitectura a las pantallas — ver sección "Orden de trabajo" más abajo) — los tres son la fuente de verdad del proyecto y no deben contradecirse sin actualizar primero la ERS.

## Qué es este proyecto

Aplicación de escritorio en **C# / Windows Forms** sobre **.NET moderno (.NET 8/10, proyectos SDK-style)** para administrar un gimnasio de una sola sede: socios, cuotas (con facturación por ciclo), control de acceso, máquinas/mantenimientos y reportes de ingresos. Persistencia en **SQL Server** vía **Dapper** (micro-ORM sobre ADO.NET). Tres roles: Administrador, Recepcionista, Técnico.

### Notas históricas (leer en orden, documentan por qué el proyecto llegó a este estado)

1. **Lenguaje (26/08/2026):** la ERS original (v3.0) exigía Visual Basic .NET porque así lo enseña el material de la cátedra. En la v3.1 se confirmó con la cátedra que C# también está permitido y se actualizó el RNF#08 en consecuencia — ver la tabla de control de versiones del documento. El resto del proyecto (modelo de datos, roles, requisitos funcionales, arquitectura en capas) no cambió por este motivo.
2. **Target .NET (27/08/2026):** al crear la solución en Visual Studio, el asistente generó por defecto un proyecto WinForms SDK-style apuntando a **.NET 10**, no a .NET Framework clásico como se había asumido en un primer momento. Se decidió seguir con .NET moderno en vez de instalar el targeting pack de .NET Framework 4.8, para no depender de componentes viejos del Visual Studio Installer. Esto no cambia el modelo de datos, los roles, los RF/HU ni la arquitectura en 4 capas.
3. **Acceso a datos — Dapper (27/08/2026):** el proyecto pasó de "ADO.NET puro, sin ORM" a usar **Dapper**. Es un cambio deliberado y **no** contradice el espíritu de lo que enseña la cátedra: Dapper no reemplaza el SQL ni esconde `SqlConnection`/`SqlTransaction` — solo elimina el mapeo manual fila-por-fila con `SqlDataReader`. Seguís escribiendo cada consulta a mano, parametrizada, y seguís controlando la transacción explícitamente. Es, además, exactamente lo que usa el repositorio de referencia de la cátedra (ver más abajo), así que hay mucho código real para adaptar. La ERS no necesita actualizarse por esto — no especifica una técnica de acceso a datos, solo pide SQL Server.
4. **Refactor de Tesorería y baja de Gastos (31/08/2026 — ERS v4.0):** se dio de baja el módulo de Gastos completo. Desaparece la tabla `Gasto` y, con ella, RF#22 (ABM de gastos) y RF#23 (el Mantenimiento generaba un gasto asociado); el alta de Mantenimiento queda reducida a dos tablas (`Mantenimiento` + `Maquina.estado`). En su lugar entra `Facturacion`, tabla intermedia entre `Socio`, `Plan` y `Pago`: representa un ciclo de cuota, con su fecha de emisión, su fecha de vencimiento y el monto vigente del plan al momento de emitirla (que no se recalcula si el plan cambia de precio después, RF#13). `Pago` deja de apuntar a Socio+Plan y cuelga de una `Facturacion`, así una factura admite uno o más pagos. `Plan` reemplaza `dias_vigencia` por `tipo_periodicidad` y el vencimiento se calcula por **calendario** (`AddDays(1)`, `AddDays(7)`, `AddMonths(1)`, `AddYears(1)`), no por días fijos, para no arrastrar el desfasaje de los meses de 28/30/31 días (RF#12). `Socio.id_plan` queda como plan preferido (sólo precarga el combo al facturar) y `Socio.fecha_vencimiento_cuota` queda como **caché de lectura rápida para el Check-in** (RNF#01 pide responder en menos de 2 segundos, y leer un campo indexado en `Socio` es más rápido que resolver la `Facturacion` vigente en cada intento); ese caché se actualiza dentro de la misma transacción que registra el pago. El total sigue siendo 14 tablas. El módulo de Gastos se redefine como Reportes, con RF#24 = reporte de ingresos por cuotas filtrado por rango de fechas.

Las primeras tres notas no obligaron a tocar la ERS: el RNF#08 solo pide "C# sobre Visual Studio", sin especificar versión de .NET ni técnica de acceso a datos. La cuarta sí, y por eso la ERS pasó a v4.0 antes de que se tocara una línea de código.

## Arquitectura obligatoria: 4 capas

La cátedra exige arquitectura en capas (ver `Taller_II-material_teorico_completo.pdf` y `Curso-de-introduccion-net-con-visual-basic.pdf`, capítulo de arquitectura cliente-servidor y n-capas — el material está en VB.NET sobre .NET Framework y ADO.NET clásico, pero el concepto de capas es independiente del lenguaje, de la versión de .NET y de si hay o no un micro-ORM de por medio). La solución se organiza en **4 proyectos SDK-style** dentro de `SGIG.slnx`:

```
SGIG.slnx
├── SGIG.Entidades   (Biblioteca de clases, net8.0/net10.0)          — POCOs puros, sin lógica ni SQL
├── SGIG.Datos       (Biblioteca de clases, net8.0/net10.0)          — acceso a datos (DAL), con Dapper
├── SGIG.Negocio     (Biblioteca de clases, net8.0/net10.0)          — reglas de negocio (BLL)
└── SGIG.UI          (Windows Forms App, net8.0-windows/net10.0-windows) — proyecto de inicio
```

`SGIG.Entidades`, `SGIG.Datos` y `SGIG.Negocio` no necesitan el sufijo `-windows` en su TFM porque no usan tipos de WinForms — solo `SGIG.UI` sí. Las 4 tienen que apuntar a la misma versión de .NET entre sí.

Dependencias en un solo sentido: `SGIG.UI → SGIG.Negocio → SGIG.Datos → SGIG.Entidades`. La UI **nunca** llama directamente a `SGIG.Datos`, ni `SGIG.Datos` conoce a `SGIG.Negocio`. Ningún control de formulario debe tener código SQL — eso es una violación de capas.

### Repositorio de referencia

Esta estructura toma como referencia directa el repositorio de otro estudiante con un proyecto similar (no idéntico), disponible como fuente sincronizada del repo `alejandrogimenezescuela-max/Entrega-Practicos` (carpetas `Entities/`, `Data/`, `Business/`, `Sistema Gimnasio/`). Ahora que también nosotros usamos Dapper, ese repo se puede tomar como guía **mucho más directa** que antes — patrones, nombres de métodos y hasta fragmentos de código se pueden adaptar casi literalmente, salvo por dos diferencias que siguen en pie: (1) probablemente apunta a .NET Framework clásico en vez de .NET moderno SDK-style (ver nota histórica #2 — implica los paquetes NuGet de la sección siguiente); y (2) nuestras convenciones de nombres siguen la notación húngara y el español para los formularios/servicios, mientras que el repo de referencia mezcla inglés y español.

Tomar de ese repo, en particular:
- La organización en `Entities/` (`SGIG.Entidades`), `Data/` (`SGIG.Datos`) con **Repository por entidad**, `Business/` (`SGIG.Negocio`) con **Service por módulo**.
- El uso de excepciones propias para errores de negocio (`Data.Exceptions.DuplicateKeyException`, `Business.Exceptions.DuplicateFieldException` o equivalentes en español).
- El hash de contraseña con SHA256 en `Business/UserService.cs` — se puede tomar casi literal, es C# puro con `System.Security.Cryptography`, sin Dapper de por medio.
- La clase estática de conexión (`Data/Connection.cs`) que lee `ConfigurationManager.ConnectionStrings["chain_conecction"]` — mismo patrón que usamos en `Conexion.cs`, ver más abajo.

## Convenciones de código (no negociables)

- **Notación húngara** en todos los controles de formulario, según `Notación.pdf` de la cátedra: `frm` formularios, `txt` TextBox, `btn`/`cmd` botones, `cbo` ComboBox, `chk` CheckBox, `lst` ListBox, `dgv` DataGridView, `lbl` Label, `mnu` menú. Es una convención de nombres de controles, no depende del lenguaje ni del ORM. (RNF#05 de la ERS.)
- **Try…Catch obligatorio** en todo método público de `SGIG.Datos` y en cualquier código de `SGIG.Negocio` que orqueste una transacción. Capturar `SqlException` (namespace `Microsoft.Data.SqlClient`) y relanzarla envuelta en una excepción propia (`AccesoDatosException` o similar). Nunca dejar una excepción de base de datos llegar sin controlar a la UI. (RNF#06.)
- **Parámetros siempre, nunca concatenar SQL.** Con Dapper esto se hace pasando un objeto anónimo (o un DTO) como segundo argumento de `Query`/`Execute`, nunca interpolando strings (ni `$"..."`) con datos de usuario dentro del SQL:
  ```csharp
  const string sql = "SELECT * FROM dbo.Socio WHERE id_persona = @IdPersona";
  var socio = connection.QuerySingleOrDefault<Socio>(sql, new { IdPersona = idPersona });
  ```
- **Contraseñas con hash SHA256**, jamás en texto plano (RNF#11) — usar `System.Security.Cryptography`, igual que hace `Business/UserService.cs` en el repo de referencia (se puede tomar casi literal, esta parte no usa Dapper).
- **Transacciones (`SqlTransaction`)** para cualquier alta que toque más de una tabla: alta de Persona+Socio o Persona+Usuario, alta de Mantenimiento (inserta el mantenimiento y cambia el estado de la Máquina, RF#20), y el registro de un cobro de cuota (crea la `Facturacion`, inserta el `Pago` y actualiza `Socio.fecha_vencimiento_cuota`, RF#11/RF#12). Con Dapper la transacción se abre igual que con ADO.NET puro y se pasa explícitamente a cada llamada:
  ```csharp
  using var connection = Conexion.ObtenerConexionAbierta();
  using var transaction = connection.BeginTransaction();
  try
  {
      var idMantenimiento = connection.ExecuteScalar<int>(sqlInsertMantenimiento, parametros, transaction);
      connection.Execute(sqlUpdateEstadoMaquina, parametrosMaquina, transaction);
      transaction.Commit();
  }
  catch (SqlException ex)
  {
      transaction.Rollback();
      throw new AccesoDatosException("No se pudo registrar el mantenimiento.", ex);
  }
  ```
- **Baja lógica**, nunca DELETE físico, sobre `Socio`, `Usuario`, `Plan`. Confirmar siempre con `MessageBox` Sí/No antes (RNF#03).
- **Cadena de conexión** únicamente en `App.config` del proyecto `SGIG.UI`, leída por una clase estática `Conexion` en `SGIG.Datos` vía `ConfigurationManager` — nunca hardcodeada en un formulario. El repo de referencia hace exactamente esto en `Data/Connection.cs`; se puede tomar igual, solo renombrando la clase/campo si se prefiere en español.
- **MDIParent**: todos los formularios hijos se abren dentro de `frmMDIParent` (equivalente al `Form1`/sidebar del repo de referencia), nunca como ventanas independientes (RNF#02).
- **Repository devuelve entidades o listas de entidades**, nunca `DataSet`/`DataTable` ni tipos de Dapper sin mapear (`dynamic`). Cada método de `SGIG.Datos` tiene una firma clara: `Socio ObtenerPorId(int idPersona)`, `IEnumerable<Socio> ObtenerActivos()`, `int Alta(Socio socio)`, etc.

## Paquetes NuGet necesarios (proyecto SDK-style + Dapper)

Agregar en `SGIG.Datos` (clic derecho sobre el proyecto → *Administrar paquetes NuGet* → pestaña *Examinar* → buscar el nombre exacto → instalar la última versión estable):

- **`Dapper`** — el micro-ORM en sí. Agrega los métodos de extensión `Query<T>`, `QuerySingleOrDefault<T>`, `Execute`, `ExecuteScalar<T>` sobre `IDbConnection`.
- **`Microsoft.Data.SqlClient`** — el proveedor ADO.NET sobre el que corre Dapper (reemplaza a `System.Data.SqlClient`, que no viene incluido en .NET moderno). La API de `SqlConnection`/`SqlTransaction` es la misma de siempre, solo cambia el `using`.
- **`System.Configuration.ConfigurationManager`** — sin este paquete, `ConfigurationManager.ConnectionStrings[...]` no compila en un proyecto SDK-style. `App.config` en `SGIG.UI` sigue siendo el lugar correcto para la cadena de conexión.

No hace falta ningún otro paquete para lo que pide este proyecto — nada de Entity Framework ni Newtonsoft.Json (Dapper no lo necesita para lo que hacemos acá).

## Patrón de Repository esperado

Cada entidad tiene un repositorio en `SGIG.Datos` con esta forma (ejemplo con `Socio`, adaptar nombres de tabla/columnas al DER real):

```csharp
using Dapper;
using Microsoft.Data.SqlClient;
using SGIG.Entidades;

namespace SGIG.Datos
{
    public class RepositorioSocio
    {
        public Socio ObtenerPorDocumento(string documento)
        {
            const string sql = @"
                SELECT p.id_persona AS IdPersona, p.documento AS Documento, p.nombre AS Nombre,
                       s.activo AS Activo, s.fecha_vencimiento_cuota AS FechaVencimientoCuota
                FROM dbo.Persona p
                INNER JOIN dbo.Socio s ON s.id_persona = p.id_persona
                WHERE p.documento = @Documento";

            try
            {
                using var connection = Conexion.ObtenerConexionAbierta();
                return connection.QuerySingleOrDefault<Socio>(sql, new { Documento = documento });
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("Error al buscar el socio por documento.", ex);
            }
        }
    }
}
```

Este es el nivel de detalle que se espera en cada repositorio: SQL explícito y legible (no generado por el ORM), parámetros con objeto anónimo, `Try…Catch` que envuelve la excepción, y devuelve directamente el tipo de `SGIG.Entidades` que corresponde.

## Correspondencia con el DER (DER v6 — fuente en el repo: `/docs/SGIG_CreateDB.sql`)

14 tablas — cada una es una clase en `SGIG.Entidades` con el mismo nombre y los mismos campos (en PascalCase): Provincia, Localidad, TipoDocumento, Rol, MedioPago, Persona, Socio, Usuario, Plan, Facturacion, Pago, Checkin, Maquina, Mantenimiento. La tabla `Gasto` **ya no existe** (dada de baja en la ERS v4.0, junto con RF#22 y RF#23): en su lugar entró `Facturacion`, la tabla intermedia entre `Socio`, `Plan` y `Pago` que representa un ciclo de cuota. `Plan` no tiene `dias_vigencia` sino `tipo_periodicidad` ('Diario' | 'Semanal' | 'Mensual' | 'Anual'), y `Mantenimiento` ya no tiene `id_gasto`. `Socio` y `Usuario` comparten `IdPersona` como clave (especialización de `Persona`, patrón "tabla por subtipo") — no crear un `IdSocio` o `IdUsuario` autoincremental aparte, sería inconsistente con la base ya creada. Como Dapper mapea por nombre de columna/alias a propiedad, cada `SELECT` en `SGIG.Datos` debe usar `AS NombrePropiedad` cuando el nombre de columna en snake_case no coincida con el PascalCase de la propiedad (ver ejemplo del repositorio arriba).

Si en algún momento el modelo de datos necesita cambiar, el cambio se hace primero en la ERS y en el script SQL (ambos en `/docs`), y recién después se refleja en `SGIG.Entidades` y en la base. Nunca al revés.

## Límite importante de este entorno

Un agente Claude puede leer, escribir y editar archivos `.cs` (clases de `Entidades`, `Datos`, `Negocio`, y el código "code-behind" no generado por el diseñador de un formulario), y puede razonar sobre la arquitectura, generar SQL, revisar convenciones y mantener la documentación sincronizada. **No puede compilar ni ejecutar el proyecto de Windows Forms** (requiere Visual Studio con el diseñador de formularios en Windows) **ni diseñar visualmente un formulario** (arrastrar controles, fijar `Anchor`/`Dock`, tamaños) — esto aplica igual en .NET moderno que en .NET Framework, el diseñador de WinForms de Visual Studio 2022+ sigue generando `.Designer.cs`/`.resx` de la misma forma. Para cada formulario nuevo, el agente debe:

1. Dejar en el archivo de la clase (no el `.Designer.cs`) un comentario con la lista de controles necesarios y su nombre en notación húngara, para que el estudiante los agregue con el diseñador de Visual Studio.
2. Escribir el código de los manejadores de eventos (`btnGuardar_Click`, suscripto en el constructor o vía el diseñador) asumiendo esos nombres de control.
3. Nunca inventar o editar directamente un archivo `.Designer.cs` o `.resx` — esos los genera Visual Studio.

Cuando haga falta compilar o correr el proyecto de verdad, eso lo hace el estudiante en su máquina con Visual Studio; el agente prepara el código para que compile a la primera con solo agregar los controles indicados.

## Orden de trabajo

Hay dos documentos que trabajan juntos y que un agente no debe confundir (sólo el primero está en el repo):

- **`/docs/Plan_Trabajo_SGIG.md`** — el plan **unificado** y completo: arranca en la arquitectura (Fase 0), sigue con la infraestructura de UI (Fase 1) y después, fase por fase, cada módulo con su recorrido completo en un solo lugar — entidades → acceso a datos → lógica de negocio → la pantalla correspondiente ya definida (controles en notación húngara, rol de acceso, comportamiento). **Este es el archivo que el agente debe seguir para saber exactamente qué hacer, con qué forma, y en qué orden.**
- **Backlog de Trello** — el complemento del plan: por cada fase/sección, qué tarjeta crear y en qué lista, para que el usuario pueda ir cargando el tablero a la par que avanza el plan. **No está versionado en este repo** (vive fuera, del lado del usuario), así que un agente no debe buscarlo en `/docs` ni asumir que puede leerlo: la única fuente de verdad sobre qué construir es `Plan_Trabajo_SGIG.md`.

Cuando el usuario pida seguir con una fase o tarjeta (por nombre, número de fase, o diciendo "la próxima"), el agente debe: (1) ubicar la fase/sección correspondiente en `Plan_Trabajo_SGIG.md`, (2) encontrar el primer paso sin marcar (`[ ]`), (3) ejecutar los pasos en orden sin saltear ninguno — si el paso es una pantalla, usar exactamente los controles y el comportamiento ya definidos ahí, sin redefinirlos, (4) marcar como hecho (`[x]`) lo que va entregando.

Orden de dependencias entre fases: **Fase 0 (Arquitectura) → Fase 1 (UI base) → Fase 2 (Seguridad) → Fase 3 (Personas) → Fase 4 (Tesorería) → Fase 5 (Control de Acceso) → Fase 6 (Activos) → Fase 7 (Reportes y Backup) → Fase 8 (Documentación)**. No empezar una fase si la anterior no compila y no tiene, como mínimo, su capa de Datos y Negocio terminada — cada módulo depende de que el anterior ya tenga datos reales para trabajar (por ejemplo, Tesorería necesita Socios cargados).

## Skills / herramientas a usar en este proyecto

- **docx**: para mantener actualizada la ERS (`/docs/ERS_SGIG_v4_0.docx`) cada vez que cambie un requisito, una historia de usuario o el DER — nunca editar el docx a mano fuera de este flujo.
- **pdf**: para releer material de la cátedra (`Manual-Visual-Basic-NET.pdf`, `Curso-de-introduccion-net-con-visual-basic.pdf`, `Taller_II-material_teorico_completo.pdf`, `Notación.pdf`) cuando haya dudas de convención o de un tema puntual de ADO.NET/Dapper (recordar traducir los ejemplos de VB.NET a C# al aplicarlos, y que Dapper no está en esos manuales — es una capa fina encima de lo que ahí se enseña).
- No se usan skills de generación de imágenes, presentaciones ni hojas de cálculo para el código en sí — solo para entregables de documentación si la cátedra los pide.
- Ningún otro MCP o conector es necesario para el desarrollo del código; el trabajo de base de datos (SSMS, DbVisualizer) queda fuera del alcance de lo que un agente Claude puede operar directamente en este entorno.
