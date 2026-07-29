// Esta clave identifica la preferencia dentro de localStorage.
const claveTema = "temaOpenWeather";

// Si todavía no existe una elección, se utiliza Bootstrap claro.
const temaGuardado =
    localStorage.getItem(claveTema) ?? "bootstrap-light";

// Bootswatch no indica automáticamente a Bootstrap que su paleta es oscura.
const temasOscuros = [
    "bootswatch-cyborg",
    "bootswatch-darkly",
    "bootswatch-quartz",
    "bootswatch-slate",
    "bootswatch-solar",
    "bootswatch-superhero",
    "bootswatch-vapor"
];

function cambiarTema(tema) {
    // El prefijo propio distingue Bootswatch de los dos temas base.
    const esBootswatch = tema.startsWith("bootswatch-");
    const nombre = tema.replace("bootswatch-", "");
    const hojaTema = document.getElementById("temaCss");

    // Se cambia solamente la dirección de la hoja; el HTML continúa igual.
    hojaTema.href = esBootswatch
        ? `https://cdn.jsdelivr.net/npm/bootswatch@5.3.8/dist/${nombre}/bootstrap.min.css`
        : "https://cdn.jsdelivr.net/npm/bootstrap@5.3.8/dist/css/bootstrap.min.css";

    // data-bs-theme adapta fondos, textos, tablas y controles a la paleta.
    const esOscuro =
        tema === "bootstrap-dark" || temasOscuros.includes(tema);
    document.documentElement.dataset.bsTheme =
        esOscuro ? "dark" : "light";

    // La elección se conserva aunque se cierre el navegador.
    localStorage.setItem(claveTema, tema);
}

const selectorTema = document.getElementById("selectorTema");

if (selectorTema) {
    // El control debe reflejar la elección guardada antes de escuchar cambios.
    selectorTema.value = temaGuardado;

    selectorTema.addEventListener("change", () => {
        cambiarTema(selectorTema.value);
    });
}

// El tema se aplica al terminar de cargar este archivo.
cambiarTema(temaGuardado);
