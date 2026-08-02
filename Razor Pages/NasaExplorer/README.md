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

> **Método recomendado:** utiliza WebFTP. El archivo ZIP se conserva como
> alternativa.

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
