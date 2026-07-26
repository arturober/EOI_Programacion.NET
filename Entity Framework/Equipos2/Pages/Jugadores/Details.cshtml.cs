using Equipos.DTOs;
using Equipos.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Equipos.Pages.Jugadores;

public class DetailsModel(IJugadorService jugadorService) : PageModel
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
}
