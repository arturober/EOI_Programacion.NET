# Versión 4: búsqueda, filtrado y paginación

Esta etapa no añade nuevas operaciones CRUD. Mejora la consulta y presentación
del listado de preguntas para trabajar cómodamente con 1.000 filas.

Se amplían únicamente el PageModel, su vista y un archivo JavaScript.

## Objetivos

- Representar filtros mediante parámetros GET.
- Construir progresivamente un `IQueryable`.
- Combinar filtros.
- Normalizar texto.
- Ignorar mayúsculas y tildes.
- Calcular páginas.
- Utilizar `Skip` y `Take`.
- Conservar parámetros al navegar.
- Comprender un debounce.
- Recuperar el foco después de una recarga.

## Funcionalidad nueva

- Búsqueda por parte del enunciado.
- Comparación sin distinguir mayúsculas.
- Comparación sin distinguir tildes.
- Filtro por categoría.
- Combinación de ambos filtros.
- 25 preguntas por página.
- Enlaces Anterior y Siguiente.
- Conservación de los filtros.
- Búsqueda automática a los 300 ms.
- Foco y cursor restaurados.

## Ejecutar

```bash
cd 04-Busqueda-Paginacion/TrivialApi
dotnet restore
dotnet run
```

## Publicación en MonsterASP.NET

> **Método recomendado:** genera los archivos con **Publish** de VS Code o con
> `dotnet publish -c Release`, detén la aplicación si vas a actualizarla y
> sube los archivos sin comprimir mediante **WebFTP**, seleccionando la opción
> de sobrescritura. El ZIP se conserva como alternativa.

### Generar los archivos de publicación

Hay dos procedimientos recomendados. Ambos producen la misma aplicación
preparada para desplegarse.

#### Opción 1: Publish desde VS Code

1. En el explorador de soluciones de VS Code, haz clic con el botón derecho
   —o utiliza el clic secundario— sobre el **proyecto web**.
2. Selecciona **Publish**.
3. Espera a que termine y consulta en la salida de VS Code la carpeta de
   destino.
4. Utiliza el contenido de esa carpeta para el despliegue.

Si **Publish** no aparece, utiliza la terminal. La disponibilidad de esta
acción depende de las herramientas de C# instaladas en VS Code.

#### Opción 2: carpeta predeterminada de .NET

Desde la terminal integrada de VS Code, situada en la carpeta que contiene el
archivo `.csproj`, ejecuta:

```bash
dotnet publish -c Release
```

En estos proyectos .NET 10, los archivos se generan normalmente en:

```text
bin/Release/net10.0/publish
```

Si se especifica un runtime o un perfil cambia la configuración, la ruta puede
incluir carpetas adicionales. La terminal muestra siempre la ubicación
utilizada. Debe desplegarse **el contenido** de `publish`, no sus carpetas
padre.

Como alternativa puede elegirse una carpeta más fácil de localizar:

```bash
dotnet publish -c Release -o publicacion
```

En ese caso se utilizará el contenido de `publicacion`. Esta opción se
mantiene en algunos ejemplos posteriores para preparar el ZIP, pero la carpeta
predeterminada de .NET es la recomendada.

### Método recomendado: WebFTP sin comprimir

WebFTP permite subir directamente los archivos y carpetas publicados; no es
necesario comprimirlos.

#### Primera publicación

1. En el panel de MonsterASP.NET abre el sitio y entra en
   **Files → WebFTP**. También puedes abrir
   [WebFTP](https://webftp.monsterasp.net/) desde el enlace del panel.
2. En WebFTP, entra en `/wwwroot`.
3. Selecciona **el contenido** de la carpeta generada por Publish o
   `dotnet publish` y súbelo sin comprimir. No subas la propia carpeta
   `publish` o `publicacion` como un nivel adicional.
4. Si WebFTP encuentra archivos existentes, elige **Overwrite** o
   **Overwrite all**.
5. Comprueba que `web.config`, el ensamblado principal, `appsettings.json` y
   las demás carpetas publicadas están directamente en `/wwwroot`.
6. Abre la dirección HTTPS y realiza las comprobaciones específicas indicadas
   en este README.

#### Actualización de una aplicación existente

> **Importante:** antes de sobrescribir los archivos, detén la aplicación con
> **Stop** desde el panel de MonsterASP.NET. Así se evitan archivos bloqueados y
> que la web se ejecute temporalmente con componentes de versiones distintas.

1. Pulsa **Stop** en las acciones rápidas del sitio.
2. Con la aplicación detenida, descarga una copia de seguridad de la base de
   datos y de cualquier otro archivo persistente.
3. En WebFTP, abre `/wwwroot` y sube sin comprimir el contenido de la nueva
   publicación.
4. Selecciona **Overwrite** o **Overwrite all**, pero conserva las bases de
   datos y demás archivos que este README indique que no deben sustituirse.
5. Cuando termine la transferencia, pulsa **Start**.
6. Comprueba la aplicación y revisa los registros si se produce algún error.

Como alternativa a **Stop**, puede subirse primero `app_offline.htm`, realizar
la actualización y eliminarlo al terminar. Para transferencias grandes o
frecuentes también puede utilizarse FileZilla u otro cliente FTP/SFTP con las
credenciales disponibles en **Deploy (FTP/WebDeploy/Git)**. Consulta la
[guía oficial de FTP/SFTP](https://help.monsterasp.net/books/deploy/page/how-to-deploy-website-content-via-ftpsftp).

### Alternativa: despliegue mediante ZIP

Las instrucciones específicas para crear el ZIP se mantienen más adelante en
este apartado. En MonsterASP.NET:

1. Detén primero la aplicación si se trata de una actualización.
2. Abre **Files**, sube `publicacion.zip` y pulsa **Unzip**.
3. Elige `/wwwroot` como destino.
4. En una actualización, marca **Overwrite files in target path** y
   **Restart application pool before unzip**.
5. Conserva las bases de datos y demás archivos persistentes indicados en este
   README.
6. Inicia o reinicia la aplicación y comprueba su funcionamiento.

Consulta la
[guía oficial de despliegue mediante ZIP](https://help.monsterasp.net/books/deploy/page/how-to-deploy-website-content-from-zip-file).

Publica solo el `TrivialApi` de esta versión:

```bash
dotnet publish -c Release -o publicacion
```

Incluye `Data/trivial.db` en el primer despliegue. Para una actualización que
deba conservar las modificaciones del servidor:

```powershell
Remove-Item .\publicacion\Data\trivial.db
Compress-Archive -Path .\publicacion\* -DestinationPath .\publicacion.zip -Force
```

En Linux o macOS (si el comando no está disponible, instala el paquete
`zip` desde el gestor de paquetes del sistema):

```bash
rm -f publicacion/Data/trivial.db publicacion.zip
(cd publicacion && zip -r ../publicacion.zip .)
```

Descarga antes la base del servidor, sube el ZIP a `/wwwroot` y no borres el
directorio completo. Comprueba después búsqueda, filtro, paginación y
persistencia de una pregunta modificada.

La administración no tiene autenticación. Consulta el procedimiento detallado
en el [README general](../README.md).


## Archivos ampliados

```text
Pages/Preguntas/Index.cshtml
Pages/Preguntas/Index.cshtml.cs
```

Crear, Editar y Eliminar no cambian.

## Propiedades de filtro

```csharp
[BindProperty(SupportsGet = true)]
public string? Busqueda { get; set; }
```

`SupportsGet` indica que la propiedad puede recibir valores desde la URL.

Las tres propiedades son:

- `Busqueda`.
- `CategoriaId`.
- `Pagina`.

Ejemplo:

```text
/Preguntas?Busqueda=capital&CategoriaId=3&Pagina=2
```

Ventajas del método GET:

- La dirección puede copiarse.
- El navegador mantiene el historial.
- Los filtros pueden conservarse en enlaces.
- Actualizar la página repite una consulta, no una modificación.

## Consulta inicial

Se conserva:

```csharp
IQueryable<Pregunta> consulta = contexto.Preguntas
    .Include(pregunta => pregunta.Categoria);
```

Todavía no se ejecuta.

## Filtro de categoría

```csharp
if (CategoriaId.HasValue)
{
    consulta = consulta.Where(
        pregunta =>
            pregunta.CategoriaId == CategoriaId);
}
```

Este filtro se traduce a SQL porque compara una columna numérica.

## Búsqueda sin tildes

La función `Normalizar`:

1. Separa vocales y tildes con `FormD`.
2. Recorre los caracteres.
3. Descarta los de categoría `NonSpacingMark`.
4. Convierte el resto a minúsculas.

Ejemplos:

```text
ÁRBOL   → arbol
árbol   → arbol
Arbol   → arbol
```

La misma función se aplica al texto buscado y al enunciado.

## Por qué parte del filtro se realiza en C#

SQLite puede filtrar fácilmente por `CategoriaId`, pero la normalización
didáctica de tildes está escrita en C#.

Por eso:

1. SQLite aplica primero la categoría.
2. Se ejecuta la consulta.
3. C# aplica la búsqueda normalizada.
4. C# pagina la colección resultante.

La base está limitada a 1.000 preguntas, por lo que el enfoque sigue siendo
razonable y fácil de explicar.

Para una base muy grande habría que trasladar la normalización a la base,
guardar una columna normalizada o utilizar un motor de búsqueda.

## Cálculo de páginas

```csharp
TotalPaginas = Math.Max(
    1,
    (int)Math.Ceiling(
        TotalResultados / (double)TamanoPagina));
```

La conversión a `double` evita una división entera.

`Ceiling` redondea hacia arriba:

```text
1 resultado   → 1 página
25 resultados → 1 página
26 resultados → 2 páginas
```

`Math.Max` conserva una página incluso con cero resultados.

## Corregir la página

```csharp
Pagina = Math.Clamp(Pagina, 1, TotalPaginas);
```

Si alguien escribe `Pagina=-20` o `Pagina=999`, la aplicación selecciona un
valor válido.

## Seleccionar una página

```csharp
.Skip((Pagina - 1) * TamanoPagina)
.Take(TamanoPagina)
```

Para página 3 y tamaño 25:

```text
Skip(50)
Take(25)
```

## Formulario de filtros

El formulario utiliza:

```html
method="get"
```

No necesita botón porque JavaScript lo envía automáticamente.

El selector usa la misma técnica `SelectList` aprendida en el CRUD.

## Paginación y conservación de filtros

Los enlaces contienen:

```razor
asp-route-busqueda="@Model.Busqueda"
asp-route-categoriaId="@Model.CategoriaId"
asp-route-pagina="..."
```

Cambiar de página no pierde la búsqueda ni la categoría.

## Conservar filtros después de borrar

El formulario de eliminación incluye campos ocultos para:

- Búsqueda.
- Categoría.
- Página.

El handler redirige con esos valores.

## Debounce de 300 ms

Sin retraso, cada letra enviaría inmediatamente un formulario.

El archivo JavaScript:

1. Cancela el temporizador anterior.
2. Crea otro de 300 ms.
3. Solo envía si no se ha escrito otra letra.

```javascript
clearTimeout(temporizadorBusqueda);
temporizadorBusqueda = setTimeout(..., 300);
```

Esto se denomina debounce.

## `requestSubmit`

Se utiliza:

```javascript
formularioBusqueda.requestSubmit();
```

en vez de llamar directamente a `submit()`. `requestSubmit` reproduce el envío
normal de un formulario.

## Recuperar foco y cursor

Después de recargar:

```javascript
entradaBusqueda.focus();
entradaBusqueda.setSelectionRange(
    entradaBusqueda.value.length,
    entradaBusqueda.value.length
);
```

El usuario puede seguir escribiendo al final del texto.

## Diferencias con la versión 3

| Versión 3 | Versión 4 |
|---|---|
| Primeras 25 preguntas | Página seleccionada |
| Sin filtros | Texto y categoría |
| Sin parámetros GET | Estado visible en URL |
| Sin JavaScript propio | Debounce de 300 ms |
| No calcula totales | Resultados y páginas |

## Pruebas manuales

1. Buscar una palabra exacta.
2. Repetirla con mayúsculas.
3. Repetirla sin tildes.
4. Elegir una categoría.
5. Combinar texto y categoría.
6. Cambiar de página.
7. Copiar y abrir la URL.
8. Escribir con rapidez.
9. Comprobar la posición del cursor.
10. Buscar algo inexistente.
11. Escribir `Pagina=999` manualmente.
12. Borrar una pregunta manteniendo filtros.

## Preguntas para evaluar los conceptos aprendidos

1. ¿Por qué los filtros utilizan GET?
2. ¿Cuándo se ejecuta `IQueryable`?
3. ¿Qué filtro ejecuta SQLite?
4. ¿Qué filtro ejecuta C#?
5. ¿Por qué se utiliza `Ceiling`?
6. ¿Qué hace `Skip`?
7. ¿Qué hace `Take`?
8. ¿Por qué se conservan los filtros en los enlaces?
9. ¿Qué problema evita el debounce?
10. ¿Por qué se llama a `clearTimeout`?

## Ejercicios sugeridos

1. Cambiar a 10 preguntas por página.
2. Añadir botones para primera y última página.
3. Mostrar el número de cada resultado.
4. Añadir búsqueda en respuestas.
5. Permitir seleccionar el tamaño de página.
6. Añadir un botón para limpiar filtros.
7. Cambiar el retraso a 500 ms.

## Paso siguiente

La versión 5 mantiene la administración completa y añade otra forma de acceder
a los mismos datos: una API REST que devuelve JSON.
