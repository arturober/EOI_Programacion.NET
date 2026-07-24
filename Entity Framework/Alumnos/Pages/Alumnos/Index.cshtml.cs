using Alumnos.DTOs;
using Alumnos.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Alumnos.Pages.Alumnos;

public class IndexModel(IAlumnoService alumnoService) : PageModel
{
    public IReadOnlyList<AlumnoSummaryDto> Alumnos { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken ct)
    {
        ViewData["ActivePage"] = "Alumnos";
        Alumnos = await alumnoService.GetAlumnosAsync(ct);
    }
}
