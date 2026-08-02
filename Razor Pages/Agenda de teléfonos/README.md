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

> **Método recomendado:** utiliza WebFTP. El archivo ZIP se conserva como
> alternativa.

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
