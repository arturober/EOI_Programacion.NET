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

> **Método recomendado:** utiliza WebFTP.

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

Descarga antes una copia de seguridad de la base del servidor. Sube y extrae el
ZIP en `/wwwroot` sin borrar todo el directorio.

El CRUD no tiene autenticación y cualquier visitante podría alterar las
categorías. Consulta el procedimiento completo en el
[README general](../README.md).

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
