using Recetas.Modelos;

namespace Recetas.Servicios;

// Describe favoritos y planificación semanal del usuario.
public interface IColeccionServicio
{
    Task<HashSet<int>> ObtenerIdsFavoritosAsync(
        string usuarioId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Favorito>> ListarFavoritosAsync(
        string usuarioId,
        CancellationToken cancellationToken = default);

    Task AgregarFavoritoAsync(
        string usuarioId,
        RecetaDetalle receta,
        CancellationToken cancellationToken = default);

    Task QuitarFavoritoAsync(
        string usuarioId,
        int recetaId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MenuSemanal>> ObtenerMenuAsync(
        string usuarioId,
        CancellationToken cancellationToken = default);

    Task AsignarDiaAsync(
        string usuarioId,
        DiaMenu dia,
        RecetaDetalle receta,
        CancellationToken cancellationToken = default);

    Task QuitarDiaAsync(
        string usuarioId,
        DiaMenu dia,
        CancellationToken cancellationToken = default);
}
