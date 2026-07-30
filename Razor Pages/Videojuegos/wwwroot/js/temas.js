"use strict";

// Cambia entre Bootstrap y Bootswatch y recuerda la elección.
document.addEventListener("DOMContentLoaded", () => {
    const selector = document.getElementById("selectorTema");
    const hoja = document.getElementById("temaCss");

    if (!selector || !hoja) {
        return;
    }

    const temasOscuros = [
        "bootstrap-dark",
        "bootswatch-cyborg",
        "bootswatch-darkly",
        "bootswatch-quartz",
        "bootswatch-slate",
        "bootswatch-solar",
        "bootswatch-superhero",
        "bootswatch-vapor"
    ];

    const aplicarTema = (valor) => {
        if (valor.startsWith("bootstrap-")) {
            hoja.href =
                "https://cdn.jsdelivr.net/npm/bootstrap@5.3.8/dist/css/bootstrap.min.css";
        } else {
            const nombre = valor.replace("bootswatch-", "");
            hoja.href =
                `https://cdn.jsdelivr.net/npm/bootswatch@5.3.8/dist/${nombre}/bootstrap.min.css`;
        }

        // Esta propiedad adapta fondos, bordes y controles al tema oscuro.
        document.documentElement.dataset.bsTheme =
            temasOscuros.includes(valor) ? "dark" : "light";

        try {
            localStorage.setItem("tema-videojuegos", valor);
        } catch {
            // La página sigue funcionando si se bloquea el almacenamiento.
        }
    };

    let temaGuardado = "bootstrap-light";
    try {
        temaGuardado =
            localStorage.getItem("tema-videojuegos") || temaGuardado;
    } catch {
        // Se conserva Bootstrap claro como tema predeterminado.
    }

    const existe = Array.from(selector.options)
        .some((opcion) => opcion.value === temaGuardado);

    selector.value = existe ? temaGuardado : "bootstrap-light";
    aplicarTema(selector.value);

    selector.addEventListener("change", () => {
        aplicarTema(selector.value);
    });
});
