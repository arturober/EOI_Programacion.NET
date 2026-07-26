using Alumnos.DTOs;
using Alumnos.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Alumnos.Pages.Asignaturas;

public class EditModel(IAsignaturaService asignaturaService) : PageModel
{
    [BindProperty]
    public Guid Id { get; set; }

    [BindProperty]
    public UpdateAsignaturaInput Input { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken ct)
    {
        ViewData["ActivePage"] = "Asignaturas";
        var asignatura = await asignaturaService.GetAsignaturaByIdAsync(id, ct);
        if (asignatura is null)
        {
            TempData["ErrorMessage"] = "La asignatura solicitada no fue encontrada.";
            return RedirectToPage("./Index");
        }

        Id = asignatura.Id;
        Input = new UpdateAsignaturaInput
        {
            Nombre = asignatura.Nombre,
            Codigo = asignatura.Codigo,
            Creditos = asignatura.Creditos
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var success = await asignaturaService.UpdateAsignaturaAsync(Id, Input, ct);
        if (!success)
        {
            TempData["ErrorMessage"] = "No se pudo actualizar la asignatura.";
            return RedirectToPage("./Index");
        }

        TempData["SuccessMessage"] = $"Asignatura '{Input.Nombre}' actualizada correctamente.";
        return RedirectToPage("./Index");
    }
}
