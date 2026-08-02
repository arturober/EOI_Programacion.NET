# Rick and Morty — Razor Pages

Aplicación educativa desarrollada con **ASP.NET Core Razor Pages y .NET 10**.
Consume [The Rick and Morty API](https://rickandmortyapi.com/), permite crear
cuentas locales y guarda los personajes favoritos de cada usuario en SQLite.

La API es pública y no requiere token, clave ni registro.

## Funcionalidades

- Portada con cifras generales y personajes destacados.
- Catálogo paginado de personajes.
- Filtros por nombre, estado, especie y género.
- Ficha completa con origen, última localización y apariciones.
- Guía de episodios con búsqueda por título o código.
- Detalle de cada episodio y todos sus personajes.
- Catálogo de ubicaciones con nombre, tipo y dimensión.
- Detalle de localización y selección de residentes.
- Navegación entre recursos relacionados.
- Página de ayuda con recursos, filtros y ejemplos de la API.
- Registro e inicio de sesión con ASP.NET Core Identity.
- Registro inmediato, sin confirmación obligatoria por correo.
- Colección privada de personajes favoritos por usuario.
- SQLite creada automáticamente en el primer arranque.
- Caché en memoria para reducir llamadas externas.
- Bootstrap, Bootswatch, Bootstrap Icons y SweetAlert2 desde CDN.
- Selector de temas guardado mediante `localStorage`.
- Mensajes y confirmaciones con SweetAlert2.
- Interfaz adaptable a ordenador, tableta y móvil.

## Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Conexión a internet para la API y los recursos CDN.

No hay que obtener ni configurar ninguna clave de acceso.

## Puesta en marcha

Abre un terminal en la carpeta que contiene `RickAndMorty.csproj`:

```powershell
cd "ruta\hasta\RickAndMorty"
dotnet restore
dotnet run
```

Abre la dirección que aparece en la consola, normalmente:

```text
https://localhost:7174
```

o:

```text
http://localhost:5193
```

## Publicación en MonsterASP.NET

> **Método recomendado:** utiliza WebFTP.

MonsterASP.NET es compatible con aplicaciones ASP.NET Core creadas con .NET 10.
El procedimiento recomendado desde Visual Studio Code es publicar mediante la
terminal integrada y subir el resultado mediante WebFTP.

### Preparar la publicación desde VS Code

Desde la carpeta `RickAndMorty`:

```bash
dotnet restore
dotnet publish -c Release -o publicacion
```

En PowerShell:

```powershell
Compress-Archive -Path .\publicacion\* -DestinationPath .\publicacion.zip -Force
```

Para desplegarlo:

1. Abre **Files** en el panel de MonsterASP.NET.
2. Entra en `/wwwroot`.
3. Sube y extrae `publicacion.zip`.
4. Sustituye los archivos anteriores, pero no borres previamente todo el
   directorio.
5. Reinicia la aplicación o el AppPool.

En `/wwwroot` deben quedar directamente `RickAndMorty.dll`, `web.config`,
`appsettings.json` y la carpeta `wwwroot`. No subas el código fuente, el
`.csproj`, `bin` ni `obj`.

Consulta la
[guía oficial de publicación mediante ZIP](https://help.monsterasp.net/books/deploy/page/how-to-deploy-website-content-from-zip-file).
WebDeploy queda como
[alternativa para Visual Studio](https://help.monsterasp.net/books/deploy/page/how-to-deploy-net-core-web-application-using-visual-studio).

### Configuración externa

The Rick and Morty API es pública. No necesita una clave, un token ni una
variable de entorno. Tampoco debes crear una credencial ficticia en
`appsettings.json`.

La autenticación de los usuarios pertenece exclusivamente a la aplicación y se
realiza con ASP.NET Core Identity.

### Identity y conservación de SQLite

No hay que configurar Google, Microsoft ni un servidor SMTP. El correo no
necesita confirmación. Identity almacena cuentas, contraseñas protegidas y
personajes favoritos en `Data/rickandmorty.db`.

`EnsureCreatedAsync` crea la carpeta, el archivo y todas las tablas en el
primer arranque. En las actualizaciones:

- conserva `Data/rickandmorty.db`;
- no habilites una opción que elimine archivos adicionales del destino;
- no subas encima una base local vacía;
- realiza una copia de seguridad antes de cambiar el modelo;
- recuerda que `EnsureCreatedAsync` no migra una base ya existente.

### Comprobación posterior

1. Abre personajes, episodios y localizaciones.
2. Registra una cuenta.
3. Cierra la sesión y vuelve a iniciarla.
4. Guarda un personaje favorito.
5. Reinicia la aplicación y comprueba que cuenta y favorito continúan.

Si aparece un error HTTP 500, consulta
`Control Panel → Websites → Manage → Logs`. Los
[logs de depuración de ASP.NET Core](https://help.monsterasp.net/books/debugging/page/aspnet-core-debug-logging)
pueden habilitarse temporalmente y deben desactivarse después.

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

## Registro e inicio de sesión

Identity se configura en `Program.cs` con:

- correo electrónico único;
- contraseña de al menos 8 caracteres;
- una mayúscula, una minúscula y un número;
- bloqueo de cinco minutos después de cinco intentos fallidos;
- confirmación de correo desactivada.

El usuario inicia sesión automáticamente al registrarse. Las contraseñas no
se guardan en texto plano: Identity aplica hash, sal y sus mecanismos de
seguridad internos.

## Base de datos

La aplicación crea `Data/rickandmorty.db` en el primer arranque mediante
`EnsureCreatedAsync()`. En ella se guardan:

- las tablas de usuarios, roles, cookies y tokens de Identity;
- la tabla `PersonajesFavoritos`.

Existe un índice único para `UsuarioId + PersonajeId`. Un usuario no puede
guardar dos veces el mismo personaje, pero usuarios diferentes sí pueden
guardar el mismo.

Para reiniciar todos los usuarios y favoritos durante una práctica:

1. Detén la aplicación.
2. Elimina `Data/rickandmorty.db`.
3. Ejecuta de nuevo `dotnet run`.

No lo hagas si necesitas conservar los datos.

## Estructura

```text
RickAndMorty/
├── Configuracion/        Opciones de caché
├── Data/                 DbContext y base de datos en ejecución
├── DTOs/                 Respuestas JSON de la API
├── Modelos/              Usuario, favorito y paginación
├── Pages/
│   ├── Cuenta/           Registro, login y logout
│   ├── Episodios/        Listado y detalle
│   ├── Favoritos/        Colección privada y acciones POST
│   ├── Localizaciones/   Listado, filtros y residentes
│   ├── Personajes/       Catálogo, filtros y ficha completa
│   ├── Shared/           Layout, tarjetas y paginación
│   ├── Acerca.*          Información sobre el proyecto
│   └── AyudaApi.*        Guía de la API externa
├── Servicios/            API, caché, favoritos y traducciones
└── wwwroot/              JavaScript e imagen alternativa
```

## Endpoints externos utilizados

La dirección base es `https://rickandmortyapi.com`. La aplicación no publica
una API propia: consume los siguientes endpoints públicos y muestra los datos
mediante Razor Pages.

| Función | Endpoint |
|---|---|
| Personajes | `GET /api/character` |
| Personaje individual | `GET /api/character/{id}` |
| Varios personajes | `GET /api/character/{id1,id2,...}` |
| Episodios | `GET /api/episode` |
| Episodio individual | `GET /api/episode/{id}` |
| Varios episodios | `GET /api/episode/{id1,id2,...}` |
| Ubicaciones | `GET /api/location` |
| Ubicación individual | `GET /api/location/{id}` |

Todos los listados aceptan los filtros documentados por la API. Cada página
contiene hasta 20 resultados. La guía integrada se abre desde el enlace
**Ayuda de la API** del pie de página.

## Cómo funciona el servicio de la API

`RickAndMortyServicio` es el único componente que conoce la dirección de la
API. Sus responsabilidades son:

1. Construir las URL y sus filtros.
2. Realizar peticiones asíncronas con `HttpClient`.
3. Deserializar JSON en DTO.
4. Guardar resultados en `IMemoryCache`.
5. Convertir los errores técnicos en mensajes comprensibles.
6. Obtener varios recursos relacionados en una única petición.

Una búsqueda sin resultados recibe un código HTTP 404 de esta API. El
servicio lo transforma en una página vacía para que la interfaz pueda mostrar
«No hay resultados» en lugar de tratarlo como un error.

## Relaciones entre recursos

La API no devuelve objetos completos dentro de las relaciones, sino URL:

- un personaje contiene las URL de sus episodios;
- un episodio contiene las URL de sus personajes;
- una localización contiene las URL de sus residentes;
- un personaje contiene la URL de su origen y localización actual.

`TextoRickAndMorty.ExtraerId()` obtiene el identificador final de cada URL y el
servicio utiliza los endpoints de múltiples identificadores para reducir el
número de peticiones.

Las ubicaciones con más de 40 residentes muestran los primeros 40 para
evitar direcciones excesivamente largas y respuestas innecesariamente
pesadas. La interfaz lo indica expresamente.

## Caché

La duración predeterminada es de 30 minutos:

```json
{
  "RickAndMortyApi": {
    "MinutosCache": 30
  }
}
```

Se puede cambiar en `appsettings.json`. El servicio limita el valor efectivo
entre 5 y 240 minutos.

## Recursos desde CDN

El proyecto no instala dependencias front-end:

- Bootstrap 5.3.8
- Bootswatch 5.3.8
- Bootstrap Icons 1.13.1
- SweetAlert2 11.26.25

El selector permite cambiar entre Bootstrap claro, Bootstrap oscuro y varios
temas Bootswatch. La elección se conserva en el navegador.

## Seguridad

- Las dependencias de Identity, Entity Framework Core y SQLite se mantienen en
  versiones corregidas de .NET 10.
- Los cambios en favoritos utilizan formularios POST.
- Razor Pages añade automáticamente el token antifalsificación.
- Las páginas privadas llevan el atributo `[Authorize]`.
- Cada consulta SQLite filtra por el identificador del usuario.
- Las URL de retorno se validan con `Url.IsLocalUrl`.
- Los errores externos no muestran trazas internas en la interfaz.

## Posibles ampliaciones

- Añadir notas personales a los favoritos.
- Permitir ordenar personajes por nombre o número de episodios.
- Crear una lista de episodios vistos por usuario.
- Guardar una valoración personal de cada episodio.
- Sustituir `EnsureCreated` por migraciones de Entity Framework.
- Añadir pruebas unitarias para servicios y PageModel.
- Crear una API propia de favoritos.
- Comparar el consumo REST con el endpoint GraphQL disponible.

## Créditos y licencia

Datos e imágenes proporcionados por
[The Rick and Morty API](https://rickandmortyapi.com/).

Rick and Morty y sus personajes pertenecen a sus respectivos titulares. Este
repositorio es un proyecto educativo no oficial.

El código del proyecto se distribuye con licencia MIT.
