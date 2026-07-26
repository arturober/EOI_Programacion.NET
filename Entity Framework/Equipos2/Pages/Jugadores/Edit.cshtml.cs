using Equipos.DTOs;
using Equipos.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Equipos.Pages.Jugadores;

public class EditModel(IJugadorService jugadorService, IEquipoService equipoService, ILogger<EditModel> logger) : PageModel
{
    [BindProperty]
    public required UpdateJugadorInput Input { get; set; }

    public SelectList EquiposOptions { get; private set; } = default!;

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken ct)
    {
        var jugador = await jugadorService.GetJugadorByIdAsync(id, ct);
        if (jugador == null)
        {
            return NotFound();
        }

        Input = new UpdateJugadorInput
        {
            Id = jugador.Id,
            Nickname = jugador.Nickname,
            NombreCompleto = jugador.NombreCompleto,
            Rol = jugador.Rol,
            EquipoId = jugador.EquipoId
        };

        await LoadEquiposOptionsAsync(ct);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await LoadEquiposOptionsAsync(ct);
            return Page();
        }

        bool success = await jugadorService.UpdateJugadorAsync(Input, ct);
        if (!success)
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = $"Jugador '{Input.Nickname}' actualizado correctamente.";
        return RedirectToPage("./Index");
    }

    private async Task LoadEquiposOptionsAsync(CancellationToken ct)
    {
        var equipos = await equipoService.GetEquiposAsync(null, ct);
        EquiposOptions = new SelectList(equipos, nameof(EquipoDto.Id), nameof(EquipoDto.Nombre));
    }
}
