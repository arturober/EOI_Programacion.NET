using Microsoft.AspNetCore.Mvc.RazorPages;
using Recetas.Modelos;
using Recetas.Servicios;

namespace Recetas.Pages.Recetas;

// Obtiene las categorías con su imagen y descripción.
public class CategoriasModel : PageModel
{
    private readonly ITheMealDbServicio _theMealDb;

    public CategoriasModel(ITheMealDbServicio theMealDb)
    {
        _theMealDb = theMealDb;
    }

    public IReadOnlyList<CategoriaReceta> Categorias { get; private set; } = [];

    public async Task OnGetAsync()
    {
        try
        {
            Categorias = await _theMealDb.ObtenerCategoriasAsync(
                HttpContext.RequestAborted);
        }
        catch (TheMealDbExcepcion excepcion)
        {
            ViewData["Error"] = excepcion.Message;
        }
    }
}
