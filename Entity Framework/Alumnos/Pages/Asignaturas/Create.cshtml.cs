using Alumnos.DTOs;
using Alumnos.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Alumnos.Pages.Asignaturas;

public class CreateModel(IAsignaturaService asignaturaService) : PageModel
{
    [BindProperty]
    public CreateAsignaturaInput Input { get; set; } = null!;

    public void OnGet()
    {
        ViewData["ActivePage"] = "Asignaturas";
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        await asignaturaService.CreateAsignaturaAsync(Input, ct);

        TempData["SuccessMessage"] = $"Asignatura '{Input.Nombre}' creada correctamente.";
        return RedirectToPage("./Index");
    }
}
