using Equipos.DTOs;
using Equipos.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Equipos.Pages.Equipos;

public class IndexModel(IEquipoService equipoService) : PageModel
{
    public IReadOnlyList<EquipoDto> Equipos { get; private set; } = [];

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    public async Task OnGetAsync(CancellationToken ct)
    {
        Equipos = await equipoService.GetEquiposAsync(Search, ct);
    }
}
