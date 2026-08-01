# Pokémon — etapa 1: listado

Primera versión didáctica del proyecto. Muestra los 151 Pokémon de la primera
generación en una lista sencilla, sin imágenes, enlaces ni página de detalles.

## Conceptos que se practican

- Crear un proyecto con ASP.NET Core Razor Pages.
- Registrar un servicio con `AddHttpClient`.
- Consultar una API mediante `GetFromJsonAsync`.
- Convertir JSON en objetos de C#.
- Pasar una lista desde un `PageModel` hasta una página Razor.
- Recorrer datos con `foreach`.
- Aplicar clases básicas de Bootstrap.

## Ejecutar el proyecto

Abre una terminal dentro de la carpeta `Pokemon` y ejecuta:

```bash
dotnet restore
dotnet run
```

## Publicación en MonsterASP.NET

Esta etapa no necesita claves, usuarios ni base de datos. Desde la terminal
integrada de VS Code:

```bash
dotnet publish -c Release -o publicacion
```

```powershell
Compress-Archive -Path .\publicacion\* -DestinationPath .\publicacion.zip -Force
```

Sube y extrae el ZIP en `/wwwroot` desde **Files**. Solo debe publicarse esta
etapa en el sitio, no toda la carpeta `Pokemon`. El servidor necesita conexión
saliente a PokeAPI.

Después del despliegue, abre el listado y comprueba que aparecen los 151
Pokémon. La aplicación no conserva datos entre reinicios.

La aplicación consulta:

```text
GET https://pokeapi.co/api/v2/pokemon?limit=151&offset=0
```
