using Equipos.DTOs;
using Equipos.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Equipos.Pages.Equipos;

public class CreateModel(IEquipoService equipoService, ILogger<CreateModel> logger) : PageModel
{
    [BindProperty]
    public required CreateEquipoInput Input { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        logger.LogInformation("Registrando nuevo equipo {Nombre}", Input.Nombre);
        await equipoService.CreateEquipoAsync(Input, ct);

        TempData["SuccessMessage"] = $"Equipo '{Input.Nombre}' registrado exitosamente.";
        return RedirectToPage("./Index");
    }
}
