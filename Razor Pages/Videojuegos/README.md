# Videojuegos · ASP.NET Core + RAWG

Aplicación educativa desarrollada con **ASP.NET Core Razor Pages** para buscar
videojuegos en [RAWG](https://rawg.io/), consultar sus fichas y mantener una
biblioteca privada.

El proyecto está pensado para alumnado que está aprendiendo C#: las
responsabilidades están separadas, los nombres y comentarios están en español
y el JavaScript se limita a los temas, SweetAlert2 y las imágenes alternativas.

## Funcionalidades

- Inicio con videojuegos populares, mejor valorados y próximos lanzamientos.
- Búsqueda por nombre.
- Listados paginados de novedades, acción, rol, estrategia, independientes,
  deportes y carreras.
- Ficha con imagen, descripción, fechas, géneros, plataformas, empresas,
  puntuaciones, clasificación de edad, tiendas y capturas.
- Registro e inicio de sesión con ASP.NET Core Identity.
- Registro inmediato, sin confirmación obligatoria del correo.
- Bloqueo temporal después de cinco intentos de acceso fallidos.
- Biblioteca privada guardada en SQLite para cada usuario.
- Estados personales: pendiente, jugando, completado y abandonado.
- Puntuación personal del 1 al 10 y comentario opcional.
- Filtros y ordenación de la biblioteca sin nuevas llamadas a RAWG.
- Copia local de los datos básicos de cada videojuego guardado.
- Selector de Bootstrap claro/oscuro y todos los temas Bootswatch.
- Confirmaciones y avisos con SweetAlert2.
- Sustitución automática de imágenes que no están disponibles.
- Caché en memoria para reducir el consumo de la cuota mensual.
- Atribución y enlaces activos a RAWG.

## Tecnologías

- .NET 10 y C#.
- ASP.NET Core Razor Pages.
- ASP.NET Core Identity.
- Entity Framework Core y SQLite.
- RAWG Video Games Database API.
- Bootstrap 5.3.8, Bootswatch 5.3.8, Bootstrap Icons y SweetAlert2 desde CDN.

## Requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download/dotnet/10.0).
- Una cuenta y una clave de [RAWG API](https://rawg.io/apidocs).
- Conexión a Internet para RAWG y los recursos CDN.

## Obtener y guardar la clave

RAWG exige una clave en todas las peticiones. Después de crearla, abre un
terminal dentro de la carpeta que contiene `Videojuegos.csproj`:

```powershell
dotnet user-secrets set "Rawg:ApiKey" "TU_CLAVE_DE_RAWG"
```

Comprueba que se ha guardado para este proyecto:

```powershell
dotnet user-secrets list
```

Debe aparecer una entrada similar a:

```text
Rawg:ApiKey = 123456789...
```

La clave queda fuera del proyecto y no se publica al subir el repositorio a
GitHub. La aplicación solo la utiliza en el servidor; no aparece en el HTML ni
en el JavaScript del navegador.

Como alternativa local, copia `appsettings.Local.example.json` con el nombre
`appsettings.Local.json` y sustituye su valor:

```json
{
  "Rawg": {
    "ApiKey": "TU_CLAVE_DE_RAWG"
  }
}
```

Ese archivo está excluido mediante `.gitignore`.

## Ejecutar la aplicación

Desde la carpeta `Videojuegos`:

```powershell
dotnet restore
dotnet run
```

La consola mostrará la dirección local. Con el perfil incluido normalmente
será:

```text
http://localhost:5200
```

En el primer arranque se crea automáticamente:

```text
Data/videojuegos.db
```

El archivo contiene las cuentas y bibliotecas locales y está excluido de Git.

## Publicación en MonsterASP.NET

MonsterASP.NET admite aplicaciones ASP.NET Core con .NET 10. Desde Visual
Studio Code se recomienda publicar con la terminal integrada y subir el
resultado en un ZIP.

### Preparar la publicación desde VS Code

Desde la carpeta `Videojuegos`:

```bash
dotnet restore
dotnet publish -c Release -o publicacion
```

En PowerShell:

```powershell
Compress-Archive -Path .\publicacion\* -DestinationPath .\publicacion.zip -Force
```

El ZIP debe contener directamente los archivos publicados, no otra carpeta
`publicacion`. Para desplegarlo:

1. Abre **Files** en MonsterASP.NET.
2. Entra en `/wwwroot`.
3. Sube y extrae `publicacion.zip`.
4. Permite sobrescribir los archivos anteriores sin borrar todo el directorio.
5. Reinicia la aplicación o el AppPool.

`Videojuegos.dll`, `web.config`, `appsettings.json` y la carpeta
`wwwroot` deben quedar directamente dentro del `/wwwroot` del alojamiento.
No subas el código fuente, el `.csproj`, `bin` ni `obj`.

Consulta la
[guía de publicación mediante ZIP](https://help.monsterasp.net/books/deploy/page/how-to-deploy-website-content-from-zip-file).
WebDeploy se mantiene como
[alternativa para Visual Studio](https://help.monsterasp.net/books/deploy/page/how-to-deploy-net-core-web-application-using-visual-studio).

### Configurar la clave de RAWG

Los valores de `dotnet user-secrets` no se transfieren al alojamiento. En
MonsterASP.NET abre:

```text
Websites → Manage website → Scripting → Environment Variables
```

Añade:

```text
Nombre: Rawg__ApiKey
Valor:  TU_CLAVE_DE_RAWG
```

Introduce únicamente la clave, sin comillas y sin añadir `key=`.
`Rawg__ApiKey` equivale a `Rawg:ApiKey`. Guarda el cambio y reinicia la
aplicación o el AppPool. Consulta la
[guía de variables de entorno](https://help.monsterasp.net/books/development/page/environment-variables-as-configuration-store).

No publiques `appsettings.Local.json` ni guardes la clave real en GitHub.

### Identity y conservación de SQLite

Las cuentas son locales y no requieren un proveedor externo de acceso ni un
servidor de correo. El correo no necesita confirmación. Identity guarda
usuarios, contraseñas protegidas y bibliotecas personales en
`Data/videojuegos.db`.

La base y sus tablas se crean en el primer arranque. En publicaciones
posteriores:

- conserva `Data/videojuegos.db`;
- no actives la eliminación de archivos adicionales del destino en WebDeploy;
- no reemplaces la base del servidor por una copia local vacía;
- realiza una copia de seguridad antes de actualizar;
- recuerda que `EnsureCreatedAsync` no migra una base existente.

### Comprobar el despliegue

1. Comprueba que la portada y los listados reciben datos de RAWG.
2. Registra una cuenta, cierra la sesión y vuelve a entrar.
3. Añade un juego a la biblioteca y cambia su estado.
4. Reinicia la aplicación y verifica que cuenta y biblioteca permanecen.

Un error `401` suele indicar una clave ausente o incorrecta; un `429`, que
se ha superado la cuota. Para errores HTTP 500 consulta
`Websites → Manage → Logs` o habilita temporalmente los
[logs de ASP.NET Core](https://help.monsterasp.net/books/debugging/page/aspnet-core-debug-logging);
desactívalos al terminar.

## Cómo funciona Identity

La clase `Usuario` hereda de `IdentityUser`. Cuando alguien se registra:

1. Identity valida el correo y la política de contraseña.
2. Genera un hash seguro de la contraseña; nunca guarda el texto original.
3. Inserta el usuario en las tablas `AspNetUsers` de SQLite.
4. Crea inmediatamente la cookie de autenticación.

La confirmación de correo está desactivada:

```csharp
opciones.SignIn.RequireConfirmedEmail = false;
```

Para una aplicación pública real convendría añadir confirmación, recuperación
de contraseña, un proveedor de correo y las medidas correspondientes contra
cuentas falsas.

## Base de datos y biblioteca personal

SQLite contiene:

- Las tablas creadas por Identity.
- `Videojuegos`, con una copia breve de cada elemento guardado.
- `Bibliotecas`, que relaciona un usuario con un videojuego.

La relación `VideojuegoUsuario` utiliza una clave compuesta formada por
`UsuarioId` y `VideojuegoId`. También contiene:

- Estado personal.
- Puntuación personal.
- Comentario.
- Fechas de creación y actualización.

Todas las operaciones filtran por el identificador obtenido desde la cookie de
Identity. Un usuario no puede consultar ni modificar la colección de otro.

## Configuración de RAWG

Los valores generales están en `appsettings.json`:

```json
{
  "Rawg": {
    "ApiKey": "",
    "MinutosCache": 30,
    "TamanoPagina": 20
  }
}
```

| Opción | Función |
|---|---|
| `ApiKey` | Clave privada de RAWG. Debe configurarse fuera de Git. |
| `MinutosCache` | Duración de la caché de búsquedas y listados. |
| `TamanoPagina` | Resultados por página, limitado entre 6 y 40. |

## Estructura

```text
Videojuegos/
├── Configuracion/       Opciones enlazadas con appsettings
├── Data/                Contexto de Entity Framework Core
├── DTOs/                Clases que reciben el JSON de RAWG
├── Modelos/             Entidades y modelos de interfaz
├── Pages/
│   ├── Biblioteca/      Colección privada y actualización del progreso
│   ├── Cuenta/          Registro, acceso y cierre de sesión
│   ├── Videojuegos/     Búsqueda, listados y ficha
│   └── Shared/          Layout, tarjeta y paginación
├── Servicios/           Cliente RAWG, biblioteca e Identity
├── wwwroot/             JavaScript e imagen alternativa
├── Program.cs           Configuración de servicios y middleware
└── appsettings.json     Configuración general
```

## Cliente de RAWG

`RawgServicio` concentra todo el acceso externo:

- Añade la clave a cada petición.
- Construye los filtros y la paginación.
- Convierte los DTO en modelos sencillos para las páginas.
- Guarda listados, búsquedas y fichas en `IMemoryCache`.
- Distingue claves inválidas, elementos inexistentes, límites y fallos de red.
- Evita que las Razor Pages dependan directamente de `HttpClient`.

La aplicación no expone una API propia con datos de RAWG, porque sus
condiciones no permiten redistribuir el catálogo como un servicio para
terceros.

## Condiciones de RAWG

En el momento de preparar este proyecto, el plan gratuito indica:

- Uso personal y para proyectos no comerciales.
- Hasta 20.000 peticiones mensuales.
- Obligación de enlazar a RAWG desde las páginas que muestran sus datos o
  imágenes.
- Prohibición de revender o redistribuir sus datos como otra API.

Las condiciones pueden cambiar. Consulta siempre la
[documentación oficial de RAWG](https://rawg.io/apidocs) antes de publicar el
proyecto.

## Solución de problemas

### La aplicación dice que falta la clave

Comprueba que el terminal está situado en la carpeta que contiene
`Videojuegos.csproj`:

```powershell
dir Videojuegos.csproj
dotnet user-secrets list
dotnet run
```

La clave debe llamarse exactamente `Rawg:ApiKey`, respetando los dos puntos.
No utilices `RAWG_API_KEY` ni `Rawg__ApiKey` con `user-secrets`.

También debes ejecutar la aplicación con el perfil de desarrollo incluido. Si
usas `dotnet run --no-launch-profile`, establece primero:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet run
```

### La clave aparece, pero RAWG la rechaza

Comprueba que no se han incluido comillas dentro del valor:

```powershell
dotnet user-secrets remove "Rawg:ApiKey"
dotnet user-secrets set "Rawg:ApiKey" "CLAVE_CORRECTA"
```

Detén completamente `dotnet run` y arráncalo de nuevo.

### Algunas imágenes no aparecen

Los recursos externos pueden eliminarse o cambiar. `imagenes.js` sustituye
automáticamente las imágenes fallidas por una ilustración local.

### El primer acceso tarda más

La primera visita consulta RAWG. Las siguientes visitas equivalentes utilizan
la caché en memoria hasta que vence su duración.

### Se ha cambiado el modelo de SQLite

Este proyecto didáctico usa `EnsureCreatedAsync` para reducir pasos. Durante el
desarrollo puedes detener la aplicación, borrar `Data/videojuegos.db` y volver
a ejecutar. En una aplicación real deberían utilizarse migraciones.

### Los temas o SweetAlert no cargan

Los recursos llegan desde jsDelivr. Comprueba la conexión y que ninguna
extensión del navegador esté bloqueando el CDN.

## Aviso

Los datos e imágenes pertenecen a RAWG y a sus correspondientes titulares. El
proyecto no está afiliado oficialmente con RAWG y debe utilizarse respetando
sus términos.

## Licencia

El código propio se distribuye con licencia MIT. Consulta `LICENSE`.
