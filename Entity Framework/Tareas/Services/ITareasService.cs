using Tareas.Models;

namespace Tareas.Services;

public interface ITareasService
{
  List<Tarea> GetTareas(CancellationToken ct = default);
  bool CambiarEstadoTarea(int id);
}
