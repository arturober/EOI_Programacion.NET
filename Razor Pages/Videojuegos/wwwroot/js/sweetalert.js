// Muestra con SweetAlert2 los mensajes enviados desde el servidor.
document.addEventListener("DOMContentLoaded", () => {
    const cuerpo = document.body;
    const mensaje = cuerpo.dataset.mensaje;

    if (mensaje && window.Swal) {
        Swal.fire({
            title: cuerpo.dataset.titulo,
            text: mensaje,
            icon: cuerpo.dataset.icono || "info",
            confirmButtonText: "Aceptar"
        });
    }

    // Quitar un videojuego requiere confirmación; añadirlo es inmediato.
    document.querySelectorAll("form[data-confirmar-quitar='true']")
        .forEach((formulario) => {
            formulario.addEventListener("submit", async (evento) => {
                if (!window.Swal || formulario.dataset.confirmado === "true") {
                    return;
                }

                evento.preventDefault();

                const resultado = await Swal.fire({
                    title: "¿Quitar este videojuego?",
                    text: "También se borrarán tu estado, nota y comentario.",
                    icon: "question",
                    showCancelButton: true,
                    confirmButtonText: "Sí, quitar",
                    cancelButtonText: "Cancelar",
                    confirmButtonColor: "#dc3545"
                });

                if (resultado.isConfirmed) {
                    formulario.dataset.confirmado = "true";
                    formulario.requestSubmit();
                }
            });
        });
});
