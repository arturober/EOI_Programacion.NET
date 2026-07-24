using Alumnos.DTOs;
using Alumnos.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Alumnos.Pages.Alumnos;

public class DetailsModel(IAlumnoService alumnoService, IAsignaturaService asignaturaService) : PageModel
{
    public AlumnoDetailDto Alumno { get; private set; } = null!;
    public IReadOnlyList<AsignaturaSummaryDto> AvailableAsignaturas { get; private set; } = [];

    [BindProperty]
    public List<Guid> SelectedAsignaturaIds { get; set; } = [];

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken ct)
    {
        ViewData["ActivePage"] = "Alumnos";
        return await LoadDataAsync(id, ct);
    }

    public async Task<IActionResult> OnPostEnrollAsync(Guid id, CancellationToken ct)
    {
        if (SelectedAsignaturaIds.Count == 0)
        {
            TempData["ErrorMessage"] = "Selecciona al menos una asignatura para matricular.";
            return RedirectToPage(new { id });
        }

        await alumnoService.EnrollInAsignaturasAsync(id, SelectedAsignaturaIds, ct);
        TempData["SuccessMessage"] = "Alumno matriculado correctamente en las asignaturas seleccionadas.";
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostUnenrollAsync(Guid id, Guid asignaturaId, CancellationToken ct)
    {
        await alumnoService.UnenrollAsignaturaAsync(id, asignaturaId, ct);
        TempData["SuccessMessage"] = "Asignatura desmatriculada correctamente.";
        return RedirectToPage(new { id });
    }

    private async Task<IActionResult> LoadDataAsync(Guid id, CancellationToken ct)
    {
        var alumno = await alumnoService.GetAlumnoByIdAsync(id, ct);
        if (alumno is null)
        {
            TempData["ErrorMessage"] = "Alumno no encontrado.";
            return RedirectToPage("./Index");
        }

        Alumno = alumno;
        var allAsignaturas = await asignaturaService.GetAsignaturasAsync(ct);
        var enrolledIds = Alumno.Asignaturas.Select(a => a.Id).ToHashSet();
        AvailableAsignaturas = allAsignaturas.Where(a => !enrolledIds.Contains(a.Id)).ToList();

        return Page();
    }
}
