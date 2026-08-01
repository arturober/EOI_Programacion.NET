// Claves propias para no mezclar las preferencias con otras aplicaciones.
const CLAVE_TEMA = "trivial-cliente-javascript-tema";
const TEMA_PREDETERMINADO = "bootstrap-light";

// Bootswatch no indica en el nombre de la hoja si el tema es oscuro.
const TEMAS_OSCUROS = new Set([
    "bootstrap-dark",
    "bootswatch-cyborg",
    "bootswatch-darkly",
    "bootswatch-quartz",
    "bootswatch-slate",
    "bootswatch-solar",
    "bootswatch-superhero",
    "bootswatch-vapor"
]);

const selectorTema = document.getElementById("selectorTema");
const hojaTema = document.getElementById("temaCss");

// Cambia la hoja de estilos y comunica a Bootstrap si el tema es oscuro.
function aplicarTema(nombreTema) {
    const esBootswatch = nombreTema.startsWith("bootswatch-");
    const nombreBootswatch = nombreTema.replace("bootswatch-", "");

    hojaTema.href = esBootswatch
        ? `https://cdn.jsdelivr.net/npm/bootswatch@5.3.8/dist/${nombreBootswatch}/bootstrap.min.css`
        : "https://cdn.jsdelivr.net/npm/bootstrap@5.3.8/dist/css/bootstrap.min.css";

    document.documentElement.dataset.bsTheme = TEMAS_OSCUROS.has(nombreTema)
        ? "dark"
        : "light";

    localStorage.setItem(CLAVE_TEMA, nombreTema);
}

// Ignora un valor antiguo si ya no existe en el selector.
const temaGuardado = localStorage.getItem(CLAVE_TEMA);
const temaInicial = [...selectorTema.options]
    .some(opcion => opcion.value === temaGuardado)
    ? temaGuardado
    : TEMA_PREDETERMINADO;

selectorTema.value = temaInicial;
aplicarTema(temaInicial);

selectorTema.addEventListener("change", evento => {
    aplicarTema(evento.target.value);
});
