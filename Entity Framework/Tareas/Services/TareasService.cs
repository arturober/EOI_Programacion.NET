using Microsoft.EntityFrameworkCore;
using Tareas.Data;
using Tareas.Models;

namespace Tareas.Services;

public class TareasService(AppDbContext db) : ITareasService
{
  public async Task<IReadOnlyList<TareaDto>> GetTareas(CancellationToken ct = default)
  {
    return await db.Tareas
      .AsNoTracking()
      .OrderByDescending(t => t.Fecha)
      .Select(t => new TareaDto(t.Id, t.Descripcion, t.EstaAcabada, t.Fecha))
      .ToListAsync(ct);
  }

  public async Task<TareaDto> CrearTarea(string descripcion, DateTime? fecha, CancellationToken ct = default)
  {
    var tarea = new Tarea
    {
      Descripcion = descripcion,
      Fecha = fecha
    };
    await db.Tareas.AddAsync(tarea, ct);
    db.SaveChanges();
    return new TareaDto(tarea.Id, tarea.Descripcion, tarea.EstaAcabada, tarea.Fecha);
  }

  public async Task<bool> CambiarEstadoTarea(int id, CancellationToken ct = default)
  {
    int updatedRows = await db.Tareas
      .Where(t => t.Id == id)
      .ExecuteUpdateAsync(setters => setters.SetProperty(t => t.EstaAcabada, t => !t.EstaAcabada), ct);
    return updatedRows > 0;
  }


  public async Task<bool> BorrarTarea(int id, CancellationToken ct = default)
  {
    int deleted = await db.Tareas.Where(t => t.Id == id).ExecuteDeleteAsync(ct);
    return deleted > 0;
  }
}
