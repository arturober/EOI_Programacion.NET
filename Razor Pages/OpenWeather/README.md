# Open Weather

Aplicación educativa desarrollada con **ASP.NET Core 10**, **Razor Pages** y
la API de **OpenWeather**. Permite buscar cualquier localidad y consultar sus
condiciones meteorológicas sin enviar la clave privada al navegador.

La interfaz utiliza **Bootstrap**, **Bootswatch**, **Bootstrap Icons** y
**SweetAlert2**, todos cargados desde CDN. No contiene CSS personalizado.

## Funciones

- Búsqueda de localidades mediante la API de geocodificación.
- Elección entre lugares que comparten el mismo nombre.
- Uso opcional de la ubicación del navegador.
- Tiempo actual:
  - temperatura y sensación térmica;
  - humedad y presión atmosférica;
  - viento, dirección y rachas;
  - visibilidad y nubosidad;
  - horas locales de amanecer y atardecer.
- Previsión de cinco días.
- Detalle de las próximas 48 horas en intervalos de tres horas.
- Probabilidad y cantidad prevista de lluvia o nieve.
- Índice de calidad del aire y concentraciones de ocho contaminantes.
- Unidades métricas e imperiales.
- Bootstrap claro y oscuro, además de todos los temas de Bootswatch.
- Tema recordado mediante `localStorage`.
- Caché en memoria para reducir el número de llamadas externas.
- API JSON propia que nunca devuelve la clave de OpenWeather.
- Diseño adaptable a ordenadores, tabletas y móviles.

## Tecnologías

- .NET 10
- ASP.NET Core Razor Pages
- `HttpClient` e inyección de dependencias
- `System.Text.Json`
- `IMemoryCache`
- Bootstrap 5.3.8
- Bootswatch 5.3.8
- Bootstrap Icons 1.13.1
- SweetAlert2 11.26.25

No hay paquetes NuGet adicionales: todo lo necesario para el servidor forma
parte de ASP.NET Core.

## Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Una cuenta de [OpenWeather](https://openweathermap.org/)
- Una clave de API activa

La activación de una clave nueva puede tardar un tiempo. Si aparece un error
`401` justo después de crearla, espera y vuelve a intentarlo.

## Configurar la clave

La opción recomendada para desarrollar en local es **Secret Manager**. Abre
una terminal dentro de la carpeta `OpenWeather` y ejecuta:

```bash
dotnet user-secrets set "OpenWeather:ApiKey" "TU_CLAVE"
```

La clave se almacena fuera del proyecto y no se sube a GitHub.

### Alternativa 1: variable de entorno

PowerShell:

```powershell
$env:OpenWeather__ApiKey = "TU_CLAVE"
dotnet run
```

Símbolo del sistema de Windows:

```bat
set OpenWeather__ApiKey=TU_CLAVE
dotnet run
```

Linux o macOS:

```bash
export OpenWeather__ApiKey="TU_CLAVE"
dotnet run
```

ASP.NET Core utiliza dos guiones bajos (`__`) para representar los dos puntos
de `OpenWeather:ApiKey`.

### Alternativa 2: archivo local ignorado por Git

1. Copia `appsettings.Local.example.json`.
2. Cambia el nombre de la copia a `appsettings.Local.json`.
3. Sustituye el texto de ejemplo por tu clave.

`appsettings.Local.json` está incluido en `.gitignore`. No elimines esa regla.

## Ejecutar el proyecto

```bash
dotnet restore
dotnet run
```

La terminal mostrará una dirección local parecida a
`https://localhost:7000`. Ábrela en el navegador.

También se puede abrir `OpenWeather.sln` desde Visual Studio 2022 o una versión
posterior compatible con .NET 10.

## Publicación en MonsterASP.NET

> **Método recomendado:** utiliza WebFTP. El archivo ZIP se conserva como
> alternativa.

Open Weather no utiliza cuentas ni Identity, pero el servidor necesita la clave
de OpenWeather para consultar la API. MonsterASP.NET permite ejecutar el
proyecto con .NET 10.

### Preparar y subir la aplicación

Abre la carpeta del proyecto en Visual Studio Code y ejecuta desde su terminal
integrada:

```bash
dotnet restore
dotnet publish -c Release -o publicacion
```

En PowerShell, crea el archivo que se subirá:

```powershell
Compress-Archive -Path .\publicacion\* -DestinationPath .\publicacion.zip -Force
```

El comodín hace que los archivos queden en la raíz del ZIP. A continuación:

1. Abre **Files** en el panel de MonsterASP.NET.
2. Entra en `/wwwroot`.
3. Sube `publicacion.zip` y extráelo allí.
4. Permite sobrescribir los archivos anteriores sin borrar todo el directorio.
5. Reinicia la aplicación o el AppPool.

`OpenWeather.dll`, `web.config`, `appsettings.json` y la carpeta
`wwwroot` deben quedar directamente dentro del `/wwwroot` del alojamiento.
No subas el código fuente, el `.csproj`, `bin` ni `obj`.

Consulta la
[guía oficial para publicar mediante ZIP](https://help.monsterasp.net/books/deploy/page/how-to-deploy-website-content-from-zip-file).
Si se utiliza Visual Studio completo, WebDeploy está disponible como
[alternativa](https://help.monsterasp.net/books/deploy/page/how-to-deploy-net-core-web-application-using-visual-studio).

### Configurar la clave en el servidor

Los valores de `dotnet user-secrets` solo existen en el ordenador de
desarrollo. En MonsterASP.NET abre:

```text
Websites → Manage website → Scripting → Environment Variables
```

Añade:

```text
Nombre: OpenWeather__ApiKey
Valor:  TU_CLAVE
```

Introduce únicamente la clave, sin comillas ni parámetros como `appid=`.
`OpenWeather__ApiKey` equivale a `OpenWeather:ApiKey`. Guarda el cambio y
reinicia la aplicación o el AppPool. MonsterASP.NET explica esta correspondencia
en su [documentación de variables de entorno](https://help.monsterasp.net/books/development/page/environment-variables-as-configuration-store).

No subas `appsettings.Local.json` ni escribas la clave real en
`appsettings.json`.

### Comprobar el despliegue

1. Busca una localidad y selecciona uno de los resultados.
2. Comprueba tiempo actual, previsión y calidad del aire.
3. Abre `/api/lugares?texto=Alicante` para verificar la API JSON propia.
4. Cambia entre unidades métricas e imperiales.

Un error `401` suele significar que la clave es incorrecta o todavía no está
activa; un `429`, que se ha superado la cuota. Si aparece un error HTTP 500,
consulta `Control Panel → Websites → Manage → Logs` o habilita temporalmente
los [logs de depuración de ASP.NET Core](https://help.monsterasp.net/books/debugging/page/aspnet-core-debug-logging).
Desactiva esos logs cuando termines.

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

## Estructura

```text
OpenWeather/
├── Configuracion/
│   └── OpenWeatherOpciones.cs
├── Controllers/
│   └── TiempoController.cs
├── DTOs/
│   └── OpenWeatherDtos.cs
├── Modelos/
│   ├── InformeMeteorologico.cs
│   ├── Lugar.cs
│   └── Unidades.cs
├── Pages/
│   ├── Shared/_Layout.cshtml
│   ├── Acerca.cshtml
│   ├── Error.cshtml
│   ├── Index.cshtml
│   └── Index.cshtml.cs
├── Properties/
│   └── launchSettings.json
├── Servicios/
│   ├── IOpenWeatherServicio.cs
│   ├── OpenWeatherExcepcion.cs
│   └── OpenWeatherServicio.cs
├── wwwroot/js/
│   ├── sweetalert.js
│   ├── temas.js
│   └── ubicacion.js
├── OpenWeather.http
├── appsettings.json
└── Program.cs
```

## Cómo funciona

1. La Razor Page recibe el nombre de una localidad.
2. `OpenWeatherServicio` utiliza geocodificación para obtener sus coordenadas.
3. El servidor añade la clave y solicita el tiempo, la previsión y el aire.
4. Los DTO reciben únicamente los campos JSON que utiliza el proyecto.
5. El servicio transforma los DTO externos en modelos propios.
6. Razor genera el HTML y Bootstrap adapta la presentación.

La clave se utiliza exclusivamente en el servidor. Por eso no se encuentra en
el HTML, en JavaScript ni en las respuestas de la API propia.

`launchSettings.json` inicia `dotnet run` en el entorno `Development`, por lo
que las claves de usuario se cargan automáticamente durante el desarrollo.

## API JSON incluida

Buscar localidades:

```http
GET /api/lugares?texto=Alicante
```

Obtener el informe completo mediante coordenadas:

```http
GET /api/tiempo?lat=38.3452&lon=-0.4810&unidades=metrico
```

El parámetro `unidades` acepta `metrico` o `imperial`.

Estas rutas son útiles para aprender a crear un cliente JavaScript o una
aplicación móvil sin entregar al cliente la clave del proveedor.

El archivo `OpenWeather.http` contiene estas peticiones preparadas para
ejecutarlas desde Visual Studio, Rider o una extensión compatible de VS Code.

## Endpoints externos utilizados

- `GET /geo/1.0/direct`: geocodificación directa.
- `GET /geo/1.0/reverse`: geocodificación inversa.
- `GET /data/2.5/weather`: condiciones actuales.
- `GET /data/2.5/forecast`: previsión de 5 días y 3 horas.
- `GET /data/2.5/air_pollution`: calidad actual del aire.

Consulta la [documentación oficial de OpenWeather](https://openweathermap.org/api)
para comprobar los productos incluidos en tu plan y sus límites vigentes.

## Caché y consumo de cuota

Las respuestas se conservan durante 10 minutos mediante `IMemoryCache`. El
valor se puede cambiar en `appsettings.json`:

```json
{
  "OpenWeather": {
    "MinutosCache": 10
  }
}
```

La caché se guarda en la memoria del servidor y se pierde al reiniciar la
aplicación. En una instalación con varios servidores convendría sustituirla
por una caché distribuida.

## Seguridad

- No escribas una clave real en `appsettings.json`.
- No subas `appsettings.Local.json`.
- No añadas la clave a JavaScript ni a una dirección generada en el navegador.
- Revoca la clave desde OpenWeather si se publica accidentalmente.
- El registro de las peticiones de `HttpClient` está desactivado para evitar
  que una dirección con el parámetro `appid` aparezca en los registros.

La API propia consume tu cuota. Antes de publicar una aplicación abierta en
Internet, añade autenticación, limitación de peticiones o ambas.

## Datos y atribución

Weather data provided by [OpenWeather](https://openweathermap.org/).

Revisa las condiciones de tu plan antes de publicar o explotar comercialmente
el proyecto. La aplicación incluye una atribución visible en el pie de página.

## Licencia

El código del proyecto se distribuye bajo la licencia MIT. Los datos,
servicios, iconos y marcas de OpenWeather mantienen sus propias condiciones
de uso.
