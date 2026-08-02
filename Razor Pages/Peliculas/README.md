# Películas

Aplicación web educativa creada con **ASP.NET Core 10**, Razor Pages,
ASP.NET Core Identity, Entity Framework Core, SQLite y la API de
[The Movie Database (TMDB)](https://www.themoviedb.org/).

Permite explorar películas, consultar fichas completas, registrarse sin
confirmación obligatoria de correo e iniciar sesión para mantener una lista
privada de favoritas.

## Funciones principales

- Portada con carrusel de tendencias, cartelera y películas mejor valoradas.
- Listados de tendencias, cartelera, populares, mejor valoradas y próximos
  estrenos.
- Buscador con paginación.
- Ficha detallada con sinopsis, géneros, reparto, dirección, tráiler
  incrustado, recomendaciones y proveedores disponibles en España.
- Búsqueda alternativa del tráiler en inglés cuando no está disponible en
  el idioma configurado.
- Registro e inicio de sesión locales mediante ASP.NET Core Identity.
- Acceso inmediato después del registro, sin confirmar el correo.
- Contraseñas almacenadas por Identity mediante hash, nunca como texto.
- Bloqueo temporal después de cinco intentos de acceso fallidos.
- Favoritas independientes para cada usuario y persistidas en SQLite.
- Interfaz adaptable a móvil creada con Bootstrap.
- Selector de Bootstrap y temas Bootswatch servido desde CDN.
- Confirmaciones y avisos con SweetAlert2 servido desde CDN.
- Pequeña API JSON de ejemplo que reutiliza los mismos servicios.
- Página de ayuda con los endpoints completos, explicados y enlazados.
- Caché en memoria para reducir llamadas repetidas a TMDB.
- Código organizado por responsabilidades y comentado en español.

## Tecnologías

| Tecnología | Uso |
|---|---|
| .NET 10 y Razor Pages | Aplicación web y renderizado del HTML |
| ASP.NET Core Identity | Usuarios, contraseñas, cookies y bloqueos |
| Entity Framework Core 10.0.10 | Acceso a datos |
| SQLite | Usuarios, credenciales protegidas y favoritas |
| SQLitePCLRaw 2.1.12 | Biblioteca nativa de SQLite actualizada |
| API de TMDB v3 | Catálogo y fichas de películas |
| Bootstrap 5.3.8 | Componentes y diseño adaptable |
| Bootswatch 5.3.8 | Temas visuales |
| SweetAlert2 11.26.25 | Confirmaciones y mensajes |

Bootstrap, Bootswatch, Bootstrap Icons y SweetAlert2 están enlazados desde
CDN. No es necesario instalarlos con npm.

## Requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download/dotnet/10.0)
- Una cuenta gratuita de
  [TMDB](https://www.themoviedb.org/signup)
- Un **API Read Access Token** de TMDB

## 1. Conseguir el token de TMDB

1. Inicia sesión en TMDB.
2. Abre la configuración de tu cuenta.
3. Entra en la sección **API** y solicita acceso si todavía no lo tienes.
4. Copia el valor denominado **API Read Access Token**.

El proyecto utiliza el token largo de lectura como credencial `Bearer`. No
utiliza la clave corta de API v3 en el parámetro `api_key`.

## 2. Configurar el secreto

Abre un terminal en la carpeta que contiene `Peliculas.csproj` y ejecuta:

```bash
dotnet user-secrets set "Tmdb:TokenAcceso" "PEGA_AQUI_TU_TOKEN_DE_LECTURA"
```

Puedes comprobar que se ha guardado con:

```bash
dotnet user-secrets list
```

La clave debe llamarse exactamente `Tmdb:TokenAcceso`. El identificador
`UserSecretsId` ya está incluido en el archivo del proyecto.

### Alternativas

Para desarrollo también puedes copiar `appsettings.Local.example.json` como
`appsettings.Local.json` y escribir allí el token. Ese archivo está excluido
de Git:

```json
{
  "Tmdb": {
    "TokenAcceso": "PEGA_AQUI_TU_TOKEN_DE_LECTURA"
  }
}
```

Otra opción es utilizar una variable de entorno:

```bash
export Tmdb__TokenAcceso="PEGA_AQUI_TU_TOKEN_DE_LECTURA"
```

En PowerShell:

```powershell
$env:Tmdb__TokenAcceso = "PEGA_AQUI_TU_TOKEN_DE_LECTURA"
```

No escribas un token real en `appsettings.json` ni lo subas a GitHub.

## 3. Ejecutar la aplicación

Desde la carpeta del proyecto:

```bash
dotnet restore
dotnet run
```

Abre la dirección que muestre el terminal. Con el perfil incluido suele ser:

- `https://localhost:7090`
- `http://localhost:5090`

En el primer arranque se crea automáticamente `Data/peliculas.db` con las
tablas de Identity, películas y favoritas.

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

MonsterASP.NET admite aplicaciones ASP.NET Core con .NET 10. Como el proyecto
se trabaja principalmente desde Visual Studio Code, el método recomendado es
usar la terminal integrada y subir el resultado mediante WebFTP.

### Preparar la publicación desde VS Code

Desde la carpeta `Peliculas`:

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

Este comando coloca directamente en el ZIP los archivos de `publicacion`, sin
crear una carpeta intermedia.

### Subir el ZIP

1. Abre **Files** en el panel del sitio.
2. Entra en `/wwwroot`.
3. Sube `publicacion.zip`.
4. Extrae el archivo dentro de `/wwwroot`.
5. Permite sobrescribir los archivos de la aplicación, pero no borres
   previamente todo el directorio.
6. Reinicia la aplicación o el AppPool.

En `/wwwroot` deben quedar directamente `Peliculas.dll`, `web.config`,
`appsettings.json`, la carpeta `wwwroot` y el resto de archivos publicados.
No subas el código fuente, el `.csproj`, `bin` ni `obj`.

Consulta la
[guía de publicación mediante ZIP](https://help.monsterasp.net/books/deploy/page/how-to-deploy-website-content-from-zip-file).
WebDeploy queda como
[alternativa para quienes usen Visual Studio](https://help.monsterasp.net/books/deploy/page/how-to-deploy-net-core-web-application-using-visual-studio).

### Configurar el token de TMDB

User Secrets no se incluye al publicar. En el panel de MonsterASP.NET abre:

```text
Websites → Manage website → Scripting → Environment Variables
```

Añade:

```text
Nombre: Tmdb__TokenAcceso
Valor:  TU_API_READ_ACCESS_TOKEN
```

Utiliza el token largo denominado **API Read Access Token**, no la clave corta
`API Key`. Introduce solo el valor:

- sin comillas;
- sin espacios al principio o al final;
- sin escribir `Bearer`;
- sin añadir `Tmdb:TokenAcceso=`.

La aplicación construye automáticamente la cabecera `Authorization: Bearer`.
Los dos guiones bajos equivalen a `Tmdb:TokenAcceso`. Guarda los cambios y
reinicia la aplicación o el AppPool. Consulta la
[documentación de variables de entorno](https://help.monsterasp.net/books/development/page/environment-variables-as-configuration-store).

No publiques `appsettings.Local.json` ni guardes el token real en Git.

### Identity y conservación de SQLite

Las cuentas son locales: no hace falta configurar Google, Microsoft ni un
servidor de correo. El correo no necesita confirmación. Identity almacena
usuarios, contraseñas protegidas y favoritas en
`Data/peliculas.db`.

`EnsureCreatedAsync` crea la base y sus tablas en el primer arranque. En
publicaciones posteriores:

- conserva `Data/peliculas.db`;
- no actives la eliminación de archivos adicionales del destino en WebDeploy;
- no sustituyas la base del servidor por una base local vacía;
- realiza una copia de seguridad antes de actualizar;
- recuerda que `EnsureCreatedAsync` no migra una base existente.

### Comprobación posterior

1. Comprueba que desaparece el aviso «Falta el token de TMDB».
2. Abre un listado y la ficha de una película.
3. Registra una cuenta, cierra la sesión y vuelve a entrar.
4. Añade una película a favoritas.
5. Reinicia la aplicación y comprueba que cuenta y favorita permanecen.

Si el token falta o es incorrecto, TMDB normalmente devolverá `401`. Para
errores HTTP 500 consulta `Websites → Manage → Logs` o habilita temporalmente
los [logs de ASP.NET Core](https://help.monsterasp.net/books/debugging/page/aspnet-core-debug-logging);
desactívalos al terminar.


## Cómo se registra un usuario

1. Pulsa **Registrarse** en la barra superior.
2. Introduce nombre, correo, contraseña y confirmación de contraseña.
3. La contraseña debe tener al menos ocho caracteres, una mayúscula, una
   minúscula y un número.
4. Identity crea la cuenta, inicia la sesión y redirige al usuario.

No hay confirmación obligatoria de correo. Esta decisión está configurada en
`Program.cs` con:

```csharp
opciones.SignIn.RequireConfirmedEmail = false;
```

El correo actúa como nombre de acceso único. La aplicación no envía mensajes
y tampoco incluye recuperación de contraseña por correo, porque para ello
sería necesario configurar un proveedor de email y sus credenciales.

## Cómo funcionan las favoritas

La base de datos utiliza tres grupos de tablas:

- Las tablas de Identity guardan cuentas, hashes de contraseña, cookies
  persistentes y datos de seguridad.
- `Peliculas` guarda una copia breve de cada película marcada.
- `Favoritos` relaciona un usuario con una película mediante una clave
  compuesta.

Una misma película puede pertenecer a muchos usuarios, pero cada usuario solo
puede añadirla una vez. Todas las consultas de favoritas filtran por el
identificador del usuario autenticado.

El archivo SQLite y sus archivos auxiliares están excluidos de Git para no
publicar cuentas de prueba.

## Configuración

La sección `Tmdb` de `appsettings.json` contiene opciones no sensibles:

```json
{
  "Tmdb": {
    "TokenAcceso": "",
    "Idioma": "es-ES",
    "Region": "ES",
    "MinutosCache": 15
  }
}
```

| Opción | Descripción |
|---|---|
| `TokenAcceso` | Token secreto de lectura; se recomienda `user-secrets` |
| `Idioma` | Idioma solicitado a TMDB |
| `Region` | Región de estrenos y proveedores |
| `MinutosCache` | Duración de la caché de listados y fichas |

## API JSON incluida

Con la aplicación en ejecución:

| Método y ruta | Descripción |
|---|---|
| `GET /api/peliculas/populares?pagina=1` | Lista películas populares |
| `GET /api/peliculas/buscar?texto=matrix&pagina=1` | Busca por título |
| `GET /api/peliculas/603` | Devuelve una ficha completa |
| `GET /api/favoritos` | Favoritas del usuario autenticado |

El archivo `Peliculas.http` contiene ejemplos listos para Visual Studio,
Rider o la extensión REST Client de Visual Studio Code. La ruta de favoritas
necesita la cookie de una sesión iniciada.

La página `/Ayuda` muestra estas mismas direcciones utilizando el dominio y
el puerto actuales. Los enlaces se pueden abrir directamente para consultar
la respuesta JSON.

## Estructura

```text
Peliculas/
├── Configuracion/       Opciones de TMDB
├── Controllers/         API JSON de ejemplo
├── Data/                Contexto de Entity Framework
├── DTOs/                Formato exacto de las respuestas de TMDB
├── Modelos/             Usuarios, películas, favoritos y vistas
├── Pages/
│   ├── Cuenta/          Registro, acceso y cierre de sesión
│   ├── Favoritos/       Lista privada y altas o bajas
│   ├── Peliculas/       Listados, búsqueda y ficha
│   ├── Shared/          Diseño y tarjeta reutilizable
│   └── Ayuda.cshtml     Uso de la aplicación y endpoints JSON
├── Servicios/           Acceso a TMDB y lógica de favoritos
├── wwwroot/js/          Temas y avisos de SweetAlert
├── Program.cs           Configuración y canal de peticiones
└── appsettings.json     Configuración pública
```

## Base de datos y cambios del modelo

Este proyecto didáctico utiliza `EnsureCreatedAsync` para que funcione sin
crear migraciones. Si modificas las entidades durante el desarrollo, haz una
copia de los datos que necesites, detén la aplicación, elimina
`Data/peliculas.db` y vuelve a arrancar para regenerar el esquema.

En un proyecto de producción conviene sustituir este mecanismo por
migraciones de Entity Framework Core.

## Seguridad

- El token de TMDB se envía desde el servidor en la cabecera
  `Authorization: Bearer`; nunca se entrega al navegador.
- Los formularios Razor incluyen protección antifalsificación.
- Solo se aceptan redirecciones internas después de modificar favoritos.
- Las páginas y endpoints privados requieren autenticación.
- Las contraseñas las administra Identity y no se registran en logs.
- `appsettings.Local.json`, las claves y la base de datos no se versionan.

## Atribución

This product uses the TMDB API but is not endorsed or certified by TMDB.

Los datos y las imágenes proceden de
[The Movie Database](https://www.themoviedb.org/). La disponibilidad de
plataformas procede de JustWatch a través de TMDB.

Consulta las
[condiciones de uso y atribución de TMDB](https://developer.themoviedb.org/docs/faq).

## Licencia

El código se distribuye con licencia MIT. Consulta `LICENSE`.
