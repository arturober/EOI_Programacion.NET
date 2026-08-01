# Pokémon — etapa 2: listado con imágenes

Segunda versión didáctica del proyecto. Conserva el listado de la etapa anterior
y añade el identificador y la imagen de cada Pokémon. Las tarjetas todavía no
son enlaces y no existe una página de detalles.

## Novedades respecto a la etapa 1

- Se lee también la propiedad `url` devuelta por PokeAPI.
- El identificador se extrae de esa URL.
- La dirección de cada imagen se construye con el identificador.
- Los Pokémon se muestran mediante una cuadrícula responsive de Bootstrap.
- Las imágenes utilizan `loading="lazy"` para cargarse conforme hacen falta.

## Ejecutar el proyecto

Abre una terminal dentro de la carpeta `Pokemon` y ejecuta:

```bash
dotnet restore
dotnet run
```

## Publicación en MonsterASP.NET

No se necesita ninguna clave, cuenta o base de datos. Desde VS Code:

```bash
dotnet publish -c Release -o publicacion
```

```powershell
Compress-Archive -Path .\publicacion\* -DestinationPath .\publicacion.zip -Force
```

Sube y extrae `publicacion.zip` en `/wwwroot`. Publica únicamente esta etapa.
El servidor consulta PokeAPI y los navegadores descargan los sprites desde los
recursos públicos de PokeAPI, por lo que ambos necesitan conexión a Internet.

Comprueba el listado, las 151 imágenes y la carga diferida. No hay datos
persistentes que conservar al actualizar.

La aplicación consulta una única vez:

```text
GET https://pokeapi.co/api/v2/pokemon?limit=151&offset=0
```

Las imágenes proceden del repositorio de sprites de PokeAPI.
