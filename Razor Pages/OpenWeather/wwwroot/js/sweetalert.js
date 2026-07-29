// El layout deja los mensajes en atributos data-* del elemento body.
const mensaje = document.body.dataset.mensaje;

if (mensaje) {
    // SweetAlert sustituye a los avisos nativos por una ventana accesible.
    Swal.fire({
        title: document.body.dataset.titulo,
        text: mensaje,
        icon: document.body.dataset.icono,
        confirmButtonText: "Aceptar"
    });
}
