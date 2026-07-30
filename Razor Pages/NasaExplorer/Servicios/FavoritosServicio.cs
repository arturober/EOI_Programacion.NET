using Microsoft.EntityFrameworkCore;
using NasaExplorer.Data;
using NasaExplorer.Modelos;

namespace NasaExplorer.Servicios;

// Centraliza el acceso a SQLite para que las páginas no repitan consultas.
public class FavoritosServicio(NasaContext context) : IFavoritosServicio
{
    public Task<List<Favorito>> ObtenerAsync(string usuarioId)
    {
        return context.Favoritos
            .AsNoTracking()
            .Where(favorito => favorito.UsuarioId == usuarioId)
            .OrderByDescending(favorito => favorito.GuardadoUtc)
            .ToListAsync();
    }

    public async Task<HashSet<string>> ObtenerReferenciasAsync(
        string usuarioId,
        string tipo)
    {
        List<string> referencias = await context.Favoritos
            .AsNoTracking()
            .Where(favorito =>
                favorito.UsuarioId == usuarioId
                && favorito.Tipo == tipo)
            .Select(favorito => favorito.Referencia)
            .ToListAsync();

        return referencias.ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    public async Task<bool> AlternarAsync(string usuarioId, Favorito favorito)
    {
        Favorito? existente = await context.Favoritos.FirstOrDefaultAsync(actual =>
            actual.UsuarioId == usuarioId
            && actual.Tipo == favorito.Tipo
            && actual.Referencia == favorito.Referencia);

        if (existente is not null)
        {
            context.Favoritos.Remove(existente);
            await context.SaveChangesAsync();
            return false;
        }

        favorito.UsuarioId = usuarioId;
        favorito.GuardadoUtc = DateTime.UtcNow;
        context.Favoritos.Add(favorito);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EliminarAsync(string usuarioId, int id)
    {
        Favorito? favorito = await context.Favoritos.FirstOrDefaultAsync(actual =>
            actual.Id == id && actual.UsuarioId == usuarioId);

        if (favorito is null)
        {
            return false;
        }

        context.Favoritos.Remove(favorito);
        await context.SaveChangesAsync();
        return true;
    }
}
