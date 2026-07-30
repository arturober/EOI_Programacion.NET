using Futbol.Data;
using Futbol.Modelos;
using Microsoft.EntityFrameworkCore;

namespace Futbol.Servicios;

// Realiza todas las operaciones de favoritos filtrando siempre por usuario.
public class FavoritosServicio : IFavoritosServicio
{
    private readonly FutbolContext _contexto;

    public FavoritosServicio(FutbolContext contexto)
    {
        _contexto = contexto;
    }

    public async Task<IReadOnlyList<EquipoFavorito>> ObtenerAsync(
        string usuarioId,
        CancellationToken cancellationToken = default)
    {
        return await _contexto.EquiposFavoritos
            .AsNoTracking()
            .Where(favorito => favorito.UsuarioId == usuarioId)
            .OrderBy(favorito => favorito.Nombre)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ContieneAsync(
        string usuarioId,
        int equipoId,
        CancellationToken cancellationToken = default)
    {
        return _contexto.EquiposFavoritos.AnyAsync(
            favorito =>
                favorito.UsuarioId == usuarioId
                && favorito.EquipoId == equipoId,
            cancellationToken);
    }

    public async Task<bool> AgregarAsync(
        string usuarioId,
        EquipoFavorito favorito,
        CancellationToken cancellationToken = default)
    {
        if (await ContieneAsync(
            usuarioId,
            favorito.EquipoId,
            cancellationToken))
        {
            return false;
        }

        favorito.UsuarioId = usuarioId;
        favorito.GuardadoUtc = DateTime.UtcNow;

        _contexto.EquiposFavoritos.Add(favorito);
        await _contexto.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> EliminarAsync(
        string usuarioId,
        int equipoId,
        CancellationToken cancellationToken = default)
    {
        EquipoFavorito? favorito =
            await _contexto.EquiposFavoritos.SingleOrDefaultAsync(
                elemento =>
                    elemento.UsuarioId == usuarioId
                    && elemento.EquipoId == equipoId,
                cancellationToken);

        if (favorito is null)
        {
            return false;
        }

        _contexto.EquiposFavoritos.Remove(favorito);
        await _contexto.SaveChangesAsync(cancellationToken);
        return true;
    }
}
