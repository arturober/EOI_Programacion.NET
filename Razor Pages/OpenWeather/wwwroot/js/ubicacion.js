const botonUbicacion = document.getElementById("botonUbicacion");

botonUbicacion?.addEventListener("click", () => {
    // Algunos navegadores no proporcionan la API de geolocalización.
    if (!navigator.geolocation) {
        Swal.fire({
            title: "Ubicación no disponible",
            text: "Este navegador no permite obtener la ubicación.",
            icon: "warning",
            confirmButtonText: "Aceptar"
        });
        return;
    }

    // Se impiden pulsaciones repetidas mientras el navegador solicita permiso.
    botonUbicacion.disabled = true;
    botonUbicacion.innerHTML =
        '<span class="spinner-border spinner-border-sm me-2"></span>Obteniendo ubicación…';

    navigator.geolocation.getCurrentPosition(
        posicion => {
            const unidades =
                document.getElementById("unidades")?.value ?? "metrico";
            const parametros = new URLSearchParams({
                lat: posicion.coords.latitude,
                lon: posicion.coords.longitude,
                unidades: unidades
            });

            // La consulta vuelve al servidor con las coordenadas recibidas.
            window.location.href = `/?${parametros}`;
        },
        error => {
            // El código permite distinguir una denegación de otros fallos.
            const denegado = error.code === error.PERMISSION_DENIED;

            Swal.fire({
                title: "No se ha podido obtener la ubicación",
                text: denegado
                    ? "Debes permitir el acceso a la ubicación en el navegador."
                    : "Comprueba la conexión o escribe una localidad manualmente.",
                icon: "warning",
                confirmButtonText: "Aceptar"
            });

            botonUbicacion.disabled = false;
            botonUbicacion.innerHTML =
                '<i class="bi bi-geo-alt me-1"></i>Usar mi ubicación';
        },
        {
            enableHighAccuracy: false,
            timeout: 10000,
            maximumAge: 300000
        }
    );
});
