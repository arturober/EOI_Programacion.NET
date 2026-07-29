// Se guardan las referencias a los dos elementos que cambian de tema.
const selectorTema = document.getElementById("selectorTema");
const hojaTema = document.getElementById("hojaTema");

// Estas direcciones están fijadas a Bootstrap y Bootswatch 5.3.8.
const URL_BOOTSTRAP =
    "https://cdn.jsdelivr.net/npm/bootstrap@5.3.8/dist/css/bootstrap.min.css";
const URL_BOOTSWATCH =
    "https://cdn.jsdelivr.net/npm/bootswatch@5.3.8/dist";
const INTEGRIDAD_BOOTSTRAP =
    "sha384-sRIl4kxILFvY47J16cr9ZwB07vP4J8+LH7qKQnuqkuIAvNWLzeN8tE5YBujZqJLB";

// Estos temas utilizan principalmente fondos oscuros. Mantener una lista
// explícita es más sencillo de entender que analizar los colores del CSS.
const TEMAS_OSCUROS = new Set([
    "bootstrap-oscuro",
    "cyborg",
    "darkly",
    "quartz",
    "slate",
    "solar",
    "superhero",
    "vapor"
]);

// Si todavía no se ha elegido un tema, se utiliza Bootstrap oscuro.
const temaInicial =
    localStorage.getItem("tema") ?? "bootstrap-oscuro";

aplicarTema(temaInicial);

// Cada cambio del desplegable se aplica y se recuerda en el navegador.
selectorTema.addEventListener("change", () => {
    aplicarTema(selectorTema.value);
    localStorage.setItem("tema", selectorTema.value);
});

function aplicarTema(tema) {
    // Bootstrap claro y oscuro utilizan la misma hoja de estilos.
    if (tema === "bootstrap-claro" || tema === "bootstrap-oscuro") {
        hojaTema.integrity = INTEGRIDAD_BOOTSTRAP;
        hojaTema.crossOrigin = "anonymous";
        hojaTema.href = URL_BOOTSTRAP;
        document.documentElement.dataset.bsTheme =
            tema === "bootstrap-oscuro" ? "dark" : "light";
    } else {
        // Cada tema Bootswatch sustituye la hoja de estilos de Bootstrap.
        // Se quita integrity porque su hash pertenece al CSS de Bootstrap.
        hojaTema.removeAttribute("integrity");
        hojaTema.removeAttribute("crossorigin");
        hojaTema.href = `${URL_BOOTSWATCH}/${tema}/bootstrap.min.css`;

        // Bootswatch ya incluye sus colores claros u oscuros.
        document.documentElement.dataset.bsTheme = "light";
    }

    // El desplegable debe mostrar el tema que acaba de aplicarse.
    selectorTema.value = tema;

    // Un valor antiguo o incorrecto vuelve al tema predeterminado.
    if (selectorTema.selectedIndex === -1) {
        localStorage.setItem("tema", "bootstrap-oscuro");
        aplicarTema("bootstrap-oscuro");
        return;
    }

    // El canvas no hereda los colores de Bootstrap. Se le indica si debe
    // utilizar su paleta clara u oscura y se avisa al archivo del juego.
    document.documentElement.dataset.temaJuego =
        TEMAS_OSCUROS.has(tema) ? "oscuro" : "claro";

    document.dispatchEvent(new CustomEvent("temaCambiado"));
}
