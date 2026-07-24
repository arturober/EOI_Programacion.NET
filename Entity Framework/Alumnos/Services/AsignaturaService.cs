using Alumnos.Data;
using Alumnos.DTOs;
using Alumnos.Models;
using Microsoft.EntityFrameworkCore;

namespace Alumnos.Services;

public class AsignaturaService(AppDbContext db, ILogger<AsignaturaService> logger) : IAsignaturaService
{
    public async Task<IReadOnlyList<AsignaturaSummaryDto>> GetAsignaturasAsync(CancellationToken ct = default)
    {
        return await db.Asignaturas
            .AsNoTracking()
            .OrderBy(s => s.Nombre)
            .Select(s => new AsignaturaSummaryDto(
                s.Id,
                s.Nombre,
                s.Codigo,
                s.Creditos,
                s.Alumnos.Count))
            .ToListAsync(ct);
    }

    public async Task<int> GetTotalCountAsync(CancellationToken ct = default)
    {
        return await db.Asignaturas.CountAsync(ct);
    }

    public async Task<AsignaturaDetailDto?> GetAsignaturaByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await db.Asignaturas
            .AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new AsignaturaDetailDto(
                s.Id,
                s.Nombre,
                s.Codigo,
                s.Creditos,
                s.Alumnos
                    .OrderBy(a => a.Nombre)
                    .Select(a => new AlumnoSummaryDto(
                        a.Id,
                        a.Nombre,
                        a.Email,
                        a.Dni,
                        0))
                    .ToList()))
            .FirstOrDefaultAsync(ct);
    }

    public async Task<AsignaturaSummaryDto> CreateAsignaturaAsync(CreateAsignaturaInput input, CancellationToken ct = default)
    {
        var asignatura = new Asignatura
        {
            Nombre = input.Nombre,
            Codigo = input.Codigo.ToUpperInvariant(),
            Creditos = input.Creditos
        };

        db.Asignaturas.Add(asignatura);
        await db.SaveChangesAsync(ct);

        logger.LogInformation("Asignatura creada exitosamente con ID {AsignaturaId}", asignatura.Id);
        return new AsignaturaSummaryDto(asignatura.Id, asignatura.Nombre, asignatura.Codigo, asignatura.Creditos, 0);
    }

    public async Task<bool> UpdateAsignaturaAsync(Guid id, UpdateAsignaturaInput input, CancellationToken ct = default)
    {
        var rowsAffected = await db.Asignaturas
            .Where(s => s.Id == id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(s => s.Nombre, input.Nombre)
                .SetProperty(s => s.Codigo, input.Codigo.ToUpperInvariant())
                .SetProperty(s => s.Creditos, input.Creditos), ct);

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsignaturaAsync(Guid id, CancellationToken ct = default)
    {
        var rowsAffected = await db.Asignaturas
            .Where(s => s.Id == id)
            .ExecuteDeleteAsync(ct);

        return rowsAffected > 0;
    }
}
