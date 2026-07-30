using Futbol.Modelos;

namespace Futbol.Servicios;

// Separa el acceso a favoritos de la lógica de las páginas.
public interface IFavoritosServicio
{
    Task<IReadOnlyList<EquipoFavorito>> ObtenerAsync(
        string usuarioId,
        CancellationToken cancellationToken = default);

    Task<bool> ContieneAsync(
        string usuarioId,
        int equipoId,
        CancellationToken cancellationToken = default);

    Task<bool> AgregarAsync(
        string usuarioId,
        EquipoFavorito favorito,
        CancellationToken cancellationToken = default);

    Task<bool> EliminarAsync(
        string usuarioId,
        int equipoId,
        CancellationToken cancellationToken = default);
}
