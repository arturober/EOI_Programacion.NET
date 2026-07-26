using Tareas.Models;

namespace Tareas.Services;

public interface ITareasService
{
  Task<IReadOnlyList<TareaDto>> GetTareas(CancellationToken ct = default);

  Task<TareaDto> CrearTarea(string descripcion, DateTime? fecha, CancellationToken ct);

  Task<bool> CambiarEstadoTarea(int id, CancellationToken ct);

  Task<bool> BorrarTarea(int id, CancellationToken ct);
}
