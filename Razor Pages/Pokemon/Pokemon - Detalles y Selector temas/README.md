# Pokémon — etapa 4: selector de temas

Cuarta versión didáctica del proyecto. Mantiene el listado con imágenes y la
ficha de detalles esenciales de la etapa anterior, y añade un selector de
temas en la barra superior.

## Novedades respecto a la etapa 3

- Se identifica la hoja de estilos mediante `id="temaCss"`.
- El selector permite usar Bootstrap claro, Bootstrap oscuro y Bootswatch.
- Los temas Bootswatch se agrupan en claros y oscuros.
- JavaScript cambia dinámicamente la dirección de la hoja de estilos.
- `dataset.bsTheme` adapta también fondos, textos, tarjetas y formularios.
- `localStorage` conserva la elección al cambiar de página o cerrar el navegador.
- Si no existe una elección guardada, se utiliza Bootstrap claro.

El código relacionado con los temas está dentro de `_Layout.cshtml`, porque la
plantilla se utiliza en todas las páginas.

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

La aplicación necesita conexión a Internet para consultar PokeAPI y cargar las
hojas de estilos desde jsDelivr.
