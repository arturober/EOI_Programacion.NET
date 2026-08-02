# Manuales de desarrollo web con .NET 10

> **Colección actualizada:** 6 manuales, incluido **ASP.NET Core con .NET 10**. Los manuales suman 191 páginas.

Esta carpeta reúne seis manuales complementarios para aprender a desarrollar, diseñar y probar aplicaciones web y APIs con ASP.NET Core y .NET 10.

La colección cubre el ciclo completo de un proyecto: arquitectura, acceso a datos, consumo y publicación de API, interfaz responsive, confirmaciones de usuario, pruebas automatizadas y pruebas de navegador.

## Documentos incluidos

| Manual | Páginas | Contenido principal |
| --- | ---: | --- |
| [ASP.NET Core con .NET 10](Manual_ASPNET_Core_NET_10.pdf) | 49 | Minimal APIs, Controllers, REST, validación, ProblemDetails, servicios, configuración, EF Core, OpenAPI, seguridad y alternativas no REST. |
| [Razor Pages con .NET 10](Manual_Razor_Pages_NET_10.pdf) | 41 | Arquitectura, Razor, handlers, formularios, EF Core, SQLite, API, caché, seguridad, SignalR y publicación. |
| [Bootstrap y Bootstrap Icons](Manual_Bootstrap_Bootstrap_Icons.pdf) | 26 | Rejilla, utilidades, componentes, formularios, navegación, iconos, temas y responsive. |
| [SweetAlert2 con Razor Pages](Manual_SweetAlert2_Razor_Pages.pdf) | 29 | Alertas, confirmaciones de borrado, toasts, `TempData`, operaciones asíncronas, seguridad y Playwright. |
| [Pruebas con xUnit y .NET 10](Manual_xUnit_NET_10.pdf) | 22 | Pruebas unitarias y de integración para C#, PageModel, EF Core y API. |
| [Playwright para .NET 10](Manual_Playwright_NET_10.pdf) | 24 | Pruebas de navegador para navegación, formularios, tablas, SweetAlert2, temas y diseño móvil. |

## Características comunes

- Maquetación A4 coherente en toda la colección.
- Índice navegable y capítulos numerados.
- Bloques de código con resaltado sintáctico y numeración de líneas.
- Ejemplos en C#, Razor, HTML, JavaScript, JSON y línea de comandos.
- Tablas de referencia, advertencias y recomendaciones.
- Actividades prácticas y propuestas de ampliación.
- Referencias a documentación oficial.
- Franja superior libre para incorporar logotipos o un encabezado institucional.

## 1. ASP.NET Core con .NET 10

Manual para comprender la plataforma web de .NET y construir APIs HTTP mantenibles con los dos modelos de programación principales: Minimal APIs y Controllers.

### Contenidos destacados

- ASP.NET Core, Kestrel, hosting, middleware, routing y endpoints.
- Anatomía y semántica de HTTP.
- Creación de APIs con Minimal APIs.
- Routing, model binding, grupos y filtros de endpoint.
- Creación de APIs mediante `ControllerBase` y `[ApiController]`.
- Comparación razonada entre Minimal APIs y Controllers.
- Diseño REST: recursos, rutas, métodos, idempotencia y cabeceras.
- Códigos de estado y respuestas tipadas.
- DTO, mapeo, proyección y evolución de contratos.
- Validación integrada de Minimal APIs en .NET 10.
- `ProblemDetails`, tratamiento global de errores y trazabilidad.
- Inyección de dependencias, ciclos de vida y servicios de aplicación.
- Configuración tipada, Options, secretos, logging y cancelación.
- Entity Framework Core 10, SQLite, migraciones, CRUD y relaciones.
- Consultas eficientes, paginación, transacciones y concurrencia.
- OpenAPI 3.1, documentación y versionado.
- Autenticación, autorización, CORS, rate limiting y caché.
- Pruebas unitarias, de integración y de contrato.
- RPC, webhooks, SignalR, WebSockets y gRPC.
- Publicación, health checks, observabilidad y diagnóstico.
- Proyecto integrador de una API de biblioteca.

## 2. Razor Pages con .NET 10

Manual principal de la colección. Explica el funcionamiento de Razor Pages desde una petición HTTP hasta el HTML que recibe el navegador y continúa con persistencia, API, JavaScript, seguridad, pruebas y publicación.

### Contenidos destacados

- HTTP, middleware, routing y endpoints.
- Creación y estructura de un proyecto `net10.0`.
- Sintaxis Razor y directivas.
- `PageModel`, handlers y resultados.
- Routing, parámetros y Tag Helpers.
- Model binding, formularios, validación y `ModelState`.
- Layouts, parciales, `ViewData` y `TempData`.
- Inyección de dependencias, configuración, secretos y logging.
- Entity Framework Core 10 y SQLite.
- CRUD asíncrono y relaciones.
- LINQ, búsqueda, normalización y paginación.
- Consumo de API mediante `IHttpClientFactory` y DTO.
- Caché con `IMemoryCache`.
- Controladores de API, JSON y CORS.
- Introducción a SignalR y estado en tiempo real.
- JavaScript con debounce, `localStorage` y `dataset`.
- Bootstrap, Bootswatch, Bootstrap Icons y SweetAlert2.
- Antifalsificación, Identity y autorización.
- Pruebas, rendimiento, accesibilidad y publicación.
- Proyecto integrador de Trivial.

### Aplicaciones de referencia

El manual permite comprender proyectos de listas, agendas, tareas, juegos de preguntas, clientes de API externas, CRUD con SQLite y aplicaciones que exponen una API propia.

## 3. Bootstrap y Bootstrap Icons

Manual para construir interfaces responsive y accesibles reduciendo al mínimo el CSS personalizado.

### Contenidos destacados

- Instalación mediante CDN o LibMan.
- Enfoque mobile first y breakpoints.
- Contenedores y rejilla de doce columnas.
- Utilidades de espaciado, display, flex, grid, tamaños y bordes.
- Colores semánticos y modos claro y oscuro.
- Botones y grupos de acciones.
- Barra de navegación responsive.
- Formularios y validación de Razor Pages.
- Tablas y listados adaptables a móvil.
- Tarjetas, listas, badges y alertas.
- Modales, offcanvas, dropdowns, acordeones y otros componentes.
- Carruseles, imágenes y multimedia.
- Uso accesible de Bootstrap Icons.
- Bootswatch y selector persistente de temas.
- Auditoría de clases redundantes o contradictorias.

## 4. SweetAlert2 con Razor Pages

Manual centrado en la integración correcta de SweetAlert2 con formularios Razor Pages.

### Contenidos destacados

- Diferencias entre SweetAlert y SweetAlert2.
- Instalación mediante CDN, LibMan o npm.
- `Swal.fire`, Promises y `await`.
- Confirmación de borrado con formularios POST.
- Conservación del token antifalsificación.
- Listener delegado reutilizable.
- Uso de `requestSubmit` sin provocar bucles.
- Mensajes procedentes de `TempData`.
- Mixins, toasts y configuración común.
- Integración visual con Bootstrap.
- Inputs, validación y `preConfirm`.
- Peticiones asíncronas con `fetch`.
- Accesibilidad, prevención de XSS y Content Security Policy.
- Pruebas de confirmación y cancelación con Playwright.

## 5. Pruebas con xUnit y .NET 10

Manual para introducir pruebas automatizadas en proyectos C# y ASP.NET Core.

### Contenidos destacados

- Pirámide de pruebas y propiedades FIRST.
- Creación de proyectos xUnit v3 para `net10.0`.
- Patrón Arrange, Act, Assert.
- `Fact`, `Theory`, `InlineData` y `MemberData`.
- Aserciones específicas.
- Pruebas de excepciones, código asíncrono y cancelación.
- Dobles de prueba: fake, stub, spy y mock.
- Ciclo de vida, fixtures y paralelismo.
- EF Core con SQLite en memoria.
- Pruebas directas de `PageModel`.
- Pruebas de integración mediante `WebApplicationFactory`.
- Pruebas de API.
- Cobertura y Microsoft Testing Platform.
- Diagnóstico de errores frecuentes.

## 6. Playwright para .NET 10

Manual para probar la aplicación desde el punto de vista del navegador y del usuario.

### Contenidos destacados

- Instalación de Playwright con xUnit v3.
- Descarga y configuración de navegadores.
- Arranque controlado de la aplicación.
- Localizadores por rol, etiqueta, texto y `data-testid`.
- Auto-waiting y aserciones reintentables.
- Navegación y formularios Razor Pages.
- Tablas, filtros, ordenación y paginación.
- SweetAlert2, modales y diálogos nativos.
- Temas, `localStorage` y Bootswatch.
- Viewports móviles y comprobaciones responsive.
- Intercepción de API externas.
- Autenticación y estado guardado.
- Aislamiento de datos y paralelismo.
- Trazas, capturas y ejecución visible.
- Ejecución en varios navegadores y CI.

## Orden de estudio recomendado

1. [ASP.NET Core con .NET 10](Manual_ASPNET_Core_NET_10.pdf).
2. [Razor Pages con .NET 10](Manual_Razor_Pages_NET_10.pdf).
3. [Bootstrap y Bootstrap Icons](Manual_Bootstrap_Bootstrap_Icons.pdf).
4. [SweetAlert2 con Razor Pages](Manual_SweetAlert2_Razor_Pages.pdf).
5. [Pruebas con xUnit y .NET 10](Manual_xUnit_NET_10.pdf).
6. [Playwright para .NET 10](Manual_Playwright_NET_10.pdf).

Este orden sigue la evolución natural del proyecto: primero se construye la aplicación, después se mejora su interfaz y, finalmente, se comprueba su comportamiento.

## Itinerarios alternativos

### Desarrollo de interfaz

1. Razor Pages.
2. Bootstrap y Bootstrap Icons.
3. SweetAlert2.
4. Playwright.

### Pruebas automatizadas

1. ASP.NET Core.
2. Secciones de arquitectura y servicios del manual de Razor Pages.
3. xUnit.
4. Playwright.

### Desarrollo de API

1. Manual de ASP.NET Core completo.
2. Secciones de `HttpClient`, DTO, API propia y CORS del manual de Razor Pages.
3. Pruebas de integración y API con xUnit.
4. Intercepción y observación de red con Playwright.

## Requisitos previos

- Conocimientos básicos de C#.
- SDK de .NET 10.
- Visual Studio, Visual Studio Code u otro editor compatible.
- Conocimientos elementales de HTML.
- Para Playwright, PowerShell 7 o un entorno capaz de ejecutar el script de instalación de navegadores.

## Propuesta de trabajo

1. Crear una API básica y recorrer su canalización HTTP.
2. Implementar los mismos recursos con Minimal APIs y Controllers.
3. Conectar la API a Entity Framework Core y SQLite.
4. Crear un proyecto Razor Pages que consuma o complemente la API.
5. Mejorar navegación, formularios y listados con Bootstrap.
6. Incorporar Bootstrap Icons y temas Bootswatch.
7. Añadir confirmaciones SweetAlert2 a las operaciones destructivas.
8. Escribir pruebas unitarias y de integración con xUnit.
9. Probar los flujos críticos mediante Playwright.
10. Revisar accesibilidad, seguridad, rendimiento y publicación.

## Estructura recomendada de la carpeta

```text
manuales/
├── README.md
├── Manual_ASPNET_Core_NET_10.pdf
├── Manual_Razor_Pages_NET_10.pdf
├── Manual_Bootstrap_Bootstrap_Icons.pdf
├── Manual_SweetAlert2_Razor_Pages.pdf
├── Manual_xUnit_NET_10.pdf
└── Manual_Playwright_NET_10.pdf
```

## Actualización de dependencias

Los ejemplos se han preparado para .NET 10 y evitan depender innecesariamente de revisiones concretas de las bibliotecas de interfaz. Antes de utilizar los comandos en un proyecto nuevo, conviene revisar la compatibilidad de los paquetes, las notas de versión y los avisos de seguridad.
