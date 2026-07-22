using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tareas.Models;
using Tareas.Services;

namespace Tareas.Pages;

public class IndexModel(ITareasService tareasService) : PageModel
{
  public List<Tarea> Tareas { get; set; } = [];

  public void OnGet()
  {
    Tareas = tareasService.GetTareas();
  }

  public IActionResult OnPostToggle(int id)
  {
    tareasService.CambiarEstadoTarea(id);
    return RedirectToPage();
  }
}
