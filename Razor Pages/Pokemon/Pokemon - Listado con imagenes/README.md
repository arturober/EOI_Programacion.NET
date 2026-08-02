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

> **Método recomendado:** utiliza WebFTP.

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

### Método recomendado: WebFTP

Para estos proyectos se recomienda **WebFTP**, el cliente FTP que
MonsterASP.NET ofrece en el navegador. No requiere instalar programas ni crear
o extraer un ZIP.

1. Desde la terminal integrada de VS Code, genera o actualiza la carpeta
   `publicacion` con el comando `dotnet publish` indicado anteriormente.
2. En el panel de MonsterASP.NET abre el sitio y entra en
   **Files → WebFTP**. También se puede acceder a
   [WebFTP](https://webftp.monsterasp.net/) desde el enlace que muestra el
   panel.
3. Dentro de WebFTP, abre `/wwwroot`.
4. Sube **el contenido** de `publicacion`, no la carpeta como un nivel
   adicional. `web.config`, el ensamblado principal, `appsettings.json` y
   las demás carpetas publicadas deben quedar directamente en `/wwwroot`.
5. En las actualizaciones, conserva las bases de datos y los demás archivos
   persistentes indicados en este README.
6. Si algún archivo está bloqueado, reinicia o detén temporalmente el sitio.
   También puedes subir `app_offline.htm` a `/wwwroot`, completar la
   transferencia, eliminarlo y volver a iniciar la aplicación.
7. Abre la dirección HTTPS del sitio y realiza las comprobaciones específicas
   indicadas en este README.

Como segunda opción, para transferencias grandes o frecuentes, puede utilizarse
FileZilla u otro cliente FTP/SFTP con las credenciales disponibles en
**Deploy (FTP/WebDeploy/Git)**. La
[guía oficial de FTP/SFTP](https://help.monsterasp.net/books/deploy/page/how-to-deploy-website-content-via-ftpsftp)
explica esta alternativa.

### Alternativa: despliegue mediante ZIP

El procedimiento mediante ZIP descrito anteriormente se mantiene disponible.
Desde **Files**, sube `publicacion.zip`, pulsa **Unzip** y elige `/wwwroot`
como destino. Al actualizar una aplicación existente, marca
**Overwrite files in target path** y
**Restart application pool before unzip** para sustituir los archivos en uso.
El contenido publicado debe quedar directamente en `/wwwroot`, sin una
carpeta `publicacion` intermedia, y deben respetarse las indicaciones de este
README sobre bases de datos y otros archivos persistentes.

Consulta la
[guía oficial de despliegue mediante ZIP](https://help.monsterasp.net/books/deploy/page/how-to-deploy-website-content-from-zip-file).
