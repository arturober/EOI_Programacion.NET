using Alumnos.DTOs;
using Alumnos.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Alumnos.Pages.Alumnos;

public class EditModel(IAlumnoService alumnoService) : PageModel
{
    [BindProperty]
    public Guid Id { get; set; }

    [BindProperty]
    public UpdateAlumnoInput Input { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken ct)
    {
        ViewData["ActivePage"] = "Alumnos";
        var alumno = await alumnoService.GetAlumnoByIdAsync(id, ct);
        if (alumno is null)
        {
            TempData["ErrorMessage"] = "El alumno solicitado no fue encontrado.";
            return RedirectToPage("./Index");
        }

        Id = alumno.Id;
        Input = new UpdateAlumnoInput
        {
            Nombre = alumno.Nombre,
            Email = alumno.Email,
            Dni = alumno.Dni
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var success = await alumnoService.UpdateAlumnoAsync(Id, Input, ct);
        if (!success)
        {
            TempData["ErrorMessage"] = "No se pudo actualizar la información del alumno.";
            return RedirectToPage("./Index");
        }

        TempData["SuccessMessage"] = $"Alumno '{Input.Nombre}' actualizado correctamente.";
        return RedirectToPage("./Index");
    }
}
