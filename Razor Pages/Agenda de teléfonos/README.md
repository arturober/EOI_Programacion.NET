# Agenda de teléfonos

Aplicación educativa desarrollada con **C#**, **.NET 10** y **ASP.NET Core
Razor Pages**. Permite administrar contactos con nombre, teléfono y fotografía
opcional mediante un CRUD sencillo conectado directamente a SQLite.

## Funcionalidades

- Listado de contactos.
- Búsqueda por nombre mientras se escribe.
- Búsqueda que ignora mayúsculas, minúsculas y tildes.
- Ordenación adaptada al español.
- Alta y edición de contactos.
- Eliminación con confirmación.
- Fotografía opcional en JPG, PNG, WEBP o GIF.
- Límite de 2 MB por imagen.
- Corrección automática de la orientación.
- Reducción proporcional a un máximo de 48 píxeles de ancho.
- Conversión a PNG y almacenamiento en Base64 dentro de SQLite.
- Conservación de la fotografía anterior cuando no se selecciona otra.
- Interfaz responsive con Bootstrap y Bootstrap Icons.

## Tecnologías

- .NET 10.
- ASP.NET Core Razor Pages.
- SQLite con `Microsoft.Data.Sqlite`.
- SQL parametrizado.
- SixLabors.ImageSharp 3.1.12.
- Bootstrap y Bootstrap Icons locales.
- LibMan para restaurar las bibliotecas web.

## Requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download/dotnet/10.0).
- Visual Studio Code o cualquier editor compatible con C#.
- Un navegador moderno.

## Ejecutar desde Visual Studio Code

Abre la carpeta `Agenda de teléfonos` y ejecuta desde la terminal integrada:

```bash
dotnet restore
dotnet run
```

La terminal mostrará la dirección local que debe abrirse en el navegador.

Si faltan las bibliotecas de `wwwroot/lib`, instala y ejecuta LibMan:

```bash
dotnet tool install -g Microsoft.Web.LibraryManager.Cli
libman restore
```

## Base de datos

La aplicación utiliza:

```text
agenda.db
```

El archivo se busca en el directorio de trabajo de la aplicación. Al arrancar:

1. SQLite crea el archivo si no existe.
2. `Persona.PrepararTabla` crea la tabla `personas`.
3. Si se abre una base antigua, se añade la columna `imagen` cuando falta.

Una base creada desde cero comienza sin contactos. El archivo incluido en el
repositorio contiene datos de trabajo y no es imprescindible para ejecutar el
proyecto.

Las fotografías no se guardan como archivos independientes. Se convierten a
PNG, se codifican en Base64 y se almacenan en la columna `imagen`.

## Estructura principal

```text
Agenda de teléfonos/
├── Models/
│   └── Persona.cs
├── Pages/
│   ├── Crear.cshtml
│   ├── Editar.cshtml
│   └── Index.cshtml
├── wwwroot/
│   └── lib/
├── BaseDatos.cs
├── Program.cs
├── agenda.db
└── libman.json
```

- `BaseDatos.cs` abre la conexión con SQLite.
- `Persona.cs` contiene validación, SQL y procesamiento de imágenes.
- Las Razor Pages reciben los datos, comprueban el modelo y llaman a
  `Persona`.
- `Program.cs` registra Razor Pages y prepara la tabla al arrancar.

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

MonsterASP.NET permite ejecutar aplicaciones ASP.NET Core con .NET 10. Desde
VS Code se recomienda generar la publicación y subirla mediante WebFTP.

### Preparar la publicación

Desde la terminal integrada:

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

El ZIP debe contener directamente `Agenda de teléfonos.dll`, `web.config`,
`appsettings.json`, `wwwroot` y los demás archivos publicados, sin una
carpeta `publicacion` adicional.

### Subir la aplicación

1. Abre **Files** en el panel del sitio de MonsterASP.NET.
2. Entra en `/wwwroot`.
3. Sube `publicacion.zip`.
4. Extrae el ZIP dentro de `/wwwroot`.
5. Permite sobrescribir los archivos de la aplicación sin borrar todo el
   directorio.
6. Reinicia la aplicación o el AppPool.

No subas el código fuente, el `.csproj`, `bin` ni `obj`. Consulta la
[guía oficial de publicación mediante ZIP](https://help.monsterasp.net/books/deploy/page/how-to-deploy-website-content-from-zip-file).

### Primera ejecución y datos

Si el ZIP no contiene `agenda.db`, la aplicación creará una base vacía en:

```text
/wwwroot/agenda.db
```

Si se quieren utilizar los datos de ejemplo, puede subirse manualmente la base
local después de publicar. No subas contactos o fotografías reales.

El proceso del alojamiento necesita permiso de escritura sobre
`agenda.db`. En las actualizaciones:

- no elimines ni sobrescribas la base si quieres conservar los contactos;
- descarga una copia de seguridad antes de volver a publicar;
- no borres todo `/wwwroot`;
- comprueba que un contacto nuevo sigue existiendo después de reiniciar.

### Advertencia de privacidad

La aplicación no tiene autenticación ni autorización. Cualquier visitante puede
consultar, crear, editar o borrar contactos. No debe exponerse públicamente con
datos personales reales sin añadir control de acceso, medidas de privacidad y
protección frente a abuso.

### Comprobar el despliegue

1. Crea un contacto sin fotografía.
2. Añade otro con una imagen válida.
3. Busca utilizando una palabra con y sin tilde.
4. Edita un contacto sin seleccionar otra fotografía.
5. Reinicia la aplicación y comprueba que los datos permanecen.

Si aparece un error HTTP 500, revisa
`Control Panel → Websites → Manage → Logs`. También puedes habilitar
temporalmente los
[logs de ASP.NET Core](https://help.monsterasp.net/books/debugging/page/aspnet-core-debug-logging)
y desactivarlos después del diagnóstico.


## Seguridad y límites

- Las consultas utilizan parámetros; no concatenes datos del usuario en SQL.
- El tipo real de la imagen se comprueba con ImageSharp.
- La imagen se limita a 2 MB y se redimensiona.
- La aplicación es una práctica, no una agenda pública preparada para
  información personal.
- SQLite es adecuado para un despliegue pequeño con una sola instancia.

## Solución de problemas

### La aplicación crea una agenda vacía

Es el comportamiento esperado si `agenda.db` no se incluyó en el despliegue.
Crea los contactos desde la aplicación o sube deliberadamente una copia de la
base local.

### Aparece «no such table: personas»

Comprueba que el proceso pueda escribir en la raíz de la aplicación y revisa
los logs de arranque. `Program.cs` debe ejecutar `Persona.PrepararTabla`.

### Los estilos o los iconos no aparecen

Ejecuta `libman restore` antes de publicar y comprueba que
`publicacion/wwwroot/lib` exista.

### La imagen es rechazada

Debe ser JPG, PNG, WEBP o GIF, contener realmente una imagen válida y no superar
2 MB.
