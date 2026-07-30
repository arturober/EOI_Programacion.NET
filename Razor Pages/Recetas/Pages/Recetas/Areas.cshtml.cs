using Microsoft.AspNetCore.Mvc.RazorPages;
using Recetas.Servicios;

namespace Recetas.Pages.Recetas;

// Obtiene las regiones disponibles para explorar cocinas del mundo.
public class AreasModel : PageModel
{
    private readonly ITheMealDbServicio _theMealDb;

    public AreasModel(ITheMealDbServicio theMealDb)
    {
        _theMealDb = theMealDb;
    }

    public IReadOnlyList<string> Areas { get; private set; } = [];

    public async Task OnGetAsync()
    {
        try
        {
            Areas = await _theMealDb.ObtenerAreasAsync(
                HttpContext.RequestAborted);
        }
        catch (TheMealDbExcepcion excepcion)
        {
            ViewData["Error"] = excepcion.Message;
        }
    }
}
