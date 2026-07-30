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

## Posibles ampliaciones para clase

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
