using Alumnos.DTOs;
using Alumnos.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Alumnos.Pages.Alumnos;

public class CreateModel(IAlumnoService alumnoService) : PageModel
{
    [BindProperty]
    public CreateAlumnoInput Input { get; set; } = null!;

    public void OnGet()
    {
        ViewData["ActivePage"] = "Alumnos";
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        await alumnoService.CreateAlumnoAsync(Input, ct);

        TempData["SuccessMessage"] = $"Alumno '{Input.Nombre}' registrado exitosamente.";
        return RedirectToPage("./Index");
    }
}
