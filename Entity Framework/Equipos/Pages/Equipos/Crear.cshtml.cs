
using Equipos.Services;
using Equipos.Services.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Equipos.Pages.Equipos;

public class CrearEquipoModel(IEquipoService equipoService) : PageModel
{
  [BindProperty]
  public required CrearEquipoInput Input { get; set; }

  public void OnGet()
  {
  }

  public async Task<IActionResult> OnPostAsync(CancellationToken ct)
  {
    if(!ModelState.IsValid)
    {
      return Page();
    }

    await equipoService.CrearEquipo(Input.Nombre, ct);

    return RedirectToPage("/Index");
  }
}
