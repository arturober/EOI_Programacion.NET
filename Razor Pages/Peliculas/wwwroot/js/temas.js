// Recuperamos el tema guardado o utilizamos Bootstrap claro.
const temaGuardado =
    localStorage.getItem("temaPokedex") ?? "bootstrap-light";

// Esta función cambia la hoja de estilos y recuerda la elección.
function cambiarTema(tema) {
    const esBootswatch = tema.startsWith("bootswatch-");
    const nombreBootswatch = tema.replace("bootswatch-", "");

    document.getElementById("temaCss").href = esBootswatch
        ? `https://cdn.jsdelivr.net/npm/bootswatch@5.3.8/dist/${nombreBootswatch}/bootstrap.min.css`
        : "https://cdn.jsdelivr.net/npm/bootstrap@5.3.8/dist/css/bootstrap.min.css";

    document.documentElement.dataset.bsTheme =
        tema === "bootstrap-dark" ? "dark" : "light";

    localStorage.setItem("temaPokedex", tema);
}

// Marcamos en el desplegable el tema recuperado al abrir la página.
document.getElementById("selectorTema").value = temaGuardado;

// Lo aplicamos inmediatamente para reducir el cambio visual al cargar.
cambiarTema(temaGuardado);

