using Equipos.Services;
using Equipos.Services.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Equipos.Pages.Equipos;

public class DetalleEquipoModel(IEquipoService equipoService) : PageModel
{
  public EquipoJugadoresDto? Equipo { get; set; }

  public async Task<IActionResult> OnGetAsync(int id, CancellationToken ct)
  {
    Equipo = await equipoService.GetDetalleEquipoAsync(id, ct);
    if(Equipo == null)
    {
      HttpContext.Items["ErrorMessage"] = $"Equipo no encontrado con id = {id}";
      return NotFound();
    }

    return Page();
  }
}
