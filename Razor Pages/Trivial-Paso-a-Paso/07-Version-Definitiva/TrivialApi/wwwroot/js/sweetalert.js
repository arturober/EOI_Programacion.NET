document.querySelectorAll(".formulario-eliminar").forEach(formulario => {
    formulario.addEventListener("submit", function (evento) {
        evento.preventDefault();

        Swal.fire({
            title: "¿Estás seguro?",
            text: "Esta acción no se puede deshacer.",
            icon: "warning",
            showCancelButton: true,
            confirmButtonText: "Sí, eliminar",
            cancelButtonText: "Cancelar"
        }).then((result) => {
            if (result.isConfirmed) {
                formulario.submit();
            }
        });
    });
});