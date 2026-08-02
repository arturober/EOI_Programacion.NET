// Muestra con SweetAlert2 los mensajes enviados desde TempData o ViewData.
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

    // Quitar un favorito requiere confirmación; añadirlo es inmediato.
    document.querySelectorAll("form[data-confirmar-favorito='true']")
        .forEach((formulario) => {
            formulario.addEventListener("submit", async (evento) => {
                if (!window.Swal || formulario.dataset.confirmado === "true") {
                    return;
                }

                evento.preventDefault();

                const resultado = await Swal.fire({
                    title: "¿Quitar este libro?",
                    text: "Desaparecerá de tu lista de favoritos.",
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
