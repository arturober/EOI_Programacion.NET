using Microsoft.EntityFrameworkCore;
using RickAndMorty.Data;
using RickAndMorty.Modelos;

namespace RickAndMorty.Servicios;

// Todas las consultas incluyen el usuario para aislar sus datos.
public class FavoritosServicio : IFavoritosServicio
{
    private readonly RickAndMortyContext _contexto;

    public FavoritosServicio(RickAndMortyContext contexto)
    {
        _contexto = contexto;
    }

    public async Task<IReadOnlyList<PersonajeFavorito>> ObtenerAsync(
        string usuarioId,
        CancellationToken cancellationToken = default)
    {
        return await _contexto.PersonajesFavoritos
            .AsNoTracking()
            .Where(favorito => favorito.UsuarioId == usuarioId)
            .OrderBy(favorito => favorito.Nombre)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ContieneAsync(
        string usuarioId,
        int personajeId,
        CancellationToken cancellationToken = default)
    {
        return _contexto.PersonajesFavoritos.AnyAsync(
            favorito =>
                favorito.UsuarioId == usuarioId
                && favorito.PersonajeId == personajeId,
            cancellationToken);
    }

    public async Task<bool> AgregarAsync(
        string usuarioId,
        PersonajeFavorito favorito,
        CancellationToken cancellationToken = default)
    {
        if (await ContieneAsync(
            usuarioId,
            favorito.PersonajeId,
            cancellationToken))
        {
            return false;
        }

        favorito.UsuarioId = usuarioId;
        favorito.GuardadoUtc = DateTime.UtcNow;
        _contexto.PersonajesFavoritos.Add(favorito);
        await _contexto.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> EliminarAsync(
        string usuarioId,
        int personajeId,
        CancellationToken cancellationToken = default)
    {
        PersonajeFavorito? favorito =
            await _contexto.PersonajesFavoritos.SingleOrDefaultAsync(
                elemento =>
                    elemento.UsuarioId == usuarioId
                    && elemento.PersonajeId == personajeId,
                cancellationToken);

        if (favorito is null)
        {
            return false;
        }

        _contexto.PersonajesFavoritos.Remove(favorito);
        await _contexto.SaveChangesAsync(cancellationToken);
        return true;
    }
}
