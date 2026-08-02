# Versión 3: CRUD de preguntas

Esta etapa conserva los listados y el CRUD de categorías. Añade la
administración completa de preguntas.

El patrón GET, POST, validación y redirección es el mismo de la versión
anterior. La novedad es la relación con `Categoria` y un formulario con más
campos.

## Objetivos

- Reutilizar el patrón CRUD aprendido.
- Trabajar con claves ajenas.
- Construir desplegables mediante `SelectList`.
- Validar que una entidad relacionada exista.
- Recargar datos auxiliares tras una validación incorrecta.
- Reutilizar un formulario parcial grande.
- Comprender `Include`.

## Funcionalidad nueva

- Crear preguntas.
- Editar preguntas.
- Eliminar preguntas.
- Introducir el enunciado.
- Introducir cuatro respuestas.
- Seleccionar la respuesta correcta.
- Elegir una categoría.
- Validar todos los campos.
- Confirmar el borrado.

El listado todavía muestra solo 25 preguntas y no tiene búsqueda.

## Ejecutar

```bash
cd 03-CRUD-Preguntas/TrivialApi
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

Desde la carpeta `TrivialApi`:

```bash
dotnet publish -c Release -o publicacion
```

La primera publicación debe conservar `publicacion/Data/trivial.db`. En las
actualizaciones, retírala antes de crear el ZIP si quieres mantener las
preguntas creadas o editadas en el servidor:

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

Haz primero una copia de seguridad y extrae el ZIP en `/wwwroot` sin eliminar
todo el sitio. El CRUD de categorías y preguntas es público y debe protegerse
antes de usar datos que no sean de práctica.

Consulta las instrucciones completas en el
[README general](../README.md).


## Archivos añadidos

```text
Pages/Preguntas
├── Crear.cshtml
├── Crear.cshtml.cs
├── Editar.cshtml
├── Editar.cshtml.cs
└── _Formulario.cshtml
```

## Archivos ampliados

```text
Pages/Preguntas/Index.cshtml
Pages/Preguntas/Index.cshtml.cs
Pages/_ViewImports.cshtml
```

`_ViewImports.cshtml` incorpora el espacio de nombres necesario para
`SelectList`.

## Estructura del formulario

El formulario contiene:

| Campo | Control |
|---|---|
| Enunciado | `textarea` |
| Respuesta 1 | `input` |
| Respuesta 2 | `input` |
| Respuesta 3 | `input` |
| Respuesta 4 | `input` |
| Respuesta correcta | `select` |
| Categoría | `select` |

Bootstrap utiliza `col-md-6` para colocar dos campos por fila en pantallas
medianas y una columna en móvil.

## Respuesta correcta

La base guarda un número del 1 al 4:

```csharp
public int RespuestaCorrecta { get; set; }
```

El formulario muestra textos más comprensibles:

```razor
<option value="1">Respuesta 1</option>
```

El valor enviado sigue siendo un entero.

## Clave ajena

`Pregunta` contiene:

```csharp
public int CategoriaId { get; set; }
public Categoria? Categoria { get; set; }
```

El formulario modifica `CategoriaId`. La propiedad `Categoria` se utiliza al
consultar y mostrar el nombre relacionado.

## Cargar el desplegable

El método privado:

```csharp
private async Task CargarCategoriasAsync()
```

consulta las categorías y crea:

```csharp
new SelectList(categorias, "Id", "Nombre")
```

- `Id` se convierte en `value`.
- `Nombre` se convierte en texto visible.

La lista se guarda en:

```csharp
ViewData["Categorias"]
```

y el parcial la consume mediante `asp-items`.

## Por qué se carga en GET y en POST incorrecto

En GET se necesita para mostrar el formulario.

Si el POST falla:

```csharp
if (!ModelState.IsValid)
{
    await CargarCategoriasAsync();
    return Page();
}
```

también se necesita reconstruir. Los valores de `ViewData` no sobreviven de una
petición a otra.

Si se omitiera esa llamada, el desplegable aparecería vacío al mostrar los
errores.

## Validar la categoría

`[Range]` evita valores iguales o inferiores a cero. Sin embargo, un usuario
podría enviar manualmente un Id positivo que no exista.

Por eso se comprueba:

```csharp
bool categoriaExiste = await contexto.Categorias
    .AnyAsync(categoria =>
        categoria.Id == Pregunta.CategoriaId);
```

Si no existe, se añade un error a `Pregunta.CategoriaId`.

## Creación

El flujo es:

1. Recibir `Pregunta`.
2. Comprobar la categoría.
3. Comprobar `ModelState`.
4. Recargar categorías si hay errores.
5. Ejecutar `Add`.
6. Ejecutar `SaveChangesAsync`.
7. Crear el mensaje.
8. Redirigir al listado.

## Edición

El GET carga:

- La pregunta.
- Las categorías.

El POST:

- Recibe la pregunta.
- Valida la categoría.
- Recarga el selector si hay errores.
- Conecta la entidad con `Attach`.
- Marca su estado como `Modified`.
- Guarda.

## Listado

La consulta de la primera versión se conserva:

```csharp
IQueryable<Pregunta> consulta = contexto.Preguntas
    .Include(pregunta => pregunta.Categoria);
```

Se añaden las acciones de edición y eliminación, pero todavía se mantiene:

```csharp
.Take(25)
```

## Eliminación

El mismo listado contiene:

```csharp
OnPostEliminarAsync(int id)
```

El servidor no confía únicamente en el Id recibido. Primero ejecuta
`FindAsync`, comprueba null y solo después elimina.

## Formulario parcial

Crear y Editar reutilizan `_Formulario.cshtml`.

Esto evita duplicar:

- Siete etiquetas.
- Siete controles.
- Siete mensajes de validación.
- La estructura responsive.

## Diferencias con la versión 2

| Versión 2 | Versión 3 |
|---|---|
| CRUD de entidad sencilla | CRUD de entidad relacionada |
| Un único campo | Siete campos |
| Sin lista auxiliar | `SelectList` de categorías |
| Validación básica | Validación de clave ajena |
| Sin `Include` nuevo | Categoría visible en el listado |

## Pruebas manuales

1. Crear una pregunta completa.
2. Dejar vacío el enunciado.
3. Dejar vacía una respuesta.
4. Seleccionar cada respuesta correcta.
5. No seleccionar categoría.
6. Editar todos los campos.
7. Cancelar la edición.
8. Cancelar un borrado.
9. Confirmar el borrado.
10. Crear una categoría y utilizarla en una pregunta.

## Preguntas para evaluar los conceptos aprendidos

1. ¿Por qué se guarda `CategoriaId`?
2. ¿Para qué sirve `Categoria`?
3. ¿Qué datos contiene una `SelectList`?
4. ¿Por qué se carga dos veces la lista?
5. ¿Qué diferencia hay entre las dos listas desplegables?
6. ¿Qué valida `[Range(1, 4)]`?
7. ¿Por qué se utiliza un parcial?
8. ¿Qué hace `Include`?
9. ¿Qué ocurriría si la categoría no existiera?
10. ¿Qué instrucciones generan `INSERT`, `UPDATE` y `DELETE`?

## Ejercicios sugeridos

1. Mostrar el Id en el listado.
2. Ordenar por categoría y después por enunciado.
3. Cambiar a tres respuestas.
4. Añadir una quinta respuesta.
5. Hacer que una nueva pregunta seleccione por defecto una categoría.
6. Mostrar la respuesta correcta en la tabla.
7. Contar cuántas preguntas hay en el listado actual.

## Paso siguiente

La versión 4 mantiene este CRUD y amplía únicamente el listado con búsqueda,
filtro por categoría, paginación y un pequeño JavaScript.
