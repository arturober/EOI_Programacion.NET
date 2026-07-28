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

