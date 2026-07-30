using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenFoodFacts.Modelos;

namespace OpenFoodFacts.Pages.Productos;

// Expone el catálogo local de categorías sin realizar una petición externa.
public class CategoriasModel : PageModel
{
    public IReadOnlyList<CategoriaProducto> Categorias =>
        CatalogoCategorias.Todas;

    public void OnGet()
    {
    }
}
