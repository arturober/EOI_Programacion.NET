using Equipos.Data;
using Equipos.DTOs;
using Equipos.Models;
using Microsoft.EntityFrameworkCore;

namespace Equipos.Services;

public class EquipoService(ApplicationDbContext db, ILogger<EquipoService> logger) : IEquipoService
{
    public async Task<IReadOnlyList<EquipoDto>> GetEquiposAsync(string? search = null, CancellationToken ct = default)
    {
        var query = db.Equipos.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(e => e.Nombre.Contains(term) || e.Juego.Contains(term));
        }

        return await query
            .OrderBy(e => e.Nombre)
            .Select(e => new EquipoDto(
                e.Id,
                e.Nombre,
                e.Juego,
                e.LogoUrl,
                e.FechaCreacion,
                e.Jugadores.Count))
            .ToListAsync(ct);
    }

    public async Task<EquipoDetailDto?> GetEquipoByIdAsync(Guid id, CancellationToken ct = default)
    {
        var equipo = await db.Equipos
            .AsNoTracking()
            .Include(e => e.Jugadores.OrderBy(j => j.Nickname))
            .FirstOrDefaultAsync(e => e.Id == id, ct);

        if (equipo is null)
        {
            return null;
        }

        return new EquipoDetailDto(
            equipo.Id,
            equipo.Nombre,
            equipo.Juego,
            equipo.LogoUrl,
            equipo.FechaCreacion,
            equipo.Jugadores
                .Select(j => new JugadorDto(
                    j.Id,
                    j.Nickname,
                    j.NombreCompleto,
                    j.Rol,
                    j.EquipoId,
                    equipo.Nombre))
                .ToList());
    }

    public async Task<Guid> CreateEquipoAsync(CreateEquipoInput input, CancellationToken ct = default)
    {
        var equipo = new Equipo
        {
            Id = Guid.NewGuid(),
            Nombre = input.Nombre.Trim(),
            Juego = input.Juego.Trim(),
            LogoUrl = string.IsNullOrWhiteSpace(input.LogoUrl) ? null : input.LogoUrl.Trim(),
            FechaCreacion = DateTime.UtcNow
        };

        db.Equipos.Add(equipo);
        await db.SaveChangesAsync(ct);
        logger.LogInformation("Equipo creado exitosamente con ID {EquipoId}", equipo.Id);
        return equipo.Id;
    }

    public async Task<bool> UpdateEquipoAsync(UpdateEquipoInput input, CancellationToken ct = default)
    {
        int rows = await db.Equipos
            .Where(e => e.Id == input.Id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(e => e.Nombre, input.Nombre.Trim())
                .SetProperty(e => e.Juego, input.Juego.Trim())
                .SetProperty(e => e.LogoUrl, string.IsNullOrWhiteSpace(input.LogoUrl) ? null : input.LogoUrl.Trim()),
                ct);

        if (rows > 0)
        {
            logger.LogInformation("Equipo con ID {EquipoId} actualizado", input.Id);
            return true;
        }

        return false;
    }

    public async Task<bool> DeleteEquipoAsync(Guid id, CancellationToken ct = default)
    {
        int rows = await db.Equipos
            .Where(e => e.Id == id)
            .ExecuteDeleteAsync(ct);

        if (rows > 0)
        {
            logger.LogInformation("Equipo con ID {EquipoId} eliminado", id);
            return true;
        }

        return false;
    }
}
