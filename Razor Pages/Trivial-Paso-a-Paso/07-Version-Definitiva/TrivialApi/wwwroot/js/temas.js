// La clave es común para Razor Pages y para el cliente estático.
const claveTema = "temaTrivial";

// Si todavía no existe una elección, utilizamos Bootstrap claro.
const temaGuardado =
    localStorage.getItem(claveTema) ?? "bootstrap-light";

function cambiarTema(tema) {
    // Los valores de Bootswatch comienzan por un prefijo que no forma parte
    // del nombre de la carpeta utilizada en su CDN.
    const esBootswatch = tema.startsWith("bootswatch-");
    const nombre = tema.replace("bootswatch-", "");

    // Tanto el layout como el cliente incluyen un enlace con este identificador.
    const hojaTema = document.getElementById("temaCss");

    hojaTema.href = esBootswatch
        ? `https://cdn.jsdelivr.net/npm/bootswatch@5.3.8/dist/${nombre}/bootstrap.min.css`
        : "https://cdn.jsdelivr.net/npm/bootstrap@5.3.8/dist/css/bootstrap.min.css";

    // dataset.bsTheme representa el atributo HTML data-bs-theme.
    // Bootstrap consulta este atributo para elegir su paleta clara u oscura.
    document.documentElement.dataset.bsTheme =
        tema === "bootstrap-dark" ? "dark" : "light";

    // localStorage conserva la elección incluso después de cerrar el navegador.
    localStorage.setItem(claveTema, tema);
}

// El selector solo existe en Razor Pages. El cliente reutiliza el archivo,
// aplica el tema guardado y omite de forma segura este bloque.
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

