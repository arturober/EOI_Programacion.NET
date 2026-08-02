# Open Food Facts

Aplicación didáctica desarrollada con **ASP.NET Core Razor Pages y .NET 10**.
Consulta productos alimentarios de
[Open Food Facts](https://world.openfoodfacts.org/), permite crear una cuenta
local y guarda en SQLite los productos favoritos de cada usuario.

El proyecto está preparado para alumnos que está aprendiendo
C#: emplea nombres claros, comentarios normales en español y separa las
páginas, el cliente de la API y la persistencia local.

## Funcionalidades

- Portada con productos populares en España.
- Búsqueda por nombre o marca mediante un formulario.
- Acceso directo al escribir un código de barras EAN o UPC.
- Navegación por categorías populares.
- Filtros por las notas A, B, C, D y E de Nutri-Score.
- Paginación de resultados.
- Ficha con imagen, marca, cantidad, ingredientes y alérgenos.
- Tabla nutricional por 100 g o 100 ml.
- Visualización de Nutri-Score, grupo NOVA y Green-Score.
- Información sobre trazas, aditivos, envase, etiquetas y países.
- Registro e inicio de sesión con ASP.NET Core Identity.
- Registro sin confirmación obligatoria de correo.
- Favoritos independientes para cada usuario.
- Comparación de entre dos y cuatro productos favoritos.
- Persistencia local con SQLite.
- Caché para reducir las llamadas repetidas a Open Food Facts.
- Tratamiento específico de límites de peticiones y errores temporales.
- Selector de temas Bootstrap y Bootswatch guardado en el navegador.
- Confirmaciones y avisos mediante SweetAlert2.
- Diseño adaptable a móvil, tableta y escritorio.
- Ilustración local cuando una fotografía externa no está disponible.

## Tecnologías utilizadas

- .NET 10 y ASP.NET Core Razor Pages.
- Entity Framework Core 10.
- ASP.NET Core Identity.
- SQLite.
- `HttpClient`, `System.Text.Json` e `IMemoryCache`.
- Bootstrap 5.3, Bootswatch, Bootstrap Icons y SweetAlert2 mediante CDN.
- Open Food Facts como fuente externa de productos.

## Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).
- Conexión a Internet para consultar la API y cargar las bibliotecas CDN.

## Puesta en marcha

Desde la carpeta que contiene `OpenFoodFacts.csproj`, ejecuta:

```bash
dotnet restore
dotnet run
```

Abre después la dirección que aparezca en la terminal. Los perfiles incluidos
utilizan normalmente:

- `http://localhost:5400`
- `https://localhost:7400`

La base de datos se crea automáticamente en `Data/openfoodfacts.db` la primera
vez que se inicia la aplicación.

## ¿Necesita una clave de API?

No. Las operaciones de lectura de Open Food Facts no necesitan una clave ni
credenciales. El servicio exige, en cambio, que cada aplicación se identifique
con un `User-Agent` que incluya un contacto.

El ejemplo utiliza `contacto@example.com`. Antes de publicar el proyecto debes
cambiarlo por un correo de contacto real. Puedes hacerlo con User Secrets:

```bash
dotnet user-secrets set "OpenFoodFacts:Contacto" "tu-correo@ejemplo.com"
dotnet run
```

También puedes copiar `appsettings.Local.example.json` como
`appsettings.Local.json` y cambiar allí el contacto. Este último archivo está
excluido de Git.

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

MonsterASP.NET es compatible con aplicaciones ASP.NET Core creadas con .NET 10.
Desde Visual Studio Code, el método recomendado consiste en publicar desde la
terminal y subir el resultado mediante WebFTP.

### Preparar la publicación desde VS Code

Abre la terminal integrada en la carpeta `OpenFoodFacts`:

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

El ZIP contiene los archivos publicados sin una carpeta `publicacion`
adicional. Para subirlo:

1. Abre **Files** en MonsterASP.NET.
2. Entra en `/wwwroot`.
3. Sube y extrae `publicacion.zip`.
4. Permite sustituir los archivos anteriores, sin borrar todo `/wwwroot`.
5. Reinicia la aplicación o el AppPool.

Dentro de `/wwwroot` deben quedar directamente `OpenFoodFacts.dll`,
`web.config`, `appsettings.json` y la carpeta `wwwroot`. No subas el código
fuente, el `.csproj`, `bin` ni `obj`.

Consulta la
[guía oficial de publicación mediante ZIP](https://help.monsterasp.net/books/deploy/page/how-to-deploy-website-content-from-zip-file).
WebDeploy se mantiene como
[alternativa para Visual Studio](https://help.monsterasp.net/books/deploy/page/how-to-deploy-net-core-web-application-using-visual-studio).

### Configurar el contacto de producción

Open Food Facts no necesita una clave de API, pero pide que la aplicación se
identifique mediante un contacto real. User Secrets no se envía al servidor.
En el panel abre:

```text
Websites → Manage website → Scripting → Environment Variables
```

Añade:

```text
Nombre: OpenFoodFacts__Contacto
Valor:  tu-correo@dominio.es
```

No mantengas `contacto@example.com` en una publicación pública. Los dos
guiones bajos equivalen a `OpenFoodFacts:Contacto`. Guarda el cambio y
reinicia la aplicación o el AppPool. Consulta la
[guía de variables de entorno](https://help.monsterasp.net/books/development/page/environment-variables-as-configuration-store).

No publiques `appsettings.Local.json`.

### Identity y SQLite

No hay que configurar un proveedor externo de acceso ni un servidor de correo.
Las cuentas son locales, el correo no necesita confirmación e Identity guarda
usuarios, contraseñas protegidas y favoritos en
`Data/openfoodfacts.db`.

La base se crea automáticamente en el primer arranque. Para conservarla:

- no elimines ni sobrescribas `Data/openfoodfacts.db` al republicar;
- no actives la eliminación de archivos adicionales del destino;
- no subas una base local salvo que quieras reemplazar la del servidor;
- realiza una copia de seguridad antes de modificar las entidades;
- recuerda que `EnsureCreatedAsync` no migra una base existente.

### Comprobación posterior

1. Busca un producto y comprueba que no se usa el contacto de ejemplo.
2. Registra una cuenta, cierra la sesión y vuelve a entrar.
3. Añade dos productos favoritos y prueba la comparación.
4. Reinicia la aplicación y comprueba que la cuenta y los favoritos continúan.

Ante un error HTTP 500 consulta `Websites → Manage → Logs`. También puedes
habilitar temporalmente los
[logs de ASP.NET Core](https://help.monsterasp.net/books/debugging/page/aspnet-core-debug-logging)
y desactivarlos al terminar.


## Registro e inicio de sesión

La autenticación local utiliza **ASP.NET Core Identity**. El usuario indica su
nombre, correo y contraseña. El correo sirve como nombre de acceso, pero el
proyecto no obliga a confirmarlo.

Identity se ocupa de:

- validar la contraseña;
- almacenar un hash seguro y no la contraseña original;
- crear y comprobar la cookie de autenticación;
- aplicar un bloqueo temporal tras varios intentos fallidos;
- restringir las páginas de favoritos y comparación;
- proporcionar un identificador estable para relacionar los datos del usuario.

Las cuentas de esta aplicación no son cuentas de Open Food Facts. El proyecto
solo realiza consultas de lectura y no modifica la base de datos externa.

Para una aplicación real convendría añadir confirmación de correo, recuperación
de contraseña, protección adicional frente a abusos y un proveedor de correo.

## Datos guardados en SQLite

Al marcar un favorito se conserva una copia de los campos necesarios para
mostrarlo y compararlo sin realizar otra petición a la API.

Las tablas principales son:

- tablas de Identity para usuarios, credenciales, roles y tokens;
- `Productos`, con la copia de los datos nutricionales principales;
- `Favoritos`, que relaciona usuarios y productos.

La clave de un producto es su código de barras. La combinación de usuario y
código es la clave de cada favorito, por lo que no pueden existir duplicados.

> El proyecto utiliza `EnsureCreated()` para que sea sencillo en clase. En un
> proyecto que vaya a evolucionar es preferible utilizar migraciones de Entity
> Framework Core.

## Cómo se consulta Open Food Facts

`OpenFoodFactsServicio` centraliza todas las peticiones HTTP. Las Razor Pages
no construyen direcciones ni deserializan JSON directamente.

Se utilizan tres tipos de consulta:

| Acción | Endpoint |
| --- | --- |
| Producto por código | `/api/v3.6/product/{codigo}.json` |
| Categoría o Nutri-Score | `/api/v2/search` |
| Búsqueda de texto completo | `/cgi/search.pl` |

La consulta individual utiliza la API actual v3.6. Open Food Facts todavía
ofrece la búsqueda estructurada en v2 y la búsqueda textual en el endpoint
histórico. Los campos solicitados se limitan expresamente para reducir el
tamaño de las respuestas.

## Límites de uso y caché

Open Food Facts limita actualmente:

- las consultas de productos a 15 por minuto y dirección IP;
- las búsquedas a 10 por minuto y dirección IP.

Por este motivo:

- no se realiza búsqueda automática mientras se escribe;
- el usuario debe enviar el formulario;
- las respuestas se guardan temporalmente con `IMemoryCache`;
- la portada realiza una sola búsqueda;
- los códigos HTTP 429 y 503 muestran mensajes comprensibles.

Se deben tener en cuenta especialmente estas consideraciones si el proyecto va a ser utilizado por
muchas personas desde la misma conexión.

## Estructura del proyecto

```text
OpenFoodFacts/
├── Configuracion/       Opciones del cliente externo
├── Data/                DbContext y archivo SQLite al ejecutar
├── DTOs/                Clases que representan el JSON de la API
├── Modelos/             Entidades locales y modelos de presentación
├── Pages/
│   ├── Comparar/        Comparador de favoritos
│   ├── Cuenta/          Registro, login, logout y acceso denegado
│   ├── Favoritos/       Colección privada del usuario
│   ├── Productos/       Buscar, filtrar y mostrar detalles
│   └── Shared/          Diseño, tarjeta y paginador reutilizables
├── Servicios/           Cliente de API y servicio de colección
├── wwwroot/             JavaScript e imagen de sustitución
├── Program.cs           Configuración de servicios y aplicación
└── appsettings.json     Configuración no secreta
```

## Aspectos didácticos interesantes

- Diferencia entre DTO, entidad de base de datos y modelo de presentación.
- Cliente HTTP tipado mediante inyección de dependencias.
- Deserialización de nombres JSON distintos a los nombres C#.
- Conversión de datos opcionales y respuestas incompletas.
- Uso de códigos de barras como claves de texto para conservar ceros iniciales.
- Relaciones de muchos a muchos con una entidad intermedia.
- Autorización de Razor Pages mediante `[Authorize]`.
- Protección contra redirecciones externas con `Url.IsLocalUrl`.
- Caché con distintas duraciones para listados y productos.
- Paginación sin JavaScript.
- Componentes Razor reutilizables mediante vistas parciales.

## Ideas para ampliarlo

- Leer códigos mediante la cámara del móvil.
- Añadir una lista de la compra o despensa por usuario.
- Guardar notas personales y una puntuación.
- Filtrar por alérgenos, marcas, países o etiquetas.
- Mostrar gráficos nutricionales con una biblioteca de gráficos.
- Exportar la comparación a PDF.
- Incorporar roles y una zona de administración.
- Sustituir `EnsureCreated()` por migraciones.
- Crear pruebas unitarias para los servicios y las conversiones.

## Solución de problemas

### Aparece un aviso sobre demasiadas consultas

La dirección IP ha alcanzado temporalmente un límite de Open Food Facts.
Espera al menos un minuto. No recargues repetidamente la página.

### No se encuentra un producto

Comprueba los números del código de barras. Open Food Facts es colaborativo y
puede que el producto todavía no exista en su base de datos.

### Faltan nutrientes, puntuaciones o ingredientes

No todos los productos tienen todos sus campos completos. La aplicación muestra
`Sin datos`, `—` o `?` cuando la API no proporciona un valor.

### Las imágenes no cargan

Las fotografías pertenecen al servicio externo y pueden faltar. La aplicación
mostrará una ilustración local automáticamente.

### He cambiado las entidades y SQLite muestra errores

Cierra la aplicación y elimina `Data/openfoodfacts.db`. Al volver a ejecutar se
creará de nuevo, pero se perderán las cuentas y favoritos locales.

## Licencias y responsabilidad

La base de datos de Open Food Facts se distribuye bajo Open Database License;
los contenidos individuales y las imágenes tienen sus propias condiciones,
incluida atribución y compartir igual para las fotografías. Consulta siempre
las condiciones oficiales antes de publicar una reutilización.

Los datos son colaborativos y no existe garantía de que sean completos o
correctos. Esta aplicación no sustituye el envase ni ofrece consejo médico,
nutricional o sobre alergias.

El código de este proyecto se distribuye bajo licencia MIT. Consulta `LICENSE`.
