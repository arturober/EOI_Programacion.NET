using Equipos.DTOs;
using Equipos.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Equipos.Pages.Equipos;

public class EditModel(IEquipoService equipoService, ILogger<EditModel> logger) : PageModel
{
    [BindProperty]
    public required UpdateEquipoInput Input { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken ct)
    {
        var equipo = await equipoService.GetEquipoByIdAsync(id, ct);
        if (equipo == null)
        {
            return NotFound();
        }

        Input = new UpdateEquipoInput
        {
            Id = equipo.Id,
            Nombre = equipo.Nombre,
            Juego = equipo.Juego,
            LogoUrl = equipo.LogoUrl
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        bool success = await equipoService.UpdateEquipoAsync(Input, ct);
        if (!success)
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = $"Equipo '{Input.Nombre}' actualizado correctamente.";
        return RedirectToPage("./Index");
    }
}
