# Lista de tareas con Razor Pages

Aplicación web didáctica para gestionar tareas y categorías. Está desarrollada
con C#, Razor Pages, SQLite, Bootstrap, Bootstrap Icons y SweetAlert2.

El proyecto está pensado para estudiantes que están aprendiendo C#. Por eso el
código prioriza la claridad: cada clase tiene una responsabilidad sencilla, las
consultas SQL usan parámetros y los métodos tienen nombres descriptivos.

## Funciones principales

- Crear, consultar, modificar y eliminar tareas.
- Marcar una tarea como completada.
- Crear, consultar, modificar y eliminar categorías.
- Filtrar las tareas por categoría.
- Evitar que se elimine una categoría que todavía contiene tareas.
- Mostrar confirmaciones y mensajes con SweetAlert2.
- Ordenar categorías y tareas según la cultura española, incluidas las tildes y
  la letra ñ.
- Adaptar la navegación, las tablas, las tarjetas y los formularios a móviles.

## Organización del proyecto

- `BaseDatos.cs`: abre y devuelve una conexión con SQLite.
- `Models/Categoria.cs`: representa una categoría y contiene su CRUD.
- `Models/Tarea.cs`: representa una tarea y contiene su CRUD.
- `Pages/Tareas`: páginas Razor para gestionar las tareas.
- `Pages/Categorias`: páginas Razor para gestionar las categorías.
- `Pages/Shared/_Layout.cshtml`: navegación, librerías y JavaScript común.
- `lista_tareas.db`: base de datos SQLite con datos de ejemplo.
- `libman.json`: define las librerías web y sus versiones.

## Librerías mediante CDN

Bootstrap, Bootstrap Icons y SweetAlert2 se cargan desde jsDelivr. Esta variante
no necesita restaurar las bibliotecas con LibMan, pero sí requiere conexión a
Internet en el navegador.

## Ejecución

Necesitas el SDK de .NET 10. Desde la carpeta del proyecto ejecuta:

```bash
dotnet restore
dotnet run
```

Abre en el navegador la dirección que aparezca en la terminal.

## Publicación en MonsterASP.NET

> **Método recomendado:** utiliza WebFTP. El archivo ZIP se conserva como
> alternativa.

Desde la terminal integrada de VS Code:

```bash
dotnet restore
dotnet publish -c Release -o publicacion
```

En PowerShell:

```powershell
Compress-Archive -Path .\publicacion\* -DestinationPath .\publicacion.zip -Force
```

Sube y extrae `publicacion.zip` dentro de `/wwwroot` desde **Files**. Los
archivos publicados deben quedar directamente en la raíz del sitio. No subas el
código fuente, el `.csproj`, `bin` ni `obj`.

La base se guarda como `lista_tareas.db`. Si no existe, la aplicación crea una
base vacía. En las actualizaciones:

- conserva la base del servidor;
- realiza una copia de seguridad antes de publicar;
- no borres todo `/wwwroot`;
- comprueba que jsDelivr, Bootstrap Icons y SweetAlert2 cargan correctamente.

No existe autenticación. Cualquier visitante puede modificar las tareas y
categorías, por lo que solo deben utilizarse datos de práctica o añadirse
control de acceso antes de exponer la aplicación.

Consulta la
[guía de publicación mediante ZIP](https://help.monsterasp.net/books/deploy/page/how-to-deploy-website-content-from-zip-file).

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

## Ideas importantes que se practican

- Mapeo de tablas de SQLite a objetos de C#.
- Operaciones CRUD con `Microsoft.Data.Sqlite`.
- Consultas parametrizadas con `AddWithValue` para evitar inyección SQL.
- Relaciones entre tablas mediante una clave externa.
- Validación de datos con anotaciones como `Required` y `StringLength`.
- Separación entre modelos, páginas Razor y acceso a la conexión.
- Diseño responsive y accesible con Bootstrap.
