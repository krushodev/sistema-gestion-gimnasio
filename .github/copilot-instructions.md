# SGIG — instrucciones para GitHub Copilot

- Aplicación de escritorio **C# / Windows Forms sobre .NET 10** (proyectos SDK-style) para gestión de un gimnasio; persistencia en **SQL Server con Dapper** (SQL escrito a mano, nunca Entity Framework).
- **Arquitectura en 4 capas**, dependencias en un solo sentido: `SGIG.UI → SGIG.Negocio → SGIG.Datos → SGIG.Entidades`. La UI nunca llama a `SGIG.Datos` y ningún formulario contiene SQL.
- **Datos:** un repositorio por entidad, SQL en `const string`, parámetros por objeto anónimo (jamás interpolación), alias `AS` para mapear snake_case → PascalCase, `try…catch` de `SqlException` relanzada como `AccesoDatosException`. Conexión sólo vía `Conexion.ObtenerConexionAbierta()` (cadena en `App.config` de `SGIG.UI`).
- **Transacciones `SqlTransaction`** explícitas para toda escritura que toque más de una tabla; baja **lógica**, nunca `DELETE`, sobre `Socio`, `Usuario` y `Plan`. Contraseñas con hash SHA256.
- **Controles en notación húngara** (`frm`, `txt`, `btn`, `cbo`, `dgv`, `lbl`, `mnu`…) y formularios hijos siempre dentro de `frmMDIParent`. No generar ni editar archivos `.Designer.cs` / `.resx`.

**El detalle, con ejemplos de código del propio proyecto, está en [`docs/patrones/`](../docs/patrones/) — leerlo antes de escribir código.** Fuentes de verdad del proyecto: `docs/ERS_SGIG_v3.docx`, `docs/SGIG_CreateDB.sql`, `docs/Plan_Trabajo_SGIG.md`.

**Skills externas** (`.agents/skills/`, compartido con Cursor y Codex): `dotnet-winforms-basics`, `dotnet-agent-gotchas`, `dotnet-csproj-reading`, `dotnet-solution-navigation`, `dotnet-xml-docs`, `csharp-docs`, `sql-code-review`. Son genéricas: ante conflicto con las reglas de arriba o con `docs/patrones/`, mandan estas reglas. Detalle y precedencia en [`docs/patrones/skills-externas.md`](../docs/patrones/skills-externas.md).
