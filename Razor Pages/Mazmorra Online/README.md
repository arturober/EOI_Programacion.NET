# Mazmorra Online — partida única

Juego educativo en red desarrollado con .NET 10, ASP.NET Core,
Razor Pages, Canvas y SignalR.

El código está escrito y comentado en español de España. Se ha priorizado
que resulte fácil de leer para estudiantes que están aprendiendo C#.

## Características

- Una única partida, sin códigos ni salas.
- Práctica de movimiento y disparo desde que entra el primer jugador.
- Rondas para entre 2 y 16 jugadores.
- Rondas de 90 segundos.
- Tres mapas de texto elegidos aleatoriamente entre rondas.
- Cada nueva partida vacía sortea un mapa para el primer jugador.
- La primera ronda conserva el mapa utilizado durante la espera.
- Ocho power-ups por mapa.
- Física del servidor a 10 actualizaciones por segundo.
- Estados SignalR a 10 envíos por segundo.
- Acciones de los jugadores a 10 envíos por segundo.
- Reconexión con 10 segundos de cortesía ante cortes de red.
- Cierre de sesiones tras 5 minutos sin interacción.
- Controles para teclado, ratón y pantallas táctiles.
- Dos joysticks que aparecen debajo de los dedos.
- Pantalla completa.
- Estadísticas y leyenda en un modal Bootstrap.
- Confirmación de salida con SweetAlert2.
- Páginas Razor independientes para mapas y rondas.
- Tema Bootstrap oscuro predeterminado.
- Selector de Bootstrap claro, oscuro y todos los temas Bootswatch.
- Paleta del tablero adaptada automáticamente a temas claros y oscuros.
- API disponible bajo la ruta `/api`.
- Estado guardado únicamente en memoria.

## Bibliotecas

Para reducir el espacio necesario en el alojamiento, estas bibliotecas se
descargan mediante CDN:

- Bootstrap 5.3.8.
- Bootstrap Icons 1.13.1.
- Bootswatch 5.3.8.
- SweetAlert2 11.26.25.

El cliente SignalR 10.0.0 permanece en `wwwroot/lib/signalr`. De esta forma,
la versión JavaScript coincide exactamente con la versión de .NET utilizada
por el servidor.

## Estructura

```text
MazmorraOnline.sln
└── MazmorraOnline
    ├── Controllers
    │   └── JuegoController.cs
    ├── Dtos
    ├── Hubs
    │   └── JuegoHub.cs
    ├── Mapas
    │   ├── mapa1.txt
    │   ├── mapa2.txt
    │   └── mapa3.txt
    ├── Models
    │   └── AccionJugador.cs
    ├── Pages
    │   ├── Index.cshtml
    │   ├── Juego.cshtml
    │   ├── Mapas.cshtml
    │   └── Rondas.cshtml
    ├── Services
    │   ├── GestorJuego.cs
    │   └── MotorJuego.cs
    ├── wwwroot
    │   ├── css
    │   │   └── juego.css
    │   └── js
    │       ├── juego.js
    │       └── tema.js
    └── Program.cs
```

## Ejecución local

Desde la carpeta que contiene el código de la aplicación:

```powershell
dotnet run --project .\MazmorraOnline
```

La aplicación se abrirá en:

```text
http://localhost:5055
```

Para probar varios jugadores se pueden utilizar ventanas de incógnito,
navegadores diferentes o varios dispositivos.

## Controles

### Ordenador

- `WASD` o cursores: mover.
- Ratón: apuntar.
- Botón izquierdo: disparar.

### Móvil y tableta

- Se recomienda utilizar la pantalla en horizontal.
- Tocar la mitad izquierda: mostrar el joystick de movimiento.
- Tocar la mitad derecha: mostrar el joystick de disparo.
- Apoyar el dedo derecho: comenzar a disparar.
- Arrastrar el dedo derecho: elegir la dirección de disparo.
- Los controles pueden aparecer en cualquier parte de la pantalla.
- Los botones y los modales no activan los controles del juego.

## Temas

El desplegable de la navbar permite elegir:

- Bootstrap oscuro.
- Bootstrap claro.
- Los 26 temas actuales de Bootswatch.

La elección se guarda en `localStorage`, que es un pequeño almacenamiento
del propio navegador. Si todavía no existe ninguna elección, se utiliza
Bootstrap oscuro.

Bootswatch sustituye solamente la hoja CSS. El JavaScript de Bootstrap sigue
siendo el mismo para todos los temas.

Los temas se separan en una lista sencilla de claros y oscuros. Al cambiar
el tema, el canvas elige una paleta con más o menos luminosidad para que
el suelo, la cuadrícula, los muros y los textos mantengan buen contraste.

## Espera y comienzo de ronda

Una sola persona puede recorrer el mapa y disparar mientras espera. Durante
esa práctica no corre el tiempo, no se conceden victorias ni eliminaciones
y no se recogen power-ups.

Cuando entra la segunda persona, se limpian los proyectiles y se colocan
todos los jugadores de nuevo, pero se conserva el mismo mapa. Al terminar
esa primera ronda, las siguientes ya pueden elegir otro mapa al azar.

## Razor Pages

### Inicio

`Index.cshtml` contiene el formulario de entrada y las instrucciones.
`IndexModel` solo crea el jugador y redirige al tablero.

### Mapas

`Mapas.cshtml` recorre las filas y casillas mediante Razor. Cada mapa se
representa con una tabla y utilidades de Bootstrap, sin JavaScript.

### Rondas

`Rondas.cshtml` genera una tabla responsive con las últimas diez rondas.
Tampoco necesita JavaScript propio.

### Juego

`Juego.cshtml` necesita JavaScript porque Canvas y SignalR se ejecutan en el
navegador. Bootstrap se encarga del HUD, el panel responsive y el modal.

## Servicios web

### Entrar en el juego

```http
POST /api/entrar
Content-Type: application/json

{
  "nombre": "Ana"
}
```

### Salir del juego

```http
DELETE /api/jugadores/{id}
```

### Consultar los mapas

```http
GET /api/mapas
```

### Consultar la clasificación

```http
GET /api/clasificacion
```

### Consultar un jugador

```http
GET /api/jugadores/{id}
```

### Consultar las últimas rondas

```http
GET /api/resultados
```

### Comunicación en tiempo real

```text
/hubs/juego
```

La API, Razor Pages y SignalR pertenecen al mismo servidor. Por eso no es
necesario configurar CORS.

## Frecuencias del juego

`MotorJuego` utiliza un `PeriodicTimer` de 100 milisegundos:

1. Actualiza la física con `0.1f`.
2. Obtiene un DTO con el estado.
3. Envía ese DTO mediante SignalR.

El navegador también envía `AccionJugador` cada 100 milisegundos. Así, la
física, las acciones y los estados utilizan la misma frecuencia de 10 Hz.

## Desconexiones e inactividad

Un jugador no se elimina inmediatamente cuando SignalR pierde la conexión.
El servidor conserva su plaza durante 10 segundos y detiene su personaje.
Si SignalR vuelve dentro de ese plazo, la conexión nueva recupera el mismo
jugador. El identificador de conexión evita que el cierre tardío de una
conexión antigua afecte a la nueva.

El servidor también elimina los navegadores que permanecen 5 minutos sin
interacción. Moverse, disparar o cambiar la dirección de apuntado renueva
la actividad. Los paquetes idénticos que JavaScript envía automáticamente
cada 100 milisegundos no la renuevan.

Si la sesión caduca, SweetAlert informa al jugador y lo devuelve al inicio.
El botón «Salir» continúa eliminando la sesión inmediatamente.

## Datos enviados por SignalR

Los muros no se incluyen en cada estado. Razor envía los mapas una sola vez
al abrir la página y JavaScript dibuja el que corresponda.

Cada proyectil solo envía:

```text
X
Y
```

El propietario y las velocidades se utilizan únicamente en el servidor.

## Mapas de texto

Cada mapa mide 16 columnas por 9 filas. Cada carácter representa una casilla
de 60 por 60 píxeles:

```text
J.J.J.J.J.J.J.J.
..P...##...P....
....#......#....
.P..#..P...#..P.
....###.###.....
.P..#..P...#..P.
....#......#....
..P...##...P....
J.J.J.J.J.J.J.J.
```

- `.`: suelo.
- `#`: muro.
- `J`: posible posición inicial.
- `P`: posible posición para un power-up.

Para añadir un mapa basta con guardar otro archivo `.txt` válido en la
carpeta `Mapas`.

## Publicación para MonsterASP.NET

> **Método recomendado:** utiliza WebFTP.

MonsterASP.NET admite .NET 10, SignalR y WebSockets. Desde Visual Studio Code,
abre la terminal integrada en la carpeta `Mazmorra Online` y ejecuta:

```bash
dotnet restore
dotnet publish -c Release -o publicacion
```

En PowerShell, crea el ZIP:

```powershell
Compress-Archive -Path .\publicacion\* -DestinationPath .\publicacion.zip -Force
```

Después:

1. Abre **Files** en el panel de MonsterASP.NET.
2. Entra en `/wwwroot`.
3. Sube y extrae `publicacion.zip`.
4. Permite sobrescribir los archivos anteriores sin borrar todo el directorio.
5. Activa HTTPS para el sitio.
6. Reinicia la aplicación o el AppPool.

`MazmorraOnline.dll`, `web.config`, `appsettings.json`, `Mapas` y
`wwwroot` deben quedar directamente dentro de `/wwwroot`. No subas el código
fuente, el `.csproj`, `bin` ni `obj`.

MonsterASP.NET ofrece soporte para SignalR y WebSockets. No obstante, después
de publicar conviene comprobar en las herramientas del navegador que la
conexión a `/hubs/juego` se establece correctamente.

### Comprobar el despliegue

1. Abre el sitio en dos navegadores o dispositivos.
2. Entra con dos nombres diferentes.
3. Comprueba movimiento, disparos, cambio de ronda y clasificación.
4. Cierra momentáneamente una conexión y verifica la reconexión.
5. Reinicia la aplicación y comprueba que comienza un estado nuevo.

Si SignalR o el servidor devuelven un error, revisa
`Control Panel → Websites → Manage → Logs` y habilita temporalmente los
[logs de ASP.NET Core](https://help.monsterasp.net/books/debugging/page/aspnet-core-debug-logging).

### Estado en memoria y límites

No existe base de datos. Todos los jugadores, estadísticas, mapas elegidos e
historial de rondas se almacenan en la memoria del proceso:

- se pierden cuando se reinicia o se vuelve a publicar la aplicación;
- pueden perderse si el alojamiento recicla el proceso por mantenimiento o
  inactividad;
- no pueden compartirse entre varias instancias del servidor;
- no deben considerarse datos permanentes.

Los identificadores del juego no sustituyen un sistema de autenticación.
Cualquier visitante puede entrar con un nombre, por lo que el proyecto está
pensado para prácticas controladas y no para cuentas reales.

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
