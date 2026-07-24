using Equipos.Models;
using Equipos.Services.DTOs;

namespace Equipos.Services;

public interface IEquipoService
{
  Task<IReadOnlyList<EquipoDto>> GetEquiposAsync(CancellationToken ct);
}
