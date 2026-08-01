// Localizamos una sola vez los tres elementos que necesita este comportamiento.
const formularioBusqueda = document.getElementById("formularioBusqueda");
const entradaBusqueda = document.getElementById("Busqueda");
const selectorCategoria = document.getElementById("CategoriaId");

// Esta variable guardará el identificador del temporizador activo.
let temporizadorBusqueda;

function buscarConRetraso() {
    // Si el usuario vuelve a escribir antes de 300 ms, cancelamos el envío anterior.
    clearTimeout(temporizadorBusqueda);

    // Programamos un nuevo envío cuando hayan pasado 300 ms sin cambios.
    // requestSubmit respeta el funcionamiento normal y la validación del formulario.
    temporizadorBusqueda = setTimeout(
        () => formularioBusqueda.requestSubmit(),
        300
    );
}

// input se produce cada vez que cambia el contenido del cuadro de búsqueda.
entradaBusqueda.addEventListener("input", buscarConRetraso);

// change se produce cuando se selecciona otra categoría.
selectorCategoria.addEventListener("change", buscarConRetraso);

// Cada búsqueda recarga la página. Si hay texto, recuperamos el foco y colocamos
// el cursor al final para que el usuario pueda continuar escribiendo con naturalidad.
if (entradaBusqueda.value) {
    entradaBusqueda.focus();
    entradaBusqueda.setSelectionRange(
        entradaBusqueda.value.length,
        entradaBusqueda.value.length
    );
}
