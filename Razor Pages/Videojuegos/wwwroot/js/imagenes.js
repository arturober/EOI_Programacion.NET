// Sustituye las imágenes inexistentes por una ilustración local.
document.addEventListener("DOMContentLoaded", () => {
    document.querySelectorAll("img[data-imagen]").forEach((imagen) => {
        imagen.addEventListener("error", () => {
            imagen.removeAttribute("data-imagen");
            imagen.src = "/img/imagen-no-disponible.svg";
            imagen.alt = "Imagen no disponible";
            imagen.classList.remove("object-fit-cover");
            imagen.classList.add("object-fit-contain");
        }, { once: true });
    });
});
