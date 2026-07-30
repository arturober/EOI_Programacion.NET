using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenFoodFacts.Modelos;
using OpenFoodFacts.Servicios;

namespace OpenFoodFacts.Pages;

// Prepara los productos populares que aparecen en la portada.
public class IndexModel : PageModel
{
    private readonly IOpenFoodFactsServicio _openFoodFacts;
    private readonly IColeccionServicio _coleccion;
    private readonly UserManager<Usuario> _userManager;

    public IndexModel(
        IOpenFoodFactsServicio openFoodFacts,
        IColeccionServicio coleccion,
        UserManager<Usuario> userManager)
    {
        _openFoodFacts = openFoodFacts;
        _coleccion = coleccion;
        _userManager = userManager;
    }

    public IReadOnlyList<ProductoResumen> Destacados { get; private set; } = [];

    public async Task OnGetAsync()
    {
        try
        {
            ResultadoProductos resultado =
                await _openFoodFacts.ObtenerDestacadosAsync(
                    cancellationToken: HttpContext.RequestAborted);

            Destacados = resultado.Productos;
            await MarcarFavoritosAsync();
        }
        catch (OpenFoodFactsExcepcion excepcion)
        {
            ViewData["Error"] = excepcion.Message;
        }
    }

    private async Task MarcarFavoritosAsync()
    {
        string? usuarioId = _userManager.GetUserId(User);
        if (usuarioId is null)
        {
            return;
        }

        HashSet<string> codigos =
            await _coleccion.ObtenerCodigosFavoritosAsync(
                usuarioId,
                HttpContext.RequestAborted);

        foreach (ProductoResumen producto in Destacados)
        {
            producto.EsFavorito = codigos.Contains(producto.Codigo);
        }
    }
}
