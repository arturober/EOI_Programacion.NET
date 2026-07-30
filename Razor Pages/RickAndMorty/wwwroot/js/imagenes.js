// Sustituye imágenes ausentes o dañadas por una ilustración local.
document.querySelectorAll("img[data-imagen]").forEach(function (imagen) {
    if (!imagen.getAttribute("src")) {
        imagen.src = "/img/imagen-no-disponible.svg";
    }

    imagen.addEventListener(
        "error",
        function () {
            if (!imagen.src.endsWith("imagen-no-disponible.svg")) {
                imagen.src = "/img/imagen-no-disponible.svg";
            }
        },
        { once: true });
});
