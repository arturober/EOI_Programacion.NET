# NASA Explorer

Aplicación  construida con **ASP.NET Core Razor Pages y .NET 10** para
explorar varias fuentes de datos oficiales de NASA desde una única interfaz.

Incluye autenticación local con ASP.NET Core Identity y una colección de
favoritos independiente para cada usuario, guardada en SQLite.

## Funcionalidades

- **APOD**: imagen o vídeo astronómico del día, archivo por fechas y alta resolución.
- **NASA Image and Video Library**: búsqueda de imágenes, vídeos y audios,
  filtros por tipo y año, paginación, reproducción y acceso a originales.
- **DSCOVR EPIC**: imágenes recientes o por fecha de la Tierra en color natural,
  mejorado, aerosoles y nubes.
- **EONET v3**: eventos naturales activos o cerrados, filtros y mapa interactivo.
- **NeoWs**: asteroides cercanos, diámetro, velocidad, distancia y clasificación
  de peligrosidad potencial.
- **DONKI**: eyecciones de masa coronal, llamaradas solares, tormentas
  geomagnéticas y choques interplanetarios.
- **NASA Exoplanet Archive**: búsqueda segura en `pscomppars`, propiedades
  planetarias y gráfico de métodos de descubrimiento.
- Registro e inicio de sesión sin confirmación obligatoria de correo.
- Favoritos privados almacenados en SQLite.
- Caché en memoria para reducir llamadas repetidas.
- Gestión independiente de errores: si una API falla, las demás siguen funcionando.
- Temas Bootswatch intercambiables y recordados en el navegador.

## Tecnologías

- .NET 10 y Razor Pages
- Entity Framework Core 10
- ASP.NET Core Identity
- SQLite
- Bootstrap 5.3.8 y Bootswatch 5.3.8 desde CDN
- Bootstrap Icons 1.13.1 desde CDN
- SweetAlert2 11.26.25 desde CDN
- Leaflet 1.9.4 desde CDN
- Chart.js 4.5.1 desde CDN

## APIs utilizadas

| Módulo | Fuente | ¿Necesita la clave? |
|---|---|---:|
| APOD | [NASA Open APIs](https://api.nasa.gov/) | Sí |
| Asteroides | [NASA Open APIs · NeoWs](https://api.nasa.gov/) | Sí |
| Multimedia | [NASA Image and Video Library](https://images.nasa.gov/) | No |
| EPIC | [EPIC API 2.0](https://epic.gsfc.nasa.gov/about/api) | No |
| Eventos naturales | [EONET API v3](https://eonet.gsfc.nasa.gov/docs/v3) | No |
| Clima espacial | [CCMC DONKI Webservice](https://ccmc.gsfc.nasa.gov/tools/DONKI/) | No |
| Exoplanetas | [NASA Exoplanet Archive TAP](https://exoplanetarchive.ipac.caltech.edu/docs/TAP/usingTAP.html) | No |

El proyecto no utiliza **Mars Rover Photos** ni la antigua **Earth Imagery API**
porque NASA las archivó. Tampoco usa EONET v2.1, que está obsoleta.

## Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Una clave gratuita de [NASA Open APIs](https://api.nasa.gov/)
- Conexión a Internet para las APIs y los recursos CDN

## Configurar la clave de NASA

Abre PowerShell o la terminal en la carpeta que contiene `NasaExplorer.csproj`:

```powershell
dotnet user-secrets set "Nasa:ApiKey" "TU_CLAVE_DE_NASA"
```

Comprueba que se ha guardado para este proyecto:

```powershell
dotnet user-secrets list
```

Debe aparecer una entrada llamada `Nasa:ApiKey`. La clave no se escribe en
`appsettings.json`, no se incluye en Git y no se envía nunca al navegador.

Como alternativa local, copia `appsettings.Local.example.json` como
`appsettings.Local.json` y escribe allí la clave. Ese fichero está incluido en
`.gitignore`.

## Ejecutar

```powershell
dotnet restore
dotnet run
```

La consola mostrará la dirección, normalmente `https://localhost:7077` o
`http://localhost:5077`.

En el primer arranque se crea automáticamente `nasa-explorer.db` con las tablas
de Identity y favoritos.

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

MonsterASP.NET permite alojar este proyecto con .NET 10. Desde Visual Studio
Code se recomienda generar la publicación mediante la terminal integrada y
subir su contenido con WebFTP.

### Preparar la publicación desde VS Code

Desde la carpeta `NasaExplorer`:

```bash
dotnet restore
dotnet publish -c Release -o publicacion
```

En PowerShell, genera un ZIP cuyo contenido quede en la raíz:

```powershell
Compress-Archive -Path .\publicacion\* -DestinationPath .\publicacion.zip -Force
```

En Linux o macOS (si el comando no está disponible, instala el paquete
`zip` desde el gestor de paquetes del sistema):

```bash
rm -f publicacion.zip
(cd publicacion && zip -r ../publicacion.zip .)
```

En MonsterASP.NET:

1. Abre **Files** y entra en `/wwwroot`.
2. Sube `publicacion.zip`.
3. Extrae el archivo dentro de `/wwwroot`.
4. Permite sobrescribir los archivos de la aplicación, pero no elimines
   previamente todo el directorio.
5. Reinicia la aplicación o el AppPool.

`NasaExplorer.dll`, `web.config`, `appsettings.json` y la carpeta
`wwwroot` deben quedar directamente en el `/wwwroot` del alojamiento. No
subas el código fuente, el `.csproj`, `bin` ni `obj`.

MonsterASP.NET ofrece una
[guía para publicar mediante ZIP](https://help.monsterasp.net/books/deploy/page/how-to-deploy-website-content-from-zip-file).
WebDeploy puede utilizarse como
[alternativa desde Visual Studio](https://help.monsterasp.net/books/deploy/page/how-to-deploy-net-core-web-application-using-visual-studio).

### Configurar la clave de NASA

Las claves guardadas con `dotnet user-secrets` no se publican. Abre:

```text
Websites → Manage website → Scripting → Environment Variables
```

Y añade:

```text
Nombre: Nasa__ApiKey
Valor:  TU_CLAVE_DE_NASA
```

Escribe solamente la clave, sin comillas y sin añadir `api_key=`. Guarda los
cambios y reinicia la aplicación o el AppPool. `Nasa__ApiKey` representa
`Nasa:ApiKey`; este formato se explica en la
[documentación de variables de entorno](https://help.monsterasp.net/books/development/page/environment-variables-as-configuration-store).

No publiques `appsettings.Local.json`. La clave es necesaria para APOD y
NeoWs; los demás módulos utilizados tienen acceso público.

### Identity y conservación de la base

Las cuentas se crean localmente con Identity. No se necesita autenticación con
terceros ni confirmación por correo. Usuarios y favoritos se almacenan en
`nasa-explorer.db`, situado en la raíz publicada de la aplicación.

`EnsureCreatedAsync` crea el archivo y todas las tablas si todavía no existen.
En despliegues posteriores:

- conserva `nasa-explorer.db`;
- no actives la eliminación de archivos adicionales en WebDeploy;
- no subas encima una base de desarrollo vacía;
- realiza copias de seguridad antes de actualizar;
- recuerda que `EnsureCreatedAsync` no actualiza el esquema de una base ya
  creada.

### Comprobar el despliegue

1. Comprueba que APOD y Asteroides funcionan con la clave.
2. Registra una cuenta y vuelve a iniciar sesión después de cerrarla.
3. Añade un elemento a favoritos.
4. Reinicia la aplicación y comprueba que usuario y favorito permanecen.
5. Abre también un módulo público, como EONET o Exoplanetas.

Si el sitio devuelve un error HTTP 500, revisa
`Control Panel → Websites → Manage → Logs`. Los
[logs de depuración](https://help.monsterasp.net/books/debugging/page/aspnet-core-debug-logging)
pueden activarse temporalmente y deben desactivarse después de diagnosticar el
problema.


## Registro y acceso

1. Pulsa **Registrarse**.
2. Escribe un correo con formato válido y una contraseña de al menos seis
   caracteres, una minúscula y un número.
3. La sesión se abre inmediatamente.
4. Usa las estrellas de cada módulo para añadir o quitar favoritos.

No se envía un correo de confirmación. Esta configuración está pensada para
prácticas locales. En una aplicación pública convendría confirmar el correo,
añadir recuperación de contraseña, doble factor y una política de privacidad.

## Estructura

```text
NasaExplorer/
├── Configuracion/       Opciones enlazadas con appsettings y User Secrets
├── Data/                DbContext de Identity y favoritos
├── DTOs/                Modelos de las respuestas JSON
├── Modelos/             Usuario y favorito persistente
├── Pages/
│   ├── Apod/
│   ├── Asteroides/
│   ├── ClimaEspacial/
│   ├── Cuenta/
│   ├── Exoplanetas/
│   ├── Favoritos/
│   ├── Multimedia/
│   ├── Tierra/
│   └── Shared/
├── Servicios/           Acceso a APIs, caché y acceso a SQLite
├── wwwroot/              CSS y JavaScript propios
└── Program.cs            Registro de dependencias y tubería HTTP
```

## Ideas para ejercicios

- Añadir ordenación a la tabla de asteroides.
- Crear un detalle local para cada exoplaneta.
- Representar el tamaño relativo de los planetas con CSS.
- Añadir capas GIBS al mapa de EONET.
- Sustituir `EnsureCreatedAsync` por migraciones de EF Core.
- Crear pruebas unitarias para la construcción de consultas ADQL.
- Añadir roles de usuario y una zona de administración.

## Solución de problemas

### La portada dice que falta la clave

Asegúrate de ejecutar `dotnet user-secrets` en la carpeta exacta del proyecto.
El `.csproj` contiene este identificador:

```xml
<UserSecretsId>NasaExplorer-EOI-2026</UserSecretsId>
```

Después ejecuta:

```powershell
dotnet user-secrets list
dotnet clean
dotnet run
```

### Una API devuelve 503

Es un error temporal del servicio remoto. Cada sección muestra su propio aviso y
no bloquea las demás. NeoWs, en particular, puede entrar en mantenimiento.

### Quiero reiniciar todos los usuarios y favoritos

Detén la aplicación y borra `nasa-explorer.db`. Al volver a ejecutar se creará
una base vacía. Esta operación elimina todas las cuentas y colecciones locales.

### He cambiado las entidades y la base no se actualiza

`EnsureCreatedAsync` no aplica cambios de esquema. Para una práctica rápida,
borra la base. Para conservar datos, crea migraciones:

```powershell
dotnet ef migrations add NombreDelCambio
dotnet ef database update
```

## Límites y créditos

Una clave normal de `api.nasa.gov` dispone normalmente de 1.000 peticiones por
hora. La aplicación utiliza caché para no repetir consultas iguales.

Los datos y recursos pertenecen a NASA y a las fuentes indicadas por sus APIs.
Las teselas del mapa pertenecen a OpenStreetMap y mantienen su atribución
visible.

## Licencia

El código de este ejemplo se distribuye con licencia MIT. Los datos, imágenes,
vídeos, nombres y marcas de terceros mantienen sus propias condiciones de uso.
