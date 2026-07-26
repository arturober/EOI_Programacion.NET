# Lista de tareas con Razor Pages

Aplicación web didáctica para gestionar tareas y categorías. Está desarrollada
con C#, Razor Pages, SQLite, Bootstrap, Bootstrap Icons y SweetAlert2.

El proyecto está pensado para estudiantes que están aprendiendo C#. Por eso el
código prioriza la claridad: cada clase tiene una responsabilidad sencilla, las
consultas SQL usan parámetros y los métodos tienen nombres descriptivos.

## Funciones principales

- Crear, consultar, modificar y eliminar tareas.
- Marcar una tarea como completada.
- Crear, consultar, modificar y eliminar categorías.
- Filtrar las tareas por categoría.
- Evitar que se elimine una categoría que todavía contiene tareas.
- Mostrar confirmaciones y mensajes con SweetAlert2.
- Ordenar categorías y tareas según la cultura española, incluidas las tildes y
  la letra ñ.
- Adaptar la navegación, las tablas, las tarjetas y los formularios a móviles.

## Organización del proyecto

- `BaseDatos.cs`: abre y devuelve una conexión con SQLite.
- `Models/Categoria.cs`: representa una categoría y contiene su CRUD.
- `Models/Tarea.cs`: representa una tarea y contiene su CRUD.
- `Pages/Tareas`: páginas Razor para gestionar las tareas.
- `Pages/Categorias`: páginas Razor para gestionar las categorías.
- `Pages/Shared/_Layout.cshtml`: navegación, librerías y JavaScript común.
- `lista_tareas.db`: base de datos SQLite con datos de ejemplo.
- `libman.json`: define las librerías web y sus versiones.

## Librerías con LibMan

Las librerías están guardadas dentro de `wwwroot/lib`, por lo que la aplicación
no depende de una CDN para funcionar. LibMan permite restaurarlas mediante el
archivo `libman.json`.

Si no tienes instalada su herramienta de línea de comandos:

```bash
dotnet tool install -g Microsoft.Web.LibraryManager.Cli
```

Para restaurar las librerías:

```bash
libman restore
```

## Ejecución

Necesitas el SDK de .NET 10. Desde la carpeta del proyecto ejecuta:

```bash
dotnet restore
dotnet run
```

Abre en el navegador la dirección que aparezca en la terminal.

## Ideas importantes que se practican

- Mapeo de tablas de SQLite a objetos de C#.
- Operaciones CRUD con `Microsoft.Data.Sqlite`.
- Consultas parametrizadas con `AddWithValue` para evitar inyección SQL.
- Relaciones entre tablas mediante una clave externa.
- Validación de datos con anotaciones como `Required` y `StringLength`.
- Separación entre modelos, páginas Razor y acceso a la conexión.
- Diseño responsive y accesible con Bootstrap.
