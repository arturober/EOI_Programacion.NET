using Equipos.Data;
using Equipos.Models;
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

  public async Task<EquipoJugadoresDto?> GetDetalleEquipoAsync(int id, CancellationToken ct)
  {
    var equipo = await db.Equipos
      .Where(e => e.Id == id)
      .Include(e => e.Jugadores.OrderBy(j => j.Nombre)) // Incluye la relación con Jugador
      .FirstOrDefaultAsync(ct);

    if(equipo == null)
    {
      return null;
    }

    return new EquipoJugadoresDto(
      equipo.Id,
      equipo.Nombre,
      equipo.Jugadores.Select(j => new JugadorDto(j.Id, j.Nombre, equipo.Id, equipo.Nombre)).ToList()
    );
  }

  public async Task<bool> CrearEquipo(string nombre, CancellationToken ct)
  {
    var equipo = new Equipo { Nombre = nombre };
    db.Add(equipo);
    int filas = await db.SaveChangesAsync();
    return filas > 0;
  }

  public async Task<bool> BorrarEquipo(int id, CancellationToken ct)
  {
    int filas = await db.Equipos
      .Where(e => e.Id == id)
      .ExecuteDeleteAsync(ct);
    return filas > 0;
  }
}
