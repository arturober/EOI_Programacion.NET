# Trivial paso a paso con .NET 10

Este repositorio contiene siete versiones independientes y acumulativas de una
aplicación de preguntas de trivial. El objetivo es explicar el proyecto en
clase sin presentar desde el primer día todos sus conceptos a la vez.

La primera versión únicamente consulta datos. Cada etapa posterior conserva lo
anterior y añade una responsabilidad concreta: primero el CRUD, después la
búsqueda, la API, el cliente y, finalmente, las mejoras de experiencia de
usuario.

## Contenido del repositorio

```text
Trivial-Paso-a-Paso
├── 01-Listados
├── 02-CRUD-Categorias
├── 03-CRUD-Preguntas
├── 04-Busqueda-Paginacion
├── 05-API-REST
├── 06-Cliente-Juego
├── 07-Version-Definitiva
├── Clientes
│   ├── Consola
│   ├── Godot
│   └── JavaScript
├── Pruebas
├── .gitignore
└── README.md
```

Cada etapa numerada es autosuficiente e incluye:

- Su propia solución `Trivial.sln`.
- El proyecto `TrivialApi`.
- Una copia de la base `Data/trivial.db`.
- Un README específico.
- Todo el código necesario para ejecutar esa etapa.

No es necesario ejecutar las versiones anteriores para utilizar una posterior.
Esto permite abrir cada etapa por separado, compararla con la anterior o
entregarla individualmente.

## Tecnologías utilizadas

- .NET 10.
- ASP.NET Core.
- Razor Pages.
- Entity Framework Core.
- SQLite.
- API REST con controladores.
- HTML.
- JavaScript.
- Fetch API.
- Bootstrap 5.
- Bootstrap Icons.
- Bootswatch.
- SweetAlert2.

Las bibliotecas de interfaz se descargan desde CDN. Los proyectos fijan
Entity Framework Core para SQLite en la versión `10.0.10` y
`SQLitePCLRaw.bundle_e_sqlite3` en la versión `2.1.12`. Esta última referencia
evita restaurar la versión `2.1.11`, afectada por una vulnerabilidad conocida.

## Requisitos

- .NET 10 SDK.
- Visual Studio 2026, Visual Studio Code o Rider.
- Un navegador moderno.
- Conexión a Internet para descargar los paquetes y las bibliotecas CDN la
  primera vez.

Puede comprobarse el SDK instalado con:

```bash
dotnet --version
```

## Ejecutar una versión

Entre en la carpeta del proyecto que quiera utilizar. Por ejemplo:

```bash
cd 01-Listados/TrivialApi
dotnet restore
dotnet run
```

La consola mostrará la dirección asignada al servidor. Abra esa dirección en
el navegador.

El mismo procedimiento funciona con las siete versiones.

## Base de datos

Todas las etapas incluyen una copia independiente de `Data/trivial.db`.

La base contiene:

- 10 categorías.
- 1.000 preguntas.
- 100 preguntas por categoría en los datos iniciales.

Cada versión modifica únicamente su propia copia. Por ejemplo, borrar una
pregunta en `03-CRUD-Preguntas` no afecta a `07-Version-Definitiva`.

La relación es uno a muchos:

```text
Categoria 1 ─────────── N Pregunta
```

Una categoría puede contener muchas preguntas y cada pregunta pertenece a una
sola categoría.

## Publicación en MonsterASP.NET

> **Método recomendado:** utiliza WebFTP.

Cada versión es una aplicación independiente. Debe publicarse solamente el
proyecto `TrivialApi` de la etapa elegida, nunca toda la carpeta
`Trivial-Paso-a-Paso`.

### Preparar la publicación desde VS Code

Entra en la carpeta `TrivialApi` de la versión que quieras desplegar. Por
ejemplo:

```bash
cd 07-Version-Definitiva/TrivialApi
dotnet restore
dotnet publish -c Release -o publicacion
```

En PowerShell:

```powershell
Compress-Archive -Path .\publicacion\* -DestinationPath .\publicacion.zip -Force
```

En MonsterASP.NET:

1. Abre **Files** y entra en `/wwwroot`.
2. Sube `publicacion.zip`.
3. Extrae su contenido dentro de `/wwwroot`.
4. Permite sobrescribir los archivos de la aplicación sin borrar todo el
   directorio.
5. Reinicia la aplicación o el AppPool.

En la raíz deben quedar `TrivialApi.dll`, `web.config`,
`appsettings.json`, `Data`, `wwwroot` y los demás archivos publicados. No
subas el código fuente, el `.csproj`, `bin` ni `obj`.

Consulta la
[guía de publicación mediante ZIP](https://help.monsterasp.net/books/deploy/page/how-to-deploy-website-content-from-zip-file).

### Primera publicación de la base de datos

El proyecto copia automáticamente `Data/trivial.db` a la carpeta de
publicación. El primer despliegue debe incluirla porque contiene las 10
categorías y las 1.000 preguntas.

Comprueba antes de crear el ZIP:

```text
publicacion/Data/trivial.db
```

### Actualizaciones sin perder datos

Desde la versión 2, la administración puede modificar la base del servidor. Una
nueva publicación vuelve a incluir la copia local de `trivial.db`. Si se
extrae sobre el sitio, puede sustituir los cambios realizados en línea.

Para conservar la base del servidor en una actualización:

1. Descarga una copia de seguridad de `/wwwroot/Data/trivial.db`.
2. Genera de nuevo `publicacion`.
3. Retira la copia local antes de comprimir:

```powershell
Remove-Item .\publicacion\Data\trivial.db
```

4. Crea y sube el ZIP normalmente.
5. Comprueba que las categorías y preguntas del servidor continúan.

No retires la base del primer despliegue: el proyecto espera que exista y no
inserta automáticamente las 1.000 preguntas.

### API, clientes y CORS

Desde la versión 5 pueden comprobarse direcciones públicas como:

```text
https://tu-sitio.runasp.net/api/categorias
https://tu-sitio.runasp.net/api/preguntas?cantidad=10
```

En las versiones 6 y 7, el cliente situado en `wwwroot/cliente` se sirve desde
la propia aplicación. Su archivo `trivial.js` ya utiliza la dirección relativa
`/api`, por lo que funciona tanto en local como al publicarlo sin modificar el
código.

CORS solo interviene cuando un cliente web de otro origen consulta la API. En
las versiones 5, 6 y 7 está habilitada la política `PermitirTodos`, necesaria
para el cliente independiente de `Clientes/JavaScript`. La configuración
abierta es útil para prácticas, pero en una aplicación real convendría permitir
únicamente los orígenes necesarios.

### Seguridad

El CRUD de categorías y preguntas no tiene autenticación. Cualquier visitante
podría modificar o borrar el banco de preguntas. Publica datos de práctica o
protege la administración antes de abrir el sitio a terceros.

Los clientes de consola, Godot y JavaScript, así como el proyecto de pruebas,
no se publican dentro de `/wwwroot`: consumen o prueban una API que se ejecuta
por separado. El cliente JavaScript debe servirse como sitio estático y
conectarse a la dirección HTTPS de una versión 5, 6 o 7. Para las prácticas se
recomienda la versión 7 porque es la etapa más completa.

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

## Estrategia de crecimiento

La estructura, los nombres y las entidades permanecen estables desde la
primera versión. Se ha evitado crear páginas provisionales que después hubiera
que borrar.

La evolución general es:

1. Consultar datos con Entity Framework.
2. Añadir escritura para categorías.
3. Aplicar el mismo patrón a preguntas.
4. Ampliar el listado con filtros y paginación.
5. Publicar los datos mediante una API.
6. Consumir la API desde un cliente JavaScript.
7. Mejorar la experiencia de usuario.

La mayor parte de los cambios consiste en añadir archivos. Cuando es necesario
modificar uno existente, se amplía su responsabilidad sin cambiar su nombre ni
su ubicación.

## Resumen de las versiones

| Versión | Funcionalidad nueva | Concepto principal |
|---|---|---|
| 1 | Listados de categorías y preguntas | Lectura con EF Core |
| 2 | CRUD de categorías | Formularios Razor |
| 3 | CRUD de preguntas | Relaciones y `SelectList` |
| 4 | Búsqueda y paginación | Consultas y estado en URL |
| 5 | API REST | Controladores y DTO |
| 6 | Cliente del juego | `fetch` y estado en JavaScript |
| 7 | Temas y SweetAlert | Experiencia de usuario |

## Versión 1: listados

Se presenta la estructura fundamental:

- `Program.cs`.
- Modelos.
- Contexto.
- Conexión.
- PageModels.
- Vistas Razor.
- Consultas asíncronas.

La aplicación es de solo lectura. Se muestran las categorías y las primeras 25
preguntas.

Flujo principal:

```text
Navegador
   ↓
Razor Page
   ↓
PageModel
   ↓
TrivialContext
   ↓
SQLite
```

## Versión 2: CRUD de categorías

Se conservan los listados y se añaden:

- Alta de categorías.
- Modificación.
- Eliminación.
- Validación.
- Mensajes con `TempData`.
- Comprobación de nombres repetidos.
- Borrado en cascada.

La entidad `Categoria` es pequeña y permite estudiar un CRUD antes de trabajar
con el formulario más largo de preguntas.

## Versión 3: CRUD de preguntas

Se repite el patrón de categorías y se incorporan:

- Enunciado.
- Cuatro respuestas.
- Número de respuesta correcta.
- Categoría seleccionada.
- Formularios parciales.
- `SelectList`.
- Validación de la clave ajena.

Se puede comprobar que el patrón de creación, edición y borrado es el
mismo, aunque la entidad tenga más propiedades.

## Versión 4: búsqueda y paginación

El listado de preguntas se amplía sin cambiar el CRUD:

- Búsqueda por enunciado.
- Normalización de tildes.
- Comparación sin distinguir mayúsculas.
- Filtro por categoría.
- 25 preguntas por página.
- Conservación de filtros en los enlaces.
- Búsqueda automática.
- Retraso de 300 ms.
- Recuperación del foco.

La dirección pasa a representar el estado del listado:

```text
/Preguntas?Busqueda=historia&CategoriaId=2&Pagina=3
```

## Versión 5: API REST

Se añaden controladores y DTO sin sustituir las Razor Pages.

La misma base queda accesible de dos formas:

```text
Razor Pages → HTML
Controladores → JSON
```

Endpoints:

```text
GET /api/categorias
GET /api/categorias/1
GET /api/preguntas
GET /api/preguntas/1
GET /api/preguntas?cantidad=10
GET /api/preguntas?categoriaId=2&cantidad=10
```

La API es deliberadamente de solo lectura. La administración continúa
realizándose desde Razor Pages.

## Versión 6: cliente del juego

Se añade `wwwroot/cliente` con:

- `index.html`.
- `trivial.js`.

El cliente nunca utiliza `TrivialContext`. Solo conoce direcciones HTTP y
objetos JSON.

```text
HTML y JavaScript
        ↓ fetch
Controladores de la API
        ↓
Entity Framework
        ↓
SQLite
```

Se incorporan selección de categoría, diez preguntas aleatorias, corrección,
marcador, progreso y resultado final.

## Versión 7: acabado definitivo

La última etapa añade:

- SweetAlert para mensajes y borrados.
- Selector de temas.
- Bootstrap claro y oscuro.
- Temas Bootswatch.
- Persistencia con `localStorage`.
- Tema compartido con el cliente.
- Bootstrap Icons.
- Botones compactos.
- Enlaces directos al JSON.
- Navegación mejorada.

`temas.js` se comparte entre Razor Pages y el cliente. El cliente no duplica la
lista de temas oscuros ni la construcción de direcciones Bootswatch.

## Clientes independientes

La carpeta `Clientes` demuestra que una misma API puede utilizarse sin acceder
a Entity Framework ni a SQLite:

- [Consola](Clientes/Consola/) utiliza C# y `HttpClient`.
- [Godot](Clientes/Godot/) ofrece una interfaz gráfica de escritorio con C#.
- [JavaScript](Clientes/JavaScript/) es el cliente web independiente completo,
  con dirección de servidor configurable, Bootstrap Icons, SweetAlert2 y
  selector de temas.

El nuevo cliente JavaScript no se integra en `TrivialApi/wwwroot` ni contiene
enlaces hacia las Razor Pages. Para las prácticas se recomienda conectarlo con
`07-Version-Definitiva`, aunque las versiones 5 y 6 también permiten peticiones
CORS desde otro origen.

## Archivos que permanecen estables

Desde la primera versión se conservan:

- `TrivialApi.csproj`.
- `appsettings.json`.
- `Data/trivial.db`.
- `Data/TrivialContext.cs`.
- `Models/Categoria.cs`.
- `Models/Pregunta.cs`.
- `_ViewImports.cshtml`.
- `_ViewStart.cshtml`.
- Las páginas `Index` de categorías y preguntas.

Los atributos de validación están presentes desde el comienzo. En la primera
etapa documentan las reglas del dominio; en las siguientes son utilizados por
los formularios.

## Archivos añadidos por etapa

### De la versión 1 a la 2

```text
Pages/Categorias/Crear.cshtml
Pages/Categorias/Crear.cshtml.cs
Pages/Categorias/Editar.cshtml
Pages/Categorias/Editar.cshtml.cs
Pages/Categorias/_Formulario.cshtml
```

### De la versión 2 a la 3

```text
Pages/Preguntas/Crear.cshtml
Pages/Preguntas/Crear.cshtml.cs
Pages/Preguntas/Editar.cshtml
Pages/Preguntas/Editar.cshtml.cs
Pages/Preguntas/_Formulario.cshtml
```

### De la versión 4 a la 5

```text
DTOs/CategoriaDto.cs
DTOs/PreguntaDto.cs
Controllers/CategoriasController.cs
Controllers/PreguntasController.cs
```

### De la versión 5 a la 6

```text
wwwroot/cliente/index.html
wwwroot/cliente/trivial.js
```

### De la versión 6 a la 7

```text
wwwroot/js/temas.js
wwwroot/js/sweetalert.js
```

## Propuesta de trabajo con Git

Puede subirse todo como un único repositorio. Una alternativa especialmente
didáctica consiste en crear una rama por versión:

```text
version-01-listados
version-02-crud-categorias
version-03-crud-preguntas
version-04-busqueda
version-05-api
version-06-cliente
version-07-definitiva
```

Otra posibilidad es mantener las siete carpetas en `main`, como se entregan,
y utilizar una rama de trabajo para los ejercicios.

## Comparar versiones

En Visual Studio Code pueden compararse archivos concretos mediante
“Select for Compare” y “Compare with Selected”.

Desde Git también puede observarse la evolución:

```bash
git diff version-01-listados version-02-crud-categorias
```

El propósito de la comparación no es memorizar el código, sino identificar:

- Qué archivo recibe una responsabilidad nueva.
- Qué datos necesita la vista.
- Qué método responde a cada petición.
- En qué momento se ejecuta realmente una consulta.

## Orden sugerido para las clases

1. Ejecutar la versión sin leer todavía el código.
2. Dibujar el recorrido de una petición.
3. Examinar el modelo correspondiente.
4. Examinar el PageModel o controlador.
5. Examinar la vista o cliente.
6. Modificar una funcionalidad pequeña.
7. Comparar con la versión siguiente.

## Convenciones de código

- Comentarios normales `//` y `@* *@`; no se utiliza documentación XML.
- Nombres descriptivos en español.
- Métodos asíncronos para acceso a datos.
- SQL generado por Entity Framework.
- Formularios validados en el servidor.
- JavaScript separado del HTML cuando contiene lógica.
- Sin CSS personalizado.
- DTO para la interfaz pública de la API.
- No se añaden repositorios o capas que no aporten valor didáctico.

## Reiniciar una versión

Como cada carpeta tiene su propia base, la forma más sencilla de reiniciar una
etapa es volver a copiar `Data/trivial.db` desde el ZIP original o desde una
versión que todavía no se haya modificado.

No deben copiarse bases desde `bin` o `obj`. El archivo fuente se encuentra en:

```text
TrivialApi/Data/trivial.db
```

## Siguiente lectura

Cada carpeta contiene un README más detallado con:

- Objetivos.
- Funcionalidad.
- Estructura.
- Recorrido del código.
- Archivos añadidos.
- Diferencias con la etapa anterior.
- Pruebas manuales.
- Ejercicios sugeridos.
