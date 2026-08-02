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

> **Método recomendado:** genera los archivos con **Publish** de VS Code o con
> `dotnet publish -c Release`, detén la aplicación si vas a actualizarla y
> sube los archivos sin comprimir mediante **WebFTP**, seleccionando la opción
> de sobrescritura. El ZIP se conserva como alternativa.

### Generar los archivos de publicación

Hay dos procedimientos recomendados. Ambos producen la misma aplicación
preparada para desplegarse.

#### Opción 1: Publish desde VS Code

1. En el explorador de soluciones de VS Code, haz clic con el botón derecho
   —o utiliza el clic secundario— sobre el **proyecto web**.
2. Selecciona **Publish**.
3. Espera a que termine y consulta en la salida de VS Code la carpeta de
   destino.
4. Utiliza el contenido de esa carpeta para el despliegue.

Si **Publish** no aparece, utiliza la terminal. La disponibilidad de esta
acción depende de las herramientas de C# instaladas en VS Code.

#### Opción 2: carpeta predeterminada de .NET

Desde la terminal integrada de VS Code, situada en la carpeta que contiene el
archivo `.csproj`, ejecuta:

```bash
dotnet publish -c Release
```

En estos proyectos .NET 10, los archivos se generan normalmente en:

```text
bin/Release/net10.0/publish
```

Si se especifica un runtime o un perfil cambia la configuración, la ruta puede
incluir carpetas adicionales. La terminal muestra siempre la ubicación
utilizada. Debe desplegarse **el contenido** de `publish`, no sus carpetas
padre.

Como alternativa puede elegirse una carpeta más fácil de localizar:

```bash
dotnet publish -c Release -o publicacion
```

En ese caso se utilizará el contenido de `publicacion`. Esta opción se
mantiene en algunos ejemplos posteriores para preparar el ZIP, pero la carpeta
predeterminada de .NET es la recomendada.

### Método recomendado: WebFTP sin comprimir

WebFTP permite subir directamente los archivos y carpetas publicados; no es
necesario comprimirlos.

#### Primera publicación

1. En el panel de MonsterASP.NET abre el sitio y entra en
   **Files → WebFTP**. También puedes abrir
   [WebFTP](https://webftp.monsterasp.net/) desde el enlace del panel.
2. En WebFTP, entra en `/wwwroot`.
3. Selecciona **el contenido** de la carpeta generada por Publish o
   `dotnet publish` y súbelo sin comprimir. No subas la propia carpeta
   `publish` o `publicacion` como un nivel adicional.
4. Si WebFTP encuentra archivos existentes, elige **Overwrite** o
   **Overwrite all**.
5. Comprueba que `web.config`, el ensamblado principal, `appsettings.json` y
   las demás carpetas publicadas están directamente en `/wwwroot`.
6. Abre la dirección HTTPS y realiza las comprobaciones específicas indicadas
   en este README.

#### Actualización de una aplicación existente

> **Importante:** antes de sobrescribir los archivos, detén la aplicación con
> **Stop** desde el panel de MonsterASP.NET. Así se evitan archivos bloqueados y
> que la web se ejecute temporalmente con componentes de versiones distintas.

1. Pulsa **Stop** en las acciones rápidas del sitio.
2. Con la aplicación detenida, descarga una copia de seguridad de la base de
   datos y de cualquier otro archivo persistente.
3. En WebFTP, abre `/wwwroot` y sube sin comprimir el contenido de la nueva
   publicación.
4. Selecciona **Overwrite** o **Overwrite all**, pero conserva las bases de
   datos y demás archivos que este README indique que no deben sustituirse.
5. Cuando termine la transferencia, pulsa **Start**.
6. Comprueba la aplicación y revisa los registros si se produce algún error.

Como alternativa a **Stop**, puede subirse primero `app_offline.htm`, realizar
la actualización y eliminarlo al terminar. Para transferencias grandes o
frecuentes también puede utilizarse FileZilla u otro cliente FTP/SFTP con las
credenciales disponibles en **Deploy (FTP/WebDeploy/Git)**. Consulta la
[guía oficial de FTP/SFTP](https://help.monsterasp.net/books/deploy/page/how-to-deploy-website-content-via-ftpsftp).

### Alternativa: despliegue mediante ZIP

Las instrucciones específicas para crear el ZIP se mantienen más adelante en
este apartado. En MonsterASP.NET:

1. Detén primero la aplicación si se trata de una actualización.
2. Abre **Files**, sube `publicacion.zip` y pulsa **Unzip**.
3. Elige `/wwwroot` como destino.
4. En una actualización, marca **Overwrite files in target path** y
   **Restart application pool before unzip**.
5. Conserva las bases de datos y demás archivos persistentes indicados en este
   README.
6. Inicia o reinicia la aplicación y comprueba su funcionamiento.

Consulta la
[guía oficial de despliegue mediante ZIP](https://help.monsterasp.net/books/deploy/page/how-to-deploy-website-content-from-zip-file).

Desde la terminal integrada de VS Code:

```bash
dotnet restore
dotnet publish -c Release -o publicacion
```

En PowerShell:

```powershell
Compress-Archive -Path .\publicacion\* -DestinationPath .\publicacion.zip -Force
```

En Linux o macOS (si el comando no está disponible, instala el paquete
`zip` desde el gestor de paquetes del sistema):

```bash
rm -f publicacion.zip
(cd publicacion && zip -r ../publicacion.zip .)
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


## Ideas importantes que se practican

- Mapeo de tablas de SQLite a objetos de C#.
- Operaciones CRUD con `Microsoft.Data.Sqlite`.
- Consultas parametrizadas con `AddWithValue` para evitar inyección SQL.
- Relaciones entre tablas mediante una clave externa.
- Validación de datos con anotaciones como `Required` y `StringLength`.
- Separación entre modelos, páginas Razor y acceso a la conexión.
- Diseño responsive y accesible con Bootstrap.
