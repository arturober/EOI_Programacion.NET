using Biblioteca.Data;
using Biblioteca.Modelos;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Servicios;

// Mantiene toda la lógica de favoritos fuera de las Razor Pages.
public class FavoritosServicio : IFavoritosServicio
{
    private readonly BibliotecaContext _contexto;

    public FavoritosServicio(BibliotecaContext contexto)
    {
        _contexto = contexto;
    }

    public async Task<HashSet<string>> ObtenerIdsAsync(
        string usuarioId,
        CancellationToken cancellationToken = default)
    {
        List<string> ids = await _contexto.Favoritos
            .AsNoTracking()
            .Where(favorito => favorito.UsuarioId == usuarioId)
            .Select(favorito => favorito.LibroId)
            .ToListAsync(cancellationToken);

        return ids.ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    public async Task<IReadOnlyList<Favorito>> ListarAsync(
        string usuarioId,
        CancellationToken cancellationToken = default)
    {
        return await _contexto.Favoritos
            .AsNoTracking()
            .Include(favorito => favorito.Libro)
            .Where(favorito => favorito.UsuarioId == usuarioId)
            .OrderByDescending(favorito => favorito.FechaAgregadoUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task AgregarAsync(
        string usuarioId,
        LibroResumen libro,
        CancellationToken cancellationToken = default)
    {
        string libroId = libro.Id.ToUpperInvariant();

        bool yaExiste = await _contexto.Favoritos.AnyAsync(
            favorito =>
                favorito.UsuarioId == usuarioId
                && favorito.LibroId == libroId,
            cancellationToken);

        if (yaExiste)
        {
            return;
        }

        Libro? libroGuardado = await _contexto.Libros.FindAsync(
            [libroId], cancellationToken);

        if (libroGuardado is null)
        {
            libroGuardado = new Libro
            {
                OpenLibraryId = libroId
            };

            _contexto.Libros.Add(libroGuardado);
        }

        // La copia permite mostrar favoritos aunque la API no responda.
        libroGuardado.Titulo = libro.Titulo;
        libroGuardado.Autores = libro.AutoresTexto;
        libroGuardado.PortadaId = libro.PortadaId;
        libroGuardado.PrimeraPublicacion = libro.PrimeraPublicacion;
        libroGuardado.Puntuacion = libro.Puntuacion;
        libroGuardado.ActualizadoUtc = DateTime.UtcNow;

        _contexto.Favoritos.Add(new Favorito
        {
            UsuarioId = usuarioId,
            LibroId = libroId,
            FechaAgregadoUtc = DateTime.UtcNow
        });

        await _contexto.SaveChangesAsync(cancellationToken);
    }

    public async Task QuitarAsync(
        string usuarioId,
        string libroId,
        CancellationToken cancellationToken = default)
    {
        Favorito? favorito = await _contexto.Favoritos.FindAsync(
            [usuarioId, libroId.ToUpperInvariant()], cancellationToken);

        if (favorito is null)
        {
            return;
        }

        _contexto.Favoritos.Remove(favorito);
        await _contexto.SaveChangesAsync(cancellationToken);
    }
}
