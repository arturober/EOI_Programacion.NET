using Equipos.Data;
using Equipos.DTOs;
using Equipos.Models;
using Microsoft.EntityFrameworkCore;

namespace Equipos.Services;

public class JugadorService(ApplicationDbContext db, ILogger<JugadorService> logger) : IJugadorService
{
    public async Task<IReadOnlyList<JugadorDto>> GetJugadoresAsync(Guid? equipoId = null, string? search = null, CancellationToken ct = default)
    {
        var query = db.Jugadores.AsNoTracking();

        if (equipoId.HasValue && equipoId != Guid.Empty)
        {
            query = query.Where(j => j.EquipoId == equipoId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(j => j.Nickname.Contains(term) 
                                 || j.NombreCompleto.Contains(term) 
                                 || j.Rol.Contains(term));
        }

        return await query
            .OrderBy(j => j.Nickname)
            .Select(j => new JugadorDto(
                j.Id,
                j.Nickname,
                j.NombreCompleto,
                j.Rol,
                j.EquipoId,
                j.Equipo != null ? j.Equipo.Nombre : "Sin Equipo"))
            .ToListAsync(ct);
    }

    public async Task<JugadorDto?> GetJugadorByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await db.Jugadores
            .AsNoTracking()
            .Where(j => j.Id == id)
            .Select(j => new JugadorDto(
                j.Id,
                j.Nickname,
                j.NombreCompleto,
                j.Rol,
                j.EquipoId,
                j.Equipo != null ? j.Equipo.Nombre : "Sin Equipo"))
            .FirstOrDefaultAsync(ct);
    }

    public async Task<Guid> CreateJugadorAsync(CreateJugadorInput input, CancellationToken ct = default)
    {
        var jugador = new Jugador
        {
            Id = Guid.NewGuid(),
            Nickname = input.Nickname.Trim(),
            NombreCompleto = input.NombreCompleto.Trim(),
            Rol = input.Rol.Trim(),
            EquipoId = input.EquipoId
        };

        db.Jugadores.Add(jugador);
        await db.SaveChangesAsync(ct);
        logger.LogInformation("Jugador {Nickname} creado exitosamente con ID {JugadorId}", jugador.Nickname, jugador.Id);
        return jugador.Id;
    }

    public async Task<bool> UpdateJugadorAsync(UpdateJugadorInput input, CancellationToken ct = default)
    {
        int rows = await db.Jugadores
            .Where(j => j.Id == input.Id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(j => j.Nickname, input.Nickname.Trim())
                .SetProperty(j => j.NombreCompleto, input.NombreCompleto.Trim())
                .SetProperty(j => j.Rol, input.Rol.Trim())
                .SetProperty(j => j.EquipoId, input.EquipoId),
                ct);

        if (rows > 0)
        {
            logger.LogInformation("Jugador con ID {JugadorId} actualizado", input.Id);
            return true;
        }

        return false;
    }

    public async Task<bool> DeleteJugadorAsync(Guid id, CancellationToken ct = default)
    {
        int rows = await db.Jugadores
            .Where(j => j.Id == id)
            .ExecuteDeleteAsync(ct);

        if (rows > 0)
        {
            logger.LogInformation("Jugador con ID {JugadorId} eliminado", id);
            return true;
        }

        return false;
    }
}
