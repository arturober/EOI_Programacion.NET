/*
 * Este archivo contiene únicamente la programación que necesita el juego:
 * SignalR, canvas, teclado, ratón, controles táctiles y pantalla completa.
 * Las ventanas, la navbar y el diseño general los proporciona Bootstrap.
 */

// ---------------------------------------------------------------------------
// Elementos de la página
// ---------------------------------------------------------------------------

const datosJuego = document.getElementById("datosJuego");
const jugadorId = datosJuego.dataset.jugadorId;

const canvas = document.getElementById("juego");
const contexto = canvas.getContext("2d");
const mapas = JSON.parse(
    document.getElementById("mapasJuego").textContent);

const estadoPartida = document.getElementById("estadoPartida");
const numeroRonda = document.getElementById("numeroRonda");
const tiempoPartida = document.getElementById("tiempoPartida");
const numeroJugadores = document.getElementById("numeroJugadores");
const nombreMapa = document.getElementById("nombreMapa");

const estadoConexion = document.getElementById("estadoConexion");
const iconoConexion = document.getElementById("iconoConexion");
const textoConexion = document.getElementById("textoConexion");

const listaJugadoresEscritorio =
    document.getElementById("listaJugadoresEscritorio");
const listaJugadoresModal =
    document.getElementById("listaJugadoresModal");
const modalEstadisticas =
    document.getElementById("modalEstadisticas");

const joystickMovimiento = document.getElementById("joystickMovil");
const palancaMovimiento = document.getElementById("palancaJoystick");
const joystickDisparo = document.getElementById("joystickDisparo");
const palancaDisparo = document.getElementById("palancaDisparo");

const botonPantallaCompleta =
    document.getElementById("botonPantallaCompleta");
const iconoPantallaCompleta =
    document.getElementById("iconoPantallaCompleta");
const formularioSalir =
    document.getElementById("formularioSalir");

// Esta clase permite aplicar reglas exclusivas de la página del juego.
document.body.classList.add("pagina-juego");

// ---------------------------------------------------------------------------
// Constantes y estado local
// ---------------------------------------------------------------------------

const TAMANO_CASILLA = 60;
const RADIO_JOYSTICK = 38;
const ZONA_MUERTA_JOYSTICK = 10;

// Cada tipo de power-up tiene un color y una letra.
const DATOS_POWERUPS = {
    Vida: ["#51cf66", "+"],
    Escudo: ["#74c0fc", "E"],
    Velocidad: ["#ffd43b", "V"],
    DisparoRapido: ["#ff922b", "D"]
};

// Canvas no aplica automáticamente los colores del tema de Bootstrap.
// Estas dos paletas mantienen el tablero legible con temas claros y oscuros.
const PALETAS_TABLERO = {
    oscuro: {
        suelo: "#17141f",
        cuadricula: "#211d2d",
        muro: "#514a61",
        bordeMuro: "#867b99",
        proyectil: "#f8f9fa",
        puntero: "#ffffff",
        texto: "#ffffff"
    },
    claro: {
        suelo: "#e9ecef",
        cuadricula: "#ced4da",
        muro: "#adb5bd",
        bordeMuro: "#6c757d",
        proyectil: "#212529",
        puntero: "#212529",
        texto: "#212529"
    }
};

let estado = null;
let conectado = false;
let salidaConfirmada = false;
let sesionFinalizada = false;
let punteroMovimiento = null;
let punteroDisparo = null;
let centroMovimiento = { x: 0, y: 0 };
let centroDisparo = { x: 0, y: 0 };
let paletaTablero = obtenerPaletaTablero();

// tema.js lanza este evento cada vez que cambia el desplegable.
document.addEventListener("temaCambiado", () => {
    paletaTablero = obtenerPaletaTablero();
});

function obtenerPaletaTablero() {
    const tipo =
        document.documentElement.dataset.temaJuego ?? "oscuro";

    return PALETAS_TABLERO[tipo] ?? PALETAS_TABLERO.oscuro;
}

// Este objeto se envía al servidor diez veces por segundo.
const accion = {
    arriba: false,
    abajo: false,
    izquierda: false,
    derecha: false,
    disparar: false,
    angulo: 0
};

// ---------------------------------------------------------------------------
// Conexión con SignalR
// ---------------------------------------------------------------------------

const conexion = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/juego")
    .withAutomaticReconnect()
    .build();

// El servidor llama a este método cuando tiene un estado nuevo.
conexion.on("EstadoActualizado", nuevoEstado => {
    const sigoEnLaPartida = nuevoEstado.jugadores.some(
        jugador => jugador.id === jugadorId);

    if (!sigoEnLaPartida && !salidaConfirmada) {
        finalizarSesion(
            "La sesión se ha cerrado por inactividad "
            + "o por una desconexión prolongada.");
        return;
    }

    estado = nuevoEstado;
    actualizarInterfaz();
});

// Durante una reconexión se detienen las acciones para evitar movimientos.
conexion.onreconnecting(() => {
    conectado = false;
    limpiarControles();
    mostrarConexion(
        "Reconectando",
        "text-bg-warning",
        "bi-arrow-repeat");
});

// La conexión nueva debe volver a asociarse con el jugador.
conexion.onreconnected(async () => {
    try {
        const recuperada = await entrarEnPartida();

        if (!recuperada) {
            await finalizarSesion(
                "No se ha podido recuperar tu jugador. "
                + "La desconexión ha durado demasiado.");
        }
    } catch (error) {
        mostrarConexion(
            "Jugador no disponible",
            "text-bg-danger",
            "bi-wifi-off");
        console.error(error);
    }
});

// onclose solo se ejecuta si SignalR abandona todos sus reintentos.
conexion.onclose(() => {
    conectado = false;
    limpiarControles();
    mostrarConexion(
        "Sin conexión",
        "text-bg-danger",
        "bi-wifi-off");
});

// Se inicia la conexión y el dibujo al cargar la página.
iniciar();
requestAnimationFrame(dibujar);

async function iniciar() {
    try {
        await conexion.start();
        const admitido = await entrarEnPartida();

        if (!admitido) {
            await finalizarSesion(
                "El jugador ya no está disponible.");
            return;
        }

        // Cien milisegundos equivalen a diez envíos por segundo.
        setInterval(enviarAccion, 100);
    } catch (error) {
        mostrarConexion(
            "No se puede conectar",
            "text-bg-danger",
            "bi-wifi-off");
        console.error(error);
    }
}

async function entrarEnPartida() {
    const admitido = await conexion.invoke(
        "EntrarEnPartida",
        jugadorId);

    if (!admitido) {
        return false;
    }

    conectado = true;

    mostrarConexion(
        "En línea",
        "text-bg-success",
        "bi-wifi");

    return true;
}

async function finalizarSesion(mensaje) {
    if (sesionFinalizada || salidaConfirmada) {
        return;
    }

    sesionFinalizada = true;
    conectado = false;
    limpiarControles();

    await Swal.fire({
        title: "Sesión finalizada",
        text: mensaje,
        icon: "info",
        confirmButtonText: "Volver al inicio",
        allowOutsideClick: false,
        allowEscapeKey: false
    });

    window.location.href = "/";
}

function enviarAccion() {
    if (!conectado) {
        return;
    }

    conexion
        .send("EnviarAccion", accion)
        .catch(error => console.error(error));
}

function mostrarConexion(texto, claseColor, icono) {
    // Se conservan las clases comunes y solo cambia el color del badge.
    estadoConexion.className =
        `badge rounded-pill ${claseColor} ms-md-auto`;
    iconoConexion.className = `bi ${icono} me-1`;
    textoConexion.textContent = texto;
}

// ---------------------------------------------------------------------------
// Teclado y ratón
// ---------------------------------------------------------------------------

window.addEventListener("keydown", evento => {
    // Mientras el modal está abierto, las teclas pertenecen a la ventana.
    if (modalEstadisticas.classList.contains("show")) {
        return;
    }

    if (cambiarTecla(evento.code, true)) {
        evento.preventDefault();
    }
});

window.addEventListener("keyup", evento => {
    if (cambiarTecla(evento.code, false)) {
        evento.preventDefault();
    }
});

function cambiarTecla(codigoTecla, pulsada) {
    switch (codigoTecla) {
        case "KeyW":
        case "ArrowUp":
            accion.arriba = pulsada;
            return true;
        case "KeyS":
        case "ArrowDown":
            accion.abajo = pulsada;
            return true;
        case "KeyA":
        case "ArrowLeft":
            accion.izquierda = pulsada;
            return true;
        case "KeyD":
        case "ArrowRight":
            accion.derecha = pulsada;
            return true;
        default:
            return false;
    }
}

// El ratón solo apunta y dispara cuando está dentro del canvas.
canvas.addEventListener("pointermove", evento => {
    if (evento.pointerType === "mouse") {
        apuntarConPuntero(evento);
    }
});

canvas.addEventListener("pointerdown", evento => {
    if (evento.pointerType !== "mouse" || evento.button !== 0) {
        return;
    }

    evento.preventDefault();
    apuntarConPuntero(evento);
    accion.disparar = true;
});

window.addEventListener("pointerup", evento => {
    if (evento.pointerType === "mouse") {
        accion.disparar = false;
    }
});

canvas.addEventListener("contextmenu", evento => {
    evento.preventDefault();
});

function apuntarConPuntero(evento) {
    const jugador = obtenerMiJugador();

    if (!jugador) {
        return;
    }

    // Se convierten las coordenadas CSS a las coordenadas 960 x 540.
    const rectangulo = canvas.getBoundingClientRect();
    const ratonX =
        (evento.clientX - rectangulo.left)
        * canvas.width / rectangulo.width;
    const ratonY =
        (evento.clientY - rectangulo.top)
        * canvas.height / rectangulo.height;

    accion.angulo = Math.atan2(
        ratonY - jugador.y,
        ratonX - jugador.x);
}

// ---------------------------------------------------------------------------
// Controles táctiles en toda la pantalla
// ---------------------------------------------------------------------------

document.addEventListener("pointerdown", evento => {
    if (evento.pointerType === "mouse"
        || esElementoInterfaz(evento.target)) {
        return;
    }

    evento.preventDefault();

    // Cada mitad puede tener su propio dedo y su propio joystick.
    if (evento.clientX < window.innerWidth / 2) {
        if (punteroMovimiento === null) {
            iniciarJoystick(evento);
        }
    } else if (punteroDisparo === null) {
        iniciarDisparoTactil(evento);
    }
}, { passive: false });

document.addEventListener("pointermove", evento => {
    if (evento.pointerId === punteroMovimiento) {
        evento.preventDefault();
        moverJoystickMovimiento(evento);
    }

    if (evento.pointerId === punteroDisparo) {
        evento.preventDefault();
        moverJoystickDisparo(evento);
    }
}, { passive: false });

document.addEventListener("pointerup", finalizarPuntero);
document.addEventListener("pointercancel", finalizarPuntero);

function esElementoInterfaz(elemento) {
    if (!(elemento instanceof Element)) {
        return false;
    }

    // Estos elementos deben poder pulsarse sin activar el juego.
    return elemento.closest(
        "button, a, input, select, .modal, .swal2-container") !== null;
}

function iniciarJoystick(evento) {
    punteroMovimiento = evento.pointerId;
    centroMovimiento = {
        x: evento.clientX,
        y: evento.clientY
    };

    joystickMovimiento.style.left = `${evento.clientX}px`;
    joystickMovimiento.style.top = `${evento.clientY}px`;
    joystickMovimiento.classList.add("visible");

    moverJoystickMovimiento(evento);
}

function moverJoystickMovimiento(evento) {
    let diferenciaX = evento.clientX - centroMovimiento.x;
    let diferenciaY = evento.clientY - centroMovimiento.y;
    const distancia = Math.hypot(diferenciaX, diferenciaY);

    // La palanca no puede salir del círculo exterior.
    if (distancia > RADIO_JOYSTICK) {
        diferenciaX =
            diferenciaX / distancia * RADIO_JOYSTICK;
        diferenciaY =
            diferenciaY / distancia * RADIO_JOYSTICK;
    }

    palancaMovimiento.style.transform =
        `translate(${diferenciaX}px, ${diferenciaY}px)`;

    // La zona muerta evita movimientos al apoyar el dedo sin arrastrar.
    accion.izquierda =
        diferenciaX < -ZONA_MUERTA_JOYSTICK;
    accion.derecha =
        diferenciaX > ZONA_MUERTA_JOYSTICK;
    accion.arriba =
        diferenciaY < -ZONA_MUERTA_JOYSTICK;
    accion.abajo =
        diferenciaY > ZONA_MUERTA_JOYSTICK;
}

function iniciarDisparoTactil(evento) {
    punteroDisparo = evento.pointerId;
    centroDisparo = {
        x: evento.clientX,
        y: evento.clientY
    };

    joystickDisparo.style.left = `${evento.clientX}px`;
    joystickDisparo.style.top = `${evento.clientY}px`;
    joystickDisparo.classList.add("visible");

    // Se empieza a disparar al apoyar el dedo. Si todavía no se arrastra,
    // se conserva la última dirección utilizada.
    accion.disparar = true;
    moverJoystickDisparo(evento);
}

function moverJoystickDisparo(evento) {
    let diferenciaX = evento.clientX - centroDisparo.x;
    let diferenciaY = evento.clientY - centroDisparo.y;
    const distancia = Math.hypot(diferenciaX, diferenciaY);

    // La palanca visual se limita al círculo, pero el ángulo utiliza la
    // dirección original del dedo y por eso sigue siendo preciso.
    if (distancia > RADIO_JOYSTICK) {
        diferenciaX =
            diferenciaX / distancia * RADIO_JOYSTICK;
        diferenciaY =
            diferenciaY / distancia * RADIO_JOYSTICK;
    }

    palancaDisparo.style.transform =
        `translate(${diferenciaX}px, ${diferenciaY}px)`;

    // Al superar la zona muerta se cambia la dirección del disparo.
    if (distancia > ZONA_MUERTA_JOYSTICK) {
        accion.angulo = Math.atan2(
            evento.clientY - centroDisparo.y,
            evento.clientX - centroDisparo.x);
    }
}

function finalizarPuntero(evento) {
    if (evento.pointerId === punteroMovimiento) {
        terminarMovimiento();
    }

    if (evento.pointerId === punteroDisparo) {
        terminarDisparo();
    }
}

function terminarMovimiento() {
    punteroMovimiento = null;
    joystickMovimiento.classList.remove("visible");
    palancaMovimiento.style.transform = "translate(0, 0)";

    accion.arriba = false;
    accion.abajo = false;
    accion.izquierda = false;
    accion.derecha = false;
}

function terminarDisparo() {
    punteroDisparo = null;
    joystickDisparo.classList.remove("visible");
    palancaDisparo.style.transform = "translate(0, 0)";
    accion.disparar = false;
}

function limpiarControles() {
    terminarMovimiento();
    terminarDisparo();
}

// Al perder el foco no debe quedar ninguna tecla o pulsación activa.
window.addEventListener("blur", limpiarControles);

document.addEventListener("visibilitychange", () => {
    if (document.hidden) {
        limpiarControles();
    }
});

// El modal de Bootstrap tampoco debe permitir movimientos de fondo.
modalEstadisticas.addEventListener(
    "show.bs.modal",
    limpiarControles);

// ---------------------------------------------------------------------------
// Pantalla completa y salida
// ---------------------------------------------------------------------------

// Algunos navegadores no permiten poner un elemento en pantalla completa.
if (!document.fullscreenEnabled) {
    botonPantallaCompleta.hidden = true;
}

botonPantallaCompleta.addEventListener(
    "click",
    cambiarPantallaCompleta);

document.addEventListener(
    "fullscreenchange",
    actualizarBotonPantallaCompleta);

async function cambiarPantallaCompleta() {
    try {
        if (document.fullscreenElement) {
            await document.exitFullscreen();
        } else {
            // Se utiliza todo el documento para que los modales sigan visibles.
            await document.documentElement.requestFullscreen();
        }

        // Algunos móviles tardan en actualizar los estilos de :fullscreen.
        // La clase deja el estado visual explícitamente sincronizado.
        actualizarBotonPantallaCompleta();
    } catch (error) {
        console.error(error);
    }
}

function actualizarBotonPantallaCompleta() {
    const activada = document.fullscreenElement !== null;

    // Al salir se elimina la clase y el HUD vuelve a mostrarse.
    document.body.classList.toggle(
        "pantalla-completa",
        activada);

    iconoPantallaCompleta.className = activada
        ? "bi bi-fullscreen-exit"
        : "bi bi-fullscreen";

    botonPantallaCompleta.title = activada
        ? "Salir de pantalla completa"
        : "Pantalla completa";

    botonPantallaCompleta.setAttribute(
        "aria-label",
        botonPantallaCompleta.title);
}

formularioSalir.addEventListener("submit", async evento => {
    if (salidaConfirmada) {
        return;
    }

    evento.preventDefault();
    limpiarControles();

    const resultado = await Swal.fire({
        title: "¿Salir de la partida?",
        text: "Abandonarás la ronda actual.",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Sí, salir",
        cancelButtonText: "Cancelar",
        confirmButtonColor: "#dc3545",
        focusCancel: true
    });

    if (resultado.isConfirmed) {
        salidaConfirmada = true;

        // submit envía el formulario Razor sin volver a lanzar este evento.
        formularioSalir.submit();
    }
});

// ---------------------------------------------------------------------------
// Dibujo del tablero
// ---------------------------------------------------------------------------

function dibujar() {
    dibujarSuelo();

    if (estado) {
        dibujarMuros();
        dibujarPowerUps();
        dibujarProyectiles();
        dibujarJugadores();
        dibujarAvisos();
    }

    requestAnimationFrame(dibujar);
}

function dibujarSuelo() {
    contexto.fillStyle = paletaTablero.suelo;
    contexto.fillRect(0, 0, canvas.width, canvas.height);

    contexto.strokeStyle = paletaTablero.cuadricula;
    contexto.lineWidth = 1;

    // Las líneas forman una cuadrícula decorativa de 40 píxeles.
    for (let x = 0; x <= canvas.width; x += 40) {
        contexto.beginPath();
        contexto.moveTo(x, 0);
        contexto.lineTo(x, canvas.height);
        contexto.stroke();
    }

    for (let y = 0; y <= canvas.height; y += 40) {
        contexto.beginPath();
        contexto.moveTo(0, y);
        contexto.lineTo(canvas.width, y);
        contexto.stroke();
    }
}

function dibujarMuros() {
    // Los muros se buscan en los mapas que Razor envió al abrir la página.
    const mapa = mapas.find(
        candidato => candidato.nombre === estado.nombreMapa);

    if (!mapa) {
        return;
    }

    contexto.fillStyle = paletaTablero.muro;
    contexto.strokeStyle = paletaTablero.bordeMuro;
    contexto.lineWidth = 3;

    for (let fila = 0; fila < mapa.filas.length; fila++) {
        for (let columna = 0;
             columna < mapa.filas[fila].length;
             columna++) {
            if (mapa.filas[fila][columna] !== "#") {
                continue;
            }

            const x = columna * TAMANO_CASILLA;
            const y = fila * TAMANO_CASILLA;

            contexto.fillRect(
                x, y, TAMANO_CASILLA, TAMANO_CASILLA);
            contexto.strokeRect(
                x, y, TAMANO_CASILLA, TAMANO_CASILLA);
        }
    }
}

function dibujarPowerUps() {
    for (const powerUp of estado.powerUps) {
        const [color, letra] = DATOS_POWERUPS[powerUp.tipo];

        contexto.beginPath();
        contexto.arc(powerUp.x, powerUp.y, 13, 0, Math.PI * 2);
        contexto.fillStyle = color;
        contexto.fill();

        contexto.fillStyle = "#111";
        contexto.font = "bold 14px sans-serif";
        contexto.textAlign = "center";
        contexto.textBaseline = "middle";
        contexto.fillText(letra, powerUp.x, powerUp.y);
    }
}

function dibujarProyectiles() {
    contexto.fillStyle = paletaTablero.proyectil;

    for (const proyectil of estado.proyectiles) {
        contexto.beginPath();
        contexto.arc(proyectil.x, proyectil.y, 4, 0, Math.PI * 2);
        contexto.fill();
    }
}

function dibujarJugadores() {
    for (const jugador of estado.jugadores) {
        if (!jugador.vivo) {
            dibujarJugadorEliminado(jugador);
            continue;
        }

        if (jugador.tieneEscudo) {
            contexto.beginPath();
            contexto.arc(jugador.x, jugador.y, 22, 0, Math.PI * 2);
            contexto.strokeStyle = "#74c0fc";
            contexto.lineWidth = 4;
            contexto.stroke();
        }

        // Esta línea indica hacia dónde está apuntando.
        contexto.beginPath();
        contexto.moveTo(jugador.x, jugador.y);
        contexto.lineTo(
            jugador.x + Math.cos(jugador.angulo) * 26,
            jugador.y + Math.sin(jugador.angulo) * 26);
        contexto.strokeStyle = paletaTablero.puntero;
        contexto.lineWidth = 5;
        contexto.stroke();

        contexto.beginPath();
        contexto.arc(jugador.x, jugador.y, 16, 0, Math.PI * 2);
        contexto.fillStyle = jugador.color;
        contexto.fill();

        // Un borde contrastado permite reconocer al jugador propio.
        if (jugador.id === jugadorId) {
            contexto.strokeStyle = paletaTablero.puntero;
            contexto.lineWidth = 2;
            contexto.stroke();
        }

        dibujarNombreYVida(jugador);
    }
}

function dibujarNombreYVida(jugador) {
    contexto.textAlign = "center";
    contexto.textBaseline = "bottom";
    contexto.font = "13px sans-serif";
    contexto.fillStyle = paletaTablero.texto;
    contexto.fillText(
        jugador.nombre,
        jugador.x,
        jugador.y - 24);

    contexto.fillStyle = "#40252a";
    contexto.fillRect(
        jugador.x - 20,
        jugador.y + 22,
        40,
        5);

    contexto.fillStyle = "#51cf66";
    contexto.fillRect(
        jugador.x - 20,
        jugador.y + 22,
        40 * jugador.vida / 100,
        5);
}

function dibujarJugadorEliminado(jugador) {
    contexto.strokeStyle = "#868e96";
    contexto.lineWidth = 4;

    contexto.beginPath();
    contexto.moveTo(jugador.x - 10, jugador.y - 10);
    contexto.lineTo(jugador.x + 10, jugador.y + 10);
    contexto.moveTo(jugador.x + 10, jugador.y - 10);
    contexto.lineTo(jugador.x - 10, jugador.y + 10);
    contexto.stroke();
}

function dibujarAvisos() {
    if (estado.estado === "Esperando") {
        dibujarAvisoEspera();
    }

    if (estado.estado === "Finalizada") {
        const titulo = estado.ganador
            ? `Ha ganado ${estado.ganador}`
            : "Partida finalizada";

        dibujarMensaje(
            titulo,
            `Nueva ronda en ${estado.segundosParaReiniciar} segundos`);
    }

    const jugador = obtenerMiJugador();

    if (jugador
        && !jugador.vivo
        && estado.estado === "EnJuego") {
        dibujarMensaje(
            "Has sido eliminado",
            "Espera al final de la partida");
    }
}

function dibujarAvisoEspera() {
    // El aviso ocupa poco espacio para no tapar la zona de práctica.
    contexto.fillStyle = "rgba(0, 0, 0, 0.68)";
    contexto.fillRect(245, 12, 470, 50);

    contexto.textAlign = "center";
    contexto.textBaseline = "middle";
    contexto.fillStyle = "#ffffff";
    contexto.font = "16px sans-serif";
    contexto.fillText(
        "Esperando a más jugadores · Puedes practicar",
        480,
        37);
}

function dibujarMensaje(titulo, subtitulo) {
    contexto.fillStyle = "rgba(0, 0, 0, 0.72)";
    contexto.fillRect(210, 205, 540, 130);

    contexto.textAlign = "center";
    contexto.textBaseline = "middle";
    contexto.fillStyle = "#ffffff";
    contexto.font = "bold 30px sans-serif";
    contexto.fillText(titulo, 480, 250);

    contexto.fillStyle = "#ced4da";
    contexto.font = "17px sans-serif";
    contexto.fillText(subtitulo, 480, 295);
}

// ---------------------------------------------------------------------------
// Actualización del HUD y las estadísticas
// ---------------------------------------------------------------------------

function actualizarInterfaz() {
    estadoPartida.textContent =
        traducirEstado(estado.estado);
    numeroRonda.textContent =
        estado.numeroRonda;
    nombreMapa.textContent =
        estado.nombreMapa;

    if (estado.estado === "EnJuego") {
        tiempoPartida.textContent =
            `${estado.segundosRestantes} s`;
    } else if (estado.estado === "Finalizada") {
        tiempoPartida.textContent =
            `${estado.segundosParaReiniciar} s`;
    } else {
        tiempoPartida.textContent = "--";
    }

    const vivos = estado.jugadores.filter(
        jugador => jugador.vivo).length;

    numeroJugadores.textContent =
        `${vivos}/${estado.jugadores.length}`;

    const jugadoresOrdenados = [...estado.jugadores].sort(
        (primero, segundo) =>
            Number(segundo.vivo) - Number(primero.vivo)
            || segundo.victorias - primero.victorias
            || segundo.eliminaciones - primero.eliminaciones
            || primero.nombre.localeCompare(
                segundo.nombre, "es"));

    actualizarListaJugadores(
        listaJugadoresEscritorio,
        jugadoresOrdenados);
    actualizarListaJugadores(
        listaJugadoresModal,
        jugadoresOrdenados);
}

function actualizarListaJugadores(contenedor, jugadores) {
    contenedor.replaceChildren();

    for (const jugador of jugadores) {
        const fila = document.createElement("div");
        fila.className =
            "list-group-item px-3 py-2"
            + (jugador.id === jugadorId
                ? " list-group-item-primary"
                : "");

        const cabecera = document.createElement("div");
        cabecera.className =
            "d-flex align-items-center justify-content-between gap-2";

        const nombre = document.createElement("span");
        nombre.className = "fw-semibold text-truncate";

        const punto = document.createElement("span");
        punto.className =
            "d-inline-block rounded-circle me-2";
        punto.style.width = "0.75rem";
        punto.style.height = "0.75rem";
        punto.style.backgroundColor = jugador.color;

        const textoNombre = document.createElement("span");
        textoNombre.textContent =
            jugador.nombre
            + (jugador.id === jugadorId ? " (tú)" : "");

        nombre.append(punto, textoNombre);

        const estadoJugador = document.createElement("span");
        estadoJugador.className = jugador.vivo
            ? "badge text-bg-success"
            : "badge text-bg-secondary";
        estadoJugador.textContent = jugador.vivo
            ? `${jugador.vida} vida`
            : "Eliminado";

        cabecera.append(nombre, estadoJugador);

        const estadisticas = document.createElement("small");
        estadisticas.className =
            "d-block text-body-secondary mt-1";
        estadisticas.textContent =
            `${jugador.victorias} victorias · `
            + `${jugador.eliminaciones} eliminaciones`;

        fila.append(cabecera, estadisticas);
        contenedor.appendChild(fila);
    }
}

function traducirEstado(valor) {
    const traducciones = {
        Esperando: "Esperando",
        EnJuego: "En juego",
        Finalizada: "Finalizada"
    };

    return traducciones[valor] ?? valor;
}

function obtenerMiJugador() {
    return estado?.jugadores.find(
        jugador => jugador.id === jugadorId);
}
