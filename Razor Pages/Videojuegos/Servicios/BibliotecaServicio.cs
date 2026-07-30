using Microsoft.EntityFrameworkCore;
using Videojuegos.Data;
using Videojuegos.Modelos;

namespace Videojuegos.Servicios;

// Mantiene la lógica de la biblioteca fuera de las Razor Pages.
public class BibliotecaServicio : IBibliotecaServicio
{
    private readonly VideojuegosContext _contexto;

    public BibliotecaServicio(VideojuegosContext contexto)
    {
        _contexto = contexto;
    }

    public async Task<HashSet<int>> ObtenerIdsAsync(
        string usuarioId,
        CancellationToken cancellationToken = default)
    {
        List<int> ids = await _contexto.Bibliotecas
            .AsNoTracking()
            .Where(elemento => elemento.UsuarioId == usuarioId)
            .Select(elemento => elemento.VideojuegoId)
            .ToListAsync(cancellationToken);

        return ids.ToHashSet();
    }

    public async Task<IReadOnlyList<VideojuegoUsuario>> ListarAsync(
        string usuarioId,
        CancellationToken cancellationToken = default)
    {
        return await _contexto.Bibliotecas
            .AsNoTracking()
            .Include(elemento => elemento.Videojuego)
            .Where(elemento => elemento.UsuarioId == usuarioId)
            .OrderByDescending(elemento => elemento.FechaActualizadoUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task AgregarAsync(
        string usuarioId,
        VideojuegoResumen videojuego,
        EstadoVideojuego estado = EstadoVideojuego.Pendiente,
        CancellationToken cancellationToken = default)
    {
        bool yaExiste = await _contexto.Bibliotecas.AnyAsync(
            elemento =>
                elemento.UsuarioId == usuarioId
                && elemento.VideojuegoId == videojuego.Id,
            cancellationToken);

        if (yaExiste)
        {
            return;
        }

        Videojuego? guardado = await _contexto.Videojuegos.FindAsync(
            [videojuego.Id],
            cancellationToken);

        if (guardado is null)
        {
            guardado = new Videojuego
            {
                RawgId = videojuego.Id
            };

            _contexto.Videojuegos.Add(guardado);
        }

        // La copia permite mostrar la biblioteca aunque RAWG no responda.
        guardado.Nombre = videojuego.Nombre;
        guardado.Slug = videojuego.Slug;
        guardado.ImagenUrl = videojuego.ImagenUrl;
        guardado.FechaLanzamiento = videojuego.FechaLanzamiento;
        guardado.PuntuacionRawg = videojuego.Puntuacion;
        guardado.Metacritic = videojuego.Metacritic;
        guardado.ActualizadoUtc = DateTime.UtcNow;

        DateTime ahora = DateTime.UtcNow;
        _contexto.Bibliotecas.Add(new VideojuegoUsuario
        {
            UsuarioId = usuarioId,
            VideojuegoId = videojuego.Id,
            Estado = estado,
            FechaAgregadoUtc = ahora,
            FechaActualizadoUtc = ahora
        });

        await _contexto.SaveChangesAsync(cancellationToken);
    }

    public async Task ActualizarAsync(
        string usuarioId,
        int videojuegoId,
        EstadoVideojuego estado,
        int? puntuacion,
        string? comentario,
        CancellationToken cancellationToken = default)
    {
        VideojuegoUsuario? elemento =
            await _contexto.Bibliotecas.FindAsync(
                [usuarioId, videojuegoId],
                cancellationToken);

        if (elemento is null)
        {
            return;
        }

        elemento.Estado = estado;
        elemento.PuntuacionPersonal =
            puntuacion is >= 1 and <= 10 ? puntuacion : null;
        elemento.Comentario = (comentario ?? "").Trim();
        elemento.FechaActualizadoUtc = DateTime.UtcNow;

        await _contexto.SaveChangesAsync(cancellationToken);
    }

    public async Task QuitarAsync(
        string usuarioId,
        int videojuegoId,
        CancellationToken cancellationToken = default)
    {
        VideojuegoUsuario? elemento =
            await _contexto.Bibliotecas.FindAsync(
                [usuarioId, videojuegoId],
                cancellationToken);

        if (elemento is null)
        {
            return;
        }

        _contexto.Bibliotecas.Remove(elemento);
        await _contexto.SaveChangesAsync(cancellationToken);
    }
}
