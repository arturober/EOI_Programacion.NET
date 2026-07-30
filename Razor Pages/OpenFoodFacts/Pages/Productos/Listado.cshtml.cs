using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenFoodFacts.Modelos;
using OpenFoodFacts.Servicios;

namespace OpenFoodFacts.Pages.Productos;

// Reutiliza la misma página para categorías y notas de Nutri-Score.
public class ListadoModel : PageModel
{
    private readonly IOpenFoodFactsServicio _openFoodFacts;
    private readonly IColeccionServicio _coleccion;
    private readonly UserManager<Usuario> _userManager;

    public ListadoModel(
        IOpenFoodFactsServicio openFoodFacts,
        IColeccionServicio coleccion,
        UserManager<Usuario> userManager)
    {
        _openFoodFacts = openFoodFacts;
        _coleccion = coleccion;
        _userManager = userManager;
    }

    public string Tipo { get; private set; } = "categoria";
    public string Valor { get; private set; } = "";
    public string Titulo { get; private set; } = "";
    public ResultadoProductos Resultado { get; private set; } = new();
    public Paginacion Paginacion { get; private set; } = new();

    public async Task OnGetAsync(
        string? tipo,
        string? valor,
        int pagina = 1)
    {
        Tipo = tipo?.Equals(
            "nutriscore",
            StringComparison.OrdinalIgnoreCase) == true
                ? "nutriscore"
                : "categoria";
        Valor = valor?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(Valor))
        {
            ViewData["Error"] = "No se ha indicado el filtro.";
            return;
        }

        try
        {
            if (Tipo == "nutriscore")
            {
                string nota = Valor.ToLowerInvariant();
                if (nota is not ("a" or "b" or "c" or "d" or "e"))
                {
                    ViewData["Error"] = "La nota de Nutri-Score no es válida.";
                    return;
                }

                Valor = nota;
                Titulo = $"Nutri-Score {nota.ToUpperInvariant()}";
                Resultado =
                    await _openFoodFacts.FiltrarNutriScoreAsync(
                        nota,
                        pagina,
                        HttpContext.RequestAborted);
            }
            else
            {
                CategoriaProducto? categoria =
                    CatalogoCategorias.Buscar(Valor);
                if (categoria is null)
                {
                    ViewData["Error"] = "La categoría no es válida.";
                    return;
                }

                Valor = categoria.Filtro;
                Titulo = categoria.Nombre;
                Resultado =
                    await _openFoodFacts.FiltrarCategoriaAsync(
                        categoria.Filtro,
                        pagina,
                        HttpContext.RequestAborted);
            }

            await MarcarFavoritosAsync();
            PrepararPaginacion();
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

        foreach (ProductoResumen producto in Resultado.Productos)
        {
            producto.EsFavorito = codigos.Contains(producto.Codigo);
        }
    }

    private void PrepararPaginacion()
    {
        Paginacion = new Paginacion
        {
            Pagina = Resultado.Pagina,
            TotalPaginas = Math.Min(Resultado.TotalPaginas, 1000),
            PaginaRazor = "/Productos/Listado",
            Tipo = Tipo,
            Valor = Valor
        };
    }
}
