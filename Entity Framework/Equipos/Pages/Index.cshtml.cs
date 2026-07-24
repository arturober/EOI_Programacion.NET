using Equipos.Services;
using Equipos.Services.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Equipos.Pages;

public class IndexModel(IEquipoService equipoService) : PageModel
{
  public IReadOnlyList<EquipoDto> Equipos = [];

  public async void OnGetAsync(CancellationToken ct)
  {
    Equipos = await equipoService.GetEquiposAsync(ct);
  }

  public async Task<IActionResult> OnPostDeleteAsync(int id, CancellationToken ct)
  {
    await equipoService.BorrarEquipo(id, ct);
    return RedirectToPage();
  }
}
