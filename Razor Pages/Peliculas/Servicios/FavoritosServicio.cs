using Microsoft.EntityFrameworkCore;
using Peliculas.Data;
using Peliculas.Modelos;

namespace Peliculas.Servicios;

// Mantiene toda la lógica de favoritos fuera de las Razor Pages.
public class FavoritosServicio : IFavoritosServicio
{
    private readonly PeliculasContext _contexto;

    public FavoritosServicio(PeliculasContext contexto)
    {
        _contexto = contexto;
    }

    public async Task<HashSet<int>> ObtenerIdsAsync(
        string usuarioId,
        CancellationToken cancellationToken = default)
    {
        List<int> ids = await _contexto.Favoritos
            .AsNoTracking()
            .Where(favorito => favorito.UsuarioId == usuarioId)
            .Select(favorito => favorito.PeliculaId)
            .ToListAsync(cancellationToken);

        return ids.ToHashSet();
    }

    public async Task<IReadOnlyList<Favorito>> ListarAsync(
        string usuarioId,
        CancellationToken cancellationToken = default)
    {
        return await _contexto.Favoritos
            .AsNoTracking()
            .Include(favorito => favorito.Pelicula)
            .Where(favorito => favorito.UsuarioId == usuarioId)
            .OrderByDescending(favorito => favorito.FechaAgregadaUtc)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> EsFavoritaAsync(
        string usuarioId,
        int peliculaId,
        CancellationToken cancellationToken = default)
    {
        return _contexto.Favoritos.AnyAsync(
            favorito =>
                favorito.UsuarioId == usuarioId
                && favorito.PeliculaId == peliculaId,
            cancellationToken);
    }

    public async Task AgregarAsync(
        string usuarioId,
        PeliculaResumen pelicula,
        CancellationToken cancellationToken = default)
    {
        bool yaExiste = await EsFavoritaAsync(
            usuarioId, pelicula.Id, cancellationToken);

        if (yaExiste)
        {
            return;
        }

        Pelicula? peliculaGuardada = await _contexto.Peliculas.FindAsync(
            [pelicula.Id], cancellationToken);

        if (peliculaGuardada is null)
        {
            peliculaGuardada = new Pelicula
            {
                TmdbId = pelicula.Id
            };

            _contexto.Peliculas.Add(peliculaGuardada);
        }

        // Actualizamos la copia local con los datos recibidos de TMDB.
        peliculaGuardada.Titulo = pelicula.Titulo;
        peliculaGuardada.TituloOriginal = pelicula.TituloOriginal;
        peliculaGuardada.RutaPoster = pelicula.RutaPoster;
        peliculaGuardada.FechaEstreno = pelicula.FechaEstreno;
        peliculaGuardada.Puntuacion = pelicula.Puntuacion;
        peliculaGuardada.ActualizadaUtc = DateTime.UtcNow;

        _contexto.Favoritos.Add(new Favorito
        {
            UsuarioId = usuarioId,
            PeliculaId = pelicula.Id,
            FechaAgregadaUtc = DateTime.UtcNow
        });

        await _contexto.SaveChangesAsync(cancellationToken);
    }

    public async Task QuitarAsync(
        string usuarioId,
        int peliculaId,
        CancellationToken cancellationToken = default)
    {
        Favorito? favorito = await _contexto.Favoritos.FindAsync(
            [usuarioId, peliculaId], cancellationToken);

        if (favorito is null)
        {
            return;
        }

        _contexto.Favoritos.Remove(favorito);
        await _contexto.SaveChangesAsync(cancellationToken);
    }
}
