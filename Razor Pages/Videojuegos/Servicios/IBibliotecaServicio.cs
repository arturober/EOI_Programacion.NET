using Videojuegos.Modelos;

namespace Videojuegos.Servicios;

// Describe las operaciones de la colección privada del usuario.
public interface IBibliotecaServicio
{
    Task<HashSet<int>> ObtenerIdsAsync(
        string usuarioId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<VideojuegoUsuario>> ListarAsync(
        string usuarioId,
        CancellationToken cancellationToken = default);

    Task AgregarAsync(
        string usuarioId,
        VideojuegoResumen videojuego,
        EstadoVideojuego estado = EstadoVideojuego.Pendiente,
        CancellationToken cancellationToken = default);

    Task ActualizarAsync(
        string usuarioId,
        int videojuegoId,
        EstadoVideojuego estado,
        int? puntuacion,
        string? comentario,
        CancellationToken cancellationToken = default);

    Task QuitarAsync(
        string usuarioId,
        int videojuegoId,
        CancellationToken cancellationToken = default);
}
