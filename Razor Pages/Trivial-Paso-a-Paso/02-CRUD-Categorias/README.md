# Versión 2: CRUD de categorías

Esta etapa parte de `01-Listados` y añade la primera funcionalidad de escritura.
Se aprende el ciclo completo de un formulario Razor utilizando la
entidad más sencilla del proyecto.

No se elimina ningún listado ni se cambia la estructura de datos anterior.

## Objetivos

- Comprender la diferencia entre GET y POST.
- Recibir formularios con `[BindProperty]`.
- Utilizar Tag Helpers.
- Validar datos en el servidor.
- Insertar, actualizar y borrar con Entity Framework.
- Aplicar Post/Redirect/Get.
- Enviar mensajes mediante `TempData`.
- Comprender el borrado en cascada.
- Reutilizar campos con un parcial.

## Funcionalidad nueva

- Crear categorías.
- Editar categorías.
- Eliminar categorías.
- Confirmar el borrado con `confirm()`.
- Validar el nombre.
- Detectar nombres repetidos.
- Mostrar un mensaje después de cada operación.
- Eliminar las preguntas asociadas a una categoría.

Las preguntas siguen siendo de solo lectura.

## Ejecutar

```bash
cd 02-CRUD-Categorias/TrivialApi
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

Publica solo el proyecto `TrivialApi` de esta etapa:

```bash
dotnet publish -c Release -o publicacion
```

En el primer despliegue, el ZIP debe incluir `Data/trivial.db`. En
actualizaciones posteriores, esa copia local puede sobrescribir las categorías
y preguntas modificadas en el servidor. Para conservarlas:

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

Descarga antes una copia de seguridad de la base del servidor. Sube y extrae el
ZIP en `/wwwroot` sin borrar todo el directorio.

El CRUD no tiene autenticación y cualquier visitante podría alterar las
categorías. Consulta el procedimiento completo en el
[README general](../README.md).


## Archivos añadidos

```text
Pages/Categorias
├── Crear.cshtml
├── Crear.cshtml.cs
├── Editar.cshtml
├── Editar.cshtml.cs
└── _Formulario.cshtml
```

## Archivos ampliados

```text
Pages/Categorias/Index.cshtml
Pages/Categorias/Index.cshtml.cs
Pages/Shared/_Layout.cshtml
```

El resto permanece igual que en la versión 1.

## Flujo de creación

```text
GET /Categorias/Crear
        ↓
Formulario vacío
        ↓ POST
OnPostAsync
        ↓
ModelState
        ↓
Add + SaveChangesAsync
        ↓
RedirectToPage
        ↓
Listado con mensaje
```

## Crear.cshtml.cs

La propiedad:

```csharp
[BindProperty]
public Categoria Categoria { get; set; } = new();
```

recibe los controles llamados `Categoria.Nombre`.

Al enviarse el formulario, ASP.NET Core:

1. Crea un objeto `Categoria`.
2. Copia los valores recibidos.
3. Ejecuta los atributos de validación.
4. Guarda los errores en `ModelState`.
5. Llama a `OnPostAsync`.

El primer control es:

```csharp
if (!ModelState.IsValid)
{
    return Page();
}
```

`Page()` vuelve a representar el formulario sin redirigir y mantiene errores y
valores introducidos.

## Comprobación de duplicados

Antes de insertar se consulta:

```csharp
bool nombreRepetido = await contexto.Categorias.AnyAsync(...);
```

`AnyAsync` devuelve un booleano y no carga una colección completa.

Si el nombre existe:

```csharp
ModelState.AddModelError(
    "Categoria.Nombre",
    "Ya existe una categoría con ese nombre.");
```

El texto aparece junto al control gracias a `asp-validation-for`.

Además, `TrivialContext` conserva el índice único. La validación ofrece un
mensaje amigable y el índice protege la integridad de la base.

## Inserción

```csharp
contexto.Categorias.Add(Categoria);
await contexto.SaveChangesAsync();
```

`Add` no escribe todavía en SQLite. Cambia el estado interno de la entidad a
`Added`. `SaveChangesAsync` genera y ejecuta el `INSERT`.

## Post/Redirect/Get

Después de guardar:

```csharp
return RedirectToPage("Index");
```

La redirección provoca una nueva petición GET. Si el usuario actualiza el
navegador ya no está repitiendo el POST.

## TempData

Antes de redirigir:

```csharp
TempData["Mensaje"] =
    "La categoría se ha creado correctamente.";
```

`TempData` conserva el dato durante la siguiente petición. `_Layout.cshtml`
comprueba su existencia y muestra una alerta Bootstrap.

En la versión definitiva se mantendrá `TempData`; únicamente se cambiará su
presentación por SweetAlert.

## Formulario parcial

`_Formulario.cshtml` contiene el campo `Nombre`.

Crear y Editar lo insertan mediante:

```razor
<partial name="_Formulario" model="Model.Categoria" />
```

Ventajas:

- El control solo se escribe una vez.
- Las validaciones se muestran igual.
- Los cambios posteriores afectan a ambos formularios.

## Tag Helpers utilizados

| Tag Helper | Función |
|---|---|
| `asp-for` | Relaciona el control con una propiedad |
| `asp-validation-for` | Muestra el error de una propiedad |
| `asp-validation-summary` | Muestra errores generales |
| `asp-page` | Genera una URL de Razor Pages |
| `asp-route-id` | Añade el Id a la URL |
| `asp-page-handler` | Selecciona un handler POST |

## Flujo de edición

La ruta:

```razor
@page "{id:int}"
```

acepta direcciones como:

```text
/Categorias/Editar/3
```

`OnGetAsync(int id)` busca la entidad:

```csharp
Categoria? categoria =
    await contexto.Categorias.FindAsync(id);
```

Si no existe se devuelve `NotFound()`.

El formulario incluye:

```razor
<input asp-for="Categoria.Id" type="hidden" />
```

Ese campo conserva la identidad de la fila durante el POST.

## Actualización

La entidad procede del formulario, no de una consulta en ese POST. Por eso se
conecta al contexto:

```csharp
contexto.Attach(Categoria).State = EntityState.Modified;
await contexto.SaveChangesAsync();
```

Entity Framework genera un `UPDATE`.

## Flujo de eliminación

El formulario utiliza:

```razor
asp-page-handler="Eliminar"
```

Por convención, se ejecuta:

```csharp
OnPostEliminarAsync(int id)
```

El Id se envía mediante un campo oculto.

La confirmación provisional:

```html
onsubmit="return confirm(...);"
```

cancela el POST si el usuario responde negativamente.

## Borrado en cascada

Al ejecutar:

```csharp
contexto.Categorias.Remove(categoria);
await contexto.SaveChangesAsync();
```

la configuración del contexto hace que también se eliminen las preguntas
dependientes.

Debe explicarse que no es un segundo bucle de borrado escrito en el
PageModel. Es una regla de la relación.

## Diferencias con la versión 1

| Antes | Ahora |
|---|---|
| Solo GET | GET y POST |
| Solo consultas | Inserción, actualización y borrado |
| Listado sin acciones | Botones Crear, Editar y Eliminar |
| Sin mensajes | `TempData` y alerta Bootstrap |
| Validaciones no visibles | Errores junto al formulario |

## Pruebas manuales

1. Crear una categoría válida.
2. Comprobar el mensaje.
3. Intentar crearla sin nombre.
4. Intentar repetir un nombre.
5. Editar una categoría.
6. Intentar editarla con un nombre ya utilizado.
7. Cancelar un formulario.
8. Cancelar un borrado.
9. Confirmar un borrado.
10. Escribir manualmente un Id inexistente en la URL de edición.

## Precaución con los datos

Borrar una categoría elimina sus preguntas. Cada versión tiene su propia base,
pero conviene trabajar sobre una copia si se quieren conservar los 1.000 datos.

## Preguntas para evaluar los conceptos aprendidos

1. ¿Por qué Crear no necesita `OnGetAsync`?
2. ¿Qué rellena la propiedad marcada con `[BindProperty]`?
3. ¿Quién crea los errores de `ModelState`?
4. ¿Qué diferencia hay entre `Page()` y `RedirectToPage()`?
5. ¿Por qué se utiliza un campo oculto en Editar?
6. ¿Cuándo se ejecuta realmente el `INSERT`?
7. ¿Cómo se relaciona `asp-page-handler` con el método C#?
8. ¿Qué impide nombres duplicados?
9. ¿Por qué se comprueba si `FindAsync` devuelve null?
10. ¿Dónde se configura el borrado en cascada?

## Ejercicios sugeridos

1. Cambiar la longitud máxima del nombre.
2. Añadir un contador total de categorías.
3. Ordenar de forma descendente.
4. Cambiar el texto de los mensajes.
5. Impedir borrar categorías con preguntas.
6. Añadir una validación que rechace nombres de dos caracteres.

## Paso siguiente

La versión 3 aplica el mismo ciclo CRUD a `Pregunta`. La dificultad adicional
será manejar más campos y seleccionar una categoría relacionada.
