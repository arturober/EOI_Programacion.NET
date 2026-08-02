# Materiales de Razor Pages, Identity, interfaz y pruebas con .NET 10

Esta colección reúne ocho documentos didácticos para aprender a desarrollar aplicaciones web completas con ASP.NET Core y .NET 10.

Incluye tres presentaciones para explicaciones y sesiones guiadas, junto con cinco manuales de consulta y trabajo autónomo. En conjunto, los materiales cubren Razor Pages, Entity Framework Core, SQLite, consumo y creación de API, Bootstrap, Bootstrap Icons, Bootswatch, SweetAlert2, Identity, xUnit y Playwright.

## Resumen de la colección

- **3 presentaciones:** 102 diapositivas.
- **5 manuales:** 142 páginas.
- **Total:** 8 documentos coordinados.
- **Nivel:** desde iniciación hasta desarrollo y pruebas de una aplicación completa.

## Documentos incluidos

### Presentaciones

| Documento | Extensión | Nivel | Finalidad |
| --- | ---: | --- | --- |
| [Razor Pages en .NET 10: introducción](razor_pages_introduccion.pdf) | 17 diapositivas | Inicial | Presentar la relación entre URL, `.cshtml` y `PageModel`. |
| [Razor Pages en ASP.NET Core](Razor%20Pages%20en%20ASP.NET.pdf) | 50 diapositivas | Inicial-intermedio | Curso paso a paso con Agenda de teléfonos y Lista de tareas. |
| [Identity en .NET 10](Identity_NET10.pdf) | 35 diapositivas | Intermedio | Explicar cuentas, cookies, roles, claims, políticas y seguridad. |

### Manuales

| Documento | Páginas | Finalidad |
| --- | ---: | --- |
| [Razor Pages con .NET 10](Manual_Razor_Pages_NET_10.pdf) | 41 | Manual general de arquitectura, datos, API, seguridad y publicación. |
| [Bootstrap y Bootstrap Icons](Manual_Bootstrap_Bootstrap_Icons.pdf) | 26 | Diseño responsive, componentes, formularios, iconos y temas. |
| [SweetAlert2 con Razor Pages](Manual_SweetAlert2_Razor_Pages.pdf) | 29 | Confirmaciones, toasts, formularios POST, seguridad y pruebas. |
| [Pruebas con xUnit y .NET 10](Manual_xUnit_NET_10.pdf) | 22 | Pruebas unitarias y de integración para C#, EF Core y ASP.NET Core. |
| [Playwright para .NET 10](Manual_Playwright_NET_10.pdf) | 24 | Pruebas de navegador, responsive, SweetAlert2, temas y flujos completos. |

## Cómo se complementan los materiales

| Necesidad | Presentación recomendada | Manual de ampliación |
| --- | --- | --- |
| Comprender la arquitectura básica | Razor Pages en .NET 10: introducción | Razor Pages con .NET 10 |
| Construir el primer CRUD | Razor Pages en ASP.NET Core | Razor Pages con .NET 10 |
| Añadir persistencia | Razor Pages en ASP.NET Core | Razor Pages con .NET 10 |
| Mejorar la interfaz | Ejemplos visuales de las presentaciones | Bootstrap y Bootstrap Icons |
| Confirmar borrados y mostrar avisos | Ejemplos del CRUD | SweetAlert2 con Razor Pages |
| Incorporar usuarios y permisos | Identity en .NET 10 | Capítulo de seguridad del manual de Razor Pages |
| Probar reglas y API | - | Pruebas con xUnit y .NET 10 |
| Probar la aplicación en navegador | - | Playwright para .NET 10 |

## Itinerario completo recomendado

### Fase 1. Comprender Razor Pages

1. Estudiar [Razor Pages en .NET 10: introducción](razor_pages_introduccion.pdf).
2. Identificar la ruta, la vista `.cshtml` y el archivo `.cshtml.cs` de varias páginas.
3. Reproducir la configuración mínima de `Program.cs`.
4. Practicar con `OnGet`, `OnPost`, model binding y validación.

### Fase 2. Construir aplicaciones completas

1. Seguir [Razor Pages en ASP.NET Core](Razor%20Pages%20en%20ASP.NET.pdf).
2. Crear la Agenda de teléfonos en memoria.
3. Crear la Lista de tareas con Entity Framework Core y SQLite.
4. Utilizar [Razor Pages con .NET 10](Manual_Razor_Pages_NET_10.pdf) para profundizar en routing, relaciones, LINQ, paginación, API y publicación.

### Fase 3. Diseñar la interfaz

1. Aplicar [Bootstrap y Bootstrap Icons](Manual_Bootstrap_Bootstrap_Icons.pdf).
2. Construir una barra de navegación responsive.
3. Adaptar formularios, tablas y tarjetas a móvil.
4. Añadir iconos con nombres accesibles.
5. Incorporar temas claros, oscuros y Bootswatch.

### Fase 4. Mejorar la interacción

1. Estudiar [SweetAlert2 con Razor Pages](Manual_SweetAlert2_Razor_Pages.pdf).
2. Añadir una confirmación reutilizable a todos los formularios de borrado.
3. Mostrar mensajes procedentes de `TempData`.
4. Revisar antifalsificación, accesibilidad y prevención de XSS.

### Fase 5. Incorporar cuentas y permisos

1. Estudiar [Identity en .NET 10](Identity_NET10.pdf).
2. Crear un proyecto con cuentas individuales.
3. Proteger páginas mediante `[Authorize]` o convenciones.
4. Añadir roles, claims o políticas cuando el caso de uso lo requiera.
5. Relacionar los datos de negocio con el identificador del usuario autenticado.

### Fase 6. Automatizar las pruebas

1. Comenzar con [Pruebas con xUnit y .NET 10](Manual_xUnit_NET_10.pdf).
2. Probar reglas de negocio, servicios y `PageModel`.
3. Probar EF Core mediante SQLite aislado.
4. Añadir pruebas de integración con `WebApplicationFactory`.
5. Continuar con [Playwright para .NET 10](Manual_Playwright_NET_10.pdf).
6. Probar navegación, formularios, filtros, borrados, temas y diseño móvil.

## Itinerarios según el tiempo disponible

### Sesión breve

- Razor Pages en .NET 10: introducción.
- Creación de una página sencilla con `OnGet`.
- Formulario básico con `OnPost`.

### Unidad didáctica

- Razor Pages en ASP.NET Core.
- Agenda de teléfonos.
- Lista de tareas con SQLite.
- Selección de capítulos del manual principal.

### Proyecto trimestral

- Las tres presentaciones.
- Los cinco manuales como documentación de consulta.
- Aplicación CRUD con API propia, autenticación, interfaz responsive y pruebas automatizadas.

## Resultados de aprendizaje

Al completar el itinerario, el alumnado podrá:

- Explicar cómo ASP.NET Core procesa una petición HTTP.
- Crear y organizar proyectos Razor Pages dirigidos a `net10.0`.
- Utilizar sintaxis Razor, Tag Helpers, handlers y model binding.
- Validar formularios en cliente y servidor.
- Implementar CRUD asíncrono con EF Core y SQLite.
- Modelar relaciones y escribir consultas LINQ eficientes.
- Consumir API externas mediante `IHttpClientFactory` y DTO.
- Exponer una API propia y configurar CORS cuando sea necesario.
- Diseñar interfaces responsive con Bootstrap y Bootstrap Icons.
- Mantener temas mediante Bootswatch, `localStorage` y `data-bs-theme`.
- Integrar SweetAlert2 sin romper formularios ni antifalsificación.
- Añadir autenticación y autorización mediante Identity.
- Escribir pruebas unitarias y de integración con xUnit.
- Automatizar flujos de navegador mediante Playwright.
- Aplicar criterios de seguridad, accesibilidad, rendimiento y mantenibilidad.

## Tecnologías cubiertas

- .NET 10 y C#.
- ASP.NET Core y Razor Pages.
- Sintaxis Razor y Tag Helpers.
- Entity Framework Core 10.
- SQLite.
- ASP.NET Core Identity.
- `HttpClient`, JSON, DTO, caché y CORS.
- Bootstrap 5.3.
- Bootstrap Icons y Bootswatch.
- SweetAlert2.
- xUnit v3.
- `WebApplicationFactory`.
- Playwright para .NET.
- JavaScript, `fetch`, `localStorage` y `dataset`.

## Requisitos previos

- Fundamentos de C#: tipos, condiciones, bucles, métodos, clases y colecciones.
- SDK de .NET 10.
- Visual Studio, Visual Studio Code u otro editor compatible con C#.
- Conocimientos básicos de HTML.
- Navegador web moderno.
- PowerShell 7 o entorno equivalente para instalar los navegadores de Playwright.

## Estructura recomendada de la carpeta

```text
materiales-razor-pages-net10/
├── README.md
├── razor_pages_introduccion.pdf
├── Razor Pages en ASP.NET.pdf
├── Identity_NET10.pdf
├── Manual_Razor_Pages_NET_10.pdf
├── Manual_Bootstrap_Bootstrap_Icons.pdf
├── Manual_SweetAlert2_Razor_Pages.pdf
├── Manual_xUnit_NET_10.pdf
└── Manual_Playwright_NET_10.pdf
```

## Organización alternativa del repositorio

Si se prefiere separar los materiales por formato:

```text
materiales-razor-pages-net10/
├── README.md
├── presentaciones/
│   ├── README.md
│   ├── razor_pages_introduccion.pdf
│   ├── Razor Pages en ASP.NET.pdf
│   └── Identity_NET10.pdf
└── manuales/
    ├── README.md
    ├── Manual_Razor_Pages_NET_10.pdf
    ├── Manual_Bootstrap_Bootstrap_Icons.pdf
    ├── Manual_SweetAlert2_Razor_Pages.pdf
    ├── Manual_xUnit_NET_10.pdf
    └── Manual_Playwright_NET_10.pdf
```

En esta segunda estructura deberán ajustarse los enlaces relativos del README principal para incluir `presentaciones/` o `manuales/` delante de cada nombre de archivo.

## Uso de los materiales

Los documentos pueden utilizarse como apoyo para explicaciones, prácticas guiadas, proyectos y consulta. Antes de iniciar una nueva edición del curso, conviene revisar las versiones de .NET, EF Core, Bootstrap, SweetAlert2, xUnit y Playwright, además de los avisos de seguridad de sus dependencias.
