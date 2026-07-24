using Equipos.DTOs;

namespace Equipos.Services;

public interface IEquipoService
{
    Task<IReadOnlyList<EquipoDto>> GetEquiposAsync(string? search = null, CancellationToken ct = default);
    Task<EquipoDetailDto?> GetEquipoByIdAsync(Guid id, CancellationToken ct = default);
    Task<Guid> CreateEquipoAsync(CreateEquipoInput input, CancellationToken ct = default);
    Task<bool> UpdateEquipoAsync(UpdateEquipoInput input, CancellationToken ct = default);
    Task<bool> DeleteEquipoAsync(Guid id, CancellationToken ct = default);
}
