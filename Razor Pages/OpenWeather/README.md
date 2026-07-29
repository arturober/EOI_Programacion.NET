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
que los secretos de usuario se cargan automáticamente durante el desarrollo.

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
