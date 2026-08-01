// Utilizamos una ruta relativa al mismo servidor que entrega este cliente.
const api = "/api";

// Estas variables representan el estado de la partida actual.
let preguntas = [];
let posicion = 0;
let aciertos = 0;

// Guardamos las referencias a elementos que utilizaremos varias veces.
const inicio = document.getElementById("inicio");
const juego = document.getElementById("juego");
const selectorCategoria = document.getElementById("categoria");
const progreso = document.getElementById("progreso");
const puntos = document.getElementById("puntos");
const nombreCategoria = document.getElementById("nombreCategoria");
const enunciado = document.getElementById("enunciado");
const respuestas = document.getElementById("respuestas");

async function obtenerJson(direccion) {
    // fetch realiza una petición GET y devuelve una promesa con la respuesta HTTP.
    const respuesta = await fetch(direccion);

    // fetch no lanza una excepción por respuestas 404 o 500, por eso
    // comprobamos expresamente la propiedad ok.
    if (!respuesta.ok) {
        throw new Error(`La petición ha fallado: ${respuesta.status}`);
    }

    // json convierte el texto recibido en objetos y arrays de JavaScript.
    return await respuesta.json();
}

async function cargarCategorias() {
    // La API devuelve un array de objetos con las propiedades id y nombre.
    const categorias = await obtenerJson(`${api}/categorias`);

    categorias.forEach(categoria => {
        // Creamos una option independiente para cada categoría recibida.
        const opcion = document.createElement("option");
        opcion.value = categoria.id;
        opcion.textContent = categoria.nombre;
        selectorCategoria.append(opcion);
    });
}

async function empezar() {
    // Cuando la cadena está vacía no incorporamos el filtro categoriaId.
    const categoria = selectorCategoria.value;
    const filtro = categoria
        ? `&categoriaId=${categoria}`
        : "";

    // Solicitamos como máximo diez preguntas aleatorias.
    preguntas = await obtenerJson(
        `${api}/preguntas?cantidad=10${filtro}`
    );

    // Una categoría recién creada puede no tener todavía preguntas.
    if (preguntas.length === 0) {
        await Swal.fire({
            icon: "info",
            title: "Categoría vacía",
            text: "Añade preguntas desde la aplicación Razor.",
            confirmButtonText: "Aceptar"
        });
        return;
    }

    // Reiniciamos el estado para que cada partida empiece desde cero.
    posicion = 0;
    aciertos = 0;

    // Ocultamos la selección y hacemos visible la tarjeta del juego.
    inicio.classList.add("d-none");
    juego.classList.remove("d-none");

    mostrarPregunta();
}

function mostrarPregunta() {
    // posicion siempre apunta a la pregunta que debe mostrarse ahora.
    const pregunta = preguntas[posicion];

    progreso.textContent =
        `Pregunta ${posicion + 1} de ${preguntas.length}`;
    puntos.textContent = `${aciertos} aciertos`;
    nombreCategoria.textContent = pregunta.categoria.nombre;
    enunciado.textContent = pregunta.enunciado;

    // Eliminamos los botones pertenecientes a la pregunta anterior.
    respuestas.replaceChildren();

    pregunta.respuestas.forEach((respuesta, indice) => {
        const boton = document.createElement("button");
        boton.type = "button";
        boton.className =
            "btn btn-outline-primary btn-lg text-start";
        boton.textContent = respuesta;

        // La API numera las respuestas del 1 al 4; los arrays empiezan en 0.
        boton.addEventListener(
            "click",
            () => responder(indice + 1)
        );

        respuestas.append(boton);
    });
}

async function responder(numero) {
    const pregunta = preguntas[posicion];
    const esCorrecta = numero === pregunta.respuestaCorrecta;

    if (esCorrecta) {
        aciertos++;
    }

    // Cuando se falla mostramos tanto el enunciado como la respuesta correcta.
    const respuestaCorrecta =
        pregunta.respuestas[pregunta.respuestaCorrecta - 1];

    await Swal.fire({
        icon: esCorrecta ? "success" : "error",
        title: esCorrecta
            ? "¡Correcto!"
            : "¡No es correcto!",
        text: esCorrecta
            ? undefined
            : `${pregunta.enunciado}: ${respuestaCorrecta}`,
        confirmButtonText: "Continuar"
    });

    // Avanzamos únicamente después de cerrar el mensaje.
    posicion++;

    if (posicion < preguntas.length) {
        mostrarPregunta();
        return;
    }

    // Si no quedan preguntas, presentamos el resultado de la partida.
    await Swal.fire({
        icon: "info",
        title: "Partida terminada",
        text: `Has conseguido ${aciertos} de ${preguntas.length} aciertos.`,
        confirmButtonText: "Volver a jugar"
    });

    // Regresamos a la selección de categoría.
    juego.classList.add("d-none");
    inicio.classList.remove("d-none");
}

// El botón inicia la función asíncrona. catch centraliza los errores inesperados.
document.getElementById("jugar").addEventListener("click", () => {
    empezar().catch(mostrarErrorConexion);
});

function mostrarErrorConexion() {
    Swal.fire({
        icon: "error",
        title: "No se puede conectar",
        text: "No se han podido obtener los datos de la API.",
        confirmButtonText: "Aceptar"
    });
}

// Cargamos las categorías nada más abrir la página.
cargarCategorias().catch(mostrarErrorConexion);
