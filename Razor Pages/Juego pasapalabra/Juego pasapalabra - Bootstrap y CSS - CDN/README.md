# Pasapalabra web con C#, Razor Pages y SQLite

Versión web del juego de Pasapalabra. El proyecto prioriza un código claro para estudiantes que están aprendiendo C#.

## Qué incluye

- Rosco gráfico de 27 letras, incluida la Ñ.
- Diseño responsive con Bootstrap para ordenador, tableta y móvil.
- Barra de navegación fija con menú hamburguesa.
- Estados visuales para respuestas pendientes, correctas e incorrectas.
- Selección de un tema o mezcla de todos los temas.
- Una pregunta aleatoria por letra.
- CRUD completo de preguntas y temas.
- Búsqueda automática mientras se escribe, sin pulsar ningún botón.
- Búsqueda que ignora mayúsculas y tildes.
- Ordenación alfabética según la cultura española: Á con A y Ñ después de N.
- Confirmaciones y mensajes con SweetAlert2.
- SQLite con consultas parametrizadas mediante `AddWithValue`.
- Mapeo manual entre las tablas y los objetos `Tema` y `Pregunta`.
- Datos de ejemplo que se insertan automáticamente si la base de datos está vacía.

## Librerías del lado del cliente

Las bibliotecas se cargan desde jsDelivr:

- Bootstrap 5.3.8.
- Bootstrap Icons 1.13.1.
- SweetAlert2 11.26.25.

No es necesario utilizar LibMan ni guardar esas bibliotecas en
`wwwroot/lib`. El navegador necesita conexión a Internet. El CSS propio de la
aplicación sí permanece en `wwwroot/css/site.css`.

## Crear y ejecutar el proyecto desde Visual Studio Code

Necesitas tener instalado el SDK de .NET 10.

1. Abre en Visual Studio Code la carpeta `Juego pasapalabra`.
2. Abre la terminal integrada.
3. Restaura los paquetes NuGet:

   ```bash
   dotnet restore
   ```

4. Ejecuta la aplicación:

   ```bash
   dotnet run
   ```

5. Abre en el navegador la dirección que aparezca en la terminal.

Para detener el servidor, pulsa `Ctrl + C`.

## Estructura principal

```text
Juego pasapalabra/
├── Models/
│   ├── Partida.cs
│   ├── Pregunta.cs
│   └── Tema.cs
├── Pages/
│   ├── Jugar/
│   ├── Preguntas/
│   ├── Temas/
│   └── Shared/_Layout.cshtml
├── wwwroot/
│   ├── css/site.css
│   └── lib/
├── BaseDatos.cs
├── TextoUtil.cs
├── libman.json
└── Program.cs
```

## Cómo funciona una Razor Page

Cada página suele tener dos ficheros:

- `.cshtml`: contiene el HTML que ve el usuario.
- `.cshtml.cs`: contiene el código C# que responde a sus acciones.

Por ejemplo, el botón Pasapalabra ejecuta el método `OnPostPasapalabra`. La
partida se convierte a JSON y se guarda en la sesión para conservar su estado
entre peticiones.

## Base de datos y mapeo

No se utiliza Entity Framework para que los alumnos pueda estudiar las
consultas SQL, sus parámetros y el mapeo de cada fila a un objeto.

`BaseDatos.cs` solo abre la conexión. Los modelos crean sus tablas y contienen
las operaciones relacionadas con sus datos. Los valores escritos por el
usuario nunca se concatenan en el SQL: se envían mediante parámetros con
`AddWithValue`, lo que ayuda a evitar SQL Injection.

La conexión activa las claves foráneas de SQLite. Por eso no se puede borrar un
tema que todavía contiene preguntas.

## Ordenación y comparación de textos

SQLite no ordena de forma natural todos los caracteres españoles. Los datos se
leen primero y se ordenan en C# mediante `StringComparer` con la cultura
`es-ES`. Así, las vocales con tilde ocupan su posición alfabética y la Ñ se
coloca después de la N.

`TextoUtil.NormalizarParaComparar` elimina las tildes de las vocales para las
búsquedas y las respuestas, pero conserva la Ñ porque es una letra diferente.

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

Desde la terminal integrada de VS Code:

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

Sube y extrae `publicacion.zip` en `/wwwroot`. Esta variante carga Bootstrap,
Bootstrap Icons y SweetAlert2 desde jsDelivr, por lo que el navegador necesita
conexión a Internet. La publicación sí debe incluir
`wwwroot/css/site.css`.

La base `pasapalabra.db` se crea con tablas y preguntas iniciales si no existe.
Para conservar las preguntas y temas añadidos, no sobrescribas la base en las
actualizaciones y descarga antes una copia de seguridad.

Las partidas activas se almacenan en sesión y se pierden al reiniciar. El CRUD
no tiene autenticación y cualquier visitante podría modificarlo. Utiliza datos
de práctica o protege la administración antes de exponer el sitio.

Después de publicar, comprueba que cargan los recursos de jsDelivr, juega una
partida y verifica la persistencia de una pregunta tras reiniciar.

Consulta la
[guía de publicación mediante ZIP](https://help.monsterasp.net/books/deploy/page/how-to-deploy-website-content-from-zip-file).


## ¿Qué puedo aprender?

- Clases, objetos, constructores y propiedades.
- Listas y expresiones lambda sencillas.
- Separación entre presentación y lógica.
- Razor Pages, formularios y métodos `OnGet` y `OnPost`.
- Sesiones web y serialización JSON.
- SQLite y ADO.NET.
- CRUD con `INSERT`, `SELECT`, `UPDATE` y `DELETE`.
- Consultas SQL parametrizadas.
- Diseño responsive con Bootstrap.
- Uso de bibliotecas web mediante CDN.

> La administración no tiene autenticación porque el objetivo es mantener el
> ejemplo fácil de entender. Antes de publicarlo en Internet habría que proteger
> las páginas del CRUD.
