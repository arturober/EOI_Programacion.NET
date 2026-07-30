using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Recetas.Data;
using Recetas.Modelos;

namespace Recetas.Servicios;

// Mantiene la lógica de favoritos y menú fuera de las Razor Pages.
public class ColeccionServicio : IColeccionServicio
{
    private readonly RecetasContext _contexto;

    public ColeccionServicio(RecetasContext contexto)
    {
        _contexto = contexto;
    }

    public async Task<HashSet<int>> ObtenerIdsFavoritosAsync(
        string usuarioId,
        CancellationToken cancellationToken = default)
    {
        List<int> ids = await _contexto.Favoritos
            .AsNoTracking()
            .Where(elemento => elemento.UsuarioId == usuarioId)
            .Select(elemento => elemento.RecetaId)
            .ToListAsync(cancellationToken);

        return ids.ToHashSet();
    }

    public async Task<IReadOnlyList<Favorito>> ListarFavoritosAsync(
        string usuarioId,
        CancellationToken cancellationToken = default)
    {
        return await _contexto.Favoritos
            .AsNoTracking()
            .Include(elemento => elemento.Receta)
            .Where(elemento => elemento.UsuarioId == usuarioId)
            .OrderByDescending(elemento => elemento.FechaAgregadaUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task AgregarFavoritoAsync(
        string usuarioId,
        RecetaDetalle receta,
        CancellationToken cancellationToken = default)
    {
        bool yaExiste = await _contexto.Favoritos.AnyAsync(
            elemento =>
                elemento.UsuarioId == usuarioId
                && elemento.RecetaId == receta.Id,
            cancellationToken);

        if (yaExiste)
        {
            return;
        }

        await GuardarRecetaAsync(receta, cancellationToken);

        _contexto.Favoritos.Add(new Favorito
        {
            UsuarioId = usuarioId,
            RecetaId = receta.Id,
            FechaAgregadaUtc = DateTime.UtcNow
        });

        await _contexto.SaveChangesAsync(cancellationToken);
    }

    public async Task QuitarFavoritoAsync(
        string usuarioId,
        int recetaId,
        CancellationToken cancellationToken = default)
    {
        Favorito? favorito = await _contexto.Favoritos.FindAsync(
            [usuarioId, recetaId],
            cancellationToken);

        if (favorito is null)
        {
            return;
        }

        _contexto.Favoritos.Remove(favorito);
        await _contexto.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MenuSemanal>> ObtenerMenuAsync(
        string usuarioId,
        CancellationToken cancellationToken = default)
    {
        return await _contexto.MenusSemanales
            .AsNoTracking()
            .Include(elemento => elemento.Receta)
            .Where(elemento => elemento.UsuarioId == usuarioId)
            .OrderBy(elemento => elemento.Dia)
            .ToListAsync(cancellationToken);
    }

    public async Task AsignarDiaAsync(
        string usuarioId,
        DiaMenu dia,
        RecetaDetalle receta,
        CancellationToken cancellationToken = default)
    {
        if (!Enum.IsDefined(dia))
        {
            return;
        }

        await GuardarRecetaAsync(receta, cancellationToken);

        MenuSemanal? elemento = await _contexto.MenusSemanales.FindAsync(
            [usuarioId, dia],
            cancellationToken);

        if (elemento is null)
        {
            elemento = new MenuSemanal
            {
                UsuarioId = usuarioId,
                Dia = dia
            };

            _contexto.MenusSemanales.Add(elemento);
        }

        elemento.RecetaId = receta.Id;
        elemento.FechaActualizadaUtc = DateTime.UtcNow;

        await _contexto.SaveChangesAsync(cancellationToken);
    }

    public async Task QuitarDiaAsync(
        string usuarioId,
        DiaMenu dia,
        CancellationToken cancellationToken = default)
    {
        MenuSemanal? elemento = await _contexto.MenusSemanales.FindAsync(
            [usuarioId, dia],
            cancellationToken);

        if (elemento is null)
        {
            return;
        }

        _contexto.MenusSemanales.Remove(elemento);
        await _contexto.SaveChangesAsync(cancellationToken);
    }

    private async Task GuardarRecetaAsync(
        RecetaDetalle receta,
        CancellationToken cancellationToken)
    {
        Receta? guardada = await _contexto.Recetas.FindAsync(
            [receta.Id],
            cancellationToken);

        if (guardada is null)
        {
            guardada = new Receta
            {
                TheMealDbId = receta.Id
            };

            _contexto.Recetas.Add(guardada);
        }

        guardada.Nombre = receta.Nombre;
        guardada.ImagenUrl = receta.ImagenUrl;
        guardada.Categoria = receta.Categoria;
        guardada.Area = receta.Area;
        guardada.IngredientesJson = JsonSerializer.Serialize(
            receta.Ingredientes.Select(ingrediente => new
            {
                ingrediente.Nombre,
                ingrediente.Medida
            }));
        guardada.ActualizadaUtc = DateTime.UtcNow;
    }
}
