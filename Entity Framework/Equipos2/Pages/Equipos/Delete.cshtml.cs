using Equipos.DTOs;
using Equipos.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Equipos.Pages.Equipos;

public class DeleteModel(IEquipoService equipoService, ILogger<DeleteModel> logger) : PageModel
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

    public async Task<IActionResult> OnPostAsync(Guid id, CancellationToken ct)
    {
        var result = await equipoService.GetEquipoByIdAsync(id, ct);
        string nombre = result?.Nombre ?? "el equipo";

        bool success = await equipoService.DeleteEquipoAsync(id, ct);
        if (!success)
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = $"Se ha eliminado exitosamente '{nombre}'.";
        return RedirectToPage("./Index");
    }
}
