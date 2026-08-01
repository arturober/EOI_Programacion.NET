// El layout copia el mensaje de TempData a un atributo data del body.
const mensajeOperacion = document.body.dataset.mensaje;

// Solo abrimos una ventana cuando una operación ha dejado un mensaje.
if (mensajeOperacion) {
    Swal.fire({
        icon: document.body.dataset.icono,
        title: document.body.dataset.titulo,
        text: mensajeOperacion,
        confirmButtonText: "Aceptar"
    });
}

// Todos los formularios de borrado comparten la misma clase.
// De este modo no repetimos un script distinto para categorías y preguntas.
document.querySelectorAll(".formulario-eliminar").forEach(formulario => {
    formulario.addEventListener("submit", async evento => {
        // Detenemos temporalmente el POST mientras esperamos la decisión.
        evento.preventDefault();

        // Las categorías incluyen un aviso sobre el borrado en cascada.
        // Las preguntas no tienen data-aviso y utilizan una cadena vacía.
        const aviso = formulario.dataset.aviso
            ? ` ${formulario.dataset.aviso}`
            : "";

        const resultado = await Swal.fire({
            title: "¿Estás seguro?",
            text: `Se eliminará ${formulario.dataset.elemento} "${formulario.dataset.nombre}".${aviso} Esta operación no se puede deshacer.`,
            icon: "warning",
            showCancelButton: true,
            confirmButtonText: "Sí, eliminar",
            cancelButtonText: "Cancelar",
            reverseButtons: true
        });

        if (resultado.isConfirmed) {
            // submit envía ahora el formulario sin volver a producir el evento submit,
            // por lo que no se abre una segunda ventana de confirmación.
            formulario.submit();
        }
    });
});
