using Equipos.DTOs;
using Equipos.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Equipos.Pages.Jugadores;

public class DeleteModel(IJugadorService jugadorService, ILogger<DeleteModel> logger) : PageModel
{
    public JugadorDto Jugador { get; private set; } = default!;

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken ct)
    {
        var result = await jugadorService.GetJugadorByIdAsync(id, ct);
        if (result == null)
        {
            return NotFound();
        }

        Jugador = result;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(Guid id, CancellationToken ct)
    {
        var result = await jugadorService.GetJugadorByIdAsync(id, ct);
        string nickname = result?.Nickname ?? "el jugador";

        bool success = await jugadorService.DeleteJugadorAsync(id, ct);
        if (!success)
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = $"Se ha eliminado al jugador '{nickname}'.";
        return RedirectToPage("./Index");
    }
}
