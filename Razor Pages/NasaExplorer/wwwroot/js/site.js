// Conserva el tema elegido entre páginas sin necesitar una base de datos.
const selectorTema = document.getElementById("selector-tema");
const enlaceTema = document.getElementById("tema-css");
const temasClaros = new Set(["flatly"]);

function aplicarTema(nombre) {
    const tema = nombre || "darkly";
    enlaceTema.href =
        `https://cdn.jsdelivr.net/npm/bootswatch@5.3.8/dist/${tema}/bootstrap.min.css`;
    document.documentElement.dataset.bsTheme =
        temasClaros.has(tema) ? "light" : "dark";
    selectorTema.value = tema;
}

if (selectorTema && enlaceTema) {
    aplicarTema(localStorage.getItem("nasa-tema") || "darkly");
    selectorTema.addEventListener("change", () => {
        localStorage.setItem("nasa-tema", selectorTema.value);
        aplicarTema(selectorTema.value);
    });
}

// Las páginas usan esta función para mantener todos los avisos coherentes.
function mostrarAviso(mensaje, tipo = "success") {
    Swal.fire({
        text: mensaje,
        icon: tipo,
        confirmButtonText: "Aceptar"
    });
}

// Pide confirmación en formularios que puedan eliminar información guardada.
document.querySelectorAll("[data-confirmar]").forEach(formulario => {
    formulario.addEventListener("submit", evento => {
        if (formulario.dataset.confirmado === "true") {
            return;
        }

        evento.preventDefault();
        Swal.fire({
            title: "¿Continuar?",
            text: formulario.dataset.confirmar,
            icon: "question",
            showCancelButton: true,
            confirmButtonText: "Sí",
            cancelButtonText: "Cancelar"
        }).then(resultado => {
            if (resultado.isConfirmed) {
                formulario.dataset.confirmado = "true";
                formulario.requestSubmit();
            }
        });
    });
});
