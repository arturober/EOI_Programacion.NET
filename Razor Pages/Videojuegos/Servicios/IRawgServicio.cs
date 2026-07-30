using Videojuegos.Modelos;

namespace Videojuegos.Servicios;

// Permite consumir RAWG sin que las páginas conozcan HttpClient.
public interface IRawgServicio
{
    Task<PaginaVideojuegos> ObtenerListadoAsync(
        TipoListado tipo,
        int pagina = 1,
        CancellationToken cancellationToken = default);

    Task<PaginaVideojuegos> BuscarAsync(
        string texto,
        int pagina = 1,
        CancellationToken cancellationToken = default);

    Task<VideojuegoDetalle> ObtenerDetalleAsync(
        int id,
        CancellationToken cancellationToken = default);
}
