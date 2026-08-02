# Aplicaciones con ASP.NET Core Razor Pages

Esta carpeta reúne una colección de aplicaciones educativas desarrolladas
principalmente con **C#**, **.NET 10** y **ASP.NET Core Razor Pages**. Los
proyectos están pensados para aprender de forma práctica: empiezan con páginas
y operaciones CRUD sencillas y avanzan hacia consumo de APIs, autenticación,
Entity Framework Core, servicios JSON, pruebas de integración, SignalR y
clientes externos.

El código prioriza la legibilidad:

- nombres descriptivos, generalmente en español;
- comentarios normales `//` y `@* *@`, sin documentación XML innecesaria;
- separación entre páginas, modelos, servicios y acceso a datos;
- formularios validados en el servidor;
- consultas asíncronas cuando intervienen Entity Framework o servicios HTTP;
- interfaces adaptables construidas principalmente con Bootstrap;
- JavaScript limitado a los casos donde realmente aporta valor;
- proyectos independientes que se pueden ejecutar y estudiar por separado.

En total hay **15 familias de aplicaciones** y **32 proyectos `.csproj`**. La
mayoría son aplicaciones web con destino `net10.0`. La única excepción es el
cliente de Godot del trivial, que utiliza `net8.0` por compatibilidad con Godot
4.7.1 y se comunica con la API mediante HTTP.

> Cada subcarpeta es independiente. No debe intentarse ejecutar toda la carpeta
> `Razor Pages` como si fuese una única aplicación.

## Índice general

| Aplicación | Finalidad principal | Datos | Servicio externo | Nivel orientativo |
|---|---|---|---|---|
| [Agenda de teléfonos](<Agenda de teléfonos/>) | CRUD de contactos con fotografía | SQLite y ADO.NET | Ninguno | Inicial |
| [Biblioteca](Biblioteca/) | Buscador y colección privada de libros | EF Core, Identity y SQLite | Open Library | Avanzado |
| [Fútbol](Futbol/) | Competiciones, partidos, equipos y clasificación | EF Core, Identity y SQLite | football-data.org | Avanzado |
| [Juego Pasapalabra](<Juego pasapalabra/>) | Juego, sesión web y administración de preguntas | SQLite y ADO.NET | Ninguno | Intermedio |
| [Lista de tareas](<Lista de tareas/>) | CRUD relacionado de tareas y categorías | SQLite y ADO.NET | Ninguno | Inicial-intermedio |
| [Mazmorra Online](<Mazmorra Online/>) | Juego multijugador en tiempo real | Memoria del servidor | SignalR propio | Avanzado |
| [NASA Explorer](NasaExplorer/) | Exploración de varias fuentes oficiales de NASA | EF Core, Identity y SQLite | APIs de NASA | Avanzado |
| [Open Food Facts](OpenFoodFacts/) | Búsqueda y comparación de alimentos | EF Core, Identity y SQLite | Open Food Facts | Avanzado |
| [Open Weather](OpenWeather/) | Tiempo, previsión y calidad del aire | Caché en memoria | OpenWeather | Intermedio |
| [Películas](Peliculas/) | Catálogo de películas y favoritas | EF Core, Identity y SQLite | TMDB | Avanzado |
| [Pokémon](Pokemon/) | Itinerario progresivo de consumo de PokeAPI | Caché en la versión final | PokeAPI | Inicial-avanzado |
| [Recetas](Recetas/) | Recetas, favoritos, menú y lista de compra | EF Core, Identity y SQLite | TheMealDB | Avanzado |
| [Rick and Morty](RickAndMorty/) | Personajes, episodios, lugares y favoritos | EF Core, Identity y SQLite | Rick and Morty API | Intermedio-avanzado |
| [Trivial paso a paso](Trivial-Paso-a-Paso/) | Razor Pages, CRUD, API, clientes y pruebas | EF Core y SQLite | API propia | Inicial-avanzado |
| [Videojuegos](Videojuegos/) | Catálogo RAWG y biblioteca personal | EF Core, Identity y SQLite | RAWG | Avanzado |

## Itinerario didáctico recomendado

No existe un único orden obligatorio, pero la siguiente secuencia introduce
los conceptos de forma gradual.

### 1. Primer contacto con Razor Pages

1. [Pokémon: listado básico](<Pokemon/Pokemon - Listado basico/>) para estudiar
   una página, un `PageModel`, `HttpClient` y un `foreach`.
2. [Pokémon: listado con imágenes](<Pokemon/Pokemon - Listado con imagenes/>)
   para añadir tarjetas, imágenes diferidas y una cuadrícula responsive.
3. [Pokémon: detalles](<Pokemon/Pokemon - Detalles/>) para trabajar con rutas,
   parámetros y Tag Helpers.

### 2. Formularios, SQLite y CRUD

1. [Agenda de teléfonos](<Agenda de teléfonos/>) para comenzar con un CRUD
   pequeño y consultas SQL parametrizadas.
2. [Lista de tareas](<Lista de tareas/>) para añadir una relación entre tareas y
   categorías.
3. [Juego Pasapalabra](<Juego pasapalabra/>) para incorporar búsquedas,
   normalización de texto, sesión y estado de una partida.
4. [Trivial, versiones 1 a 4](Trivial-Paso-a-Paso/) para repetir los mismos
   conceptos con Entity Framework Core, relaciones y paginación.

### 3. Consumo de APIs públicas

1. [Pokémon: versión final](<Pokemon/Pokemon - Version final/>) para estudiar un
   cliente HTTP completo sin autenticación ni base de datos.
2. [Open Weather](OpenWeather/) para introducir claves, configuración segura,
   geocodificación, caché y una API JSON propia.
3. [Rick and Morty](RickAndMorty/) para navegar entre recursos relacionados y
   añadir usuarios y favoritos.

### 4. Aplicaciones completas con Identity

Se puede continuar con [Recetas](Recetas/), [Open Food Facts](OpenFoodFacts/),
[Biblioteca](Biblioteca/), [Películas](Peliculas/), [Fútbol](Futbol/),
[Videojuegos](Videojuegos/) y [NASA Explorer](NasaExplorer/). Todas combinan
servicios externos, cuentas locales, datos privados por usuario, SQLite y una
interfaz responsive.

### 5. APIs, clientes, pruebas y tiempo real

1. [Trivial, versiones 5 a 7](Trivial-Paso-a-Paso/) para crear una API REST y
   consumirla desde JavaScript.
2. [Cliente JavaScript independiente](Trivial-Paso-a-Paso/Clientes/JavaScript/)
   para separar por completo el sitio estático y la API, configurar el servidor
   y estudiar CORS.
3. [Cliente de consola](Trivial-Paso-a-Paso/Clientes/Consola/) para consumir la
   misma API desde C#.
4. [Cliente Godot](Trivial-Paso-a-Paso/Clientes/Godot/) para comprobar que una
   API no depende de la tecnología del cliente.
5. [Pruebas de integración](Trivial-Paso-a-Paso/Pruebas/) con xUnit,
   `WebApplicationFactory` y SQLite en memoria.
6. [Mazmorra Online](<Mazmorra Online/>) para estudiar SignalR, bucles de juego,
   sincronización, reconexión e interfaces táctiles.

## Requisitos generales

- [.NET SDK 10](https://dotnet.microsoft.com/download/dotnet/10.0).
- Visual Studio, Visual Studio Code o Rider.
- Un navegador moderno.
- Conexión a Internet para los proyectos que consumen APIs o recursos CDN.
- Git, si se quiere comparar versiones o realizar los ejercicios en ramas.
- Godot 4.7.1 con soporte para .NET, únicamente para el cliente Godot.

Comprueba los SDK instalados con:

```bash
dotnet --list-sdks
```

## Cómo ejecutar un proyecto

Entra en la carpeta que contiene el archivo `.csproj` y ejecuta:

```bash
dotnet restore
dotnet run
```

También puede indicarse directamente el proyecto desde esta carpeta. Cuando la
ruta contenga espacios, debe escribirse entre comillas:

```bash
dotnet run --project "Agenda de teléfonos/Agenda de teléfonos.csproj"
dotnet run --project "Mazmorra Online/MazmorraOnline.csproj"
dotnet run --project "Pokemon/Pokemon - Version final/Pokemon.csproj"
```

La dirección exacta aparece en la terminal. Los puertos pueden variar si hay
otra aplicación utilizándolos o si se elige un perfil distinto.

Para detener el servidor, pulsa `Ctrl+C`.

## Configuración de claves y secretos

Los secretos de desarrollo no deben escribirse en Git. Los proyectos que los
necesitan incluyen un `UserSecretsId` y, generalmente, un archivo
`appsettings.Local.example.json` que puede copiarse como
`appsettings.Local.json`. La copia local está excluida mediante `.gitignore`.

| Proyecto | Configuración en desarrollo | Variable en producción | Obligatoria |
|---|---|---|---:|
| NASA Explorer | `Nasa:ApiKey` | `Nasa__ApiKey` | Sí, para APOD y NeoWs |
| Fútbol | `FootballData:ApiKey` | `FootballData__ApiKey` | Sí |
| Open Weather | `OpenWeather:ApiKey` | `OpenWeather__ApiKey` | Sí |
| Películas | `Tmdb:TokenAcceso` | `Tmdb__TokenAcceso` | Sí |
| Videojuegos | `Rawg:ApiKey` | `Rawg__ApiKey` | Sí |
| Open Food Facts | `OpenFoodFacts:Contacto` | `OpenFoodFacts__Contacto` | Contacto recomendado |
| Biblioteca | `OpenLibrary:Contacto` | `OpenLibrary__Contacto` | Contacto recomendado |
| Recetas | `TheMealDb:ApiKey` | `TheMealDb__ApiKey` | No; incluye la clave educativa `1` |

Ejemplo para desarrollo:

```bash
cd Peliculas
dotnet user-secrets set "Tmdb:TokenAcceso" "TU_TOKEN_DE_LECTURA"
dotnet user-secrets list
dotnet run
```

En variables de entorno se utilizan dos guiones bajos (`__`) para representar
la separación jerárquica de la configuración. Por ejemplo,
`Tmdb__TokenAcceso` se transforma en `Tmdb:TokenAcceso`.

> No incluyas `Bearer` en el valor del token de TMDB: el servicio ya construye
> la cabecera de autorización.

## Tecnologías que se practican

### Servidor y renderizado

- C# y .NET 10.
- ASP.NET Core Razor Pages.
- PageModels, handlers `OnGet`, `OnPost` y sus variantes asíncronas.
- Model binding, validación y anotaciones de datos.
- Tag Helpers como `asp-page`, `asp-route-*` y `asp-for`.
- Inyección de dependencias.
- Clientes HTTP tipados.
- Configuración mediante JSON, User Secrets y variables de entorno.
- Caché con `IMemoryCache`.

### Persistencia y seguridad

- SQLite.
- ADO.NET mediante `Microsoft.Data.Sqlite` en los proyectos introductorios.
- Entity Framework Core en los proyectos de mayor tamaño.
- Relaciones uno a muchos y muchos a muchos.
- ASP.NET Core Identity, cookies y autorización.
- Datos privados asociados al identificador del usuario.
- Consultas parametrizadas y protección antifalsificación en formularios.

### Cliente web

- HTML generado con Razor.
- Bootstrap 5 y Bootstrap Icons.
- Bootswatch y temas persistidos en `localStorage`.
- SweetAlert2 para confirmaciones y avisos.
- JavaScript, Fetch API y manipulación del DOM donde es necesario.
- Diseño responsive para ordenador, tableta y móvil.
- Canvas y controles táctiles en Mazmorra Online.
- Leaflet y Chart.js en NASA Explorer.

### Servicios y calidad

- Controladores de API y DTO.
- JSON con `System.Text.Json`.
- CORS en el trivial para clientes externos.
- SignalR para comunicación bidireccional en tiempo real.
- xUnit y `WebApplicationFactory` para pruebas de integración.
- Clientes de consola, Godot y JavaScript desacoplados de la base de datos.

## Patrones comunes de los proyectos

Una Razor Page suele estar formada por dos archivos:

```text
Pages/Peliculas/Index.cshtml
Pages/Peliculas/Index.cshtml.cs
```

- `.cshtml` contiene la plantilla HTML y las expresiones Razor.
- `.cshtml.cs` contiene el `PageModel`, los datos que necesita la vista y los
  métodos que responden a las peticiones.

En las aplicaciones que consumen APIs, el recorrido habitual es:

```text
Navegador
   ↓
Razor Page
   ↓
PageModel
   ↓
Servicio inyectado
   ↓
HttpClient
   ↓
API externa
```

Cuando existe persistencia con Entity Framework:

```text
Razor Page o controlador
   ↓
Servicio o DbContext
   ↓
Entity Framework Core
   ↓
SQLite
```

Los proyectos introductorios de Agenda, Lista de tareas y Pasapalabra utilizan
ADO.NET de forma intencionada para que las consultas `SELECT`, `INSERT`,
`UPDATE` y `DELETE` sean visibles. Los proyectos más grandes utilizan Entity
Framework Core para trabajar con entidades, relaciones y consultas LINQ.

## 1. Agenda de teléfonos

[Abrir el proyecto](<Agenda de teléfonos/>)

Aplicación CRUD sencilla para administrar contactos. Es uno de los proyectos
más apropiados para comenzar porque contiene pocas páginas y muestra
directamente la relación entre formularios Razor, objetos de C# y SQL.

### Funcionalidad

- Listado de contactos.
- Búsqueda por nombre mientras se escribe.
- Alta y modificación de nombre y teléfono.
- Eliminación con confirmación.
- Fotografía opcional de cada contacto.
- Redimensionado de imágenes con ImageSharp.
- Conversión a PNG y almacenamiento en Base64 dentro de SQLite.
- Conservación de la imagen existente al editar sin seleccionar otra.
- Ordenación y búsqueda adaptadas al español.
- Interfaz responsive con Bootstrap y recursos administrados con LibMan.

### Conceptos principales

- Razor Pages y handlers GET/POST.
- `Microsoft.Data.Sqlite` y SQL parametrizado.
- Creación y actualización sencilla del esquema al arrancar.
- Validación de formularios.
- Procesamiento de imágenes con `SixLabors.ImageSharp` 3.1.12.
- Representación de una imagen Base64 mediante una URL `data:`.

El repositorio incluye `agenda.db` con datos de trabajo y una carpeta `doc` con
material complementario en imagen, PDF y vídeo.

## 2. Biblioteca

[Abrir el proyecto](Biblioteca/) · [README específico](Biblioteca/README.md)

Aplicación completa para buscar obras en Open Library y mantener una colección
privada.

### Funcionalidad destacada

- Inicio con tendencias, libros mejor valorados y programación.
- Búsqueda por título, autor, ISBN, materia y otros campos.
- Listados de novedades, fantasía, misterio, ciencia ficción, romance y
  programación.
- Fichas con autores, ediciones, idiomas, ISBN, materias, valoración,
  disponibilidad y recomendaciones.
- Registro e inicio de sesión con Identity.
- Favoritos privados con copia local de los datos básicos.
- Temas Bootstrap y Bootswatch.
- Confirmaciones con SweetAlert2.
- Sustitución de portadas ausentes.
- Caché y limitación de frecuencia para respetar Open Library.
- API JSON propia.

Open Library no necesita clave. Se recomienda configurar un contacto para
identificar correctamente las peticiones de la aplicación.

## 3. Fútbol

[Abrir el proyecto](Futbol/) · [README específico](Futbol/README.md)

Cliente educativo de la API v4 de football-data.org.

### Funcionalidad destacada

- Partidos de hoy y calendario por fecha.
- Competiciones permitidas por el plan contratado.
- Clasificaciones y estadísticas.
- Partidos recientes y próximos.
- Goleadores cuando la API los permite.
- Ficha de equipos, entrenador, plantilla y calendario.
- Cuentas locales y equipos favoritos por usuario.
- Caché para respetar el límite del plan gratuito.

Necesita un token de football-data.org. El plan gratuito puede limitar las
competiciones, retrasar resultados y restringir el número de peticiones por
minuto. Es una buena aplicación para estudiar códigos de error HTTP, cuotas y
caché.

## 4. Juego Pasapalabra

[Abrir la familia de proyectos](<Juego pasapalabra/>)

Juego web de 27 letras, incluida la Ñ, acompañado de un CRUD de temas y
preguntas. Utiliza SQLite mediante ADO.NET y guarda la partida en la sesión.

### Funcionalidad común

- Pregunta aleatoria para cada letra.
- Selección de tema o mezcla de todos.
- Estados pendiente, correcto e incorrecto.
- Acción «Pasapalabra».
- CRUD completo de preguntas y temas.
- Búsqueda automática que ignora mayúsculas y tildes.
- Ordenación con cultura española y tratamiento correcto de la Ñ.
- SQL parametrizado y claves ajenas de SQLite.
- Datos de ejemplo insertados cuando la base está vacía.
- Confirmaciones y avisos con SweetAlert2.

### Versiones disponibles

| Versión | Diferencia principal |
|---|---|
| [Bootstrap](<Juego pasapalabra/Juego pasapalabra - Bootstrap/>) | Solo clases de Bootstrap; librerías locales restaurables con LibMan |
| [Bootstrap y CSS](<Juego pasapalabra/Juego pasapalabra - Bootstrap y CSS/>) | Añade `wwwroot/css/site.css` para representar una interfaz más personalizada |
| [Bootstrap y CSS mediante CDN](<Juego pasapalabra/Juego pasapalabra - Bootstrap y CSS - CDN/>) | Conserva el CSS propio, pero Bootstrap, Icons y SweetAlert2 se cargan desde jsDelivr |

La administración no incorpora autenticación porque el objetivo es mantener un
código pequeño. Antes de publicar la aplicación habría que proteger el CRUD.

## 5. Lista de tareas

[Abrir la familia de proyectos](<Lista de tareas/>)

Aplicación CRUD de tareas y categorías, apropiada para estudiar relaciones y
reglas de integridad antes de introducir Entity Framework.

### Funcionalidad

- Alta, listado, edición y eliminación de tareas.
- Marcado de tareas completadas.
- CRUD de categorías.
- Filtrado por categoría.
- Impedimento de borrar categorías que todavía contienen tareas.
- Validación, mensajes y confirmaciones.
- Ordenación según la cultura española.
- Tablas, tarjetas y formularios adaptables a móvil.

### Versiones disponibles

| Versión | Recursos de interfaz |
|---|---|
| [Bootstrap local](<Lista de tareas/Lista de tareas - Bootstrap/>) | Bootstrap, Icons y SweetAlert2 dentro de `wwwroot/lib`; restaurables con LibMan |
| [Bootstrap mediante CDN](<Lista de tareas/Lista de tareas - Bootstrap - CDN/>) | Las mismas bibliotecas se descargan desde jsDelivr y el proyecto ocupa menos espacio |

Ambas variantes utilizan `Microsoft.Data.Sqlite` y la base incluida
`lista_tareas.db`.

## 6. Mazmorra Online

[Abrir el proyecto](<Mazmorra Online/>) ·
[README específico](<Mazmorra Online/README.md>)

Juego multijugador de una única partida global construido con Razor Pages,
Canvas y SignalR. Todo el estado se conserva en memoria.

### Funcionalidad destacada

- Entre 2 y 16 jugadores por ronda.
- Movimiento y disparo de práctica desde el primer jugador.
- Rondas de 90 segundos.
- Tres mapas de texto elegidos aleatoriamente.
- Muros y ocho potenciadores por mapa.
- Física, acciones y estados a 10 actualizaciones por segundo.
- Reconexión con periodo de cortesía.
- Expulsión tras cinco minutos sin interacción.
- Teclado, ratón y dos joysticks táctiles.
- Pantalla completa y modal de estadísticas.
- Historial de rondas, mapas y clasificación.
- Temas claros, oscuros y Bootswatch.
- API propia y hub de SignalR.

### Aspectos didácticos

- Servicios singleton y estado compartido.
- Bucle periódico del servidor.
- DTO pequeños para reducir el tráfico.
- Comunicación bidireccional y reconexión.
- Diferencia entre frecuencia de simulación y frecuencia de envío.
- Controles táctiles y conversión de coordenadas de Canvas.

Al reiniciar o reciclar el proceso se pierden jugadores, clasificación e
historial porque no existe base de datos. Es una decisión deliberada para
centrar el ejemplo en SignalR.

## 7. NASA Explorer

[Abrir el proyecto](NasaExplorer/) ·
[README específico](NasaExplorer/README.md)

Aplicación que reúne varias fuentes oficiales de NASA bajo una misma interfaz.

### Módulos

- APOD: imagen o vídeo astronómico del día y archivo por fechas.
- NASA Image and Video Library: imágenes, vídeos y audios.
- DSCOVR EPIC: imágenes de la Tierra.
- EONET v3: eventos naturales con mapa Leaflet.
- NeoWs: asteroides cercanos y evaluación de peligrosidad.
- DONKI: actividad y clima espacial.
- NASA Exoplanet Archive: búsqueda y gráficos con Chart.js.

Incluye Identity, favoritos privados, caché y gestión independiente de errores
por módulo. Solo APOD y NeoWs necesitan la clave de NASA; el resto de fuentes
utilizadas son públicas. No se usan las APIs antiguas de Mars Rover Photos,
Earth Imagery ni EONET 2.1.

Es el proyecto con mayor variedad de formatos de datos, visualizaciones y
servicios externos de la colección.

## 8. Open Food Facts

[Abrir el proyecto](OpenFoodFacts/) ·
[README específico](OpenFoodFacts/README.md)

Aplicación para consultar productos alimentarios y conservar favoritos.

### Funcionalidad destacada

- Portada con productos populares en España.
- Búsqueda por nombre, marca o código de barras EAN/UPC.
- Categorías y filtros por Nutri-Score.
- Ficha con ingredientes, alérgenos, aditivos, envase y países.
- Tabla nutricional por 100 g o 100 ml.
- Nutri-Score, NOVA y Green-Score.
- Registro, favoritos y persistencia con Identity y SQLite.
- Comparación simultánea de entre dos y cuatro productos favoritos.
- Caché y tratamiento de límites de peticiones.
- Temas y avisos reutilizables.

Las consultas de lectura no necesitan clave, pero Open Food Facts exige que el
cliente se identifique con un `User-Agent` y un contacto real antes de
publicarlo.

## 9. Open Weather

[Abrir el proyecto](OpenWeather/) ·
[README específico](OpenWeather/README.md)

Aplicación meteorológica sin base de datos ni cuentas de usuario. La clave se
mantiene siempre en el servidor.

### Funcionalidad destacada

- Geocodificación y selección entre localidades homónimas.
- Ubicación opcional del navegador.
- Tiempo actual y sensación térmica.
- Humedad, presión, viento, visibilidad y nubosidad.
- Amanecer y atardecer en hora local.
- Previsión de cinco días y próximas 48 horas.
- Lluvia, nieve y probabilidad de precipitación.
- Calidad del aire y ocho contaminantes.
- Unidades métricas e imperiales.
- Caché y API JSON propia.
- Temas Bootstrap/Bootswatch sin CSS personalizado.

Necesita una clave de OpenWeather. Una clave recién creada puede tardar un
tiempo en activarse.

## 10. Películas

[Abrir el proyecto](Peliculas/) · [README específico](Peliculas/README.md)

Catálogo de películas conectado con TMDB.

### Funcionalidad destacada

- Carrusel de tendencias y listados de cartelera, populares, mejor valoradas y
  próximos estrenos.
- Búsqueda paginada.
- Ficha con sinopsis, géneros, dirección, reparto y recomendaciones.
- Tráiler de YouTube incrustado y alternativa en inglés.
- Proveedores disponibles en España mediante los datos de JustWatch de TMDB.
- Registro, inicio de sesión y favoritas privadas.
- Caché para reducir llamadas a TMDB.
- Página de ayuda y API JSON propia.
- Interfaz responsive, temas y SweetAlert2.

Necesita el **API Read Access Token** largo de TMDB. No debe utilizarse la
clave corta como `api_key` ni añadirse manualmente la palabra `Bearer`.

## 11. Pokémon

[Abrir la familia de proyectos](Pokemon/)

Itinerario de cinco proyectos autosuficientes que muestran la evolución desde
una petición HTTP básica hasta una Pokédex completa.

| Etapa | Proyecto | Contenido nuevo |
|---:|---|---|
| 1 | [Listado básico](<Pokemon/Pokemon - Listado basico/>) | 151 nombres, `HttpClient`, deserialización y `foreach` |
| 2 | [Listado con imágenes](<Pokemon/Pokemon - Listado con imagenes/>) | Identificadores, sprites, tarjetas y carga diferida |
| 3 | [Detalles](<Pokemon/Pokemon - Detalles/>) | Rutas, Tag Helpers, tipos, altura, peso y habilidades |
| 4 | [Detalles y selector de temas](<Pokemon/Pokemon - Detalles y Selector temas/>) | Bootstrap oscuro, Bootswatch, `dataset` y `localStorage` |
| 5 | [Versión final](<Pokemon/Pokemon - Version final/>) | Búsqueda, paginación, especies, evolución, encuentros, movimientos, sonidos y carrusel |

La versión final consulta todos los Pokémon y variedades, muestra datos en
español cuando están disponibles, utiliza caché para el buscador y no contiene
archivos CSS o JavaScript propios. PokeAPI no necesita clave.

## 12. Recetas

[Abrir el proyecto](Recetas/) · [README específico](Recetas/README.md)

Aplicación conectada con TheMealDB que combina catálogo externo y organización
personal.

### Funcionalidad destacada

- Recetas aleatorias, categorías y zonas gastronómicas.
- Búsqueda por nombre.
- Ingredientes, cantidades, preparación, vídeo y fuente.
- Favoritos privados.
- Menú semanal con una receta por día.
- Lista de la compra generada desde el menú.
- Registro, Identity y SQLite.
- Caché, temas, SweetAlert2 e imagen alternativa.

TheMealDB permite la clave educativa `1`, ya configurada. Una clave de
colaborador puede guardarse con User Secrets.

## 13. Rick and Morty

[Abrir el proyecto](RickAndMorty/) ·
[README específico](RickAndMorty/README.md)

Aplicación que permite navegar por los recursos relacionados de The Rick and
Morty API.

### Funcionalidad destacada

- Personajes paginados y filtros por nombre, estado, especie y género.
- Origen, última localización y episodios de cada personaje.
- Guía y detalle de episodios.
- Catálogo de localizaciones, dimensiones y residentes.
- Navegación entre personajes, episodios y lugares.
- Registro, Identity y personajes favoritos.
- Caché, temas y confirmaciones.

La API es pública y no necesita token, clave ni registro.

## 14. Trivial paso a paso

[Abrir el itinerario](Trivial-Paso-a-Paso/) ·
[README general](Trivial-Paso-a-Paso/README.md)

Itinerario acumulativo de siete versiones independientes. Cada etapa conserva
lo anterior y añade una responsabilidad concreta. Todas incluyen su propio
proyecto, solución, base de datos y README.

La base de cada etapa contiene 10 categorías y 1.000 preguntas.

| Versión | Carpeta | Aportación principal |
|---:|---|---|
| 1 | [Listados](Trivial-Paso-a-Paso/01-Listados/) | Modelos, DbContext, consultas asíncronas y páginas de solo lectura |
| 2 | [CRUD de categorías](Trivial-Paso-a-Paso/02-CRUD-Categorias/) | Formularios, validación, PRG, `TempData` y borrado en cascada |
| 3 | [CRUD de preguntas](Trivial-Paso-a-Paso/03-CRUD-Preguntas/) | Claves ajenas, `SelectList`, formulario parcial y respuesta correcta |
| 4 | [Búsqueda y paginación](Trivial-Paso-a-Paso/04-Busqueda-Paginacion/) | Filtros, búsqueda sin tildes, estado en URL, debounce y recuperación del foco |
| 5 | [API REST](Trivial-Paso-a-Paso/05-API-REST/) | Controladores, DTO, rutas y API JSON de solo lectura |
| 6 | [Cliente del juego](Trivial-Paso-a-Paso/06-Cliente-Juego/) | HTML, JavaScript, Fetch API, marcador y estado de partida |
| 7 | [Versión definitiva](Trivial-Paso-a-Paso/07-Version-Definitiva/) | Temas, SweetAlert2, CORS, iconos y acabado responsive |

### Clientes adicionales

- [Cliente de consola](Trivial-Paso-a-Paso/Clientes/Consola/): aplicación
  `net10.0` que usa `HttpClient` y `GetFromJsonAsync`; no abre SQLite.
- [Cliente Godot](Trivial-Paso-a-Paso/Clientes/Godot/): interfaz gráfica creada
  mediante C# en Godot 4.7.1; utiliza `net8.0` y consume la versión 5 o posterior.
- [Cliente JavaScript](Trivial-Paso-a-Paso/Clientes/JavaScript/): sitio estático
  independiente con servidor configurable, temas, iconos y SweetAlert2; para
  ejecutarlo desde otro origen se conecta con la versión 7, que habilita CORS.

### Pruebas

La carpeta [Pruebas](Trivial-Paso-a-Paso/Pruebas/) contiene una copia de la API
y un proyecto xUnit. `WebApplicationFactory` inicia el servidor en memoria y
sustituye el contexto por una base SQLite temporal. Las pruebas comprueban
categorías, cantidad, filtrado, estructura de respuestas y códigos 404.

## 15. Videojuegos

[Abrir el proyecto](Videojuegos/) ·
[README específico](Videojuegos/README.md)

Catálogo de videojuegos conectado con RAWG y biblioteca privada por usuario.

### Funcionalidad destacada

- Inicio con populares, mejor valorados y próximos lanzamientos.
- Búsqueda y listados de novedades, acción, rol, estrategia, independientes,
  deportes y carreras.
- Descripción, plataformas, géneros, empresas, edad, tiendas y capturas.
- Biblioteca privada con estados pendiente, jugando, completado y abandonado.
- Puntuación personal del 1 al 10 y comentario.
- Filtros y ordenación locales de la biblioteca.
- Copia de los datos básicos para mostrar los guardados sin depender de RAWG.
- Identity, SQLite, caché, temas e imágenes alternativas.

Necesita una clave de RAWG y debe conservar los enlaces y la atribución exigida
por el proveedor.

---

## APIs JSON expuestas por las propias aplicaciones

Además de consumir servicios externos, varios proyectos publican sus propios
endpoints. Esto permite crear otros clientes sin acceder directamente a sus
bases de datos.

### Biblioteca

```http
GET /api/libros/buscar?texto=Don%20Quijote&pagina=1
GET /api/libros/OL45804W
```

### Open Weather

```http
GET /api/lugares?texto=Alicante
GET /api/tiempo?lat=38.3452&lon=-0.4810&unidades=metrico
```

`unidades` acepta `metrico` o `imperial`.

### Películas

```http
GET /api/peliculas/populares?pagina=1
GET /api/peliculas/buscar?texto=matrix&pagina=1
GET /api/peliculas/603
GET /api/favoritos
```

`/api/favoritos` requiere una sesión iniciada.

### Trivial

Disponible desde la versión 5:

```http
GET /api/categorias
GET /api/categorias/1
GET /api/preguntas
GET /api/preguntas/1
GET /api/preguntas?cantidad=10
GET /api/preguntas?categoriaId=2&cantidad=10
```

La API es de solo lectura; el CRUD se mantiene en Razor Pages.

### Mazmorra Online

```http
POST   /api/entrar
DELETE /api/jugadores/{jugadorId}
GET    /api/mapas
GET    /api/clasificacion
GET    /api/jugadores/{jugadorId}
GET    /api/resultados
```

El canal de tiempo real se publica en:

```text
/hubs/juego
```

## APIs externas utilizadas

| Proyecto | Proveedor | Credencial |
|---|---|---|
| Biblioteca | Open Library Search, Works y Covers | Sin clave; contacto recomendado |
| Fútbol | football-data.org v4 | Token obligatorio |
| NASA Explorer | NASA Open APIs, Images, EPIC, EONET, DONKI y Exoplanet Archive | Clave para algunos módulos |
| Open Food Facts | Open Food Facts | Sin clave; `User-Agent` identificable |
| Open Weather | OpenWeather Geocoding, Current, Forecast y Air Pollution | Clave obligatoria |
| Películas | TMDB API v3 | API Read Access Token obligatorio |
| Pokémon | PokeAPI | Sin clave |
| Recetas | TheMealDB | Clave educativa incluida |
| Rick and Morty | The Rick and Morty API | Sin clave |
| Videojuegos | RAWG Video Games Database API | Clave obligatoria |

## Bases de datos y persistencia

### SQLite con ADO.NET

- Agenda de teléfonos.
- Las dos variantes de Lista de tareas.
- Las tres variantes de Pasapalabra.

Estos proyectos utilizan `Microsoft.Data.Sqlite`, SQL visible y parámetros con
`AddWithValue`. Incluyen bases con datos de trabajo o inicialización automática.

### SQLite con Entity Framework Core

- Biblioteca.
- Fútbol.
- NASA Explorer.
- Open Food Facts.
- Películas.
- Recetas.
- Rick and Morty.
- Trivial.
- Videojuegos.

Los proyectos con Identity guardan cuentas y datos privados. Las aplicaciones
de catálogo suelen conservar solo una copia mínima del elemento marcado, no
una réplica completa de la API externa.

Biblioteca, Fútbol, NASA Explorer, Open Food Facts, Películas, Recetas, Rick and
Morty y Videojuegos utilizan `EnsureCreatedAsync` para facilitar el primer
arranque. Es una decisión didáctica. En un proyecto real con cambios continuos
de esquema conviene utilizar migraciones de Entity Framework Core.

### Sin persistencia permanente

- Open Weather y Pokémon dependen de la API y de la caché en memoria.
- Mazmorra Online guarda la partida en memoria.
- Los clientes de consola, Godot y JavaScript no acceden a una base de datos.
- Las pruebas del trivial utilizan SQLite en memoria y lo destruyen al terminar.

## Autenticación e Identity

Biblioteca, Fútbol, NASA Explorer, Open Food Facts, Películas, Recetas, Rick and
Morty y Videojuegos utilizan ASP.NET Core Identity.

Patrón general:

- correo único utilizado para iniciar sesión;
- contraseña almacenada mediante hash, nunca en texto plano;
- cookie de autenticación;
- registro inmediato sin confirmación obligatoria de correo;
- acceso restringido a las colecciones privadas;
- separación de favoritos y datos por identificador de usuario.

La ausencia de confirmación de correo es adecuada para prácticas locales. Para
una aplicación pública real habría que valorar confirmación, recuperación de
contraseña, proveedor de correo, protección frente a abuso y políticas de
privacidad.

## Bootstrap, Bootswatch, LibMan y CDN

Los proyectos siguen dos estrategias:

1. **Recursos locales con LibMan**: apropiados cuando se quiere trabajar sin
   conexión después de restaurar las bibliotecas. Agenda, Lista de tareas local
   y las variantes locales de Pasapalabra siguen este enfoque.
2. **Recursos desde CDN**: reducen el tamaño del proyecto y simplifican la
   publicación, pero necesitan conexión a Internet. Es el enfoque mayoritario
   en las aplicaciones que consumen APIs.

Para restaurar un proyecto con `libman.json`:

```bash
dotnet tool install -g Microsoft.Web.LibraryManager.Cli
libman restore
```

No todos los proyectos necesitan CSS personalizado. Pokémon final y Open
Weather, por ejemplo, se apoyan exclusivamente en clases de Bootstrap.

## Publicación

Antes de publicar:

1. Ejecuta `dotnet restore` y `dotnet build`.
2. Configura las claves fuera del repositorio.
3. Comprueba los límites y las condiciones de la API utilizada.
4. Revisa la ruta y los permisos de escritura de SQLite.
5. Decide si deben conservarse cuentas y datos entre despliegues.
6. Prueba la interfaz con distintos anchos y temas.
7. Revisa logs y respuestas HTTP sin mostrar secretos al usuario.

### Publicar desde Visual Studio Code

Cada proyecto web debe publicarse por separado. Abre en VS Code la carpeta que
contiene su archivo `.csproj` y ejecuta:

```bash
dotnet restore
dotnet publish -c Release -o publicacion
```

En Windows, crea desde PowerShell un ZIP con el contenido publicado:

```powershell
Compress-Archive -Path .\publicacion\* -DestinationPath .\publicacion.zip -Force
```

El comodín hace que el ZIP contenga directamente `web.config`, el DLL
principal, `appsettings.json`, `wwwroot` y los demás archivos, sin una
carpeta `publicacion` intermedia.

En MonsterASP.NET:

1. Abre **Files** en el panel del sitio.
2. Entra en `/wwwroot`.
3. Sube `publicacion.zip`.
4. Extrae el ZIP dentro de `/wwwroot`.
5. Permite sobrescribir los archivos de la aplicación sin borrar previamente
   todo el directorio.
6. Reinicia la aplicación o el AppPool.

No deben subirse el código fuente, el `.csproj`, `bin` ni `obj`. Cada
alojamiento ejecuta una aplicación web; no debe intentarse publicar toda la
carpeta `Razor Pages` como un único sitio.

Consulta la
[guía de MonsterASP.NET para publicar mediante ZIP](https://help.monsterasp.net/books/deploy/page/how-to-deploy-website-content-from-zip-file).
WebDeploy queda como alternativa para quien utilice Visual Studio completo.

### Variables de entorno

En MonsterASP.NET pueden configurarse desde:

```text
Websites → Manage website → Scripting → Environment Variables
```

Por ejemplo:

```text
Nombre: Tmdb__TokenAcceso
Valor:  TU_API_READ_ACCESS_TOKEN
```

Los `user-secrets` no se publican y solo deben considerarse una solución de
desarrollo. En producción deben utilizarse variables de entorno o un almacén
de secretos del proveedor.

### Consideraciones sobre SQLite al publicar

- El proceso necesita permiso de escritura sobre la carpeta de la base.
- No sobrescribas la base del servidor al desplegar una nueva versión si deseas
  conservar usuarios y favoritos.
- Descarga una copia de seguridad antes de extraer una actualización.
- No borres todo `/wwwroot` antes de copiar los archivos nuevos.
- Realiza copias de seguridad antes de modificar entidades o esquema.
- `EnsureCreatedAsync` crea una base nueva, pero no migra automáticamente una
  base existente.
- En Trivial, `Data/trivial.db` se incluye en la publicación. Desde la versión
  2 debe retirarse del ZIP de las actualizaciones si se quieren conservar los
  cambios realizados en el servidor.
- En despliegues con varias instancias del servidor, un archivo SQLite local no
  es una base compartida adecuada.

## Seguridad

- No subas tokens, claves, correos privados o bases con usuarios reales.
- No guardes contraseñas por tu cuenta; utiliza Identity.
- Mantén las consultas SQL parametrizadas.
- Conserva la protección antifalsificación de los formularios Razor.
- Valida identificadores, rangos, páginas y textos en el servidor.
- No devuelvas entidades de Identity desde una API JSON.
- Limita el tamaño y el tipo de los archivos subidos.
- Protege los CRUD administrativos antes de exponerlos públicamente.
- Revisa CORS y permite únicamente los orígenes necesarios.
- Respeta atribución, licencias y condiciones de cada proveedor externo.

## Mantenimiento de paquetes

Los proyectos se han creado en momentos diferentes y no todos fijan las mismas
revisiones de Entity Framework Core o SQLitePCLRaw. Algunos utilizan EF Core
10.0.0 y SQLitePCLRaw 2.1.11, mientras que los más recientes utilizan EF Core
10.0.10 y SQLitePCLRaw 2.1.12.

Antes de desplegar conviene ejecutar, dentro de cada proyecto:

```bash
dotnet restore
dotnet list package --outdated
dotnet list package --vulnerable
```

Las actualizaciones deben probarse por proyecto. En los itinerarios progresivos
puede ser útil conservar versiones iguales entre etapas para que las diferencias
de código sigan siendo fáciles de estudiar.

## Solución de problemas comunes

### Falta una clave después de publicar

Los `user-secrets` del ordenador no viajan al servidor. Configura la variable de
entorno correspondiente con dos guiones bajos, reinicia la aplicación y no
incluyas comillas ni prefijos como `Bearer` salvo que el README específico lo
solicite.

### La aplicación no encuentra una API local

- Comprueba el puerto mostrado por `dotnet run`.
- Abre primero un endpoint JSON en el navegador.
- Mantén la API ejecutándose mientras utilizas el cliente.
- En clientes de otro dispositivo no utilices `localhost`; utiliza la IP del
  ordenador que ejecuta la API.
- Revisa CORS únicamente cuando navegador y API tengan orígenes distintos.

### SQLite muestra columnas o tablas incorrectas

`EnsureCreatedAsync` no actualiza esquemas existentes. Durante una práctica,
detén la aplicación, guarda los datos que necesites, elimina la base local y
vuelve a ejecutar. No hagas esto en producción.

### No aparecen estilos o iconos

- Con CDN, comprueba la conexión a Internet y las políticas del navegador.
- Con LibMan, ejecuta `libman restore`.
- Comprueba las rutas de `wwwroot` y que `UseStaticFiles` o `MapStaticAssets`
  estén configurados según el proyecto.
- Fuerza una recarga con `Ctrl+F5` si el navegador conserva archivos antiguos.

### Una API devuelve 401, 403, 404 o 429

- `401`: clave ausente, incorrecta o todavía no activa.
- `403`: recurso no permitido por el plan o credencial sin permisos.
- `404`: identificador inexistente, ruta incorrecta o controlador no mapeado.
- `429`: se ha superado la cuota o la frecuencia permitida.

Consulta el README del proyecto porque cada servicio aplica reglas diferentes.

### El puerto ya está ocupado

Cierra la otra aplicación o ejecuta temporalmente con otra dirección:

```bash
dotnet run --urls http://localhost:5500
```

## Qué proyecto elegir según el concepto

| Quiero practicar... | Proyecto recomendado |
|---|---|
| Primera petición HTTP y deserialización | Pokémon, etapa 1 |
| Cuadrículas responsive e imágenes | Pokémon, etapa 2 |
| Rutas y parámetros | Pokémon, etapa 3 |
| Temas y `localStorage` | Pokémon, etapa 4 o Trivial 7 |
| CRUD pequeño con SQL | Agenda de teléfonos |
| Relaciones y claves ajenas | Lista de tareas o Trivial 2-3 |
| Sesiones y estado de juego | Pasapalabra |
| Entity Framework Core | Trivial 1-4 |
| Búsqueda, filtros y paginación | Trivial 4, Pokémon final u Open Food Facts |
| Crear una API REST | Trivial 5, Biblioteca, Open Weather o Películas |
| Consumir una API desde JavaScript | Trivial 6 o cliente JavaScript independiente |
| Consumir una API desde C# | Cliente de consola del trivial |
| Cliente gráfico desacoplado | Cliente Godot del trivial |
| Pruebas de integración | Pruebas del trivial |
| Identity y datos por usuario | Biblioteca, Recetas o Rick and Morty |
| Comparación de datos | Open Food Facts |
| Caché y límites de cuota | Fútbol, RAWG, NASA u OpenWeather |
| Mapas y gráficos | NASA Explorer |
| Multimedia y carruseles | Películas o Pokémon final |
| SignalR y tiempo real | Mazmorra Online |
| Controles móviles y Canvas | Mazmorra Online |

## Documentación específica

La mayoría de las aplicaciones incluyen su propio `README.md` con instrucciones
detalladas, estructura, endpoints, ejercicios y solución de problemas. En los
itinerarios de Pokémon y Trivial debe leerse primero el README general de la
familia o el de la etapa que se vaya a utilizar.

## Licencias, atribución y datos externos

Cada API conserva sus propias condiciones de uso, límites y requisitos de
atribución. Revisa el README y los enlaces del proyecto correspondiente antes de
publicar, redistribuir datos o retirar créditos.

Entre las fuentes utilizadas se encuentran Open Library, football-data.org,
NASA, Open Food Facts, OpenWeather, TMDB, PokeAPI, TheMealDB, The Rick and Morty
API y RAWG. Los proyectos no implican respaldo ni certificación por parte de
estos proveedores.

La licencia del código puede estar indicada en el `LICENSE` o README de cada
aplicación. La ausencia de un archivo de licencia en una subcarpeta no debe
interpretarse automáticamente como permiso para redistribuir cualquier recurso
externo que aparezca en ella.
