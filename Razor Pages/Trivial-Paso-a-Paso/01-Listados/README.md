# Versión 1: consulta de datos con Razor Pages

Esta es la primera etapa del proyecto. Su finalidad es comprender la estructura
de una aplicación Razor Pages que consulta una base SQLite mediante Entity
Framework Core.

La aplicación todavía no modifica datos. No contiene formularios de alta,
edición o borrado, ni API ni cliente JavaScript.

## Objetivos

Al terminar esta versión, se debería poder explicar:

- Qué responsabilidad tiene `Program.cs`.
- Qué representa una entidad.
- Qué representa un `DbContext`.
- Qué relación existe entre `Categoria` y `Pregunta`.
- Qué diferencia hay entre `.cshtml` y `.cshtml.cs`.
- Cuándo se ejecuta una consulta de Entity Framework.
- Por qué se utiliza `async` y `await`.
- Cómo Razor transforma objetos C# en HTML.
- Cómo Bootstrap mejora el diseño sin CSS propio.

## Funcionalidad

- Página de inicio.
- Recuento de preguntas.
- Recuento de categorías.
- Listado completo de categorías.
- Número de preguntas de cada categoría.
- Listado de las primeras 25 preguntas.
- Nombre de la categoría de cada pregunta.
- Navegación responsive.

## Lo que todavía no contiene

- Creación.
- Edición.
- Eliminación.
- Búsqueda.
- Paginación completa.
- Controladores de API.
- DTO.
- JavaScript propio.
- Cliente del juego.
- Temas.
- SweetAlert.

## Ejecutar

```bash
cd 01-Listados/TrivialApi
dotnet restore
dotnet run
```

Abra la dirección indicada en la terminal.

## Publicación en MonsterASP.NET

> **Método recomendado:** utiliza WebFTP. El archivo ZIP se conserva como
> alternativa.

Publica únicamente `01-Listados/TrivialApi`:

```bash
dotnet publish -c Release -o publicacion
```

```powershell
Compress-Archive -Path .\publicacion\* -DestinationPath .\publicacion.zip -Force
```

Sube y extrae el ZIP en `/wwwroot`. El primer despliegue debe incluir
`publicacion/Data/trivial.db`, que contiene las 1.000 preguntas. Esta etapa es
de solo lectura, por lo que una actualización no pierde modificaciones hechas
desde la interfaz.

No publiques las siete versiones a la vez. Consulta las instrucciones completas
en el [README general](../README.md).

### Método recomendado: WebFTP

Para estos proyectos se recomienda **WebFTP**, el cliente FTP que
MonsterASP.NET ofrece en el navegador. No requiere instalar programas ni crear
o extraer un ZIP.

1. Desde la terminal integrada de VS Code, genera o actualiza la carpeta
   `publicacion` con el comando `dotnet publish` indicado anteriormente.
2. En el panel de MonsterASP.NET abre el sitio y entra en
   **Files → WebFTP**. También se puede acceder a
   [WebFTP](https://webftp.monsterasp.net/) desde el enlace que muestra el
   panel.
3. Dentro de WebFTP, abre `/wwwroot`.
4. Sube **el contenido** de `publicacion`, no la carpeta como un nivel
   adicional. `web.config`, el ensamblado principal, `appsettings.json` y
   las demás carpetas publicadas deben quedar directamente en `/wwwroot`.
5. En las actualizaciones, conserva las bases de datos y los demás archivos
   persistentes indicados en este README.
6. Si algún archivo está bloqueado, reinicia o detén temporalmente el sitio.
   También puedes subir `app_offline.htm` a `/wwwroot`, completar la
   transferencia, eliminarlo y volver a iniciar la aplicación.
7. Abre la dirección HTTPS del sitio y realiza las comprobaciones específicas
   indicadas en este README.

Como segunda opción, para transferencias grandes o frecuentes, puede utilizarse
FileZilla u otro cliente FTP/SFTP con las credenciales disponibles en
**Deploy (FTP/WebDeploy/Git)**. La
[guía oficial de FTP/SFTP](https://help.monsterasp.net/books/deploy/page/how-to-deploy-website-content-via-ftpsftp)
explica esta alternativa.

### Alternativa: despliegue mediante ZIP

El procedimiento mediante ZIP descrito anteriormente se mantiene disponible.
Desde **Files**, sube `publicacion.zip`, pulsa **Unzip** y elige `/wwwroot`
como destino. Al actualizar una aplicación existente, marca
**Overwrite files in target path** y
**Restart application pool before unzip** para sustituir los archivos en uso.
El contenido publicado debe quedar directamente en `/wwwroot`, sin una
carpeta `publicacion` intermedia, y deben respetarse las indicaciones de este
README sobre bases de datos y otros archivos persistentes.

Consulta la
[guía oficial de despliegue mediante ZIP](https://help.monsterasp.net/books/deploy/page/how-to-deploy-website-content-from-zip-file).

## Estructura

```text
01-Listados
├── README.md
├── Trivial.sln
└── TrivialApi
    ├── Data
    │   ├── TrivialContext.cs
    │   └── trivial.db
    ├── Models
    │   ├── Categoria.cs
    │   └── Pregunta.cs
    ├── Pages
    │   ├── Categorias
    │   │   ├── Index.cshtml
    │   │   └── Index.cshtml.cs
    │   ├── Preguntas
    │   │   ├── Index.cshtml
    │   │   └── Index.cshtml.cs
    │   ├── Shared
    │   │   └── _Layout.cshtml
    │   ├── Index.cshtml
    │   ├── Index.cshtml.cs
    │   ├── _ViewImports.cshtml
    │   └── _ViewStart.cshtml
    ├── Program.cs
    ├── TrivialApi.csproj
    └── appsettings.json
```

## 1. Configuración del proyecto

`TrivialApi.csproj` indica:

- Que se trata de un proyecto web.
- Que utiliza .NET 10.
- Que el análisis de valores nulos está activado.
- Que se emplean `using` implícitos.
- Que se necesita el proveedor SQLite de Entity Framework.
- Que la base debe copiarse al compilar y publicar.

Los paquetes relevantes son:

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite"
                  Version="10.0.10" />
<PackageReference Include="SQLitePCLRaw.bundle_e_sqlite3"
                  Version="2.1.12" />
```

La segunda referencia fija una versión corregida de la biblioteca nativa de
SQLite y evita que NuGet restaure la versión vulnerable `2.1.11`.

## 2. Cadena de conexión

`appsettings.json` contiene:

```json
"Trivial": "Data Source=Data/trivial.db"
```

`Trivial` es el nombre de la cadena y `Data/trivial.db` es la ruta al archivo.

No se escribe la cadena directamente en cada página. Se configura una vez y se
recupera desde `Program.cs`.

## 3. Configuración de servicios

`Program.cs` registra:

```csharp
builder.Services.AddRazorPages();
```

Esto permite localizar las páginas de la carpeta `Pages`.

También registra:

```csharp
builder.Services.AddDbContext<TrivialContext>(...);
```

Gracias a este registro, los PageModels pueden solicitar `TrivialContext` en
su constructor sin crearlo manualmente.

Finalmente:

```csharp
app.MapRazorPages();
app.Run();
```

El primer método publica las rutas y el segundo inicia el servidor.

## 4. Modelo Categoria

`Categoria` representa una fila de la tabla `Categorias`.

Propiedades:

| Propiedad | Finalidad |
|---|---|
| `Id` | Clave primaria |
| `Nombre` | Nombre visible |
| `Preguntas` | Colección relacionada |

La colección:

```csharp
public List<Pregunta> Preguntas { get; set; } = [];
```

se inicializa vacía para evitar valores nulos.

Los atributos de validación ya están presentes. En esta versión documentan las
reglas del modelo; se utilizarán en los formularios posteriores.

## 5. Modelo Pregunta

La entidad contiene:

- Identificador.
- Enunciado.
- Cuatro respuestas.
- Número de respuesta correcta.
- Clave ajena de categoría.
- Propiedad de navegación.

`CategoriaId` es el valor almacenado en la tabla. `Categoria` permite acceder
al objeto relacionado cuando se carga la relación.

## 6. TrivialContext

El contexto hereda de `DbContext`:

```csharp
public class TrivialContext(...) : DbContext(...)
```

Sus `DbSet` representan las tablas:

```csharp
public DbSet<Categoria> Categorias => Set<Categoria>();
public DbSet<Pregunta> Preguntas => Set<Pregunta>();
```

`OnModelCreating` configura:

- Índice único para el nombre de la categoría.
- Relación uno a muchos.
- Clave ajena.
- Borrado en cascada.

Aunque todavía no se borra nada, la relación se define desde el principio para
que el modelo permanezca estable en todas las versiones.

## 7. Página de inicio

`Pages/Index.cshtml.cs` ejecuta:

```csharp
TotalPreguntas = await contexto.Preguntas.CountAsync();
TotalCategorias = await contexto.Categorias.CountAsync();
```

`CountAsync` genera consultas de recuento en SQLite. No carga las 1.000
preguntas para contarlas.

`Pages/Index.cshtml` accede a esos valores con:

```razor
@Model.TotalPreguntas
@Model.TotalCategorias
```

## 8. Listado de categorías

La consulta utiliza:

```csharp
contexto.Categorias
    .Include(categoria => categoria.Preguntas)
    .OrderBy(categoria => categoria.Nombre)
    .ToListAsync();
```

`Include` carga las preguntas relacionadas para poder consultar
`categoria.Preguntas.Count`.

`ToListAsync` es el punto en el que la consulta se envía a SQLite.

## 9. Listado de preguntas

La consulta comienza como `IQueryable`:

```csharp
IQueryable<Pregunta> consulta = contexto.Preguntas
    .Include(pregunta => pregunta.Categoria);
```

Después se completa:

```csharp
Preguntas = await consulta
    .OrderBy(pregunta => pregunta.Enunciado)
    .Take(25)
    .ToListAsync();
```

`Take(25)` se traduce a SQL. La aplicación no recibe las 1.000 filas.

Esta estructura se ha elegido porque en la versión 4 se añadirán filtros y
paginación alrededor de la misma consulta.

## 10. PageModel y vista

Cada Razor Page se divide en:

```text
Index.cshtml.cs → consulta y prepara datos
Index.cshtml    → genera el HTML
```

La vista no abre conexiones ni escribe consultas. El PageModel no construye
etiquetas HTML.

## 11. Layout

`_ViewStart.cshtml` establece:

```razor
Layout = "_Layout";
```

El layout contiene:

- `head`.
- Bootstrap.
- Navbar.
- Menú hamburguesa.
- `RenderBody`.
- Footer.
- JavaScript de Bootstrap.

Cada página aporta únicamente su contenido principal.

## 12. Recorrido de una petición

Al abrir `/Preguntas`:

1. Routing localiza `Pages/Preguntas/Index`.
2. ASP.NET Core crea `IndexModel`.
3. Inyecta `TrivialContext`.
4. Ejecuta `OnGetAsync`.
5. Entity Framework genera SQL.
6. SQLite devuelve 25 filas.
7. El PageModel guarda los objetos en `Preguntas`.
8. Razor ejecuta el `foreach`.
9. El servidor devuelve HTML al navegador.

## Pruebas manuales

1. Abrir la página de inicio.
2. Confirmar que aparecen 1.000 preguntas y 10 categorías.
3. Abrir Categorías.
4. Comprobar que están ordenadas.
5. Comprobar el número de preguntas.
6. Abrir Preguntas.
7. Confirmar que solo aparecen 25 filas.
8. Reducir el ancho del navegador.
9. Comprobar el menú hamburguesa.
10. Comprobar el desplazamiento de la tabla.

## Preguntas para evaluar los conceptos aprendidos

1. ¿Por qué `Categoria` tiene una lista de preguntas?
2. ¿Dónde se encuentra realmente la clave ajena?
3. ¿Qué diferencia hay entre `CategoriaId` y `Categoria`?
4. ¿Qué método ejecuta la consulta?
5. ¿Qué ocurriría si se quitara `Include`?
6. ¿Por qué se utiliza `Take(25)`?
7. ¿Qué ventaja aporta `CountAsync`?
8. ¿Qué recibe el constructor del PageModel?
9. ¿Qué función cumple `RenderBody`?
10. ¿Por qué la vista no accede directamente a SQLite?

## Ejercicios sugeridos

1. Cambiar el límite de 25 a 10.
2. Ordenar las preguntas por Id.
3. Mostrar el Id de cada categoría.
4. Mostrar el Id de cada pregunta.
5. Añadir una tarjeta con el nombre de la primera categoría.
6. Cambiar el texto del footer.
7. Añadir un enlace de navegación sin modificar el PageModel.

## Paso siguiente

La versión 2 conserva todos estos archivos y añade el CRUD de categorías. El
listado dejará de ser exclusivamente informativo y recibirá acciones para
crear, modificar y eliminar.
