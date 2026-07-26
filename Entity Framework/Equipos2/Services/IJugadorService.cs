using Equipos.DTOs;

namespace Equipos.Services;

public interface IJugadorService
{
    Task<IReadOnlyList<JugadorDto>> GetJugadoresAsync(Guid? equipoId = null, string? search = null, CancellationToken ct = default);
    Task<JugadorDto?> GetJugadorByIdAsync(Guid id, CancellationToken ct = default);
    Task<Guid> CreateJugadorAsync(CreateJugadorInput input, CancellationToken ct = default);
    Task<bool> UpdateJugadorAsync(UpdateJugadorInput input, CancellationToken ct = default);
    Task<bool> DeleteJugadorAsync(Guid id, CancellationToken ct = default);
}
