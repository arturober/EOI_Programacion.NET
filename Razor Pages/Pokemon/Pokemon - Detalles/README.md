# Pokémon — etapa 3: detalles esenciales

Tercera versión didáctica del proyecto. Las tarjetas del listado se convierten
en enlaces y se añade una página que muestra únicamente los datos esenciales:
número, imagen, tipos, altura, peso y habilidades.

## Novedades respecto a la etapa 2

- Se utilizan Tag Helpers (`asp-page` y `asp-route-nombre`) para crear enlaces.
- La ruta de detalles recibe el nombre del Pokémon.
- El servicio realiza una segunda clase de petición a PokeAPI.
- Se comprueba si la respuesta HTTP es correcta.
- Nuevas clases representan el JSON del endpoint de detalles.
- La ficha utiliza una tarjeta responsive y una lista de descripción.

## Ejecutar el proyecto

Abre una terminal dentro de la carpeta `Pokemon` y ejecuta:

```bash
dotnet restore
dotnet run
```

## Publicación en MonsterASP.NET

La aplicación no necesita claves, usuarios ni SQLite. Desde la terminal
integrada de VS Code:

```bash
dotnet publish -c Release -o publicacion
```

```powershell
Compress-Archive -Path .\publicacion\* -DestinationPath .\publicacion.zip -Force
```

Sube y extrae el ZIP dentro de `/wwwroot`. Solo debe desplegarse esta etapa del
itinerario. El servidor necesita acceder a PokeAPI y los sprites se descargan
desde Internet.

Después de publicar, abre el listado y varias fichas de detalle. No existe base
de datos ni información que deba preservarse entre despliegues.

## Endpoints utilizados

```text
GET https://pokeapi.co/api/v2/pokemon?limit=151&offset=0
GET https://pokeapi.co/api/v2/pokemon/{nombre}
```
