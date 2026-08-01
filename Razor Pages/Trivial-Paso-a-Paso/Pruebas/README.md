# Pruebas de TrivialApi: xUnit y Playwright

Esta carpeta está preparada para colocarse directamente dentro de
`Razor Pages/Trivial-Paso-a-Paso`. Las pruebas apuntan a la aplicación real de:

```text
07-Version-Definitiva/TrivialApi
```

No contienen una copia de la API. De esta forma, al modificar la versión
7, las pruebas comprueban exactamente ese código.

## Tecnologías

- xUnit para organizar y ejecutar todas las pruebas.
- `HttpClient` para las pruebas de integración de la API.
- Playwright para abrir Chromium y comprobar Razor Pages y el cliente JavaScript.
- Kestrel en un puerto libre para que el navegador acceda a una dirección HTTP real.
- SQLite temporal con datos conocidos. Nunca se modifica `Data/trivial.db`.

## Estructura

```text
Pruebas
├── TrivialApi.Testing
│   ├── TrivialTestServer.cs
│   └── InformeConsola.cs
├── TrivialApi.Tests
│   └── 37 casos de integración HTTP
├── TrivialApi.PlaywrightTests
│   └── 27 casos de navegador
└── TrivialApiConPruebas.slnx
```

Los casos parametrizados de xUnit cuentan como ejecuciones independientes.
El total es de **64 casos ejecutables**.

## Correcciones incluidas

- los enlaces de navegación se buscan únicamente dentro del menú principal;
- los botones de edición se identifican por su `aria-label` exacto;
- el filtro de categoría espera la nueva URL antes de comprobar la tabla;
- la salida en directo se desactiva para evitar líneas duplicadas;
- los colores ANSI quedan desactivados por defecto.

Estas pruebas esperan que los cuatro formularios corregidos de la aplicación
utilicen `for="Categoria"` y `for="Pregunta"` al cargar `_Formulario`.

## Ejecución

Desde esta carpeta:

```console
dotnet test --logger "console;verbosity=detailed"
```

No se genera un informe TRX, no hay un script de PowerShell propio y no es
necesario ejecutar antes `dotnet run`.

La primera ejecución instala automáticamente solo Chromium mediante la API
oficial de Playwright. Las siguientes ejecuciones reutilizan el navegador ya
instalado.

## Salida por consola

`xunit.runner.json` desactiva `showLiveOutput` para evitar que el logger
detallado repita cada línea. Los pasos aparecen dentro del resultado de cada
prueba. Los colores ANSI están desactivados por defecto porque algunos runners
muestran sus códigos como texto:

- azul: inicio de la prueba;
- amarillo: petición HTTP;
- cian: acción o comprobación;
- verde: prueba superada.

Para activar expresamente los colores en una terminal compatible, desde
PowerShell:

```console
$env:TRIVIAL_TEST_COLORS = "1"
```

La variable estándar `NO_COLOR` continúa teniendo prioridad y permite
desactivarlos.

## Qué se prueba con xUnit y HttpClient

Se mantienen las 37 pruebas de la API:

- categorías, orden e identificadores;
- preguntas, cantidades, límites y filtros;
- códigos 400, 404 y 405;
- contrato JSON y DTO;
- ausencia de propiedades internas;
- rutas generales y disponibilidad de Razor Pages.

## Qué se prueba con Playwright

### Navegación y diseño

- carga de la página principal;
- enlaces de categorías, preguntas y juego;
- barra de navegación sticky;
- menú hamburguesa;
- ausencia de desbordamiento horizontal en móvil.

### Categorías

- creación;
- validación de nombres duplicados;
- edición;
- cancelación y confirmación del borrado con SweetAlert.

### Preguntas

- creación completa;
- validaciones;
- edición;
- búsqueda sin distinguir mayúsculas ni tildes;
- conservación del foco del buscador;
- filtro por categoría;
- cancelación y confirmación del borrado.

### Temas

- Bootstrap oscuro;
- persistencia en `localStorage`;
- cambio de hoja Bootswatch;
- tema compartido con el cliente del juego.

### Juego

- carga de categorías desde la API real;
- ocultación de la zona inicial al comenzar;
- cuatro respuestas por pregunta;
- respuesta correcta;
- respuesta incorrecta con explicación;
- resultado final y vuelta al inicio.

Las pruebas del flujo del juego interceptan las respuestas de la API para que
las preguntas y respuestas sean conocidas. Así no dependen del orden aleatorio
del endpoint.

## Base de datos y servidor

Cada proyecto de pruebas inicia su propio servidor y crea una base SQLite en
la carpeta temporal del sistema. Contiene tres categorías y doce preguntas.
El servidor se detiene y la base temporal se elimina al finalizar.

La aplicación se inicia mediante `dotnet run --no-build`, porque la referencia
de proyecto hace que la versión 7 se compile antes de ejecutar las pruebas.
