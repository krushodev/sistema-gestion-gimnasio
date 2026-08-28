# Skills externas instaladas (de skills.sh)

Skills de terceros vendorizadas en el repo, elegidas de [skills.sh](https://www.skills.sh/) por encajar
con el stack real de SGIG (WinForms de escritorio + Dapper + SQL Server + capas). Se instalan en dos
carpetas porque cada agente busca en la suya:

- **`.agents/skills/`** — directorio compartido: lo leen **Cursor, GitHub Copilot, Codex, Gemini/Antigravity** y otros.
- **`.claude/skills/`** — lo lee **Claude Code**, que no soporta `.agents/`.

Es la única duplicación deliberada del repo, y es contenido *upstream*, no convenciones nuestras:
las convenciones de SGIG siguen viviendo una sola vez en el resto de `docs/patrones/`.

## Qué está instalado y para qué

| Skill | Origen | Para qué sirve acá |
|---|---|---|
| `dotnet-winforms-basics` | `wshaddix/dotnet-skills` | WinForms sobre .NET 8+/10: alto DPI (`PerMonitorV2`), `ApplicationConfiguration.Initialize()`, plantillas SDK-style, errores típicos. Es el material más cercano a `frmMDIParent` y los ABM. |
| `dotnet-agent-gotchas` | `wshaddix/dotnet-skills` | Catálogo de errores que cometen los agentes al escribir C#: APIs deprecadas, NuGet, `.Result`/`.Wait()`. Red de seguridad, no hay que leerla entera. |
| `dotnet-csproj-reading` | `wshaddix/dotnet-skills` | Leer y modificar `.csproj` SDK-style sin romperlos: `PropertyGroup`, `ItemGroup`, `ProjectReference`, `PackageReference`. Útil al tocar referencias de proyecto y paquetes sin romper el build. |
| `dotnet-solution-navigation` | `wshaddix/dotnet-skills` | Orientarse en una solución: `.sln`/`.slnx`, punto de entrada, grafo de dependencias entre proyectos. |
| `dotnet-xml-docs` | `wshaddix/dotnet-skills` | Comentarios `///` en entidades, repositorios y servicios: `<summary>`, `<param>`, `<exception>`, `<inheritdoc>`. |
| `csharp-docs` | `github/awesome-copilot` | Versión corta y opinada de lo mismo, en modo checklist. |
| `sql-code-review` | `github/awesome-copilot` | Revisión de SQL (cubre SQL Server) centrada en inyección y anti-patrones. Refuerza la regla dura del proyecto: **parámetros siempre, nunca concatenar**. |

## Precedencia: las convenciones de SGIG ganan siempre

Estas skills son genéricas y en varios puntos **contradicen** decisiones ya tomadas para este proyecto.
Ante conflicto manda `CLAUDE.md` + el resto de `docs/patrones/`. En concreto:

- `dotnet-winforms-basics` propone **Generic Host + inyección de dependencias** y modo oscuro. Acá **no** se usa:
  la instanciación es directa y la navegación es por `frmMDIParent` (RNF#02).
- Varias skills asumen **ASP.NET Core, EF Core o async/await** en todas partes. SGIG es escritorio, con
  **Dapper síncrono** y SQL escrito a mano — ver [repository-dapper.md](repository-dapper.md).
- Ninguna conoce la **notación húngara** ni el español en los nombres de controles: eso lo fija
  [notacion-hungara.md](notacion-hungara.md) y no se negocia (RNF#05).
- `dotnet-xml-docs` / `csharp-docs` empujan documentar **todo** lo público; acá alcanza con las
  entidades, los métodos de repositorio y los servicios.

## Reinstalar o actualizar

```bash
npx skills add wshaddix/dotnet-skills \
  -s dotnet-winforms-basics -s dotnet-agent-gotchas -s dotnet-xml-docs \
  -s dotnet-csproj-reading -s dotnet-solution-navigation \
  -a claude-code -a cursor -a github-copilot --copy -y

npx skills add github/awesome-copilot -s sql-code-review -s csharp-docs \
  -a claude-code -a cursor -a github-copilot --copy -y
```

**Descartadas a propósito:** `dotnet-data-access-strategy` (la decisión EF vs Dapper ya está tomada),
`exception-handling` (es de ASP.NET Core Razor Pages, no aplica), `efcore-patterns` y todo el bloque
`dotnet-aspire`/`blazor`/`maui`/`test` (fuera de alcance), `csharp-coding-standards` (choca de frente
con la notación húngara de la cátedra).
