# Cliente de consola para la API de trivial

Este proyecto es un cliente de consola muy sencillo desarrollado con .NET 10.
Su finalidad es comprobar que la API de trivial funciona correctamente y
practicar el consumo de una API REST desde C#.

No utiliza Entity Framework ni abre directamente `trivial.db`. Toda la
información se obtiene mediante peticiones HTTP.

## Funcionalidad

El programa:

1. Se conecta con la API.
2. Descarga las categorías.
3. Permite elegir una categoría o utilizar todas.
4. Solicita diez preguntas aleatorias.
5. Muestra las cuatro posibles respuestas.
6. Comprueba cada respuesta.
7. Indica la respuesta correcta cuando se falla.
8. Muestra la puntuación final.

## Estructura

```text
TrivialClienteConsola
├── Program.cs
├── README.md
└── TrivialClienteConsola.csproj
```

Todo el código C# está concentrado en `Program.cs` para que resulte fácil
seguirlo en clase.

## Requisitos

- .NET 10 SDK.
- La API de trivial ejecutándose.

No hay que instalar paquetes NuGet adicionales. `HttpClient` y
`GetFromJsonAsync` forman parte de .NET.

## 1. Ejecutar primero la API

Desde la carpeta de la API:

```bash
dotnet run
```

La terminal mostrará una dirección parecida a:

```text
http://localhost:5074
```

La API debe seguir ejecutándose mientras se utiliza el cliente.

Puede comprobarse manualmente abriendo:

```text
http://localhost:5074/api/categorias
```

Si aparece un JSON con las categorías, el servidor está preparado.

## 2. Ejecutar el cliente

Desde la carpeta de este proyecto:

```bash
dotnet run
```

Por defecto intentará utilizar:

```text
http://localhost:5074
```

## Utilizar otro puerto

Si la API muestra otra dirección, puede pasarse como argumento:

```bash
dotnet run -- http://localhost:5000
```

El separador `--` indica que lo escrito después se entrega al programa y no al
comando `dotnet run`.

También puede utilizarse HTTPS:

```bash
dotnet run -- https://localhost:7001
```

## Dirección base

El programa prepara:

```csharp
string urlApi = $"{urlServidor.TrimEnd('/')}/api/";
```

Si el servidor es:

```text
http://localhost:5074
```

la dirección base será:

```text
http://localhost:5074/api/
```

## HttpClient

Se crea una única instancia:

```csharp
using HttpClient cliente = new()
{
    BaseAddress = new Uri(urlApi)
};
```

`BaseAddress` permite realizar peticiones utilizando rutas cortas:

```csharp
"categorias"
"preguntas?cantidad=10"
```

El `using` libera los recursos del cliente al terminar el programa.

## Descargar y convertir JSON

Para descargar las categorías:

```csharp
await cliente.GetFromJsonAsync<List<CategoriaDto>>(
    "categorias");
```

Este método realiza dos operaciones:

1. Envía una petición GET.
2. Convierte el JSON en objetos C#.

El operador:

```csharp
?? []
```

utiliza una lista vacía si el resultado fuera `null`.

## Elegir categoría

El cliente muestra:

```text
0. Todas las categorías
1. Historia
2. Ciencia
...
```

No se supone que los identificadores sean consecutivos. La selección se
comprueba mediante:

```csharp
categorias.Any(categoria =>
    categoria.Id == categoriaId);
```

Esto sigue funcionando aunque se haya eliminado alguna categoría.

## Construir la petición de preguntas

Para todas las categorías:

```text
preguntas?cantidad=10
```

Para una categoría concreta:

```text
preguntas?categoriaId=2&cantidad=10
```

El operador condicional elige una de las dos rutas.

## DTO

Los DTO reproducen la estructura del JSON:

```csharp
public record CategoriaDto(int Id, string Nombre);
```

y:

```csharp
public record PreguntaDto(
    int Id,
    string Enunciado,
    string[] Respuestas,
    int RespuestaCorrecta,
    CategoriaDto Categoria);
```

No son modelos de Entity Framework. El cliente no necesita conocer las tablas
de SQLite.

## Mostrar respuestas

Las respuestas se encuentran en un array:

```csharp
pregunta.Respuestas
```

El bucle muestra el índice más uno porque los arrays empiezan en cero, pero las
respuestas visibles deben numerarse desde uno.

## Comprobar la respuesta

La API devuelve `RespuestaCorrecta` como un número del 1 al 4:

```csharp
if (respuesta == pregunta.RespuestaCorrecta)
```

Para recuperar el texto correcto del array hay que restar uno:

```csharp
pregunta.Respuestas[
    pregunta.RespuestaCorrecta - 1
]
```

## Validación de números

`LeerNumero` utiliza:

```csharp
int.TryParse(...)
```

El método no produce una excepción si el usuario escribe texto. Simplemente
vuelve a solicitar un número válido.

## Gestión de errores

Las peticiones están dentro de:

```csharp
try
{
    // Peticiones.
}
catch (HttpRequestException error)
{
    // Mensaje de error.
}
```

Esto permite mostrar un aviso comprensible cuando:

- La API no está ejecutándose.
- El puerto es incorrecto.
- La dirección no existe.
- El servidor devuelve un error HTTP.

## ¿Necesita CORS?

No. CORS es una restricción aplicada por los navegadores. Un cliente de consola
con `HttpClient` puede realizar la petición sin que la API habilite CORS.

La API puede mantener CORS porque también dispone de un cliente web, pero este
programa de consola no depende de esa configuración.

## Flujo completo

```text
Cliente de consola
        ↓ HTTP GET
Controladores de la API
        ↓
Entity Framework Core
        ↓
SQLite
        ↓ JSON
Cliente de consola
```

## Pruebas sugeridas

1. Ejecutar el cliente con la API detenida.
2. Ejecutarlo con el puerto incorrecto.
3. Consultar todas las categorías.
4. Elegir una categoría concreta.
5. Introducir letras al seleccionar.
6. Introducir un Id inexistente.
7. Introducir respuestas inferiores a 1.
8. Introducir respuestas superiores a 4.
9. Acertar una pregunta.
10. Fallar y comprobar la respuesta correcta.

## Posibles ampliaciones

- Permitir elegir la cantidad de preguntas.
- Repetir otra partida sin cerrar el programa.
- Mostrar el porcentaje final.
- Guardar un historial de puntuaciones.
- Añadir colores con `Console.ForegroundColor`.
- Consultar una pregunta directamente por Id.
