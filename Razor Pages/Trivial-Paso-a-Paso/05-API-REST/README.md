# Versión 5: API REST de solo lectura

Esta etapa conserva la administración completa con Razor Pages y añade
controladores que publican los mismos datos en formato JSON.

Las Razor Pages no son sustituidas. La aplicación dispone ahora de dos
interfaces sobre el mismo contexto:

```text
Razor Pages  → HTML para personas
Controladores → JSON para programas
```

## Objetivos

- Comprender qué es una API REST.
- Diferenciar una vista HTML de una respuesta JSON.
- Registrar y mapear controladores.
- Definir rutas con atributos.
- Utilizar parámetros de ruta y consulta.
- Devolver códigos HTTP.
- Separar entidades y DTO.
- Configurar CORS.
- Consumir los endpoints desde el navegador.

## Funcionalidad nueva

- Listar categorías mediante JSON.
- Consultar una categoría por Id.
- Obtener preguntas aleatorias.
- Elegir la cantidad.
- Filtrar preguntas por categoría.
- Consultar una pregunta por Id.
- Responder con 404 cuando no existe un elemento.
- Abrir endpoints desde la página principal.

La API no permite crear, modificar o borrar. Es de solo lectura.

## Ejecutar

```bash
cd 05-API-REST/TrivialApi
dotnet restore
dotnet run
```

La página principal construye los enlaces con el servidor y puerto actuales.

## Archivos añadidos

```text
Controllers
├── CategoriasController.cs
└── PreguntasController.cs

DTOs
├── CategoriaDto.cs
└── PreguntaDto.cs
```

## Archivos ampliados

```text
Program.cs
Pages/Index.cshtml
```

Los CRUD no cambian.

## Registrar controladores

En servicios:

```csharp
builder.Services.AddControllers();
```

En endpoints:

```csharp
app.MapControllers();
```

La primera línea registra lo necesario para construir los controladores. La
segunda activa sus rutas.

## Rutas

El controlador declara:

```csharp
[Route("api/categorias")]
```

Y una acción:

```csharp
[HttpGet]
```

La combinación produce:

```text
GET /api/categorias
```

Una acción con:

```csharp
[HttpGet("{id:int}")]
```

produce:

```text
GET /api/categorias/3
```

`int` es una restricción de ruta. `/api/categorias/abc` no coincide con esa
acción.

## DTO

Una entidad describe cómo trabaja la aplicación con la base. Un DTO describe
qué datos publica la API.

`Categoria` incluye una colección de preguntas. `CategoriaDto` contiene:

```csharp
public record CategoriaDto(int Id, string Nombre);
```

La API no expone una navegación completa que pueda producir referencias
circulares.

## PreguntaDto

La entidad tiene cuatro propiedades:

```text
Respuesta1
Respuesta2
Respuesta3
Respuesta4
```

El DTO las agrupa:

```csharp
string[] Respuestas
```

Esto permite recorrerlas fácilmente desde cualquier cliente.

## Consultar categorías

```csharp
return await contexto.Categorias
    .OrderBy(...)
    .Select(...)
    .ToListAsync();
```

`Select` se ejecuta como parte de la consulta. SQLite devuelve únicamente las
columnas necesarias para el DTO.

## Resultado por Id

La firma:

```csharp
Task<ActionResult<CategoriaDto>>
```

permite devolver:

- Un DTO correcto.
- Una respuesta HTTP como `NotFound`.

El resultado:

```csharp
return categoria is null
    ? NotFound()
    : Ok(categoria);
```

corresponde a:

```text
404 Not Found
200 OK
```

## Parámetros de consulta

La acción de preguntas recibe:

```csharp
int? categoriaId,
int cantidad = 10
```

Ejemplos:

```text
/api/preguntas
/api/preguntas?cantidad=20
/api/preguntas?categoriaId=4
/api/preguntas?categoriaId=4&cantidad=20
```

Si `cantidad` no aparece se utiliza 10.

## Limitar cantidad

```csharp
cantidad = Math.Clamp(cantidad, 1, 1000);
```

Valores inferiores a 1 se convierten en 1 y valores superiores a 1.000 se
convierten en 1.000.

## Selección aleatoria

Después de consultar:

```csharp
preguntas
    .OrderBy(_ => Random.Shared.Next())
    .Take(cantidad)
```

La colección se mezcla y después se recorta.

Se carga como máximo una base de 1.000 preguntas. El enfoque se ha escogido por
su claridad didáctica. Para millones de filas sería necesaria otra estrategia.

## Conversión a DTO

`ConvertirDto`:

1. Construye el array de respuestas.
2. Construye `CategoriaDto`.
3. Construye `PreguntaDto`.

La conversión está centralizada y se reutiliza en el listado y en la consulta
por Id.

## CORS

CORS controla si un navegador permite que una página de otro origen consulte
la API.

Se define la política:

```csharp
AllowAnyOrigin()
AllowAnyHeader()
AllowAnyMethod()
```

Esta configuración es apropiada para el ejercicio, pero en una aplicación real
conviene especificar orígenes permitidos.

## Direcciones disponibles

| Método | Dirección | Resultado |
|---|---|---|
| GET | `/api/categorias` | Todas las categorías |
| GET | `/api/categorias/1` | Una categoría |
| GET | `/api/preguntas` | 10 preguntas |
| GET | `/api/preguntas?cantidad=50` | 50 preguntas |
| GET | `/api/preguntas?categoriaId=2` | 10 de una categoría |
| GET | `/api/preguntas/25` | Una pregunta |

## JSON aproximado de categoría

```json
{
  "id": 1,
  "nombre": "Historia"
}
```

## JSON aproximado de pregunta

```json
{
  "id": 25,
  "enunciado": "¿...?",
  "respuestas": [
    "Respuesta A",
    "Respuesta B",
    "Respuesta C",
    "Respuesta D"
  ],
  "respuestaCorrecta": 2,
  "categoria": {
    "id": 1,
    "nombre": "Historia"
  }
}
```

## Diferencias con la versión 4

| Versión 4 | Versión 5 |
|---|---|
| Razor Pages | Razor Pages y controladores |
| Respuesta HTML | HTML y JSON |
| Entidades en PageModel | DTO públicos |
| Rutas de páginas | Rutas `/api` |
| Uso humano | Uso humano y programático |

## Pruebas manuales

1. Abrir `/api/categorias`.
2. Abrir una categoría válida.
3. Abrir una categoría inexistente.
4. Solicitar preguntas sin parámetros.
5. Solicitar otra vez y comprobar el orden.
6. Solicitar una cantidad concreta.
7. Solicitar una categoría concreta.
8. Combinar ambos parámetros.
9. Solicitar una pregunta válida.
10. Solicitar una pregunta inexistente.
11. Pedir cantidad cero.
12. Pedir cantidad superior a 1.000.

## Preguntas para evaluar los conceptos aprendidos

1. ¿Qué diferencia hay entre entidad y DTO?
2. ¿Por qué no se devuelve `Categoria` directamente?
3. ¿Qué genera una respuesta 404?
4. ¿Dónde se define `/api/preguntas`?
5. ¿Qué diferencia hay entre ruta y query string?
6. ¿Por qué `categoriaId` es nullable?
7. ¿Qué valor tiene `cantidad` si se omite?
8. ¿Para qué sirve CORS?
9. ¿Qué método convierte las respuestas en un array?
10. ¿Por qué la API no necesita una vista `.cshtml`?

## Ejercicios sugeridos

1. Añadir endpoint de recuento.
2. Filtrar por texto.
3. Devolver solo preguntas de una categoría.
4. Crear un DTO sin respuesta correcta.
5. Añadir un endpoint de categoría con su número de preguntas.
6. Limitar la cantidad máxima a 100.

## Paso siguiente

La versión 6 añadirá un cliente HTML y JavaScript que usa exclusivamente esta
API. No accederá al contexto ni a SQLite.

