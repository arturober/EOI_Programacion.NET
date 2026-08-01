# Cliente JavaScript independiente del Trivial

Este cliente permite jugar al trivial desde una página estática creada con
HTML y JavaScript. Consume la API REST por HTTP, pero no forma parte de la
aplicación Razor Pages, no utiliza su layout y no contiene ningún enlace hacia
la administración.

La dirección del servidor se escribe en la propia interfaz. Por tanto, el
mismo cliente puede utilizar una API local, una API publicada en
MonsterASP.NET u otra instalación compatible sin modificar el código.

## Objetivos didácticos

- Separar con claridad el cliente y el servidor.
- Consumir una API REST con `fetch` y `async/await`.
- Construir elementos del DOM sin insertar HTML recibido del servidor.
- Mantener el estado de una partida en JavaScript.
- Gestionar errores HTTP y de conexión.
- Comprender por qué un cliente web externo necesita CORS.
- Guardar preferencias sencillas con `localStorage`.
- Aplicar temas de Bootstrap y Bootswatch.
- Mostrar avisos con SweetAlert2.

## Tecnologías

- HTML5.
- JavaScript sin frameworks.
- Fetch API.
- Bootstrap 5.3.8.
- Bootstrap Icons 1.13.1.
- Bootswatch 5.3.8.
- SweetAlert2 11.26.25.

Las bibliotecas visuales se descargan desde CDN. El cliente no utiliza npm,
Node.js, un proceso de compilación ni paquetes NuGet.

## Estructura

```text
JavaScript
├── index.html
├── README.md
└── js
    ├── temas.js
    └── trivial.js
```

- `index.html` contiene la estructura y los controles vacíos.
- `js/temas.js` cambia el tema y guarda la elección.
- `js/trivial.js` gestiona la conexión, las peticiones y la partida.

No hay CSS personalizado. La presentación se construye exclusivamente con
clases de Bootstrap y los temas seleccionados.

## API compatible

El cliente utiliza dos operaciones de solo lectura:

```http
GET /api/categorias
GET /api/preguntas?cantidad=10
GET /api/preguntas?categoriaId=2&cantidad=10
```

Además de los endpoints, un navegador exige que el servidor permita el origen
del cliente mediante CORS. El itinerario incluye esa configuración desde
`05-API-REST` y la conserva en las versiones 6 y 7.

Puede utilizarse cualquier versión desde `05-API-REST`. Para una demostración
con todas las mejoras de interfaz se recomienda ejecutar o publicar:

```text
07-Version-Definitiva/TrivialApi
```

Los clientes de consola y Godot no tienen esta limitación porque no ejecutan
las peticiones dentro de un navegador.

## Ejecutar la API desde Visual Studio Code

Abra la carpeta del repositorio en Visual Studio Code y cree una terminal.
Desde la raíz de `Trivial-Paso-a-Paso` ejecute:

```bash
cd 07-Version-Definitiva/TrivialApi
dotnet restore
dotnet run
```

La terminal mostrará una dirección parecida a:

```text
http://localhost:5000
```

Mantenga esa terminal abierta. Antes de iniciar el cliente puede comprobar la
API en el navegador:

```text
http://localhost:5000/api/categorias
```

Si el puerto es diferente, utilice la dirección que muestre `dotnet run`.

## Abrir el cliente desde Visual Studio Code

La opción más cómoda es servir la carpeta con una extensión de servidor web
local, como Live Server:

1. Abra `Clientes/JavaScript/index.html`.
2. Utilice la orden **Open with Live Server** o la equivalente de la extensión.
3. En **Dirección del servidor**, escriba la URL mostrada por `dotnet run`.
4. Pulse **Conectar**.
5. Elija una categoría y comience la partida.

También se puede abrir `index.html` directamente mediante doble clic. Sin
embargo, un servidor web local evita las restricciones que algunos navegadores
aplican a las páginas cargadas con el protocolo `file://`.

El cliente y la API se ejecutan por separado:

```text
Cliente estático                 API del trivial
http://127.0.0.1:5500            http://localhost:5000
          │                               │
          └──────── HTTP y JSON ──────────┘
```

No es necesario copiar la carpeta `JavaScript` dentro de `TrivialApi/wwwroot`.

## Conectar con MonsterASP.NET

Publique primero `07-Version-Definitiva/TrivialApi` siguiendo el README de esa
etapa. Compruebe que responde, por ejemplo:

```text
https://tu-sitio.runasp.net/api/categorias
```

Después abra este cliente por separado y escriba únicamente la raíz:

```text
https://tu-sitio.runasp.net
```

No añada `/api`, `/api/categorias` ni una barra final. El código normaliza la
dirección y añade las rutas necesarias.

La interfaz guarda la última dirección válida en `localStorage`. Al volver a
abrir el cliente aparecerá escrita, pero será necesario pulsar **Conectar** de
nuevo. Así no se realiza una petición de red sin que el usuario lo decida.

## Independencia respecto a Razor Pages

Este cliente:

- no contiene código Razor;
- no necesita .NET para mostrar la interfaz;
- no utiliza rutas relativas como `/api`;
- no abre directamente `trivial.db`;
- no comparte archivos con `TrivialApi/wwwroot`;
- no enlaza con la portada ni con el CRUD de la aplicación;
- solo conoce la dirección HTTP y el JSON público de la API.

La API sí debe estar ejecutándose en algún servidor porque es la responsable
de leer las categorías y preguntas de SQLite.

## Flujo de uso

```text
Escribir servidor
        ↓
GET /api/categorias
        ↓
Elegir categoría
        ↓
GET /api/preguntas?cantidad=10
        ↓
Responder y contar aciertos
        ↓
Mostrar el resultado final
```

Si se elige una categoría se añade `categoriaId` a la segunda petición.

## Conexión

`normalizarUrl` utiliza `new URL` para comprobar que la dirección comienza por
`http://` o `https://`. También elimina la barra final.

`obtenerJson` centraliza:

1. La petición con `fetch`.
2. La cabecera `Accept: application/json`.
3. La comprobación de `respuesta.ok`.
4. La conversión del cuerpo a JSON.

`fetch` no considera automáticamente que un código 404 o 500 sea una
excepción. Por eso se comprueba el estado HTTP antes de leer el JSON.

## Construcción segura del contenido

Los nombres, enunciados y respuestas recibidos se asignan mediante
`textContent`. Los botones se crean con `document.createElement`.

No se introduce en la página HTML procedente de la API. Esto evita interpretar
como marcado un texto almacenado en una pregunta.

## Estado de la partida

JavaScript conserva cuatro valores principales:

```javascript
let urlServidor = "";
let preguntas = [];
let posicion = 0;
let aciertos = 0;
```

- `urlServidor` identifica la API conectada.
- `preguntas` contiene el JSON de la partida.
- `posicion` indica la pregunta mostrada.
- `aciertos` guarda la puntuación.

Cada respuesta se desactiva al pulsarla para impedir dobles envíos mientras se
muestra el aviso de SweetAlert.

## Selector de temas

El selector incluye:

- Bootstrap claro.
- Bootstrap oscuro.
- Los temas disponibles de Bootswatch utilizados en la versión definitiva.

`temas.js` cambia el atributo `href` de la hoja con Id `temaCss`. Para los
temas oscuros también establece:

```html
<html data-bs-theme="dark">
```

La elección se guarda en `localStorage` con una clave propia de este cliente.
No depende de la preferencia almacenada por la aplicación Razor Pages.

## SweetAlert2

SweetAlert2 se utiliza para:

- errores de dirección o conexión;
- categorías sin preguntas;
- respuestas correctas e incorrectas;
- resultado final.

El fondo y el texto se obtienen de las variables CSS del tema activo para que
los avisos sigan siendo legibles en modo claro y oscuro.

## Errores frecuentes

### No se ha podido conectar

Compruebe:

1. Que la API sigue ejecutándose.
2. Que el puerto coincide con el mostrado por `dotnet run`.
3. Que se ha escrito la raíz, sin `/api`.
4. Que `/api/categorias` responde en el navegador.
5. Que se está utilizando la versión 5 o posterior, o un servidor con CORS
   equivalente.

### El navegador menciona CORS

El cliente y la API tienen orígenes distintos aunque ambos estén en el mismo
ordenador si utilizan puertos diferentes. Ejecute la versión 5, 6 o 7, que
registran la política `PermitirTodos` para las prácticas.

En una aplicación real no se debería utilizar `AllowAnyOrigin`; convendría
permitir únicamente las direcciones desde las que se publique el cliente.

### Mixed Content o contenido mixto

Una página servida mediante HTTPS no puede consultar normalmente una API HTTP.
Utilice HTTPS en ambos lados. Para MonsterASP.NET escriba siempre la dirección
pública que comienza por `https://`.

### No se aplican los temas o faltan iconos

Bootstrap, Bootswatch, Bootstrap Icons y SweetAlert2 se descargan desde CDN.
Compruebe la conexión a Internet y que el navegador no esté bloqueando
`cdn.jsdelivr.net`.

### La categoría no contiene preguntas

Una categoría creada desde el CRUD puede estar vacía. Elija otra categoría o
añada preguntas desde la administración de la API.

## Pruebas manuales recomendadas

1. Conectar con una dirección local válida.
2. Probar una dirección incorrecta y comprobar el aviso.
3. Jugar con todas las categorías.
4. Jugar filtrando una categoría.
5. Acertar y fallar preguntas.
6. Comprobar progreso y puntuación.
7. Terminar la partida y volver a jugar.
8. Cambiar entre un tema claro y otro oscuro.
9. Recargar y comprobar que se conservan tema y dirección.
10. Probar la API publicada mediante HTTPS.
11. Reducir el ancho del navegador y comprobar la adaptación móvil.
12. Verificar que la interfaz no contiene enlaces hacia Razor Pages.

## Posibles ejercicios

- Permitir elegir entre 5, 10 y 20 preguntas.
- Añadir un límite de tiempo por pregunta.
- Mostrar el porcentaje final de aciertos.
- Guardar la mejor puntuación en `localStorage`.
- Añadir un botón para interrumpir la partida.
- Restringir CORS a la dirección concreta donde se aloje el cliente.

Estas ampliaciones pueden realizarse sin modificar las entidades ni acceder
directamente a SQLite: el cliente debe continuar dependiendo solo del contrato
HTTP de la API.
