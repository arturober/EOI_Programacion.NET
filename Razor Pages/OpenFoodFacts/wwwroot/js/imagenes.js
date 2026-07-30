// Sustituye una imagen externa por una ilustración incluida en el proyecto.
function sustituirImagen(imagen) {
    if (imagen.dataset.sustituida === "true") {
        return;
    }

    imagen.dataset.sustituida = "true";
    imagen.src = "/img/imagen-no-disponible.svg";
    imagen.classList.remove("object-fit-cover");
    imagen.classList.add("object-fit-contain", "p-3");
}

document.querySelectorAll("img[data-imagen]").forEach(function (imagen) {
    imagen.addEventListener("error", function () {
        sustituirImagen(imagen);
    });

    // También contempla imágenes que hayan fallado antes de cargar el script.
    if (imagen.complete && imagen.naturalWidth === 0) {
        sustituirImagen(imagen);
    }
});
