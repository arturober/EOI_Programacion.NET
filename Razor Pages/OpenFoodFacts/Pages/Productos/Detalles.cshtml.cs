using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenFoodFacts.Modelos;
using OpenFoodFacts.Servicios;

namespace OpenFoodFacts.Pages.Productos;

// Obtiene todos los datos disponibles para la ficha del producto.
public class DetallesModel : PageModel
{
    private readonly IOpenFoodFactsServicio _openFoodFacts;
    private readonly IColeccionServicio _coleccion;
    private readonly UserManager<Usuario> _userManager;

    public DetallesModel(
        IOpenFoodFactsServicio openFoodFacts,
        IColeccionServicio coleccion,
        UserManager<Usuario> userManager)
    {
        _openFoodFacts = openFoodFacts;
        _coleccion = coleccion;
        _userManager = userManager;
    }

    public ProductoDetalle Producto { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(string codigo)
    {
        try
        {
            Producto = await _openFoodFacts.ObtenerProductoAsync(
                codigo,
                HttpContext.RequestAborted);

            string? usuarioId = _userManager.GetUserId(User);
            if (usuarioId is not null)
            {
                HashSet<string> codigos =
                    await _coleccion.ObtenerCodigosFavoritosAsync(
                        usuarioId,
                        HttpContext.RequestAborted);

                Producto.EsFavorito = codigos.Contains(Producto.Codigo);
            }

            return Page();
        }
        catch (OpenFoodFactsExcepcion excepcion)
            when (excepcion.CodigoEstado == HttpStatusCode.NotFound)
        {
            return NotFound();
        }
        catch (OpenFoodFactsExcepcion excepcion)
        {
            ViewData["Error"] = excepcion.Message;
            return Page();
        }
    }
}
