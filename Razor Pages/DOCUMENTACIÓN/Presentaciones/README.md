# Presentaciones sobre Razor Pages e Identity en .NET 10

Esta carpeta contiene tres presentaciones didácticas para introducir el desarrollo de aplicaciones web con ASP.NET Core, Razor Pages, Entity Framework Core e Identity en .NET 10.

Los materiales están organizados para poder utilizarlos tanto en explicaciones de aula como en sesiones de trabajo guiado. Las dos primeras presentaciones desarrollan Razor Pages con distinto nivel de profundidad y la tercera aborda la autenticación, la autorización y la gestión de cuentas mediante ASP.NET Core Identity.

## Contenido de la carpeta

| Documento | Extensión | Nivel | Finalidad principal |
| --- | ---: | --- | --- |
| [Razor Pages en .NET 10: introducción](razor_pages_introduccion.pdf) | 17 diapositivas | Inicial | Comprender cómo colaboran una URL, una vista `.cshtml` y una clase `PageModel`. |
| [Razor Pages en ASP.NET Core](Razor%20Pages%20en%20ASP.NET.pdf) | 50 diapositivas | Inicial-intermedio | Aprender Razor Pages paso a paso mediante una agenda de teléfonos y una lista de tareas con SQLite. |
| [Identity en .NET 10](Identity_NET10.pdf) | 35 diapositivas | Intermedio | Incorporar cuentas, cookies, roles, claims, políticas y protección de páginas. |

## 1. Razor Pages en .NET 10: introducción

Presentación breve y visual que explica la arquitectura fundamental de Razor Pages. Está pensada para una primera toma de contacto, antes de comenzar a escribir una aplicación completa.

### Contenidos principales

- Organización de una aplicación alrededor de páginas.
- Relación entre ruta, archivo `.cshtml` y clase `PageModel`.
- Recorrido de una petición GET desde la URL hasta la respuesta HTML.
- Configuración mínima de Razor Pages en `Program.cs`.
- Directivas `@page` y `@model`.
- Inserción de expresiones C# dentro del HTML.
- Responsabilidades del `PageModel`.
- Diferencias entre los manejadores `OnGet` y `OnPost`.
- Model binding mediante parámetros y propiedades.
- Uso de `[BindProperty]`.
- Validación con anotaciones y `ModelState`.
- Inyección de dependencias.
- Programación asíncrona con `async` y `await`.
- Organización habitual de un CRUD.
- Buenas prácticas para mantener páginas claras y seguras.

### Cuándo utilizarla

- Como presentación inicial de una unidad sobre Razor Pages.
- Para explicar la arquitectura antes de abrir un proyecto en el editor.
- Como repaso rápido antes de comenzar un CRUD.
- Como resumen previo a la presentación de 50 diapositivas.

## 2. Razor Pages en ASP.NET Core

Curso de iniciación formado por 50 diapositivas. Desarrolla los conceptos desde cero y los aplica en dos proyectos completos: una agenda de teléfonos almacenada en memoria y una lista de tareas persistida mediante Entity Framework Core y SQLite.

### Bloques del curso

1. **Fundamentos de la web en .NET**
   - ASP.NET Core y Kestrel.
   - Peticiones HTTP y canalización de middleware.
   - Qué es Razor Pages.
   - Comparación con MVC y Blazor.
   - Arquitectura y patrón Page Model.

2. **Primera aplicación**
   - Creación y ejecución del proyecto.
   - Estructura de carpetas.
   - `Program.cs`.
   - `_ViewImports.cshtml`, `_ViewStart.cshtml` y `_Layout.cshtml`.
   - Navegación mediante Tag Helpers.

3. **Sintaxis Razor**
   - Expresiones y bloques de código.
   - Variables y tipos.
   - Condicionales y bucles.
   - Directivas Razor.
   - Tag Helpers para enlaces y formularios.

4. **PageModel y formularios**
   - Clase `PageModel`.
   - Model binding y `[BindProperty]`.
   - Manejadores `OnGet`, `OnPost` y sus versiones asíncronas.
   - Formularios, redirecciones y `TempData`.

5. **Agenda de teléfonos**
   - Modelo `Contacto`.
   - Servicio de almacenamiento en memoria.
   - Listado, creación, modificación y borrado.
   - Validación en cliente y servidor.

6. **Lista de tareas con base de datos**
   - Introducción a Entity Framework Core.
   - Proveedor SQLite.
   - Entidades y `DbContext`.
   - Migraciones.
   - Operaciones CRUD asíncronas.

### Proyecto 1: Agenda de teléfonos

Permite aprender el flujo completo de Razor Pages sin introducir todavía una base de datos. Los alumnos puede concentrarse en páginas, formularios, validación, handlers y operaciones CRUD.

### Proyecto 2: Lista de tareas

Introduce la persistencia real mediante Entity Framework Core y SQLite. Las tareas se conservan entre ejecuciones y las operaciones utilizan las versiones asíncronas de EF Core.

## 3. Identity en .NET 10

Presentación centrada en la seguridad y la gestión del ciclo de vida de las cuentas dentro de aplicaciones Razor Pages.

### Contenidos principales

- Diferencias entre autenticación y autorización.
- Ciclo completo de una cuenta: registro, confirmación, inicio de sesión, cookie, autorización y cierre de sesión.
- Integración entre navegador, middleware, managers, EF Core y base de datos.
- `ApplicationUser`, `UserManager`, `SignInManager` y `ApplicationDbContext`.
- Creación de un proyecto con cuentas individuales.
- Registro de Identity en `Program.cs`.
- Middleware de autenticación y autorización.
- Tablas creadas por Identity.
- Registro, hash de contraseñas e inicio de sesión.
- Cookies de autenticación y cierre de sesión.
- Protección de páginas con `[Authorize]` y convenciones.
- Roles, claims y políticas.
- Datos asociados al usuario autenticado.
- Personalización de `ApplicationUser` y migraciones.
- Configuración de contraseñas, bloqueo y cookies.
- Confirmación de correo y recuperación de contraseña.
- Tokens, autenticación en dos pasos y proveedores externos.
- Razor Class Library y scaffolding de la interfaz de Identity.
- Errores habituales y recomendaciones de seguridad.
- Práctica guiada de películas favoritas por usuario.

## Orden recomendado

1. [Razor Pages en .NET 10: introducción](razor_pages_introduccion.pdf).
2. [Razor Pages en ASP.NET Core](Razor%20Pages%20en%20ASP.NET.pdf).
3. Desarrollo de una aplicación CRUD propia.
4. [Identity en .NET 10](Identity_NET10.pdf).
5. Incorporación de usuarios, permisos y datos asociados a cada cuenta.

## Continuidad con los manuales

Las tres presentaciones pueden ampliarse con los seis manuales de la colección. El nuevo manual de ASP.NET Core desarrolla con mayor profundidad la plataforma web, la creación de APIs y las alternativas a REST.

| Manual | Ampliación que aporta |
| --- | --- |
| [ASP.NET Core con .NET 10](../manuales/Manual_ASPNET_Core_NET_10.pdf) | Minimal APIs, Controllers, diseño REST, validación, `ProblemDetails`, servicios, configuración, EF Core, OpenAPI, seguridad, RPC, webhooks, SignalR, WebSockets y gRPC. |
| [Razor Pages con .NET 10](../manuales/Manual_Razor_Pages_NET_10.pdf) | Desarrollo completo de aplicaciones Razor Pages, acceso a datos, consumo de API, seguridad y publicación. |
| [Bootstrap y Bootstrap Icons](../manuales/Manual_Bootstrap_Bootstrap_Icons.pdf) | Diseño responsive, componentes, formularios, iconos y temas. |
| [SweetAlert2 con Razor Pages](../manuales/Manual_SweetAlert2_Razor_Pages.pdf) | Confirmaciones, avisos, formularios POST, seguridad e integración con Razor Pages. |
| [Pruebas con xUnit y .NET 10](../manuales/Manual_xUnit_NET_10.pdf) | Pruebas unitarias y de integración para servicios, EF Core, Razor Pages y APIs. |
| [Playwright para .NET 10](../manuales/Manual_Playwright_NET_10.pdf) | Automatización de navegación, formularios, responsive y flujos completos. |

## Requisitos previos

- Conocer los fundamentos de C#: variables, condiciones, bucles, métodos, clases y colecciones.
- Disponer del SDK de .NET 10.
- Utilizar Visual Studio, Visual Studio Code u otro editor compatible con C#.
- Conocer HTML básico resulta conveniente, aunque no es imprescindible para la presentación introductoria.

## Tecnologías tratadas

- .NET 10 y C#.
- ASP.NET Core.
- Razor Pages y sintaxis Razor.
- HTML y Tag Helpers.
- Entity Framework Core.
- SQLite.
- ASP.NET Core Identity.
- Programación asíncrona con `async` y `await`.

## Estructura de la carpeta

```text
presentaciones/
├── README.md
├── razor_pages_introduccion.pdf
├── Razor Pages en ASP.NET.pdf
└── Identity_NET10.pdf
```

## Uso de los materiales

Los documentos están preparados para formación y consulta. Si se reutilizan o adaptan, conviene mantener la referencia a la autoría original y revisar las versiones de .NET y de los paquetes antes de impartir el contenido.
