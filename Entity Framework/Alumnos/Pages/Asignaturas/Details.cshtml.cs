using Alumnos.DTOs;
using Alumnos.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Alumnos.Pages.Asignaturas;

public class DetailsModel(IAsignaturaService asignaturaService) : PageModel
{
    public AsignaturaDetailDto Asignatura { get; private set; } = null!;

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken ct)
    {
        ViewData["ActivePage"] = "Asignaturas";
        var asignatura = await asignaturaService.GetAsignaturaByIdAsync(id, ct);
        if (asignatura is null)
        {
            TempData["ErrorMessage"] = "Asignatura no encontrada.";
            return RedirectToPage("./Index");
        }

        Asignatura = asignatura;
        return Page();
    }
}
