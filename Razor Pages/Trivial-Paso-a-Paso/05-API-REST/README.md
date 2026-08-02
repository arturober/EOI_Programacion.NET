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

Desde `05-API-REST/TrivialApi`:

```bash
dotnet publish -c Release -o publicacion
```

El primer despliegue debe incluir `Data/trivial.db`. Para actualizar sin
sobrescribir cambios del servidor:

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

Haz antes una copia de seguridad. Sube y extrae el ZIP en `/wwwroot` sin
borrar todo el sitio.

Después comprueba:

```text
https://tu-sitio.runasp.net/api/categorias
https://tu-sitio.runasp.net/api/preguntas?cantidad=10
```

La API JSON es de solo lectura, pero la administración Razor mantiene el CRUD
público. Esta etapa también registra CORS para que los clientes web externos
puedan consultar esos endpoints durante las prácticas.

Consulta el procedimiento completo en el
[README general](../README.md).


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

Una entidad describe cómo trabaja la aplicación con la base de datos. Un DTO describe
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

## CORS y clientes externos

CORS controla si un navegador permite que una página de otro origen consulte
la API.

Esta etapa registra la política `PermitirTodos`:

```csharp
builder.Services.AddCors(opciones =>
{
    opciones.AddPolicy("PermitirTodos", politica =>
    {
        politica
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
```

Después de `UseRouting` se aplica con:

```csharp
app.UseCors("PermitirTodos");
```

Por ello, el [cliente JavaScript independiente](../Clientes/JavaScript/) puede
conectarse a esta versión o a cualquiera posterior. La configuración abierta es
apropiada para el ejercicio; en una aplicación real conviene especificar
únicamente los orígenes permitidos.

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
