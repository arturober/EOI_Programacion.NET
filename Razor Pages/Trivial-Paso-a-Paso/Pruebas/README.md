# Pruebas de integración de TrivialApi

Este proyecto utiliza **xUnit**, `WebApplicationFactory` y SQLite en memoria
para comprobar la API del trivial sin abrir un navegador y sin ejecutar antes
`dotnet run`.

> El proyecto de pruebas se ejecuta localmente o desde un sistema de
> integración continua mediante `dotnet test`. No debe publicarse como parte
> de la aplicación web.

## Estructura

```text
Pruebas
├── TrivialApi
│   └── Aplicación ASP.NET Core, Razor Pages y API REST
├── TrivialApi.Tests
│   ├── CustomWebApplicationFactory.cs
│   ├── ApiTestBase.cs
│   ├── CategoriasApiTests.cs
│   ├── PreguntasListadoApiTests.cs
│   ├── PreguntasDetalleApiTests.cs
│   ├── RutasApiTests.cs
│   └── xunit.runner.json
└── TrivialApiConPruebas.slnx
```

## Nombres utilizados

La clase de infraestructura se llama `CustomWebApplicationFactory`, un nombre
muy habitual en proyectos ASP.NET Core y utilizado también en los ejemplos de
pruebas de integración de Microsoft.

Las pruebas se han separado por responsabilidad:

- `CategoriasApiTests`: endpoints de categorías.
- `PreguntasListadoApiTests`: listado, límites, filtros y formato.
- `PreguntasDetalleApiTests`: consulta de una pregunta concreta.
- `RutasApiTests`: enrutamiento y funcionamiento general.
- `ApiTestBase`: código común para realizar peticiones y mostrar mensajes.

## Ejecutar las pruebas

Abre una terminal en esta carpeta y ejecuta:

```console
dotnet test --logger "console;verbosity=detailed"
```

El resultado se muestra únicamente en la consola y no se necesita ningún script adicional.

## Salida durante la ejecución

Cada prueba escribe información mediante `ITestOutputHelper`. La configuración
`showLiveOutput` de `xunit.runner.json` permite mostrarla mientras se ejecutan
las pruebas.

Ejemplo:

```text
======================================================================
INICIO: Filtrar las preguntas por la categoría Ciencia
PETICIÓN: GET /api/categorias
RESPUESTA: 200 OK
CONTENIDO: application/json
PETICIÓN: GET /api/preguntas?categoriaId=2&cantidad=100
RESPUESTA: 200 OK
CONTENIDO: application/json
COMPROBACIÓN: Se reciben las cuatro preguntas de Ciencia
RESULTADO: prueba superada
```

La ejecución paralela está desactivada para que los mensajes de diferentes
pruebas no se mezclen en la consola.

## Qué hace CustomWebApplicationFactory

`CustomWebApplicationFactory`:

1. Inicia internamente la aplicación definida en `Program.cs`.
2. Crea un servidor HTTP de pruebas.
3. Proporciona instancias de `HttpClient` conectadas con ese servidor.
4. Sustituye la base de datos real por una base SQLite en memoria.
5. Crea el esquema e inserta datos conocidos antes de las peticiones.
6. Elimina la base en memoria al terminar la ejecución.

Las peticiones recorren realmente:

```text
HttpClient → enrutamiento → controlador → Entity Framework Core → SQLite → JSON
```

## Base de datos de pruebas

Las pruebas no utilizan ni modifican `Data/trivial.db`.

La base en memoria contiene:

- 3 categorías: Arte, Ciencia y Cultura.
- 12 preguntas: 4 por categoría.

Los doce registros permiten comprobar correctamente que el endpoint devuelve
diez preguntas cuando no se especifica el parámetro `cantidad`.

## Casos cubiertos

La suite contiene **37 casos de prueba ejecutables**.


### Categorías

- Listado completo y orden alfabético.
- Identificadores positivos y sin duplicados.
- Consulta de una categoría existente.
- Identificadores inexistentes, cero y negativos.
- Identificador no numérico.
- Tipo de contenido JSON.
- Rechazo de peticiones POST con código 405.

### Listado de preguntas

- Cantidad predeterminada de diez elementos.
- Cantidades válidas de 1, 2, 5 y 12.
- Cantidades iguales o inferiores a cero.
- Cantidad superior al máximo de 1000.
- Cantidad no numérica.
- Filtrado por categoría existente.
- Categoría inexistente.
- Categorías con identificador cero o negativo.
- Categoría no numérica.
- Estructura completa de los DTO.
- Ausencia de preguntas duplicadas.
- Tipo de contenido JSON.
- Rechazo de peticiones POST con código 405.

### Detalle de preguntas

- Consulta de una pregunta existente y comprobación de sus datos.
- Identificadores inexistentes, cero y negativos.
- Identificador no numérico.
- Contrato JSON público.
- Ausencia de propiedades internas como `respuesta1` o `categoriaId`.
- Tipo de contenido JSON.

### Comportamiento general

- Una ruta de API inexistente devuelve 404.
- La página principal de Razor Pages continúa disponible.

## Cambio necesario en Program.cs

Al final de `Program.cs` se mantiene esta declaración:

```csharp
public partial class Program
{
}
```

Las aplicaciones con instrucciones de nivel superior generan internamente la
clase `Program`. Esta declaración permite que el proyecto de pruebas la utilice
como parámetro de `WebApplicationFactory<Program>`.
