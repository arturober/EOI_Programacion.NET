using Microsoft.EntityFrameworkCore;
using Tareas.Data;
using Tareas.Models;

namespace Tareas.Services;

public class TareasService(AppDbContext db) : ITareasService
{
  public List<Tarea> GetTareas(CancellationToken ct = default)
  {
    return db.Tareas.OrderByDescending(t => t.Fecha).ToList();
  }

  public bool CambiarEstadoTarea(int id)
  {
    int updatedRows = db.Tareas
      .Where(t => t.Id == id)
      .ExecuteUpdate(setters => setters.SetProperty(t => t.EstaAcabada, t => !t.EstaAcabada));
    return updatedRows > 0;
  }
}
