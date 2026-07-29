// Muestra los mensajes que el servidor deja en los atributos del body.
const cuerpo = document.body;
const mensaje = cuerpo.dataset.mensaje;

if (mensaje) {
    Swal.fire({
        title: cuerpo.dataset.titulo,
        text: mensaje,
        icon: cuerpo.dataset.icono,
        confirmButtonText: "Aceptar"
    });
}

// SweetAlert pide confirmación antes de modificar una favorita.
document
    .querySelectorAll("form[data-confirmar-favorito='true']")
    .forEach((formulario) => {
        formulario.addEventListener("submit", async (evento) => {
            evento.preventDefault();

            const boton = formulario.querySelector("button[type='submit']");
            const quitar = boton?.dataset.accion === "quitar";

            const resultado = await Swal.fire({
                title: quitar
                    ? "¿Quitar esta película?"
                    : "¿Añadir esta película?",
                text: quitar
                    ? "Dejará de aparecer en tu lista personal."
                    : "Se guardará en tu lista personal de favoritas.",
                icon: "question",
                showCancelButton: true,
                confirmButtonText: quitar ? "Sí, quitar" : "Sí, añadir",
                cancelButtonText: "Cancelar",
                confirmButtonColor: quitar ? "#dc3545" : "#0d6efd"
            });

            if (resultado.isConfirmed) {
                // submit() evita volver a disparar este mismo evento.
                formulario.submit();
            }
        });
    });
