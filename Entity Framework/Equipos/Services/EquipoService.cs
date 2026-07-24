using Equipos.Data;
using Equipos.Services.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Equipos.Services;

public class EquipoService(ApplicationDbContext db) : IEquipoService
{
  public async Task<IReadOnlyList<EquipoDto>> GetEquiposAsync(CancellationToken ct)
  {
    return await db.Equipos.AsNoTracking()
      .OrderBy(e => e.Nombre)
      .Select(e => new EquipoDto(e.Id, e.Nombre, e.Jugadores.Count))
      .ToListAsync(ct);
  }
}
