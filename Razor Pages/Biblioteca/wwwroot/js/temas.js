"use strict";

// Cambia entre Bootstrap y Bootswatch y recuerda la elección en el navegador.
document.addEventListener("DOMContentLoaded", () => {
    const selector = document.getElementById("selectorTema");
    const hoja = document.getElementById("temaCss");

    if (!selector || !hoja) {
        return;
    }

    const aplicarTema = (valor) => {
        const esBootstrap = valor.startsWith("bootstrap-");
        const esOscuro = valor === "bootstrap-dark";

        if (esBootstrap) {
            hoja.href =
                "https://cdn.jsdelivr.net/npm/bootstrap@5.3.8/dist/css/bootstrap.min.css";
        } else {
            const nombre = valor.replace("bootswatch-", "");
            hoja.href =
                `https://cdn.jsdelivr.net/npm/bootswatch@5.3.8/dist/${nombre}/bootstrap.min.css`;
        }

        // Bootstrap oscuro usa la misma hoja y cambia sus variables de color.
        document.documentElement.setAttribute(
            "data-bs-theme",
            esOscuro ? "dark" : "light"
        );

        try {
            localStorage.setItem("tema-biblioteca", valor);
        } catch {
            // La aplicación sigue funcionando si el navegador bloquea el almacenamiento.
        }
    };

    let temaGuardado = "bootswatch-minty";
    try {
        temaGuardado = localStorage.getItem("tema-biblioteca") || temaGuardado;
    } catch {
        // Se conserva el tema predeterminado.
    }

    const existe = Array.from(selector.options)
        .some((opcion) => opcion.value === temaGuardado);

    selector.value = existe ? temaGuardado : "bootswatch-minty";
    aplicarTema(selector.value);

    selector.addEventListener("change", () => {
        aplicarTema(selector.value);
    });
});
