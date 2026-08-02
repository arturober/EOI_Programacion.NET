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

> **Método recomendado:** utiliza WebFTP.

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
