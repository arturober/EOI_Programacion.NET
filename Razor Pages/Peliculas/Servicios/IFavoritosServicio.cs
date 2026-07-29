using Peliculas.Modelos;

namespace Peliculas.Servicios;

// Describe las operaciones que las páginas pueden realizar con favoritos.
public interface IFavoritosServicio
{
    Task<HashSet<int>> ObtenerIdsAsync(
        string usuarioId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Favorito>> ListarAsync(
        string usuarioId,
        CancellationToken cancellationToken = default);

    Task<bool> EsFavoritaAsync(
        string usuarioId,
        int peliculaId,
        CancellationToken cancellationToken = default);

    Task AgregarAsync(
        string usuarioId,
        PeliculaResumen pelicula,
        CancellationToken cancellationToken = default);

    Task QuitarAsync(
        string usuarioId,
        int peliculaId,
        CancellationToken cancellationToken = default);
}
