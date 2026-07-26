using Equipos.DTOs;
using Equipos.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Equipos.Pages.Jugadores;

public class IndexModel(IJugadorService jugadorService, IEquipoService equipoService) : PageModel
{
    public IReadOnlyList<JugadorDto> Jugadores { get; private set; } = [];
    public IReadOnlyList<EquipoDto> Equipos { get; private set; } = [];

    [BindProperty(SupportsGet = true)]
    public Guid? EquipoId { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    public async Task OnGetAsync(CancellationToken ct)
    {
        Equipos = await equipoService.GetEquiposAsync(null, ct);
        Jugadores = await jugadorService.GetJugadoresAsync(EquipoId, Search, ct);
    }
}
