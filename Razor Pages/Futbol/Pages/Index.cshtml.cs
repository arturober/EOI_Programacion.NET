using Futbol.DTOs;
using Futbol.Servicios;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Futbol.Pages;

// Prepara un resumen del día para la página de inicio.
public class IndexModel : PageModel
{
    private readonly IFutbolServicio _futbol;

    public IndexModel(IFutbolServicio futbol)
    {
        _futbol = futbol;
    }

    public bool ApiConfigurada => _futbol.EstaConfigurada;

    public IReadOnlyList<PartidoDto> Partidos { get; private set; } = [];

    public string? ErrorApi { get; private set; }

    public async Task OnGetAsync(
        CancellationToken cancellationToken)
    {
        if (!ApiConfigurada)
        {
            return;
        }

        try
        {
            Partidos = await _futbol.ObtenerPartidosPorFechaAsync(
                DateOnly.FromDateTime(DateTime.Today),
                cancellationToken);
        }
        catch (FutbolApiExcepcion excepcion)
        {
            ErrorApi = excepcion.Message;
        }
    }
}
