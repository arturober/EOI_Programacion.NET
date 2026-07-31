const api = "http://localhost:5000/api";

let preguntas = [];
let posicion = 0;
let aciertos = 0;

const selectorCategorias = document.getElementById("categoria");
const inicio = document.getElementById("inicio");
const juego = document.getElementById("juego");
const progreso = document.getElementById("progreso");
const enunciado = document.getElementById("enunciado");
const respuestas = document.getElementById("respuestas");

async function obtenerJson(direccion) {
    const respuesta = await fetch(`${api}/${direccion}`);

    if (!respuesta.ok) {
        throw new Error(`Error al obtener datos de ${direccion}`);
    }
    return await respuesta.json();
}

async function cargarCategorias() {
    const categorias = await obtenerJson("categorias");

    console.log(categorias);

    categorias.forEach(categoria => {
        const opcion = document.createElement("option");
        opcion.value = categoria.id;
        opcion.textContent = categoria.nombre;
        selectorCategorias.appendChild(opcion);
    });
}

async function empezar() {
    const categoriaId = selectorCategorias.value;
    // https://trivial.runasp.net/api/preguntas?categoriaId=1&cantidad=100
    preguntas = await obtenerJson(`preguntas?categoriaId=${categoriaId}&cantidad=10`);

    console.log(preguntas);

    inicio.classList.add("d-none");
    juego.classList.remove("d-none");

    posicion = 0;
    aciertos = 0;
    mostrarPregunta();
}

function mostrarPregunta() {
    const pregunta = preguntas[posicion];
    console.log(pregunta);

    progreso.textContent = `Pregunta ${posicion + 1} de ${preguntas.length}`;

    enunciado.textContent = pregunta.enunciado;

    respuestas.innerHTML = "";

    pregunta.respuestas.forEach((respuesta, indice) => {
        const boton = document.createElement("button");
        boton.classList.add("btn", "btn-outline-primary", "d-block", "w-100", "mb-2");
        boton.textContent = respuesta;
        boton.addEventListener("click", () => responder(indice + 1));
        respuestas.append(boton);
    });
}

async function responder(numero) {
    console.log(`Respuesta seleccionada: ${numero}`);

    const pregunta = preguntas[posicion];
    const esCorrecta = numero === pregunta.respuestaCorrecta;

    if (esCorrecta) {
        aciertos++;
        mensaje = "¡Correcto!";
    }
    else {
        mensaje = `La respuesta correcta a la pregunta '${pregunta.enunciado}' es: '${pregunta.respuestas[pregunta.respuestaCorrecta - 1]}'`;
    }

    await Swal.fire({
        title: esCorrecta ? "¡Correcto!" : "Incorrecto",
        text: esCorrecta ? null : mensaje,
        icon: esCorrecta ? "success" : "error"
    });

    if (posicion == preguntas.length-1) {
        await Swal.fire({
            title: "Juego terminado",
            text: `Has acertado ${aciertos} de ${preguntas.length} preguntas.`,
            icon: "info",
            confirmButtonText: "Aceptar"
        });

        juego.classList.add("d-none");
        inicio.classList.remove("d-none");
    }
    else {
        posicion++;
        mostrarPregunta();
    }
}

document.getElementById("empezar").addEventListener("click", empezar);

cargarCategorias().catch(error => {
    console.error("Error al cargar categorías:", error);
});