using OpenFoodFacts.Modelos;

namespace OpenFoodFacts.Servicios;

// Define el acceso a favoritos y a sus datos guardados en SQLite.
public interface IColeccionServicio
{
    Task<HashSet<string>> ObtenerCodigosFavoritosAsync(
        string usuarioId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Favorito>> ListarFavoritosAsync(
        string usuarioId,
        CancellationToken cancellationToken = default);

    Task AgregarFavoritoAsync(
        string usuarioId,
        ProductoDetalle producto,
        CancellationToken cancellationToken = default);

    Task QuitarFavoritoAsync(
        string usuarioId,
        string codigo,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductoGuardado>> ObtenerParaCompararAsync(
        string usuarioId,
        IEnumerable<string> codigos,
        CancellationToken cancellationToken = default);
}
