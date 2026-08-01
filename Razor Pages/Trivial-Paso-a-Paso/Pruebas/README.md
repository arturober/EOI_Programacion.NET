# Pruebas de integración de TrivialApi

Este proyecto utiliza **xUnit** y `WebApplicationFactory` para probar la API
del trivial sin abrir un navegador y sin ejecutar previamente `dotnet run`.

> Este proyecto no debe publicarse en MonsterASP.NET. Las pruebas se ejecutan
> localmente o desde un sistema de integración continua mediante
> `dotnet test`. La aplicación web que se quiera mostrar debe publicarse desde
> una de las versiones 5, 6 o 7 del itinerario principal.

## Proyectos

```text
TrivialApiConPruebas
├── TrivialApi
│   └── Aplicación ASP.NET Core y API REST
├── TrivialApi.Tests
│   ├── FabricaApi.cs
│   └── ApiTrivialTests.cs
└── TrivialApiConPruebas.slnx
```

## Ejecutar las pruebas

Abre una terminal en la carpeta `TrivialApiConPruebas` y ejecuta:

```bash
dotnet test
```

Para mostrar más información:

```bash
dotnet test --logger "console;verbosity=detailed"
```

También pueden ejecutarse desde el Explorador de pruebas de Visual Studio.

El proyecto fija `SQLitePCLRaw.bundle_e_sqlite3` en la versión `2.1.12` para
evitar que NuGet restaure la versión vulnerable `2.1.11`.

## Qué hace WebApplicationFactory

`WebApplicationFactory<Program>`:

1. Inicia internamente la aplicación definida en `Program.cs`.
2. Crea un servidor HTTP de pruebas.
3. Proporciona un `HttpClient` conectado con ese servidor.
4. Permite realizar peticiones a rutas como `/api/categorias`.

Por tanto, las peticiones recorren realmente:

```text
HttpClient → enrutamiento → controlador → Entity Framework → JSON
```

## Base de datos de pruebas

Las pruebas no utilizan ni modifican `Data/trivial.db`.

`FabricaApi` sustituye el `TrivialContext` de la aplicación por una base SQLite
en memoria. Antes de empezar inserta dos categorías y cuatro preguntas con
valores conocidos.

Cuando terminan las pruebas, la conexión se cierra y esa base desaparece.

## Pruebas incluidas

- La ruta `/api/categorias` devuelve las categorías.
- El parámetro `cantidad` limita el número de preguntas.
- Cada pregunta contiene cuatro respuestas y una respuesta correcta válida.
- `categoriaId` filtra correctamente las preguntas.
- Una cantidad igual a cero se limita a una pregunta.
- Una pregunta inexistente devuelve el código HTTP 404.

## Pequeño cambio necesario en Program.cs

Al final de `Program.cs` se ha añadido:

```csharp
public partial class Program
{
}
```

Las aplicaciones con instrucciones de nivel superior generan internamente la
clase `Program`. La declaración anterior permite que el proyecto de pruebas la
utilice como parámetro de `WebApplicationFactory<Program>`.
