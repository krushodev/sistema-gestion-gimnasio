# Patrón: identidad visual consistente (`Tema.cs`)

> **Decisión (16/09/2026):** hasta esta fecha convivían dos estilos visuales: `frmLogin` y
> `frmMDIParent` tenían una paleta armada a mano (slate oscuro + azul, `"Segoe UI"`, botones
> planos, tarjetas), mientras que el resto de las pantallas (ABM de Socios, Usuarios, etc.) eran
> WinForms por defecto sin color ni fuente propios. `SGIG.UI/Tema.cs` centraliza esa paleta para
> que toda la app se vea como un solo sistema.

## Por qué así y no de otra forma

Un agente Claude **no puede editar `.Designer.cs`** (ver "Límite importante de este entorno" en
`CLAUDE.md`) ni verificar visualmente cómo queda un formulario — no hay forma de abrir el
diseñador de WinForms ni de renderizar la app. Eso descarta cualquier enfoque que dependa de
reposicionar controles o cambiarles la fuente a mano en cada pantalla (el tamaño/`Location` de
cada control en el `.Designer.cs` está calculado para una fuente y un tamaño de texto puntuales;
cambiarlos sin poder ver el resultado arriesga solapar o cortar controles). Por eso `Tema.cs` se
limita a dos tipos de cambio, elegidos porque **no dependen del layout interno de cada
formulario**:

1. **Pintura pura sobre controles ya existentes** (color de fondo, `FlatStyle` de botones, colores
   de grilla) — nunca `Size`, `Location`, `Anchor`, `Dock` ni `Font` de un control que ya estaba
   ahí. `Tema.EstilizarControles(Control raiz)` recorre el árbol de controles y aplica esto
   recursivamente.
2. **Una barra de cabecera consistente que vive afuera de cada formulario hijo**, no adentro:
   `frmMDIParent.AbrirFormularioEnPanel` ya envolvía cada pantalla embebida con una barra
   "Volver al Inicio"; ahora esa misma barra muestra también el título de la sección. Como la
   barra es hermana del formulario hijo dentro de `pnlContenedor` (no un control insertado dentro
   del propio formulario), agregarle contenido no puede solapar nada del `.Designer.cs` ajeno.

## Cómo se aplica

- **Pantallas de listado** (`frmSocios`, `frmUsuarios`, `frmPlanes`, `frmMaquinas`,
  `frmMantenimiento`, `frmHistorialMantenimientos`, `frmCheckin`, `frmCobroCuota`): se tematizan
  **una sola vez**, centralizado en `frmMDIParent.AbrirFormularioEnPanel`, que llama
  `Tema.EstilizarFormulario(hijo)` + `Tema.EstilizarControles(hijo)` antes de mostrarlas. No hace
  falta tocar cada pantalla individualmente.
- **Formularios modales** (`frmSocioEditor`, `frmUsuarioEditor`) no pasan por
  `AbrirFormularioEnPanel` (se abren con `ShowDialog`), así que cada uno llama
  `Tema.EstilizarFormulario(this); Tema.EstilizarControles(this);` al final de su propio
  constructor (en el `.cs`, nunca en el `.Designer.cs`).
- **`frmLogin` y `frmMDIParent` no usan `Tema`** — son la referencia de la que salió la paleta, no
  el objetivo del cambio.

## Convención para pantallas nuevas

Todo formulario modal nuevo agrega esas dos líneas al final de su constructor, después de
`InitializeComponent()`. Toda pantalla de listado nueva que se abra desde
`frmMDIParent.CargarTarjetasSegunRol` se tematiza sola, siempre que se abra vía
`AbrirFormularioEnPanel(nuevoFormulario, "título de la sección")` — no hace falta agregar nada en
el formulario en sí.

`Tema.ColorSegunNombre` elige el color de cada botón por convención de nombre (notación húngara,
ver [notacion-hungara.md](notacion-hungara.md)): `btnEliminar`/`btnDarDeBaja`/`btnRechazar` →
rojo peligro, `btnCancelar`/`btnCerrar`/`btnVolver` → gris neutro, `btnGuardar`/`btnRegistrar*`/
`btnConfirmar` → verde éxito, cualquier otro → azul primario. Si un botón nuevo necesita otro
color, nombralo siguiendo esta convención en vez de hardcodear un color aparte.
