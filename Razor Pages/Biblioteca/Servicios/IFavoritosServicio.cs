using Biblioteca.Modelos;

namespace Biblioteca.Servicios;

// Describe las operaciones de la lista privada del usuario.
public interface IFavoritosServicio
{
    Task<HashSet<string>> ObtenerIdsAsync(
        string usuarioId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Favorito>> ListarAsync(
        string usuarioId,
        CancellationToken cancellationToken = default);

    Task AgregarAsync(
        string usuarioId,
        LibroResumen libro,
        CancellationToken cancellationToken = default);

    Task QuitarAsync(
        string usuarioId,
        string libroId,
        CancellationToken cancellationToken = default);
}
