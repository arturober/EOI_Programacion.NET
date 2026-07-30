// Muestra los mensajes enviados desde los PageModel mediante TempData.
const mensaje = document.body.dataset.mensaje;
const tipoMensaje = document.body.dataset.icono ?? "success";
const tituloMensaje = document.body.dataset.titulo ?? "Información";

if (mensaje) {
    Swal.fire({
        icon: tipoMensaje,
        title: tituloMensaje,
        text: mensaje,
        confirmButtonText: "Aceptar"
    });
}

// Pide confirmación antes de quitar un personaje de favoritos.
document.querySelectorAll(
    "form[data-confirmar-quitar='true']"
).forEach(function (formulario) {
    formulario.addEventListener("submit", async function (evento) {
        evento.preventDefault();

        const respuesta = await Swal.fire({
            icon: "question",
            title: "¿Quitar de favoritos?",
            text: "El personaje dejará de aparecer en tu colección.",
            showCancelButton: true,
            confirmButtonText: "Sí, quitar",
            cancelButtonText: "Cancelar",
            confirmButtonColor: "#dc3545"
        });

        if (respuesta.isConfirmed) {
            formulario.submit();
        }
    });
});
