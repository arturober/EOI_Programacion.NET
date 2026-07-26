using Alumnos.DTOs;

namespace Alumnos.Services;

public interface IAsignaturaService
{
    Task<IReadOnlyList<AsignaturaSummaryDto>> GetAsignaturasAsync(CancellationToken ct = default);
    Task<int> GetTotalCountAsync(CancellationToken ct = default);
    Task<AsignaturaDetailDto?> GetAsignaturaByIdAsync(Guid id, CancellationToken ct = default);
    Task<AsignaturaSummaryDto> CreateAsignaturaAsync(CreateAsignaturaInput input, CancellationToken ct = default);
    Task<bool> UpdateAsignaturaAsync(Guid id, UpdateAsignaturaInput input, CancellationToken ct = default);
    Task<bool> DeleteAsignaturaAsync(Guid id, CancellationToken ct = default);
}
