using Alumnos.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Alumnos.Pages;

public class IndexModel(IAlumnoService alumnoService, IAsignaturaService asignaturaService) : PageModel
{
    public int TotalAlumnos { get; private set; }
    public int TotalAsignaturas { get; private set; }

    public async Task OnGetAsync(CancellationToken ct)
    {
        TotalAlumnos = await alumnoService.GetTotalCountAsync(ct);
        TotalAsignaturas = await asignaturaService.GetTotalCountAsync(ct);
    }
}
