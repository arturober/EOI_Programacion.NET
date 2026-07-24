using Equipos.Models;
using Equipos.Services.DTOs;

namespace Equipos.Services;

public interface IEquipoService
{
  Task<IReadOnlyList<EquipoDto>> GetEquiposAsync(CancellationToken ct);
  Task<EquipoJugadoresDto?> GetDetalleEquipoAsync(int id, CancellationToken ct);
  Task<bool> CrearEquipo(string nombre, CancellationToken ct);
  Task<bool> BorrarEquipo(int id, CancellationToken ct);
}
