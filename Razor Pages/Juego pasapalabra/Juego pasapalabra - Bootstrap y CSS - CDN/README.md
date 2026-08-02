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

> **Método recomendado:** utiliza WebFTP. El archivo ZIP se conserva como
> alternativa.

Desde la terminal integrada de VS Code:

```bash
dotnet restore
dotnet publish -c Release -o publicacion
```

En PowerShell:

```powershell
Compress-Archive -Path .\publicacion\* -DestinationPath .\publicacion.zip -Force
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
