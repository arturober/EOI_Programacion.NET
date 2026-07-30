const api = "http://localhost:5000/api";

let preguntas = [];
let posicion = 0;
let aciertos = 0;

const selectorCategorias = document.getElementById("categoria");

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

cargarCategorias().catch(error => {
    console.error("Error al cargar categorías:", error);
});