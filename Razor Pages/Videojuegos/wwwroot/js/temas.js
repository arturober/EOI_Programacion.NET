// La clave identifica la elección del tema de esta aplicación.
const claveTema = "temaVideojuegos";

// Si todavía no existe una elección, utilizamos Bootstrap claro.
const temaGuardado =
    localStorage.getItem(claveTema) ?? "bootstrap-light";

function cambiarTema(tema) {
    // Los valores de Bootswatch comienzan por un prefijo que no forma parte
    // del nombre de la carpeta utilizada en su CDN.
    const esBootswatch = tema.startsWith("bootswatch-");
    const nombre = tema.replace("bootswatch-", "");

    const hojaTema = document.getElementById("temaCss");

    hojaTema.href = esBootswatch
        ? `https://cdn.jsdelivr.net/npm/bootswatch@5.3.8/dist/${nombre}/bootstrap.min.css`
        : "https://cdn.jsdelivr.net/npm/bootstrap@5.3.8/dist/css/"
            + "bootstrap.min.css";

    // dataset.bsTheme representa el atributo HTML data-bs-theme.
    document.documentElement.dataset.bsTheme =
        tema === "bootstrap-dark" ? "dark" : "light";

    // localStorage conserva la elección después de cerrar el navegador.
    localStorage.setItem(claveTema, tema);
}

const selectorTema = document.getElementById("selectorTema");

if (selectorTema) {
    // Sincronizamos el control antes de escuchar nuevos cambios.
    selectorTema.value = temaGuardado;

    selectorTema.addEventListener("change", () => {
        cambiarTema(selectorTema.value);
    });
}

// Aplicamos siempre el tema al terminar de cargar el documento.
cambiarTema(temaGuardado);
