# Versión 6: cliente JavaScript del trivial

Esta etapa añade un cliente estático jugable. La administración Razor y la API
permanecen intactas.

El cliente demuestra que una API puede ser utilizada por una interfaz que no
conoce Entity Framework, los modelos C# ni la ubicación de SQLite.

## Objetivos

- Consumir una API con `fetch`.
- Trabajar con promesas y `async/await`.
- Convertir respuestas HTTP a JSON.
- Crear elementos HTML desde JavaScript.
- Mantener el estado de una partida.
- Asociar eventos.
- Diferenciar cliente y servidor.
- Mostrar mensajes con SweetAlert.
- Gestionar errores HTTP y de conexión.

## Funcionalidad nueva

- Abrir el cliente desde la barra.
- Cargar categorías desde la API.
- Elegir una categoría.
- Solicitar diez preguntas.
- Mostrar enunciado y cuatro respuestas.
- Comprobar la respuesta.
- Mostrar la correcta al fallar.
- Contar aciertos.
- Mostrar progreso.
- Presentar resultado final.
- Volver a jugar.
- Regresar a la administración.

## Ejecutar

```bash
cd 06-Cliente-Juego/TrivialApi
dotnet restore
dotnet run
```

Después abra:

```text
/cliente/index.html
```

No abra el HTML haciendo doble clic. Debe servirse desde la aplicación porque
utiliza la ruta relativa `/api`, que apunta al mismo servidor y puerto.

## Publicación en MonsterASP.NET

Desde `06-Cliente-Juego/TrivialApi`:

```bash
dotnet publish -c Release -o publicacion
```

En el primer despliegue conserva `Data/trivial.db`. Para actualizaciones que
deban mantener el banco del servidor:

```powershell
Remove-Item .\publicacion\Data\trivial.db
Compress-Archive -Path .\publicacion\* -DestinationPath .\publicacion.zip -Force
```

Descarga antes una copia de seguridad y extrae el ZIP en `/wwwroot`. El
cliente ya forma parte de la publicación y debe abrirse mediante:

```text
https://tu-sitio.runasp.net/cliente/index.html
```

```javascript
const api = "/api";
```

Esta ruta ya está configurada en el proyecto. El cliente integrado y la API
comparten origen, no dependen de `localhost` y no requieren cambios al
publicar. Comprueba también:

```text
https://tu-sitio.runasp.net/api/categorias
```

El CRUD administrativo no tiene autenticación. Consulta las instrucciones
completas en el [README general](../README.md).

El cliente de esta etapa está integrado en `wwwroot`. No debe confundirse con
el [cliente JavaScript independiente](../Clientes/JavaScript/), que se sirve
por separado, permite escribir la dirección de la API e incluye selector de
temas. Para ese cliente externo se recomienda la versión 7 porque es la etapa
más completa, aunque puede conectarse desde la versión 5 porque CORS ya está
habilitado desde esa etapa.

## Archivos añadidos

```text
wwwroot/cliente
├── index.html
└── trivial.js
```

## Archivos ampliados

```text
Pages/Shared/_Layout.cshtml
Pages/Index.cshtml
```

Solo se añaden enlaces al cliente.

## Separación de responsabilidades

```text
index.html
    Presentación y elementos vacíos

trivial.js
    Peticiones, estado y comportamiento

Controllers
    Datos JSON

TrivialContext
    Acceso a SQLite
```

El HTML no contiene preguntas escritas manualmente.

## Ruta de la API

El ejemplo descargado comienza con una dirección local fija. Para servir el
cliente desde la propia aplicación, especialmente al publicarlo, se recomienda
el cambio explicado anteriormente:

```javascript
const api = "/api";
```

Es una ruta del mismo servidor. A partir de ella se construyen:

```text
/api/categorias
/api/preguntas?cantidad=10
```

## Estado del juego

```javascript
let preguntas = [];
let posicion = 0;
let aciertos = 0;
```

- `preguntas` contiene el JSON recibido.
- `posicion` indica la pregunta actual.
- `aciertos` guarda el marcador.

Son variables modificables porque su valor cambia durante la partida.

## Referencias al documento

Los elementos reutilizados se guardan una vez:

```javascript
const inicio = document.getElementById("inicio");
```

Esto evita repetir búsquedas en el DOM y hace más legible el resto del código.

## obtenerJson

La función:

```javascript
async function obtenerJson(direccion)
```

centraliza:

1. Petición con `fetch`.
2. Comprobación de `respuesta.ok`.
3. Conversión con `respuesta.json()`.

`fetch` no lanza por sí mismo una excepción para un 404 o un 500. Por eso se
comprueba `ok`.

## Cargar categorías

```javascript
const categorias =
    await obtenerJson(`${api}/categorias`);
```

Después, para cada objeto:

1. Se crea una `option`.
2. Su `value` recibe el Id.
3. Su texto recibe el nombre.
4. Se añade al selector.

La opción “Todas las categorías” ya existe en el HTML.

## Comenzar partida

El valor del selector decide el filtro:

```javascript
const filtro = categoria
    ? `&categoriaId=${categoria}`
    : "";
```

La petición completa:

```javascript
`${api}/preguntas?cantidad=10${filtro}`
```

## Categoría vacía

Una categoría creada desde el CRUD puede no contener preguntas.

Si el array recibido está vacío, SweetAlert informa y la función termina con
`return`.

## Cambiar de pantalla

Bootstrap aporta la clase `d-none`.

Al empezar:

```javascript
inicio.classList.add("d-none");
juego.classList.remove("d-none");
```

Al terminar se realiza la operación inversa.

No se navega a otra página.

## Mostrar pregunta

`mostrarPregunta`:

1. Obtiene `preguntas[posicion]`.
2. Actualiza progreso.
3. Actualiza puntos.
4. Muestra categoría.
5. Muestra enunciado.
6. Borra botones anteriores.
7. Crea cuatro botones nuevos.

## Crear botones

```javascript
const boton = document.createElement("button");
```

Cada botón recibe:

- Tipo.
- Clases Bootstrap.
- Texto.
- Evento `click`.

El índice del array comienza en cero, pero la API numera las respuestas del uno
al cuatro:

```javascript
() => responder(indice + 1)
```

## Comprobar respuesta

```javascript
const esCorrecta =
    numero === pregunta.respuestaCorrecta;
```

Si es correcta se incrementa `aciertos`.

Si se falla:

```javascript
pregunta.respuestas[
    pregunta.respuestaCorrecta - 1
]
```

resta uno para volver del número de la API al índice del array.

## Esperar a SweetAlert

```javascript
await Swal.fire(...)
```

La función no avanza hasta que el usuario pulse Continuar.

## Terminar la partida

Después de cada respuesta:

```javascript
posicion++;
```

Si quedan preguntas se llama de nuevo a `mostrarPregunta`. En caso contrario
se muestra el total y se regresa al inicio.

## Gestión de errores

Las promesas se completan con:

```javascript
.catch(mostrarErrorConexion)
```

La misma función se utiliza al cargar categorías y al empezar.

## Diferencias con la versión 5

| Versión 5 | Versión 6 |
|---|---|
| API visible como JSON | API utilizada por una interfaz |
| Sin estado de partida | Posición y aciertos |
| Sin `fetch` | Peticiones desde JavaScript |
| Páginas Razor | Razor y cliente estático |
| Uso de endpoints manual | Uso programático |

## Pruebas manuales

1. Abrir el cliente.
2. Comprobar las categorías.
3. Jugar con todas.
4. Jugar con una categoría.
5. Acertar una pregunta.
6. Fallar y comprobar la respuesta correcta.
7. Terminar la partida.
8. Volver a jugar.
9. Volver a la API.
10. Crear una categoría vacía y seleccionarla.
11. Detener el servidor y comprobar el error.

## Preguntas para evaluar los conceptos aprendidos

1. ¿De dónde proceden las categorías?
2. ¿Qué representa una promesa?
3. ¿Por qué se utiliza `await`?
4. ¿Qué diferencia hay entre HTTP y JSON?
5. ¿Por qué se comprueba `respuesta.ok`?
6. ¿Qué variables forman el estado?
7. ¿Por qué se usa `indice + 1`?
8. ¿Qué hace `replaceChildren`?
9. ¿Cómo se cambia de pantalla?
10. ¿Accede JavaScript directamente a SQLite?

## Ejercicios sugeridos

1. Permitir seleccionar cinco o diez preguntas.
2. Mostrar porcentaje final.
3. Añadir botón para abandonar.
4. Deshabilitar respuestas después de pulsar.
5. Mostrar el número de errores.
6. Añadir un botón para repetir la misma categoría.

## Paso siguiente

La versión 7 mantiene intacta la lógica del juego y añade temas, iconos,
mensajes SweetAlert en Razor Pages y confirmaciones de borrado reutilizables.
