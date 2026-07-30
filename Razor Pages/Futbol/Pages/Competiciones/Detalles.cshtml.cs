using Futbol.DTOs;
using Futbol.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Futbol.Pages.Competiciones;

// Carga únicamente la sección elegida para ahorrar peticiones a la API.
public class DetallesModel : PageModel
{
    private readonly IFutbolServicio _futbol;

    public DetallesModel(IFutbolServicio futbol)
    {
        _futbol = futbol;
    }

    [BindProperty(SupportsGet = true)]
    public string Codigo { get; set; } = "";

    [BindProperty(SupportsGet = true)]
    public string Seccion { get; set; } = "clasificacion";

    public CompeticionDto? Competicion { get; private set; }
    public ClasificacionRespuestaDto? Clasificacion { get; private set; }
    public IReadOnlyList<PartidoDto> Partidos { get; private set; } = [];
    public GoleadoresRespuestaDto? Goleadores { get; private set; }
    public EquiposRespuestaDto? Equipos { get; private set; }
    public string? ErrorApi { get; private set; }

    public async Task<IActionResult> OnGetAsync(
        CancellationToken cancellationToken)
    {
        Codigo = Codigo.Trim().ToUpperInvariant();
        Seccion = Seccion.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(Codigo))
        {
            return RedirectToPage("/Competiciones/Index");
        }

        if (Seccion is not (
            "clasificacion" or "partidos" or "goleadores" or "equipos"))
        {
            Seccion = "clasificacion";
        }

        try
        {
            // La lista está en caché después de la primera visita.
            IReadOnlyList<CompeticionDto> competiciones =
                await _futbol.ObtenerCompeticionesAsync(cancellationToken);

            Competicion = competiciones.FirstOrDefault(elemento =>
                string.Equals(
                    elemento.Codigo,
                    Codigo,
                    StringComparison.OrdinalIgnoreCase));

            switch (Seccion)
            {
                case "partidos":
                    Partidos =
                        await _futbol.ObtenerPartidosCompeticionAsync(
                            Codigo,
                            cancellationToken);
                    break;
                case "goleadores":
                    Goleadores = await _futbol.ObtenerGoleadoresAsync(
                        Codigo,
                        cancellationToken);
                    break;
                case "equipos":
                    Equipos = await _futbol.ObtenerEquiposAsync(
                        Codigo,
                        cancellationToken);
                    break;
                default:
                    Clasificacion =
                        await _futbol.ObtenerClasificacionAsync(
                            Codigo,
                            cancellationToken);
                    break;
            }
        }
        catch (FutbolApiExcepcion excepcion)
        {
            ErrorApi = excepcion.Message;
        }

        return Page();
    }
}
