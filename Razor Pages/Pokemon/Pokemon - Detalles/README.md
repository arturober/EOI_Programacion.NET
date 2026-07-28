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

## Endpoints utilizados

```text
GET https://pokeapi.co/api/v2/pokemon?limit=151&offset=0
GET https://pokeapi.co/api/v2/pokemon/{nombre}
```
