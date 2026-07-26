using Equipos.DTOs;
using Equipos.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Equipos.Pages.Jugadores;

public class CreateModel(IJugadorService jugadorService, IEquipoService equipoService, ILogger<CreateModel> logger) : PageModel
{
    [BindProperty]
    public required CreateJugadorInput Input { get; set; }

    public SelectList EquiposOptions { get; private set; } = default!;

    public async Task<IActionResult> OnGetAsync(Guid? equipoId, CancellationToken ct)
    {
        await LoadEquiposOptionsAsync(ct);

        Input = new CreateJugadorInput
        {
            Nickname = string.Empty,
            NombreCompleto = string.Empty,
            Rol = string.Empty,
            EquipoId = equipoId ?? Guid.Empty
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await LoadEquiposOptionsAsync(ct);
            return Page();
        }

        logger.LogInformation("Registrando jugador {Nickname}", Input.Nickname);
        await jugadorService.CreateJugadorAsync(Input, ct);

        TempData["SuccessMessage"] = $"Jugador '{Input.Nickname}' registrado exitosamente.";
        return RedirectToPage("./Index");
    }

    private async Task LoadEquiposOptionsAsync(CancellationToken ct)
    {
        var equipos = await equipoService.GetEquiposAsync(null, ct);
        EquiposOptions = new SelectList(equipos, nameof(EquipoDto.Id), nameof(EquipoDto.Nombre));
    }
}
