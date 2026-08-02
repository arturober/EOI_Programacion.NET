# Biblioteca · ASP.NET Core + Open Library

Aplicación educativa desarrollada con **ASP.NET Core Razor Pages** para buscar
libros en [Open Library](https://openlibrary.org/), consultar sus fichas y
guardar una colección privada de favoritos.

El proyecto está pensado para poder estudiarlo: las responsabilidades están
separadas, los nombres y comentarios están en español y no utiliza generadores
de código ocultos.

## Funcionalidades

- Página de inicio con tendencias, libros mejor valorados y programación.
- Búsqueda por título, autor, ISBN, materia u otros términos admitidos por
  Open Library.
- Listados temáticos paginados: novedades, fantasía, misterio, ciencia ficción,
  romance y programación.
- Ficha de cada obra con portada, autores, fecha, valoración, ediciones,
  idiomas, ISBN, materias, disponibilidad y recomendaciones.
- Registro e inicio de sesión con ASP.NET Core Identity.
- Registro inmediato, sin confirmación obligatoria del correo.
- Bloqueo temporal después de cinco intentos de acceso fallidos.
- Favoritos privados guardados en SQLite para cada usuario.
- Copia local de los datos básicos del favorito, para que la colección pueda
  mostrarse aunque Open Library falle temporalmente.
- Selector de Bootstrap claro/oscuro y todos los temas Bootswatch.
- Avisos y confirmaciones con SweetAlert2.
- Sustitución automática de portadas que no existen.
- Pequeña API JSON propia para practicar su consumo.
- Caché en memoria y limitación de frecuencia para respetar Open Library.

## Tecnologías

- .NET 10 y C#.
- ASP.NET Core Razor Pages.
- ASP.NET Core Identity.
- Entity Framework Core y SQLite.
- Open Library Search API, Works API y Covers API.
- Bootstrap 5.3.8, Bootswatch 5.3.8, Bootstrap Icons y SweetAlert2 desde CDN.

## Requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download/dotnet/10.0).
- Conexión a Internet para Open Library y los recursos CDN.

Open Library **no necesita una clave API ni un token**.

## Puesta en marcha

Abre un terminal dentro de la carpeta `Biblioteca` y ejecuta:

```bash
dotnet restore
dotnet run
```

La consola mostrará la dirección local. Con el perfil incluido normalmente
será:

```text
http://localhost:5100
```

En el primer arranque la aplicación crea automáticamente:

```text
Data/biblioteca.db
```

Ese archivo contiene usuarios, roles y favoritos. Está excluido de Git para no
publicar datos locales.

## Identificar la aplicación ante Open Library

Open Library recomienda que las peticiones incluyan un nombre de aplicación y
un contacto. El nombre ya viene configurado. Para añadir tu correo sin subirlo
a Git, puedes utilizar **user-secrets**:

```bash
dotnet user-secrets set "OpenLibrary:Contacto" "tu-correo@ejemplo.com"
```

También puedes copiar `appsettings.Local.example.json` como
`appsettings.Local.json` y sustituir el correo de ejemplo:

```json
{
  "OpenLibrary": {
    "Contacto": "tu-correo@ejemplo.com"
  }
}
```

`appsettings.Local.json` también está excluido de Git.

> El contacto no es una credencial. Se envía en el `User-Agent` para que Open
> Library pueda identificar al responsable de la aplicación.

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

MonsterASP.NET permite ejecutar aplicaciones ASP.NET Core con .NET 10. Como el
proyecto se trabaja principalmente desde Visual Studio Code, el procedimiento
recomendado es publicar desde su terminal integrada y utilizar WebFTP.

### Preparar la publicación desde VS Code

Abre la carpeta `Biblioteca` en VS Code y ejecuta:

```bash
dotnet restore
dotnet publish -c Release -o publicacion
```

En Windows, crea el ZIP desde la misma terminal con PowerShell:

```powershell
Compress-Archive -Path .\publicacion\* -DestinationPath .\publicacion.zip -Force
```

En Linux o macOS (si el comando no está disponible, instala el paquete
`zip` desde el gestor de paquetes del sistema):

```bash
rm -f publicacion.zip
(cd publicacion && zip -r ../publicacion.zip .)
```

El uso de `publicacion\*` hace que el ZIP contenga directamente
`Biblioteca.dll`, `web.config`, `appsettings.json`, `wwwroot` y los demás
archivos publicados, sin añadir otra carpeta `publicacion`.

### Subir el ZIP

1. Entra en el sitio desde el panel de MonsterASP.NET.
2. Abre **Files** y entra en `/wwwroot`.
3. Sube `publicacion.zip`.
4. Extrae el ZIP dentro de `/wwwroot`.
5. Permite sustituir los archivos de la aplicación, pero no borres previamente
   todo el contenido del alojamiento.
6. Reinicia la aplicación o el AppPool.

No subas el código fuente, el archivo `.csproj` ni las carpetas `bin` y
`obj`. La
[guía oficial de publicación mediante ZIP](https://help.monsterasp.net/books/deploy/page/how-to-deploy-website-content-from-zip-file)
muestra el uso del administrador de archivos.

Si se utiliza Visual Studio completo, WebDeploy continúa disponible como
[alternativa](https://help.monsterasp.net/books/deploy/page/how-to-deploy-net-core-web-application-using-visual-studio),
pero no es necesario para trabajar desde VS Code.

### Identificar la aplicación en producción

Los valores de `dotnet user-secrets` no se envían al servidor. Open Library no
necesita una clave, pero conviene configurar un contacto real desde:

```text
Websites → Manage website → Scripting → Environment Variables
```

Añade:

```text
Nombre: OpenLibrary__Contacto
Valor:  tu-correo@dominio.es
```

Los dos guiones bajos representan `OpenLibrary:Contacto`. Guarda el cambio y
reinicia la aplicación o el AppPool. No publiques `appsettings.Local.json`.
MonsterASP.NET documenta este mecanismo en
[Environment variables as configuration store](https://help.monsterasp.net/books/development/page/environment-variables-as-configuration-store).

### Identity y conservación de SQLite

No hace falta configurar un proveedor externo de autenticación ni un servidor
de correo. Las cuentas son locales, el correo no necesita confirmación y
Identity guarda usuarios y contraseñas protegidas en
`Data/biblioteca.db`.

`EnsureCreatedAsync` crea la carpeta, la base y sus tablas en el primer
arranque. En las publicaciones posteriores:

- no sobrescribas ni elimines `Data/biblioteca.db`;
- no actives una opción de WebDeploy que elimine archivos adicionales del
  destino;
- conserva una copia de seguridad antes de actualizar;
- no subas la base local salvo que quieras sustituir deliberadamente la del
  servidor;
- recuerda que `EnsureCreatedAsync` crea una base nueva, pero no migra una
  base existente cuando cambia el modelo.

### Comprobar el despliegue

1. Abre la aplicación mediante HTTPS.
2. Registra una cuenta.
3. Cierra la sesión y vuelve a iniciarla.
4. Añade un libro a favoritos.
5. Reinicia la aplicación y comprueba que la cuenta y el favorito continúan.

Si aparece un error HTTP 500, revisa
`Control Panel → Websites → Manage → Logs`. También puedes habilitar
temporalmente los
[logs de depuración de ASP.NET Core](https://help.monsterasp.net/books/debugging/page/aspnet-core-debug-logging);
desactívalos al terminar porque consumen espacio y recursos.


## Registro y autenticación

La clase `Usuario` hereda de `IdentityUser`. Al registrarse una persona:

1. Identity valida el correo y la política de contraseña.
2. La contraseña se transforma mediante un hash seguro; no se guarda el texto
   original.
3. El usuario se almacena en las tablas `AspNetUsers` de SQLite.
4. Se crea inmediatamente la cookie de sesión.

La aplicación tiene:

```csharp
opciones.SignIn.RequireConfirmedEmail = false;
```

Por eso no se envía un correo de confirmación y el usuario puede entrar al
terminar el registro. Para un proyecto público real convendría añadir
confirmación, recuperación de contraseña, protección frente a correo no
verificado y un proveedor de envío.

## Cómo se guardan los favoritos

SQLite contiene tres grupos de tablas:

- Las tablas de Identity, como `AspNetUsers`.
- `Libros`, con una copia breve de cada obra guardada.
- `Favoritos`, que relaciona un usuario con un libro mediante una clave
  compuesta.

Un usuario solo consulta sus propias relaciones porque todas las operaciones
filtran por el identificador obtenido desde la sesión de Identity.

## Configuración

Los valores generales están en `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Biblioteca": "Data Source=Data/biblioteca.db"
  },
  "OpenLibrary": {
    "NombreAplicacion": "BibliotecaRazor",
    "Contacto": "",
    "Idioma": "es",
    "MinutosCache": 30,
    "TamanoPagina": 20
  }
}
```

| Opción | Función |
|---|---|
| `NombreAplicacion` | Nombre usado en el `User-Agent`. |
| `Contacto` | Correo o URL de contacto recomendado por Open Library. |
| `Idioma` | Idioma preferido de los resultados. |
| `MinutosCache` | Duración de la caché de listados y búsquedas. |
| `TamanoPagina` | Resultados solicitados por página, entre 6 y 50. |

## Estructura del proyecto

```text
Biblioteca/
├── Configuracion/       Opciones de Open Library
├── Controllers/         API JSON propia
├── Data/                Contexto de Entity Framework Core
├── DTOs/                Clases que reciben el JSON externo
├── Modelos/             Entidades y modelos para la interfaz
├── Pages/
│   ├── Cuenta/          Registro, acceso y cierre de sesión
│   ├── Favoritos/       Colección privada
│   ├── Libros/          Búsqueda, listados y detalles
│   └── Shared/          Layout, tarjetas y paginación
├── Servicios/           Open Library, favoritos y mensajes de Identity
├── wwwroot/             JavaScript y portada alternativa
├── Program.cs           Configuración de la aplicación
└── appsettings.json     Configuración general
```

## API JSON propia

Con la aplicación en ejecución:

```http
GET /api/libros/buscar?texto=Don%20Quijote&pagina=1
GET /api/libros/OL45804W
```

El archivo `Biblioteca.http` contiene ejemplos listos para Visual Studio,
Rider o la extensión REST Client.

## Uso responsable de Open Library

El servicio `OpenLibraryServicio`:

- Pide únicamente los campos que utiliza la interfaz.
- Guarda respuestas en caché.
- Serializa las peticiones externas con un `SemaphoreSlim`.
- Espera más de un segundo entre llamadas sin contacto configurado.
- Utiliza un intervalo menor cuando la petición está identificada.
- Convierte errores de red, tiempo de espera y formato en mensajes claros.

Open Library está orientada a usos humanos, educativos y de volumen moderado.
No debe utilizarse como backend de peticiones masivas. Consulta siempre su
[documentación de API](https://openlibrary.org/developers/api) y sus límites
actuales.

## Comandos útiles

Crear una base nueva:

1. Detén la aplicación.
2. Borra `Data/biblioteca.db`.
3. Ejecuta de nuevo `dotnet run`.

Ver las claves asociadas al proyecto:

```bash
dotnet user-secrets list
```

Eliminar el contacto guardado:

```bash
dotnet user-secrets remove "OpenLibrary:Contacto"
```

## Solución de problemas

### La página tarda al abrirse por primera vez

Es normal: la aplicación limita deliberadamente las llamadas a Open Library.
Las siguientes visitas suelen servirse desde la caché.

### Algunas portadas no aparecen

No todas las obras tienen portada y algunos identificadores ya no están
disponibles. El JavaScript sustituye las respuestas fallidas por una imagen
local.

### Se ha cambiado el modelo y SQLite muestra errores

Este proyecto didáctico usa `EnsureCreatedAsync` para reducir pasos. Durante el
desarrollo puedes borrar la base y volver a arrancar. Para una aplicación real,
usa migraciones de Entity Framework Core.

### Los estilos no cargan

Bootstrap, Bootswatch, Bootstrap Icons y SweetAlert2 llegan desde jsDelivr.
Comprueba la conexión a Internet y que el navegador no esté bloqueando el CDN.

## Avisos

Los datos bibliográficos y las portadas pertenecen a Open Library e Internet
Archive y pueden ser incompletos. Este repositorio no está afiliado oficialmente
con esos proyectos.

## Licencia

El código se distribuye con licencia MIT. Consulta `LICENSE`.
