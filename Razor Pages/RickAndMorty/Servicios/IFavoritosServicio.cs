using RickAndMorty.Modelos;

namespace RickAndMorty.Servicios;

// Define las operaciones de la colección privada de cada usuario.
public interface IFavoritosServicio
{
    Task<IReadOnlyList<PersonajeFavorito>> ObtenerAsync(
        string usuarioId,
        CancellationToken cancellationToken = default);

    Task<bool> ContieneAsync(
        string usuarioId,
        int personajeId,
        CancellationToken cancellationToken = default);

    Task<bool> AgregarAsync(
        string usuarioId,
        PersonajeFavorito favorito,
        CancellationToken cancellationToken = default);

    Task<bool> EliminarAsync(
        string usuarioId,
        int personajeId,
        CancellationToken cancellationToken = default);
}
