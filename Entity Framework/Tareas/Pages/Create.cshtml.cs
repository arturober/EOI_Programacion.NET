using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tareas.Services;

namespace Tareas.Pages;

public class CreateModel(ITareasService tareasService, ILogger<CreateModel> logger) : PageModel
{
  [BindProperty]
  public required CreateTareaInput Input { get; set; }

  public void OnGet()
  {
  }

  public async Task<IActionResult> OnPostAsync(CancellationToken ct)
  {
    if (!ModelState.IsValid)
    {
      return Page();
    }
    logger.LogInformation($"Creando tarea: {Input.Descripcion}");

    await tareasService.CrearTarea(Input.Descripcion, Input.Fecha, ct);
    return RedirectToPage("./Index");
  }
}

public record CreateTareaInput
{
    [Required(ErrorMessage = "La descripción de la tarea es obligatoria.")]
    [StringLength(500, MinimumLength = 3, ErrorMessage = "La descripción debe tener entre 3 y 500 caracteres.")]
    public required string Descripcion { get; init; }

    [DataType(DataType.DateTime)]
    public DateTime? Fecha { get; init; }
}
