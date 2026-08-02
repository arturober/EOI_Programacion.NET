# Materiales de ASP.NET Core, acceso a datos, Razor Pages, interfaz y pruebas con .NET 10

Esta colección reúne diez documentos didácticos para aprender a desarrollar aplicaciones web y APIs completas con ASP.NET Core, C# 14 y .NET 10.

Incluye tres presentaciones para explicaciones y sesiones guiadas, junto con siete manuales de consulta y trabajo autónomo. En conjunto, los materiales cubren HTTP y ASP.NET Core, Minimal APIs, Controllers, REST, ADO.NET, SQL directo, Entity Framework Core, SQLite, Razor Pages, protocolos alternativos, Bootstrap, Bootstrap Icons, Bootswatch, SweetAlert2, Identity, xUnit y Playwright.

## Resumen de la colección

- **3 presentaciones:** 102 diapositivas.
- **7 manuales:** 246 páginas.
- **Total:** 10 documentos coordinados.
- **Nivel:** desde iniciación hasta desarrollo, persistencia y pruebas de una aplicación completa.

## Documentos incluidos

### Presentaciones

| Documento | Extensión | Nivel | Finalidad |
| --- | ---: | --- | --- |
| [Razor Pages en .NET 10: introducción](Presentaciones/razor_pages_introduccion.pdf) | 17 diapositivas | Inicial | Presentar la relación entre URL, `.cshtml` y `PageModel`. |
| [Razor Pages en ASP.NET Core](Presentaciones/Razor%20Pages%20en%20ASP.NET.pdf) | 50 diapositivas | Inicial-intermedio | Curso paso a paso con Agenda de teléfonos y Lista de tareas. |
| [Identity en .NET 10](Presentaciones/Identity_NET10.pdf) | 35 diapositivas | Intermedio | Explicar cuentas, cookies, roles, claims, políticas y seguridad. |

### Manuales

| Documento | Páginas | Finalidad |
| --- | ---: | --- |
| [ASP.NET Core con .NET 10](Manuales/Manual_ASPNET_Core_NET_10.pdf) | 49 | APIs con Minimal APIs y Controllers, REST, validación, errores, EF Core, OpenAPI, seguridad y alternativas no REST. |
| [ADO.NET y Entity Framework Core con SQLite](Manuales/Manual_ADONET_Entity_Framework_Core_SQLite_NET_10.pdf) | 55 | Acceso directo con ADO.NET, SQL parametrizado, lectores, CRUD, transacciones, propiedades avanzadas, ORM, EF Core, migraciones y relaciones. |
| [Razor Pages con .NET 10](Manuales/Manual_Razor_Pages_NET_10.pdf) | 41 | Arquitectura, Razor, datos, API, seguridad y publicación. |
| [Bootstrap y Bootstrap Icons](Manuales/Manual_Bootstrap_Bootstrap_Icons.pdf) | 26 | Diseño responsive, componentes, formularios, iconos y temas. |
| [SweetAlert2 con Razor Pages](Manuales/Manual_SweetAlert2_Razor_Pages.pdf) | 29 | Confirmaciones, toasts, formularios POST, seguridad y pruebas. |
| [Pruebas con xUnit y .NET 10](Manuales/Manual_xUnit_NET_10.pdf) | 22 | Pruebas unitarias y de integración para C#, EF Core y ASP.NET Core. |
| [Playwright para .NET 10](Manuales/Manual_Playwright_NET_10.pdf) | 24 | Pruebas de navegador, responsive, SweetAlert2, temas y flujos completos. |

## Cómo se complementan los materiales

| Necesidad | Presentación recomendada | Manual de ampliación |
| --- | --- | --- |
| Comprender la plataforma web y HTTP | Ejemplos de API de las presentaciones | ASP.NET Core con .NET 10 |
| Comparar Minimal APIs y Controllers | — | ASP.NET Core con .NET 10 |
| Aprender SQL directo y ADO.NET | — | ADO.NET y Entity Framework Core con SQLite |
| Comprender un ORM, migraciones y relaciones | Lista de tareas con SQLite | ADO.NET y Entity Framework Core con SQLite |
| Comprender la arquitectura de Razor Pages | Razor Pages en .NET 10: introducción | Razor Pages con .NET 10 |
| Construir el primer CRUD web | Razor Pages en ASP.NET Core | Razor Pages con .NET 10 |
| Diseñar y publicar una API HTTP | Ejemplos de API de las presentaciones | ASP.NET Core con .NET 10 |
| Incorporar tiempo real o RPC | — | ASP.NET Core con .NET 10 |
| Mejorar la interfaz | Ejemplos visuales de las presentaciones | Bootstrap y Bootstrap Icons |
| Confirmar borrados y mostrar avisos | Ejemplos del CRUD | SweetAlert2 con Razor Pages |
| Incorporar usuarios y permisos | Identity en .NET 10 | Capítulo de seguridad del manual de Razor Pages |
| Probar reglas, persistencia y API | — | Pruebas con xUnit y .NET 10 |
| Probar la aplicación en navegador | — | Playwright para .NET 10 |

## Itinerario completo recomendado

### Fase 1. Comprender ASP.NET Core y HTTP

1. Estudiar [ASP.NET Core con .NET 10](Manuales/Manual_ASPNET_Core_NET_10.pdf).
2. Recorrer la canalización de middleware, routing y endpoints.
3. Implementar una API de recursos mediante Minimal APIs.
4. Repetir una parte con Controllers y comparar ambos modelos.
5. Aplicar REST, validación, códigos de estado y `ProblemDetails`.
6. Comparar REST con RPC, webhooks, SignalR, WebSockets y gRPC.

### Fase 2. Dominar el acceso a datos

1. Estudiar [ADO.NET y Entity Framework Core con SQLite](Manuales/Manual_ADONET_Entity_Framework_Core_SQLite_NET_10.pdf).
2. Crear una base SQLite y conectarse con `Microsoft.Data.Sqlite`.
3. Implementar consultas parametrizadas con `DbConnection`, `DbCommand` y `DbDataReader`.
4. Completar un CRUD con SQL directo, transacciones y tratamiento de errores.
5. Repetir el dominio con EF Core, `DbContext`, migraciones y LINQ.
6. Modelar relaciones uno a muchos, uno a uno y muchos a muchos.
7. Comparar ADO.NET, EF Core y un enfoque híbrido.

### Fase 3. Construir aplicaciones con Razor Pages

1. Estudiar [Razor Pages en .NET 10: introducción](Presentaciones/razor_pages_introduccion.pdf).
2. Continuar con [Razor Pages en ASP.NET Core](Presentaciones/Razor%20Pages%20en%20ASP.NET.pdf).
3. Crear la Agenda de teléfonos en memoria.
4. Crear la Lista de tareas con Entity Framework Core y SQLite.
5. Utilizar [Razor Pages con .NET 10](Manuales/Manual_Razor_Pages_NET_10.pdf) para profundizar en routing, formularios, relaciones, LINQ, API y publicación.

### Fase 4. Diseñar la interfaz

1. Aplicar [Bootstrap y Bootstrap Icons](Manuales/Manual_Bootstrap_Bootstrap_Icons.pdf).
2. Construir una barra de navegación responsive.
3. Adaptar formularios, tablas y tarjetas a móvil.
4. Añadir iconos con nombres accesibles.
5. Incorporar temas claros, oscuros y Bootswatch.

### Fase 5. Mejorar la interacción

1. Estudiar [SweetAlert2 con Razor Pages](Manuales/Manual_SweetAlert2_Razor_Pages.pdf).
2. Añadir una confirmación reutilizable a los formularios de borrado.
3. Mostrar mensajes procedentes de `TempData`.
4. Revisar antifalsificación, accesibilidad y prevención de XSS.

### Fase 6. Incorporar cuentas y permisos

1. Estudiar [Identity en .NET 10](Presentaciones/Identity_NET10.pdf).
2. Crear un proyecto con cuentas individuales.
3. Proteger páginas mediante `[Authorize]` o convenciones.
4. Añadir roles, claims o políticas cuando el caso de uso lo requiera.
5. Relacionar los datos de negocio con el identificador del usuario autenticado.

### Fase 7. Automatizar pruebas de código e integración

1. Estudiar [Pruebas con xUnit y .NET 10](Manuales/Manual_xUnit_NET_10.pdf).
2. Probar reglas de negocio, servicios y `PageModel`.
3. Probar EF Core mediante SQLite aislado.
4. Añadir pruebas de integración con `WebApplicationFactory`.
5. Verificar endpoints, errores y contratos de API.

### Fase 8. Automatizar pruebas de navegador

1. Continuar con [Playwright para .NET 10](Manuales/Manual_Playwright_NET_10.pdf).
2. Probar navegación, formularios, filtros y borrados.
3. Verificar SweetAlert2, temas y diseño móvil.
4. Ejecutar los flujos críticos en varios navegadores y en integración continua.

## Itinerarios según el tiempo disponible

### Sesión breve

- Razor Pages en .NET 10: introducción.
- Creación de una página sencilla con `OnGet`.
- Formulario básico con `OnPost`.

### Unidad didáctica

- Fundamentos de ASP.NET Core y HTTP.
- Razor Pages en ASP.NET Core.
- CRUD de biblioteca con SQLite.
- Selección de capítulos del manual de ADO.NET y EF Core.
- Lista de tareas mediante Razor Pages.

### Proyecto trimestral

- Las tres presentaciones.
- Los siete manuales como documentación de consulta.
- Aplicación CRUD con API propia, SQLite, autenticación, interfaz responsive y pruebas automatizadas.

## Resultados de aprendizaje

- Explicar cómo ASP.NET Core procesa una petición HTTP.
- Crear APIs HTTP con Minimal APIs y Controllers.
- Diseñar recursos, rutas, verbos, códigos y cabeceras REST coherentes.
- Validar entradas y normalizar errores mediante `ProblemDetails`.
- Utilizar inyección de dependencias, servicios y configuración tipada.
- Explicar qué es ADO.NET, cuándo utilizarlo y qué responsabilidades deja al desarrollador.
- Ejecutar SQL parametrizado con `DbConnection`, `DbCommand` y `DbDataReader`.
- Implementar CRUD, transacciones y tratamiento de errores con SQLite.
- Razonar sobre pooling, timeout, aislamiento, WAL, `CommandType`, `DataTable` y proveedores.
- Explicar qué problema resuelve un ORM y comparar sus costes con SQL directo.
- Configurar EF Core con SQLite, modelos, `DbContext` y migraciones.
- Implementar CRUD, consultas LINQ y relaciones entre entidades con EF Core.
- Crear y organizar proyectos Razor Pages dirigidos a `net10.0`.
- Utilizar sintaxis Razor, Tag Helpers, handlers y model binding.
- Validar formularios en cliente y servidor.
- Consumir API externas mediante `IHttpClientFactory` y DTO.
- Exponer una API propia y configurar CORS cuando sea necesario.
- Documentar contratos mediante OpenAPI y planificar su evolución.
- Elegir entre REST, RPC, webhooks, SignalR, WebSockets y gRPC.
- Diseñar interfaces responsive con Bootstrap y Bootstrap Icons.
- Mantener temas mediante Bootswatch, `localStorage` y `data-bs-theme`.
- Integrar SweetAlert2 sin romper formularios ni antifalsificación.
- Añadir autenticación y autorización mediante Identity.
- Escribir pruebas unitarias y de integración con xUnit.
- Automatizar flujos de navegador mediante Playwright.
- Aplicar criterios de seguridad, accesibilidad, rendimiento y mantenibilidad.

## Tecnologías cubiertas

- .NET 10 y C# 14.
- ASP.NET Core, Minimal APIs, Controllers y Razor Pages.
- HTTP, REST, `ProblemDetails` y OpenAPI.
- ADO.NET y `System.Data.Common`.
- `Microsoft.Data.Sqlite`, SQL y SQLite.
- Entity Framework Core 10, LINQ y migraciones.
- `DataTable`, transacciones, pooling, timeout, aislamiento y WAL.
- Sintaxis Razor y Tag Helpers.
- ASP.NET Core Identity.
- `HttpClient`, JSON, DTO, caché y CORS.
- RPC, webhooks, SignalR, WebSockets y gRPC.
- Bootstrap 5.3, Bootstrap Icons y Bootswatch.
- SweetAlert2.
- xUnit v3 y `WebApplicationFactory`.
- Playwright para .NET.
- JavaScript, `fetch`, `localStorage` y `dataset`.

## Requisitos previos

- Fundamentos de C#: tipos, condiciones, bucles, métodos, clases y colecciones.
- SDK de .NET 10.
- Visual Studio, Visual Studio Code u otro editor compatible con C#.
- Conocimientos básicos de HTML y SQL; el manual de datos introduce lo necesario de SQL.
- Navegador web moderno.
- PowerShell 7 o entorno equivalente para instalar los navegadores de Playwright.

## Organización del repositorio

```text
materiales-dotnet10/
├── README.md
├── Presentaciones/
│   ├── README.md
│   ├── razor_pages_introduccion.pdf
│   ├── Razor Pages en ASP.NET.pdf
│   └── Identity_NET10.pdf
└── Manuales/
    ├── README.md
    ├── Manual_ASPNET_Core_NET_10.pdf
    ├── Manual_ADONET_Entity_Framework_Core_SQLite_NET_10.pdf
    ├── Manual_Razor_Pages_NET_10.pdf
    ├── Manual_Bootstrap_Bootstrap_Icons.pdf
    ├── Manual_SweetAlert2_Razor_Pages.pdf
    ├── Manual_xUnit_NET_10.pdf
    └── Manual_Playwright_NET_10.pdf
```
