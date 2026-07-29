// Relaciona el valor del selector con una hoja servida desde CDN.
const temas = {
    "bootstrap-light":
        "https://cdn.jsdelivr.net/npm/bootstrap@5.3.8/dist/css/bootstrap.min.css",
    "bootstrap-dark":
        "https://cdn.jsdelivr.net/npm/bootstrap@5.3.8/dist/css/bootstrap.min.css",
    "bootswatch-brite":
        "https://cdn.jsdelivr.net/npm/bootswatch@5.3.8/dist/brite/bootstrap.min.css",
    "bootswatch-cerulean":
        "https://cdn.jsdelivr.net/npm/bootswatch@5.3.8/dist/cerulean/bootstrap.min.css",
    "bootswatch-cosmo":
        "https://cdn.jsdelivr.net/npm/bootswatch@5.3.8/dist/cosmo/bootstrap.min.css",
    "bootswatch-cyborg":
        "https://cdn.jsdelivr.net/npm/bootswatch@5.3.8/dist/cyborg/bootstrap.min.css",
    "bootswatch-darkly":
        "https://cdn.jsdelivr.net/npm/bootswatch@5.3.8/dist/darkly/bootstrap.min.css",
    "bootswatch-flatly":
        "https://cdn.jsdelivr.net/npm/bootswatch@5.3.8/dist/flatly/bootstrap.min.css",
    "bootswatch-journal":
        "https://cdn.jsdelivr.net/npm/bootswatch@5.3.8/dist/journal/bootstrap.min.css",
    "bootswatch-litera":
        "https://cdn.jsdelivr.net/npm/bootswatch@5.3.8/dist/litera/bootstrap.min.css",
    "bootswatch-lumen":
        "https://cdn.jsdelivr.net/npm/bootswatch@5.3.8/dist/lumen/bootstrap.min.css",
    "bootswatch-lux":
        "https://cdn.jsdelivr.net/npm/bootswatch@5.3.8/dist/lux/bootstrap.min.css",
    "bootswatch-materia":
        "https://cdn.jsdelivr.net/npm/bootswatch@5.3.8/dist/materia/bootstrap.min.css",
    "bootswatch-minty":
        "https://cdn.jsdelivr.net/npm/bootswatch@5.3.8/dist/minty/bootstrap.min.css",
    "bootswatch-morph":
        "https://cdn.jsdelivr.net/npm/bootswatch@5.3.8/dist/morph/bootstrap.min.css",
    "bootswatch-pulse":
        "https://cdn.jsdelivr.net/npm/bootswatch@5.3.8/dist/pulse/bootstrap.min.css",
    "bootswatch-quartz":
        "https://cdn.jsdelivr.net/npm/bootswatch@5.3.8/dist/quartz/bootstrap.min.css",
    "bootswatch-sandstone":
        "https://cdn.jsdelivr.net/npm/bootswatch@5.3.8/dist/sandstone/bootstrap.min.css",
    "bootswatch-simplex":
        "https://cdn.jsdelivr.net/npm/bootswatch@5.3.8/dist/simplex/bootstrap.min.css",
    "bootswatch-sketchy":
        "https://cdn.jsdelivr.net/npm/bootswatch@5.3.8/dist/sketchy/bootstrap.min.css",
    "bootswatch-slate":
        "https://cdn.jsdelivr.net/npm/bootswatch@5.3.8/dist/slate/bootstrap.min.css",
    "bootswatch-solar":
        "https://cdn.jsdelivr.net/npm/bootswatch@5.3.8/dist/solar/bootstrap.min.css",
    "bootswatch-spacelab":
        "https://cdn.jsdelivr.net/npm/bootswatch@5.3.8/dist/spacelab/bootstrap.min.css",
    "bootswatch-superhero":
        "https://cdn.jsdelivr.net/npm/bootswatch@5.3.8/dist/superhero/bootstrap.min.css",
    "bootswatch-united":
        "https://cdn.jsdelivr.net/npm/bootswatch@5.3.8/dist/united/bootstrap.min.css",
    "bootswatch-vapor":
        "https://cdn.jsdelivr.net/npm/bootswatch@5.3.8/dist/vapor/bootstrap.min.css",
    "bootswatch-yeti":
        "https://cdn.jsdelivr.net/npm/bootswatch@5.3.8/dist/yeti/bootstrap.min.css",
    "bootswatch-zephyr":
        "https://cdn.jsdelivr.net/npm/bootswatch@5.3.8/dist/zephyr/bootstrap.min.css"
};

// Estos temas necesitan que Bootstrap trate también el documento como oscuro.
const temasOscuros = new Set([
    "bootstrap-dark",
    "bootswatch-cyborg",
    "bootswatch-darkly",
    "bootswatch-quartz",
    "bootswatch-slate",
    "bootswatch-solar",
    "bootswatch-superhero",
    "bootswatch-vapor"
]);

const selectorTema = document.querySelector("#selectorTema");
const hojaTema = document.querySelector("#temaCss");
const claveAlmacenamiento = "peliculas-tema";

// Aplica el tema y lo conserva para las siguientes visitas.
function aplicarTema(nombreTema) {
    const temaValido = temas[nombreTema] ? nombreTema : "bootstrap-light";

    hojaTema.href = temas[temaValido];
    selectorTema.value = temaValido;
    document.documentElement.dataset.bsTheme =
        temasOscuros.has(temaValido) ? "dark" : "light";
    localStorage.setItem(claveAlmacenamiento, temaValido);
}

if (selectorTema && hojaTema) {
    aplicarTema(localStorage.getItem(claveAlmacenamiento));

    selectorTema.addEventListener("change", () => {
        aplicarTema(selectorTema.value);
    });
}
