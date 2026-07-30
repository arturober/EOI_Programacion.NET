using Futbol.DTOs;
using Futbol.Servicios;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Futbol.Pages.Competiciones;

// Recupera las competiciones disponibles para el token configurado.
public class IndexModel : PageModel
{
    private readonly IFutbolServicio _futbol;

    public IndexModel(IFutbolServicio futbol)
    {
        _futbol = futbol;
    }

    public IReadOnlyList<CompeticionDto> Competiciones { get; private set; } =
        [];

    public string? ErrorApi { get; private set; }

    public async Task OnGetAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            Competiciones =
                await _futbol.ObtenerCompeticionesAsync(cancellationToken);
        }
        catch (FutbolApiExcepcion excepcion)
        {
            ErrorApi = excepcion.Message;
        }
    }
}
