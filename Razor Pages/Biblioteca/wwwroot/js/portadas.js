// Sustituye las portadas inexistentes por una imagen local legible.
document.addEventListener("DOMContentLoaded", () => {
    document.querySelectorAll("img[data-portada]").forEach((imagen) => {
        imagen.addEventListener("error", () => {
            imagen.removeAttribute("data-portada");
            imagen.src = "/img/portada-no-disponible.svg";
            imagen.alt = "Portada no disponible";
        }, { once: true });
    });
});
