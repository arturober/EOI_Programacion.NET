# NASA Explorer

Aplicación docente construida con **ASP.NET Core Razor Pages y .NET 10** para
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
