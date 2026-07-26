using Equipos.DTOs;
using Equipos.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Equipos.Pages.Equipos;

public class DetailsModel(IEquipoService equipoService) : PageModel
{
    public EquipoDetailDto Equipo { get; private set; } = default!;

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken ct)
    {
        var result = await equipoService.GetEquipoByIdAsync(id, ct);
        if (result == null)
        {
            return NotFound();
        }

        Equipo = result;
        return Page();
    }
}
