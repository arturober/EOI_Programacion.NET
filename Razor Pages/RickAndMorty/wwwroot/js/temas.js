const claveTema = "tema-rick-and-morty";
const selectorTema = document.getElementById("selectorTema");
const enlaceBootstrap = document.getElementById("temaCss");

// Cambia la hoja de estilos sin recargar la página.
function aplicarTema(tema) {
    if (!enlaceBootstrap) {
        return;
    }

    const esBootstrap = tema.startsWith("bootstrap-");
    const nombreBootswatch = tema.replace("bootswatch-", "");
    const direccion = esBootstrap
        ? "https://cdn.jsdelivr.net/npm/bootstrap@5.3.8/dist/css/bootstrap.min.css"
        : `https://cdn.jsdelivr.net/npm/bootswatch@5.3.8/dist/${nombreBootswatch}/bootstrap.min.css`;

    enlaceBootstrap.href = direccion;
    enlaceBootstrap.dataset.tema = tema;

    // Bootstrap ajusta componentes y textos al modo claro u oscuro.
    document.documentElement.setAttribute(
        "data-bs-theme",
        tema === "bootstrap-dark" ? "dark" : "light");
}

// Recupera la elección anterior o utiliza Bootstrap claro.
const temaGuardado = localStorage.getItem(claveTema)
    ?? "bootstrap-light";

aplicarTema(temaGuardado);

if (selectorTema) {
    selectorTema.value = temaGuardado;

    selectorTema.addEventListener("change", function () {
        aplicarTema(this.value);
        localStorage.setItem(claveTema, this.value);
    });
}
