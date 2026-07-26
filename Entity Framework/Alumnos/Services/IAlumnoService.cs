using Alumnos.DTOs;

namespace Alumnos.Services;

public interface IAlumnoService
{
    Task<IReadOnlyList<AlumnoSummaryDto>> GetAlumnosAsync(CancellationToken ct = default);
    Task<int> GetTotalCountAsync(CancellationToken ct = default);
    Task<AlumnoDetailDto?> GetAlumnoByIdAsync(Guid id, CancellationToken ct = default);
    Task<AlumnoSummaryDto> CreateAlumnoAsync(CreateAlumnoInput input, CancellationToken ct = default);
    Task<bool> UpdateAlumnoAsync(Guid id, UpdateAlumnoInput input, CancellationToken ct = default);
    Task<bool> DeleteAlumnoAsync(Guid id, CancellationToken ct = default);
    Task<bool> EnrollInAsignaturasAsync(Guid alumnoId, List<Guid> asignaturaIds, CancellationToken ct = default);
    Task<bool> UnenrollAsignaturaAsync(Guid alumnoId, Guid asignaturaId, CancellationToken ct = default);
}
