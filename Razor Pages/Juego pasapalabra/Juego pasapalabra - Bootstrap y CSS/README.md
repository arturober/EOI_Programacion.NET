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

Las librerías se descargan con LibMan y se guardan dentro de `wwwroot/lib`:

- Bootstrap 5.3.8.
- Bootstrap Icons 1.13.1.
- SweetAlert2 11.26.25.

El archivo `libman.json` contiene la configuración. Para restaurar las
librerías desde la terminal se puede usar:

```bash
dotnet tool install -g Microsoft.Web.LibraryManager.Cli
libman restore
```

Las librerías también están incluidas en este proyecto, por lo que se puede
ejecutar directamente sin restaurarlas.

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

No se utiliza Entity Framework para que el alumnado pueda estudiar las
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

Desde la terminal integrada de VS Code:

```bash
dotnet restore
libman restore
dotnet publish -c Release -o publicacion
```

En PowerShell:

```powershell
Compress-Archive -Path .\publicacion\* -DestinationPath .\publicacion.zip -Force
```

Sube y extrae `publicacion.zip` en `/wwwroot`. Esta variante necesita que la
publicación incluya `wwwroot/lib` y `wwwroot/css/site.css`. No subas el
código fuente, el `.csproj`, `bin` ni `obj`.

`pasapalabra.db` se guarda en la raíz. Si falta, el primer arranque crea las
tablas y carga las preguntas iniciales. No sobrescribas esa base en una
actualización si quieres conservar los cambios realizados en línea y descarga
antes una copia de seguridad.

Las partidas se guardan en sesión y desaparecen cuando se reinicia el proceso.
El CRUD de preguntas y temas tampoco tiene autenticación. Cualquier visitante
podría alterar su contenido, por lo que debe utilizarse con datos de práctica o
protegerse antes de una publicación abierta.

Después de desplegar, comprueba una partida, crea una pregunta, reinicia la
aplicación y verifica que la pregunta permanece.

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
- Uso de LibMan para gestionar librerías web.

> La administración no tiene autenticación porque el objetivo es mantener el
> ejemplo fácil de entender. Antes de publicarlo en Internet habría que proteger
> las páginas del CRUD.
