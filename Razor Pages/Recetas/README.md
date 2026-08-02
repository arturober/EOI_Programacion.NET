# Recetas

Aplicación didáctica desarrollada con **ASP.NET Core Razor Pages y .NET 10**.
Consulta recetas de [TheMealDB](https://www.themealdb.com/), permite crear una
cuenta local y guarda en SQLite los favoritos y el menú semanal de cada usuario.

El proyecto está pensado para aprender de forma progresiva: el código utiliza
nombres claros, está comentado en español y separa las páginas, el acceso a la
API y el acceso a la base de datos.

## Funcionalidades

- Portada con recetas aleatorias y selecciones por categorías.
- Búsqueda de recetas por nombre.
- Navegación por categorías y zonas gastronómicas.
- Ficha completa con ingredientes, cantidades, preparación, vídeo y fuente.
- Registro e inicio de sesión con ASP.NET Core Identity.
- Correo no confirmado obligatoriamente, apropiado para un proyecto de clase.
- Contraseñas almacenadas mediante los mecanismos seguros de Identity.
- Favoritos independientes para cada usuario.
- Menú semanal con una receta por día.
- Lista de la compra generada automáticamente desde el menú.
- Persistencia local con SQLite.
- Caché para reducir llamadas repetidas a la API.
- Selector de temas de Bootswatch guardado en el navegador.
- Diálogos de confirmación y avisos con SweetAlert2.
- Diseño adaptable a móvil, tableta y escritorio.
- Imagen local de sustitución si falla una fotografía externa.

## Tecnologías utilizadas

- .NET 10 y ASP.NET Core Razor Pages.
- Entity Framework Core 10.
- ASP.NET Core Identity.
- SQLite.
- `HttpClient`, `System.Text.Json` e `IMemoryCache`.
- Bootstrap 5.3, Bootswatch, Bootstrap Icons y SweetAlert2 mediante CDN.
- TheMealDB como fuente de datos de recetas.

## Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).
- Conexión a Internet para consultar TheMealDB y cargar las bibliotecas CDN.

## Puesta en marcha

Desde la carpeta que contiene `Recetas.csproj`, ejecuta:

```bash
dotnet restore
dotnet run
```

Abre después la dirección que aparezca en la terminal. Los perfiles incluidos
usan normalmente:

- `http://localhost:5300`
- `https://localhost:7300`

La base de datos se crea automáticamente en `Data/recetas.db` la primera vez
que se inicia la aplicación.

## Clave de TheMealDB

TheMealDB permite usar la clave de prueba `1` para desarrollo y uso educativo.
El proyecto ya la incluye en `appsettings.json`, por lo que puede ejecutarse sin
configurar ningún secreto.

Si tienes una clave de colaborador, es recomendable guardarla con User Secrets:

```bash
dotnet user-secrets set "TheMealDb:ApiKey" "TU_CLAVE"
dotnet run
```

Puedes comprobar las claves configuradas con:

```bash
dotnet user-secrets list
```

No escribas claves privadas en `appsettings.json` ni las subas a GitHub.

## Publicación en MonsterASP.NET

MonsterASP.NET permite alojar aplicaciones ASP.NET Core con .NET 10. Para
trabajar desde Visual Studio Code, publica el proyecto desde la terminal
integrada y sube el resultado mediante ZIP.

### Preparar la publicación desde VS Code

Desde la carpeta `Recetas`:

```bash
dotnet restore
dotnet publish -c Release -o publicacion
```

En PowerShell:

```powershell
Compress-Archive -Path .\publicacion\* -DestinationPath .\publicacion.zip -Force
```

Después:

1. Abre **Files** en MonsterASP.NET.
2. Entra en `/wwwroot`.
3. Sube y extrae `publicacion.zip`.
4. Permite reemplazar los archivos anteriores sin borrar todo `/wwwroot`.
5. Reinicia la aplicación o el AppPool.

`Recetas.dll`, `web.config`, `appsettings.json` y la carpeta `wwwroot`
deben quedar directamente dentro de `/wwwroot`. No subas el código fuente, el
`.csproj`, `bin` ni `obj`.

Consulta la
[guía de publicación mediante ZIP](https://help.monsterasp.net/books/deploy/page/how-to-deploy-website-content-from-zip-file).
WebDeploy puede utilizarse como
[alternativa desde Visual Studio](https://help.monsterasp.net/books/deploy/page/how-to-deploy-net-core-web-application-using-visual-studio).

### Clave de TheMealDB en producción

La aplicación funciona con la clave educativa `1` incluida en
`appsettings.json`, por lo que normalmente no es necesario añadir ninguna
variable de entorno.

Si dispones de una clave de supporter, no uses User Secrets en el servidor.
Abre:

```text
Websites → Manage website → Scripting → Environment Variables
```

Y añade:

```text
Nombre: TheMealDb__ApiKey
Valor:  TU_CLAVE_DE_SUPPORTER
```

`TheMealDb__ApiKey` representa `TheMealDb:ApiKey`. Guarda el cambio y
reinicia la aplicación o el AppPool. Consulta la
[documentación de variables de entorno](https://help.monsterasp.net/books/development/page/environment-variables-as-configuration-store).
No publiques `appsettings.Local.json`.

### Identity y SQLite

No hace falta configurar un proveedor de acceso externo ni un servicio de
correo. Las cuentas son locales y el correo no necesita confirmación. Identity
guarda usuarios, contraseñas protegidas, favoritos y menús en
`Data/recetas.db`.

La base se crea automáticamente en el primer arranque. Para conservar los datos:

- no elimines ni sobrescribas `Data/recetas.db` al volver a publicar;
- no actives la eliminación de archivos adicionales del destino;
- no reemplaces la base del servidor por una base de desarrollo vacía;
- realiza una copia de seguridad antes de modificar las entidades;
- recuerda que `EnsureCreatedAsync` no migra una base existente.

### Comprobar el despliegue

1. Abre una receta y comprueba sus ingredientes.
2. Registra una cuenta, cierra la sesión y vuelve a entrar.
3. Guarda una favorita y añade una receta al menú.
4. Reinicia la aplicación y comprueba que la cuenta y los datos permanecen.

Si aparece un error HTTP 500, revisa
`Control Panel → Websites → Manage → Logs`. También puedes habilitar
temporalmente los
[logs de ASP.NET Core](https://help.monsterasp.net/books/debugging/page/aspnet-core-debug-logging)
y desactivarlos después del diagnóstico.

## Registro e inicio de sesión

La autenticación utiliza **ASP.NET Core Identity**. Al registrarse, el usuario
indica su nombre, correo y contraseña. El correo se usa como nombre de acceso,
pero el proyecto no exige confirmarlo.

Identity se encarga de:

- validar la contraseña;
- generar un hash seguro, sin guardar la contraseña original;
- crear y comprobar la cookie de autenticación;
- bloquear las páginas privadas para visitantes;
- relacionar cada favorito y cada día del menú con el identificador del usuario.

Para un proyecto real publicado en Internet convendría añadir confirmación de
correo, recuperación de contraseña, límites de intentos y un proveedor de correo.

## Datos que se guardan en SQLite

Las recetas se consultan en TheMealDB, pero se conserva una copia mínima cuando
el usuario las utiliza en su colección. De ese modo las relaciones locales no
dependen únicamente de una petición posterior.

Las tablas principales son:

- tablas de Identity para usuarios, credenciales, roles y tokens;
- `Recetas`, con la información mínima de las recetas utilizadas;
- `Favoritos`, que relaciona usuarios y recetas;
- `MenuSemanal`, que relaciona un usuario, un día y una receta.

La combinación de usuario y receta es la clave de cada favorito, así que no
pueden existir duplicados. En el menú, la combinación de usuario y día hace que
solo pueda haber una receta asignada a cada día.

> El proyecto usa `EnsureCreated()` para que resulte sencillo en clase. Si se
> amplía el modelo, lo habitual en un proyecto profesional es sustituirlo por
> migraciones de Entity Framework Core.

## Cómo se consulta la API

`TheMealDbServicio` centraliza todas las peticiones HTTP. Las páginas no conocen
las direcciones de la API: solicitan al servicio buscar, filtrar o recuperar
una receta. Esto facilita leer, probar y modificar el proyecto.

Se emplean estos endpoints gratuitos:

| Acción | Endpoint |
| --- | --- |
| Buscar por nombre | `search.php?s=...` |
| Obtener el detalle | `lookup.php?i=...` |
| Receta aleatoria | `random.php` |
| Ver categorías | `categories.php` |
| Ver zonas | `list.php?a=list` |
| Filtrar por categoría | `filter.php?c=...` |
| Filtrar por zona | `filter.php?a=...` |

TheMealDB devuelve los ingredientes en propiedades numeradas
(`strIngredient1`, `strIngredient2`, etc.). El DTO recoge esas propiedades
variables mediante `JsonExtensionData` y el servicio las transforma en una
lista normal de ingredientes y cantidades.

## Estructura del proyecto

```text
Recetas/
├── Configuracion/       Opciones de TheMealDB
├── Data/                DbContext y archivo SQLite al ejecutar
├── DTOs/                Clases que representan el JSON externo
├── Modelos/             Entidades locales y modelos de presentación
├── Pages/
│   ├── Cuenta/          Registro, login, logout y acceso denegado
│   ├── Favoritos/       Colección privada del usuario
│   ├── Menu/            Menú semanal y lista de la compra
│   ├── Recetas/         Buscar, listar y mostrar detalles
│   └── Shared/          Diseño común y tarjeta reutilizable
├── Servicios/           Cliente de API y servicio de colección
├── wwwroot/             JavaScript e imagen de sustitución
├── Program.cs           Configuración de servicios y aplicación
└── appsettings.json     Configuración no secreta
```

## Ideas para ampliarlo

- Añadir paginación local a los listados.
- Guardar notas personales y una puntuación por usuario.
- Permitir varios menús o semanas con fechas concretas.
- Marcar ingredientes ya comprados.
- Exportar la lista de la compra a PDF.
- Añadir filtros por ingrediente.
- Incorporar roles y una zona de administración.
- Sustituir `EnsureCreated()` por migraciones.
- Crear pruebas unitarias para los servicios.

## Solución de problemas

### La aplicación no arranca por un error de SQLite

Cierra todas las instancias de la aplicación. Si has cambiado manualmente las
entidades durante las prácticas, elimina `Data/recetas.db` y vuelve a ejecutar.
Esto borra las cuentas y colecciones locales.

### No aparecen recetas

Comprueba la conexión a Internet y abre la página oficial de TheMealDB. También
puede existir un límite o una incidencia temporal en el servicio externo.

### Las fotografías no cargan

La aplicación mostrará una ilustración local automáticamente. Algunas imágenes
pertenecen al servicio externo y pueden dejar de estar disponibles.

### El contenido aparece en inglés

Los textos de la interfaz están en español, pero nombres e instrucciones
proceden directamente de TheMealDB y pueden estar disponibles solo en inglés.

### El navegador no conserva el tema

El tema se guarda en `localStorage`. Revisa que el navegador permita el
almacenamiento local y que no estés usando una sesión privada que lo elimine.

## Avisos

Los datos, imágenes y enlaces de recetas pertenecen a TheMealDB y a sus
respectivos autores. Revisa las condiciones del servicio antes de publicar una
aplicación comercial. Los recursos de Bootstrap, Bootswatch, Bootstrap Icons y
SweetAlert2 se cargan desde sus CDN y conservan sus propias licencias.

## Licencia

El código de este proyecto se distribuye bajo la licencia MIT. Consulta
`LICENSE`.
