// La dirección y la partida se guardan únicamente en este cliente.
const CLAVE_URL_SERVIDOR = "trivial-cliente-javascript-url";
const NUMERO_PREGUNTAS = 10;

let urlServidor = "";
let preguntas = [];
let posicion = 0;
let aciertos = 0;

// Referencias a los elementos que se utilizan varias veces.
const formularioConexion = document.getElementById("formularioConexion");
const campoUrlServidor = document.getElementById("urlServidor");
const botonConectar = document.getElementById("conectar");
const iconoConectar = document.getElementById("iconoConectar");
const textoConectar = document.getElementById("textoConectar");
const estadoConexion = document.getElementById("estadoConexion");
const inicio = document.getElementById("inicio");
const formularioPartida = document.getElementById("formularioPartida");
const selectorCategorias = document.getElementById("categoria");
const botonEmpezar = document.getElementById("empezar");
const juego = document.getElementById("juego");
const progreso = document.getElementById("progreso");
const puntuacion = document.getElementById("puntuacion");
const barraProgreso = document.getElementById("barraProgreso");
const nombreCategoria = document.getElementById("nombreCategoria");
const enunciado = document.getElementById("enunciado");
const respuestas = document.getElementById("respuestas");

// Recupera la última dirección utilizada sin conectar automáticamente.
campoUrlServidor.value = localStorage.getItem(CLAVE_URL_SERVIDOR)
    || campoUrlServidor.value;

// SweetAlert utiliza los mismos colores que el tema activo.
function mostrarAlerta(opciones) {
    const estilos = getComputedStyle(document.body);

    return Swal.fire({
        background: estilos.getPropertyValue("--bs-body-bg"),
        color: estilos.getPropertyValue("--bs-body-color"),
        confirmButtonText: "Aceptar",
        ...opciones
    });
}

// Comprueba la dirección y elimina la barra final para construir rutas uniformes.
function normalizarUrl(valor) {
    const url = new URL(valor.trim());

    if (url.protocol !== "http:" && url.protocol !== "https:") {
        throw new Error("La dirección debe comenzar por http:// o https://.");
    }

    return url.href.replace(/\/$/, "");
}

// Centraliza las peticiones y convierte las respuestas correctas a JSON.
async function obtenerJson(ruta) {
    const respuesta = await fetch(`${urlServidor}/api/${ruta}`, {
        headers: { "Accept": "application/json" },
        cache: "no-store"
    });

    if (!respuesta.ok) {
        throw new Error(
            `El servidor ha respondido con el código ${respuesta.status}.`
        );
    }

    return respuesta.json();
}

// Evita que se envíe dos veces el formulario durante una petición.
function cambiarEstadoConexion(conectando) {
    botonConectar.disabled = conectando;
    campoUrlServidor.disabled = conectando;
    iconoConectar.className = conectando
        ? "bi bi-hourglass-split"
        : "bi bi-plug";
    textoConectar.textContent = conectando ? "Conectando..." : "Conectar";
}

// Sustituye las categorías antiguas por las recibidas del servidor elegido.
function mostrarCategorias(categorias) {
    selectorCategorias.replaceChildren();

    const todas = document.createElement("option");
    todas.value = "";
    todas.textContent = "Todas las categorías";
    selectorCategorias.appendChild(todas);

    categorias.forEach(categoria => {
        const opcion = document.createElement("option");
        opcion.value = categoria.id;
        opcion.textContent = categoria.nombre;
        selectorCategorias.appendChild(opcion);
    });
}

// Prueba el endpoint de categorías antes de permitir iniciar una partida.
async function conectar(evento) {
    evento.preventDefault();
    cambiarEstadoConexion(true);
    estadoConexion.classList.add("d-none");
    inicio.classList.add("d-none");
    juego.classList.add("d-none");

    try {
        urlServidor = normalizarUrl(campoUrlServidor.value);
        const categorias = await obtenerJson("categorias");

        mostrarCategorias(categorias);
        localStorage.setItem(CLAVE_URL_SERVIDOR, urlServidor);

        estadoConexion.textContent = `Conectado con ${urlServidor}`;
        estadoConexion.classList.remove("d-none");
        inicio.classList.remove("d-none");
    }
    catch (error) {
        await mostrarAlerta({
            title: "No se ha podido conectar",
            text: `${error.message} Comprueba la dirección, la API y la configuración de CORS.`,
            icon: "error"
        });
    }
    finally {
        cambiarEstadoConexion(false);
    }
}

// Solicita diez preguntas, con filtro solo si se ha elegido una categoría.
async function empezarPartida(evento) {
    evento.preventDefault();
    botonEmpezar.disabled = true;

    const categoriaId = selectorCategorias.value;
    const filtro = categoriaId
        ? `&categoriaId=${encodeURIComponent(categoriaId)}`
        : "";

    try {
        preguntas = await obtenerJson(
            `preguntas?cantidad=${NUMERO_PREGUNTAS}${filtro}`
        );

        if (preguntas.length === 0) {
            await mostrarAlerta({
                title: "Categoría sin preguntas",
                text: "Elige otra categoría para comenzar la partida.",
                icon: "warning"
            });
            return;
        }

        posicion = 0;
        aciertos = 0;
        inicio.classList.add("d-none");
        juego.classList.remove("d-none");
        mostrarPregunta();
    }
    catch (error) {
        await mostrarAlerta({
            title: "No se han podido cargar las preguntas",
            text: error.message,
            icon: "error"
        });
    }
    finally {
        botonEmpezar.disabled = false;
    }
}

// Construye los botones de respuesta sin introducir HTML recibido de la API.
function mostrarPregunta() {
    const pregunta = preguntas[posicion];
    const numeroActual = posicion + 1;
    const porcentaje = numeroActual / preguntas.length * 100;

    progreso.textContent = `Pregunta ${numeroActual} de ${preguntas.length}`;
    puntuacion.textContent = `${aciertos} ${aciertos === 1 ? "acierto" : "aciertos"}`;
    barraProgreso.style.width = `${porcentaje}%`;
    barraProgreso.parentElement.setAttribute("aria-valuemax", preguntas.length);
    barraProgreso.parentElement.setAttribute("aria-valuenow", numeroActual);
    nombreCategoria.textContent = pregunta.categoria.nombre;
    enunciado.textContent = pregunta.enunciado;
    respuestas.replaceChildren();

    pregunta.respuestas.forEach((respuesta, indice) => {
        const boton = document.createElement("button");
        boton.type = "button";
        boton.className = "btn btn-outline-primary text-start p-3";
        boton.textContent = respuesta;
        boton.addEventListener("click", () => responder(indice + 1));
        respuestas.appendChild(boton);
    });
}

// Desactiva las respuestas mientras SweetAlert muestra el resultado.
function desactivarRespuestas() {
    respuestas.querySelectorAll("button").forEach(boton => {
        boton.disabled = true;
    });
}

// Vuelve a la selección de categoría al terminar la partida.
function prepararOtraPartida() {
    juego.classList.add("d-none");
    inicio.classList.remove("d-none");
    selectorCategorias.focus();
}

// Comprueba la opción elegida y avanza o finaliza la partida.
async function responder(numeroRespuesta) {
    desactivarRespuestas();

    const pregunta = preguntas[posicion];
    const esCorrecta = numeroRespuesta === pregunta.respuestaCorrecta;

    if (esCorrecta) {
        aciertos++;
    }

    const respuestaCorrecta = pregunta.respuestas[
        pregunta.respuestaCorrecta - 1
    ];

    await mostrarAlerta({
        title: esCorrecta ? "¡Correcto!" : "Respuesta incorrecta",
        text: esCorrecta
            ? "Has sumado un acierto."
            : `La respuesta correcta era: ${respuestaCorrecta}`,
        icon: esCorrecta ? "success" : "error"
    });

    const esUltima = posicion === preguntas.length - 1;

    if (esUltima) {
        await mostrarAlerta({
            title: "Partida terminada",
            text: `Has acertado ${aciertos} de ${preguntas.length} preguntas.`,
            icon: "info",
            confirmButtonText: "Jugar otra vez"
        });

        prepararOtraPartida();
        return;
    }

    posicion++;
    mostrarPregunta();
}

formularioConexion.addEventListener("submit", conectar);
formularioPartida.addEventListener("submit", empezarPartida);
