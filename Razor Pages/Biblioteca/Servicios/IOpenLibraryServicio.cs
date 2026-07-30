using Biblioteca.Modelos;

namespace Biblioteca.Servicios;

// Permite que las páginas consuman Open Library sin conocer HttpClient.
public interface IOpenLibraryServicio
{
    Task<PaginaLibros> ObtenerListadoAsync(
        TipoListado tipo,
        int pagina = 1,
        CancellationToken cancellationToken = default);

    Task<PaginaLibros> BuscarAsync(
        string texto,
        int pagina = 1,
        CancellationToken cancellationToken = default);

    Task<LibroDetalle> ObtenerDetalleAsync(
        string id,
        CancellationToken cancellationToken = default);
}
