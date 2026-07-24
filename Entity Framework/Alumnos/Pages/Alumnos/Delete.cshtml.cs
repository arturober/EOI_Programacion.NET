using Alumnos.DTOs;
using Alumnos.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Alumnos.Pages.Alumnos;

public class DeleteModel(IAlumnoService alumnoService) : PageModel
{
    public AlumnoDetailDto Alumno { get; private set; } = null!;

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken ct)
    {
        ViewData["ActivePage"] = "Alumnos";
        var alumno = await alumnoService.GetAlumnoByIdAsync(id, ct);
        if (alumno is null)
        {
            TempData["ErrorMessage"] = "Alumno no encontrado.";
            return RedirectToPage("./Index");
        }

        Alumno = alumno;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(Guid id, CancellationToken ct)
    {
        var success = await alumnoService.DeleteAlumnoAsync(id, ct);
        if (!success)
        {
            TempData["ErrorMessage"] = "No se pudo eliminar el alumno.";
            return RedirectToPage("./Index");
        }

        TempData["SuccessMessage"] = "Alumno eliminado de la base de datos.";
        return RedirectToPage("./Index");
    }
}
