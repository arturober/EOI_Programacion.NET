using Alumnos.DTOs;
using Alumnos.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Alumnos.Pages.Asignaturas;

public class IndexModel(IAsignaturaService asignaturaService) : PageModel
{
    public IReadOnlyList<AsignaturaSummaryDto> Asignaturas { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken ct)
    {
        ViewData["ActivePage"] = "Asignaturas";
        Asignaturas = await asignaturaService.GetAsignaturasAsync(ct);
    }
}
