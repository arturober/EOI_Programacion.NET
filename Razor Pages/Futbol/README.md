# Fútbol — Razor Pages y football-data.org

Aplicación educativa desarrollada con **ASP.NET Core Razor Pages y .NET 10**.
Consume la API v4 de
[football-data.org](https://www.football-data.org/), permite crear cuentas
locales y guarda los equipos favoritos de cada usuario en SQLite.

## Funcionalidades

- Partidos de hoy y calendario por fecha.
- Competiciones disponibles según el plan de la API.
- Clasificación general con estadísticas completas.
- Partidos recientes y próximos por competición.
- Goleadores, cuando el plan contratado incluye esos datos.
- Equipos, ficha del club, entrenador, plantilla y calendario.
- Registro e inicio de sesión con ASP.NET Core Identity.
- Registro inmediato, sin confirmación obligatoria por correo.
- Lista privada de equipos favoritos por usuario.
- Base de datos local SQLite creada automáticamente.
- Caché en memoria para reducir llamadas a la API.
- Bootstrap, Bootswatch, Bootstrap Icons y SweetAlert2 desde CDN.
- Selector de tema con persistencia en `localStorage`.
- Tratamiento de errores, imágenes alternativas y diseño adaptable.

## Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Una cuenta y un token de
  [football-data.org](https://www.football-data.org/client/register)

El plan gratuito incluye un número limitado de competiciones, resultados con
retraso y un máximo de 10 peticiones por minuto. La caché de la aplicación
ayuda a trabajar dentro de ese límite.

## Puesta en marcha

Abre un terminal en la carpeta exacta que contiene `Futbol.csproj`:

```powershell
cd "ruta\hasta\Futbol"
dotnet restore
```

Guarda el token con User Secrets:

```powershell
dotnet user-secrets set "FootballData:ApiKey" "TU_TOKEN"
```

Comprueba que se ha guardado en este proyecto:

```powershell
dotnet user-secrets list
```

Ejecuta la aplicación:

```powershell
dotnet run
```

Abre la dirección que aparece en la consola, normalmente
`https://localhost:7168` o `http://localhost:5187`.

## Dónde guarda User Secrets el token

El archivo del proyecto contiene este identificador:

```xml
<UserSecretsId>Futbol-FootballData-2026</UserSecretsId>
```

En Windows, `dotnet user-secrets` guarda los valores fuera del proyecto en:

```text
%APPDATA%\Microsoft\UserSecrets\Futbol-FootballData-2026\secrets.json
```

El token no se añade al repositorio y no llega al navegador. El servicio
`FutbolServicio` lo incorpora en el servidor mediante la cabecera
`X-Auth-Token`.

Si la portada sigue diciendo que falta el token, comprueba que:

1. El comando se ejecutó en la carpeta donde está `Futbol.csproj`.
2. `dotnet user-secrets list` muestra `FootballData:ApiKey`.
3. Cerraste y volviste a ejecutar `dotnet run`.
4. No estás iniciando otro proyecto o perfil distinto.

## Alternativa local a User Secrets

Copia `appsettings.Local.example.json` como `appsettings.Local.json`:

```json
{
  "FootballData": {
    "ApiKey": "TU_TOKEN"
  }
}
```

`appsettings.Local.json` está excluido mediante `.gitignore`. User Secrets
sigue siendo la opción recomendada durante el desarrollo.

También se puede usar la variable de entorno:

```powershell
$env:FootballData__ApiKey = "TU_TOKEN"
dotnet run
```

Los dos guiones bajos representan los dos puntos de la clave de configuración.

## Publicación en MonsterASP.NET

> **Método recomendado:** utiliza WebFTP. El archivo ZIP se conserva como
> alternativa.

MonsterASP.NET admite aplicaciones ASP.NET Core con .NET 10. Para trabajar
desde Visual Studio Code, la opción recomendada es generar una publicación,
y subir su contenido mediante WebFTP.

### Preparar la publicación desde VS Code

Abre la terminal integrada en la carpeta `Futbol`:

```bash
dotnet restore
dotnet publish -c Release -o publicacion
```

En PowerShell:

```powershell
Compress-Archive -Path .\publicacion\* -DestinationPath .\publicacion.zip -Force
```

Después:

1. Abre **Files** en el sitio de MonsterASP.NET.
2. Entra en `/wwwroot`.
3. Sube `publicacion.zip`.
4. Extráelo dentro de `/wwwroot`.
5. Sustituye los archivos anteriores de la aplicación sin borrar el resto del
   directorio.
6. Reinicia la aplicación o el AppPool.

Dentro de `/wwwroot` deben quedar directamente `Futbol.dll`, `web.config`,
`appsettings.json`, `wwwroot` y los demás archivos publicados. No subas el
código fuente, el `.csproj`, `bin` ni `obj`.

Consulta la
[guía de publicación mediante ZIP](https://help.monsterasp.net/books/deploy/page/how-to-deploy-website-content-from-zip-file).
WebDeploy queda como
[alternativa para Visual Studio](https://help.monsterasp.net/books/deploy/page/how-to-deploy-net-core-web-application-using-visual-studio).

### Configurar el token

User Secrets solo se utiliza en el equipo de desarrollo y no acompaña a la
aplicación publicada. En el panel abre:

```text
Websites → Manage website → Scripting → Environment Variables
```

Añade esta variable:

```text
Nombre: FootballData__ApiKey
Valor:  TU_TOKEN
```

Introduce únicamente el token, sin comillas y sin escribir `X-Auth-Token`.
La aplicación añade esa cabecera a las peticiones. Guarda los cambios y
reinicia la aplicación o el AppPool. Los dos guiones bajos equivalen a
`FootballData:ApiKey`, según la
[documentación de MonsterASP.NET](https://help.monsterasp.net/books/development/page/environment-variables-as-configuration-store).

### Identity y base de datos

Las cuentas son locales y no requieren Google, Microsoft ni un servidor SMTP.
El correo no tiene que confirmarse. Identity guarda las cuentas, los hashes de
contraseña y los equipos favoritos en `Data/futbol.db`.

La base y sus tablas se crean automáticamente en el primer arranque. Para no
perder usuarios al actualizar:

- conserva `Data/futbol.db`;
- no habilites la eliminación de archivos adicionales del destino al publicar;
- no sustituyas la base del servidor por una copia local vacía;
- realiza una copia de seguridad antes de cambiar el modelo;
- ten en cuenta que `EnsureCreatedAsync` no migra una base existente.

### Comprobación posterior

1. Comprueba que se muestran las competiciones y no aparece el aviso de token.
2. Registra una cuenta, cierra la sesión y vuelve a entrar.
3. Guarda un equipo favorito.
4. Reinicia la aplicación y verifica que la cuenta y el favorito continúan.

Un `401` suele indicar un token ausente o incorrecto; un `403`, una
competición no permitida por el plan, y un `429`, exceso de peticiones. Para
errores HTTP 500 consulta `Websites → Manage → Logs` o habilita
temporalmente los
[logs de ASP.NET Core](https://help.monsterasp.net/books/debugging/page/aspnet-core-debug-logging).

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
- mayúscula, minúscula y número;
- bloqueo de 5 minutos después de 5 intentos fallidos;
- confirmación de correo desactivada.

Al registrarse correctamente, el usuario inicia sesión de forma automática.
Identity aplica hash y sal a las contraseñas; nunca se guardan en texto plano.

## Base de datos

La aplicación crea `Data/futbol.db` en el primer arranque mediante
`EnsureCreatedAsync()`. La base de datos incluye las tablas de Identity y
`EquiposFavoritos`.

Existe un índice único para la pareja `UsuarioId + EquipoId`, por lo que un
usuario no puede guardar dos veces el mismo equipo. Dos usuarios distintos sí
pueden guardar el mismo club.

Para reiniciar todos los usuarios y favoritos durante las prácticas:

1. Detén la aplicación.
2. Elimina `Data/futbol.db`.
3. Ejecuta de nuevo `dotnet run`.

No hagas esto si necesitas conservar los datos.

## Estructura del proyecto

```text
Futbol/
├── Configuracion/       Opciones de football-data.org
├── Data/                DbContext y archivo SQLite en ejecución
├── DTOs/                Clases para deserializar la API
├── Modelos/             Usuario y equipo favorito
├── Pages/
│   ├── Competiciones/   Listado, tabla, partidos, goleadores y equipos
│   ├── Cuenta/          Registro, acceso, salida y acceso denegado
│   ├── Equipos/         Ficha, plantilla y calendario
│   ├── Favoritos/       Colección privada y acciones POST
│   ├── Partidos/        Calendario por fecha
│   └── Shared/          Plantilla y tarjeta de partido
├── Servicios/           API, caché, favoritos y textos
└── wwwroot/             JavaScript e imagen alternativa
```

## Endpoints externos utilizados

| Función | Endpoint de football-data.org |
|---|---|
| Competiciones | `GET /v4/competitions` |
| Partidos por fecha | `GET /v4/matches?dateFrom=...&dateTo=...` |
| Clasificación | `GET /v4/competitions/{codigo}/standings` |
| Partidos de competición | `GET /v4/competitions/{codigo}/matches` |
| Goleadores | `GET /v4/competitions/{codigo}/scorers` |
| Equipos de competición | `GET /v4/competitions/{codigo}/teams` |
| Ficha del equipo | `GET /v4/teams/{id}` |
| Partidos del equipo | `GET /v4/teams/{id}/matches` |

Todas las peticiones se realizan desde `FutbolServicio`. Las vistas nunca
conocen el token.

## Caché y límites

`FootballData:MinutosCache` vale 15 por defecto. Las fichas y catálogos poco
variables duran cuatro veces más. Puedes modificarlo en `appsettings.json`,
pero el servicio impone un mínimo de cinco minutos para evitar llamadas
excesivas.

Las secciones de una competición se cargan por separado. Así, abrir la
clasificación no descarga también goleadores, partidos y equipos.

## Recursos desde CDN

El proyecto no instala paquetes front-end:

- Bootstrap 5.3.8
- Bootswatch 5.3.8
- Bootstrap Icons 1.13.1
- SweetAlert2 11.26.25

Se necesita conexión a internet tanto para estos recursos como para consultar
football-data.org.

## Posibles ampliaciones

- Crear migraciones de Entity Framework en lugar de `EnsureCreated`.
- Guardar competiciones favoritas además de equipos.
- Añadir notas personales a cada equipo.
- Filtrar partidos por competición o estado.
- Crear una página de enfrentamientos directos.
- Añadir roles de usuario y una zona de administración.
- Escribir pruebas unitarias para servicios y PageModel.
- Sustituir la caché en memoria por una caché distribuida.

## Atribución y licencia

Datos proporcionados por
[football-data.org](https://www.football-data.org/). La atribución también
aparece de forma visible en el pie de la aplicación.

El código de este proyecto se distribuye con licencia MIT.
