using Peliculas.Modelos;

namespace Peliculas.Servicios;

// La interfaz permite utilizar TMDB sin acoplar las páginas a HttpClient.
public interface ITmdbServicio
{
    bool EstaConfigurado { get; }

    Task<PaginaPeliculas> ObtenerListadoAsync(
        TipoListado tipo,
        int pagina = 1,
        CancellationToken cancellationToken = default);

    Task<PaginaPeliculas> BuscarAsync(
        string texto,
        int pagina = 1,
        CancellationToken cancellationToken = default);

    Task<PeliculaDetalle> ObtenerDetalleAsync(
        int id,
        CancellationToken cancellationToken = default);
}
