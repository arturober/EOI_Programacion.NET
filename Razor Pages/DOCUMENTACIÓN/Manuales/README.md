# Manuales de desarrollo web y acceso a datos con .NET 10

Esta carpeta reúne siete manuales complementarios, con 246 páginas en total, para aprender a desarrollar, conectar, diseñar y probar aplicaciones web y APIs con C# 14, ASP.NET Core y .NET 10.

La colección cubre el ciclo completo de un proyecto: plataforma web, HTTP, acceso directo a SQLite, ORM, arquitectura, API, interfaz responsive, interacción con el usuario, pruebas automatizadas y pruebas de navegador.

## Documentos incluidos

| Manual | Páginas | Contenido principal |
| --- | ---: | --- |
| [ASP.NET Core con .NET 10](Manual_ASPNET_Core_NET_10.pdf) | 49 | Minimal APIs, Controllers, REST, validación, `ProblemDetails`, servicios, configuración, EF Core, OpenAPI, seguridad y alternativas no REST. |
| [ADO.NET y Entity Framework Core con SQLite](Manual_ADONET_Entity_Framework_Core_SQLite_NET_10.pdf) | 55 | SQL directo, abstracciones ADO.NET, parámetros, lectores, CRUD, transacciones, propiedades avanzadas, ORM, EF Core, migraciones y relaciones. |
| [Razor Pages con .NET 10](Manual_Razor_Pages_NET_10.pdf) | 41 | Arquitectura, Razor, handlers, formularios, EF Core, SQLite, API, caché, seguridad, SignalR y publicación. |
| [Bootstrap y Bootstrap Icons](Manual_Bootstrap_Bootstrap_Icons.pdf) | 26 | Rejilla, utilidades, componentes, formularios, navegación, iconos, temas y responsive. |
| [SweetAlert2 con Razor Pages](Manual_SweetAlert2_Razor_Pages.pdf) | 29 | Alertas, confirmaciones de borrado, toasts, `TempData`, operaciones asíncronas, seguridad y Playwright. |
| [Pruebas con xUnit y .NET 10](Manual_xUnit_NET_10.pdf) | 22 | Pruebas unitarias y de integración para C#, `PageModel`, EF Core y API. |
| [Playwright para .NET 10](Manual_Playwright_NET_10.pdf) | 24 | Pruebas de navegador para navegación, formularios, tablas, SweetAlert2, temas y diseño móvil. |

## Características comunes

- Maquetación A4 coherente en toda la colección.
- Índice navegable y capítulos numerados.
- Bloques de código con resaltado sintáctico y numeración de líneas.
- Ejemplos en C#, SQL, Razor, HTML, JavaScript, JSON y línea de comandos.
- Tablas de referencia, advertencias y recomendaciones.
- Actividades prácticas y propuestas de ampliación.
- Referencias a documentación oficial.
- Franja superior libre para incorporar logotipos o un encabezado institucional.

## 1. ASP.NET Core con .NET 10

Manual para comprender la plataforma web de .NET y construir APIs HTTP mantenibles con los dos modelos de programación principales: Minimal APIs y Controllers.

### Contenidos destacados

- ASP.NET Core, Kestrel, hosting, middleware, routing y endpoints.
- Anatomía y semántica de HTTP.
- Minimal APIs, Controllers y comparación entre ambos modelos.
- Diseño REST, códigos de estado, DTO y evolución de contratos.
- Validación integrada de Minimal APIs en .NET 10.
- `ProblemDetails`, tratamiento global de errores y trazabilidad.
- Inyección de dependencias, servicios y configuración tipada.
- Entity Framework Core 10, SQLite, migraciones, CRUD y relaciones.
- OpenAPI 3.1, documentación y versionado.
- Autenticación, autorización, CORS, rate limiting y caché.
- Pruebas unitarias, de integración y de contrato.
- RPC, webhooks, SignalR, WebSockets y gRPC.
- Publicación, health checks, observabilidad y diagnóstico.
- Proyecto integrador de una API de biblioteca.

## 2. ADO.NET y Entity Framework Core con SQLite

Manual centrado exclusivamente en SQLite para comprender el acceso a datos desde las abstracciones de bajo nivel hasta un ORM moderno. Utiliza la misma biblioteca como proyecto conductor para que las diferencias entre SQL directo y EF Core resulten visibles.

### Contenidos destacados

- Modelo relacional, tipos de almacenamiento y restricciones de SQLite.
- Preparación de un proyecto con C# 14, .NET 10 y `Microsoft.Data.Sqlite`.
- Cadenas de conexión, rutas fiables y ciclo de vida de la conexión.
- `DbConnection`, `DbCommand`, `DbParameter`, `DbDataReader` y `DbTransaction`.
- `ExecuteNonQuery`, `ExecuteScalar` y `ExecuteReader`.
- Parámetros, `DBNull`, tipos SQLite y prevención de inyección SQL.
- Inicialización del esquema y CRUD completo con SQL directo.
- Repositorios, transacciones, savepoints y tratamiento de errores.
- Particularidades de la asincronía del proveedor SQLite.
- Pooling, timeout, locking, WAL, concurrencia y aislamiento.
- `CommandType`, proveedores, `DataTable`, `DataSet`, PRAGMA y metadatos.
- Rendimiento, índices, planes de consulta y pruebas aisladas.
- Qué es un ORM, qué problema resuelve y qué costes introduce.
- Configuración de EF Core 10 con SQLite.
- Modelos, `DbContext`, Fluent API, migraciones y esquema.
- CRUD completo, seguimiento de cambios, LINQ y SQL generado.
- Relaciones uno a muchos, uno a uno y muchos a muchos.
- Carga relacionada, transacciones, concurrencia y SQL interoperable.
- Limitaciones específicas de EF Core con SQLite.
- Criterios para elegir ADO.NET, EF Core o un enfoque híbrido.
- Proyecto integrador de biblioteca y diagnóstico de errores frecuentes.

## 3. Razor Pages con .NET 10

Manual de arquitectura web orientado a aplicaciones basadas en páginas. Explica el recorrido desde una petición HTTP hasta el HTML que recibe el navegador y continúa con persistencia, API, JavaScript, seguridad, pruebas y publicación.

### Contenidos destacados

- HTTP, middleware, routing y endpoints.
- Creación y estructura de un proyecto `net10.0`.
- Sintaxis Razor, directivas, Tag Helpers y layouts.
- `PageModel`, handlers, model binding, formularios y validación.
- Inyección de dependencias, configuración, claves y logging.
- Entity Framework Core 10, SQLite, CRUD asíncrono y relaciones.
- LINQ, búsqueda, normalización y paginación.
- Consumo de API mediante `IHttpClientFactory` y DTO.
- Caché, controladores de API, JSON y CORS.
- SignalR, JavaScript, Bootstrap, SweetAlert2 e Identity.
- Antifalsificación, autorización, pruebas, accesibilidad y publicación.
- Proyecto integrador de Trivial.

### Aplicaciones de referencia

El manual permite comprender proyectos de listas, agendas, tareas, juegos de preguntas, clientes de API externas, CRUD con SQLite y aplicaciones que exponen una API propia.

## 4. Bootstrap y Bootstrap Icons

Manual para construir interfaces responsive y accesibles reduciendo al mínimo el CSS personalizado.

### Contenidos destacados

- Instalación mediante CDN o LibMan.
- Enfoque mobile first, breakpoints, contenedores y rejilla.
- Utilidades de espaciado, display, flex, grid, tamaños y bordes.
- Colores semánticos y modos claro y oscuro.
- Botones, navegación, formularios, tablas, tarjetas y alertas.
- Modales, offcanvas, dropdowns, acordeones y carruseles.
- Uso accesible de Bootstrap Icons.
- Bootswatch y selector persistente de temas.
- Auditoría de clases redundantes o contradictorias.

## 5. SweetAlert2 con Razor Pages

Manual centrado en la integración correcta de SweetAlert2 con formularios Razor Pages.

### Contenidos destacados

- Instalación mediante CDN, LibMan o npm.
- `Swal.fire`, Promises y `await`.
- Confirmación de borrado con formularios POST.
- Conservación del token antifalsificación.
- Listener delegado y `requestSubmit` sin bucles.
- Mensajes procedentes de `TempData`, mixins y toasts.
- Integración visual con Bootstrap.
- Inputs, validación, `preConfirm` y peticiones con `fetch`.
- Accesibilidad, prevención de XSS y Content Security Policy.
- Pruebas de confirmación y cancelación con Playwright.

## 6. Pruebas con xUnit y .NET 10

Manual para introducir pruebas automatizadas en proyectos C# y ASP.NET Core.

### Contenidos destacados

- Pirámide de pruebas y propiedades FIRST.
- Proyectos xUnit v3 para `net10.0`.
- Arrange, Act, Assert; `Fact`, `Theory` y fuentes de datos.
- Excepciones, código asíncrono y cancelación.
- Dobles de prueba, fixtures y paralelismo.
- EF Core con SQLite en memoria.
- Pruebas directas de `PageModel`.
- Integración mediante `WebApplicationFactory` y pruebas de API.
- Cobertura, Microsoft Testing Platform y diagnóstico.

## 7. Playwright para .NET 10

Manual para probar la aplicación desde el punto de vista del navegador y del usuario.

### Contenidos destacados

- Instalación de Playwright con xUnit v3.
- Descarga y configuración de navegadores.
- Arranque controlado de la aplicación.
- Localizadores accesibles, auto-waiting y aserciones reintentables.
- Navegación, formularios, tablas, filtros y paginación.
- SweetAlert2, modales y diálogos nativos.
- Temas, `localStorage`, Bootswatch y viewports móviles.
- Intercepción de API externas y autenticación.
- Aislamiento de datos, paralelismo, trazas y capturas.
- Varios navegadores e integración continua.

## Orden de estudio recomendado

1. [ASP.NET Core con .NET 10](Manual_ASPNET_Core_NET_10.pdf).
2. [ADO.NET y Entity Framework Core con SQLite](Manual_ADONET_Entity_Framework_Core_SQLite_NET_10.pdf).
3. [Razor Pages con .NET 10](Manual_Razor_Pages_NET_10.pdf).
4. [Bootstrap y Bootstrap Icons](Manual_Bootstrap_Bootstrap_Icons.pdf).
5. [SweetAlert2 con Razor Pages](Manual_SweetAlert2_Razor_Pages.pdf).
6. [Pruebas con xUnit y .NET 10](Manual_xUnit_NET_10.pdf).
7. [Playwright para .NET 10](Manual_Playwright_NET_10.pdf).

Este orden presenta primero la plataforma y HTTP, después las dos formas de persistencia, a continuación la aplicación web y su interfaz, y finalmente las pruebas.

## Itinerarios alternativos

### Acceso a datos

1. ADO.NET y SQL directo.
2. Transacciones, errores y propiedades avanzadas.
3. ORM y Entity Framework Core.
4. Migraciones, CRUD y relaciones.
5. Pruebas con SQLite aislado mediante xUnit.

### Desarrollo de interfaz

1. Razor Pages.
2. Bootstrap y Bootstrap Icons.
3. SweetAlert2.
4. Playwright.

### Desarrollo de API

1. Manual de ASP.NET Core completo.
2. Manual de ADO.NET y EF Core para la persistencia.
3. Secciones de API propia y CORS del manual de Razor Pages.
4. Pruebas de integración y API con xUnit.
5. Intercepción y observación de red con Playwright.

### Pruebas automatizadas

1. Arquitectura y servicios de ASP.NET Core.
2. Persistencia con SQLite.
3. xUnit.
4. Playwright.

## Requisitos previos

- Conocimientos básicos de C#.
- SDK de .NET 10.
- Visual Studio, Visual Studio Code u otro editor compatible.
- Conocimientos elementales de HTML.
- No es imprescindible conocer SQL previamente: el manual de datos introduce el modelo y las operaciones necesarias.
- Para Playwright, PowerShell 7 o un entorno capaz de ejecutar el script de instalación de navegadores.

## Propuesta de trabajo

1. Crear una API básica y recorrer su canalización HTTP.
2. Implementar los mismos recursos con Minimal APIs y Controllers.
3. Crear el esquema SQLite y un CRUD con ADO.NET y SQL parametrizado.
4. Repetir el dominio con EF Core, migraciones y relaciones.
5. Conectar la API a la capa de persistencia elegida.
6. Crear un proyecto Razor Pages que consuma o complemente la API.
7. Mejorar navegación, formularios y listados con Bootstrap e iconos.
8. Añadir temas Bootswatch y confirmaciones SweetAlert2.
9. Escribir pruebas unitarias y de integración con xUnit.
10. Probar los flujos críticos mediante Playwright.
11. Revisar accesibilidad, seguridad, rendimiento y publicación.

## Estructura de la carpeta

```text
Manuales/
├── README.md
├── Manual_ASPNET_Core_NET_10.pdf
├── Manual_ADONET_Entity_Framework_Core_SQLite_NET_10.pdf
├── Manual_Razor_Pages_NET_10.pdf
├── Manual_Bootstrap_Bootstrap_Icons.pdf
├── Manual_SweetAlert2_Razor_Pages.pdf
├── Manual_xUnit_NET_10.pdf
└── Manual_Playwright_NET_10.pdf
```

## Actualización de dependencias

Los ejemplos se han preparado para .NET 10 y C# 14. Antes de utilizar los comandos en un proyecto nuevo, conviene mantener alineadas las versiones de los paquetes, revisar sus notas de versión y consultar los avisos de seguridad.
