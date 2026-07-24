using Alumnos.Data;
using Alumnos.DTOs;
using Alumnos.Models;
using Microsoft.EntityFrameworkCore;

namespace Alumnos.Services;

public class AlumnoService(AppDbContext db, ILogger<AlumnoService> logger) : IAlumnoService
{
    public async Task<IReadOnlyList<AlumnoSummaryDto>> GetAlumnosAsync(CancellationToken ct = default)
    {
        return await db.Alumnos
            .AsNoTracking()
            .OrderBy(a => a.Nombre)
            .Select(a => new AlumnoSummaryDto(
                a.Id,
                a.Nombre,
                a.Email,
                a.Dni,
                a.Asignaturas.Count))
            .ToListAsync(ct);
    }

    public async Task<int> GetTotalCountAsync(CancellationToken ct = default)
    {
        return await db.Alumnos.CountAsync(ct);
    }

    public async Task<AlumnoDetailDto?> GetAlumnoByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await db.Alumnos
            .AsNoTracking()
            .Where(a => a.Id == id)
            .Select(a => new AlumnoDetailDto(
                a.Id,
                a.Nombre,
                a.Email,
                a.Dni,
                a.Asignaturas
                    .OrderBy(sub => sub.Nombre)
                    .Select(sub => new AsignaturaSummaryDto(
                        sub.Id,
                        sub.Nombre,
                        sub.Codigo,
                        sub.Creditos,
                        0))
                    .ToList()))
            .FirstOrDefaultAsync(ct);
    }

    public async Task<AlumnoSummaryDto> CreateAlumnoAsync(CreateAlumnoInput input, CancellationToken ct = default)
    {
        var alumno = new Alumno
        {
            Nombre = input.Nombre,
            Email = input.Email,
            Dni = input.Dni
        };

        db.Alumnos.Add(alumno);
        await db.SaveChangesAsync(ct);

        logger.LogInformation("Alumno creado exitosamente con ID {AlumnoId}", alumno.Id);
        return new AlumnoSummaryDto(alumno.Id, alumno.Nombre, alumno.Email, alumno.Dni, 0);
    }

    public async Task<bool> UpdateAlumnoAsync(Guid id, UpdateAlumnoInput input, CancellationToken ct = default)
    {
        var rowsAffected = await db.Alumnos
            .Where(a => a.Id == id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(a => a.Nombre, input.Nombre)
                .SetProperty(a => a.Email, input.Email)
                .SetProperty(a => a.Dni, input.Dni), ct);

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAlumnoAsync(Guid id, CancellationToken ct = default)
    {
        var rowsAffected = await db.Alumnos
            .Where(a => a.Id == id)
            .ExecuteDeleteAsync(ct);

        return rowsAffected > 0;
    }

    public async Task<bool> EnrollInAsignaturasAsync(Guid alumnoId, List<Guid> asignaturaIds, CancellationToken ct = default)
    {
        var alumno = await db.Alumnos
            .Include(a => a.Asignaturas)
            .FirstOrDefaultAsync(a => a.Id == alumnoId, ct);

        if (alumno is null) return false;

        var asignaturasToAdd = await db.Asignaturas
            .Where(s => asignaturaIds.Contains(s.Id))
            .ToListAsync(ct);

        foreach (var asignatura in asignaturasToAdd)
        {
            if (!alumno.Asignaturas.Any(s => s.Id == asignatura.Id))
            {
                alumno.Asignaturas.Add(asignatura);
            }
        }

        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> UnenrollAsignaturaAsync(Guid alumnoId, Guid asignaturaId, CancellationToken ct = default)
    {
        var alumno = await db.Alumnos
            .Include(a => a.Asignaturas)
            .FirstOrDefaultAsync(a => a.Id == alumnoId, ct);

        if (alumno is null) return false;

        var asignaturaToRemove = alumno.Asignaturas.FirstOrDefault(s => s.Id == asignaturaId);
        if (asignaturaToRemove is null) return false;

        alumno.Asignaturas.Remove(asignaturaToRemove);
        await db.SaveChangesAsync(ct);
        return true;
    }
}
