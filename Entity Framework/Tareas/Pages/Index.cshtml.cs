using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tareas.Models;
using Tareas.Services;

namespace Tareas.Pages;

public class IndexModel(ITareasService tareasService, ILogger<IndexModel> logger) : PageModel
{
  public IReadOnlyList<TareaDto> Tareas { get; set; } = [];

  public async Task OnGetAsync(CancellationToken ct)
  {
    Tareas = await tareasService.GetTareas(ct);
  }

  public async Task<IActionResult> OnPostToggleAsync(int id, CancellationToken ct)
  {
    await tareasService.CambiarEstadoTarea(id, ct);
    logger.LogInformation($"Cambiando estado de la tarea {id}");
    return RedirectToPage();
  }

  public async Task<IActionResult> OnPostDeleteAsync(int id, CancellationToken ct)
  {
    await tareasService.BorrarTarea(id, ct);
    logger.LogInformation($"Borrando tarea {id}");
    return RedirectToPage();
  }
}
