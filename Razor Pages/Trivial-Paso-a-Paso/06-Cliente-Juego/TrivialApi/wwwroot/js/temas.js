const tema = localStorage.getItem('tema') || 'bootstrap-light';

function cambiarTema(tema) {
    const esBootSwatch = tema.startsWith('bootswatch-');
    const nombre = tema.replace('bootswatch-', '');

    const link = document.getElementById('temaCSS');

    link.href = esBootSwatch
        ? `https://cdn.jsdelivr.net/npm/bootswatch@5.3.8/dist/${nombre}/bootstrap.min.css`
        : `https://cdn.jsdelivr.net/npm/bootstrap@5.3.8/dist/css/bootstrap.min.css`;

    document.documentElement.setAttribute('data-bs-theme', esBootSwatch ? nombre : 'light');

    localStorage.setItem('tema', tema);
}

selectorTema = document.getElementById('selectorTema');

selectorTema.addEventListener('change', (event) => {
    const temaSeleccionado = event.target.value;
    cambiarTema(temaSeleccionado);
});

// Inicializar el tema al cargar la página
cambiarTema(tema);