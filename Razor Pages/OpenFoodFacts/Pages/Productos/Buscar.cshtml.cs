using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenFoodFacts.Modelos;
using OpenFoodFacts.Servicios;

namespace OpenFoodFacts.Pages.Productos;

// Busca por texto o redirige directamente si recibe un código de barras.
public class BuscarModel : PageModel
{
    private readonly IOpenFoodFactsServicio _openFoodFacts;
    private readonly IColeccionServicio _coleccion;
    private readonly UserManager<Usuario> _userManager;

    public BuscarModel(
        IOpenFoodFactsServicio openFoodFacts,
        IColeccionServicio coleccion,
        UserManager<Usuario> userManager)
    {
        _openFoodFacts = openFoodFacts;
        _coleccion = coleccion;
        _userManager = userManager;
    }

    public string Texto { get; private set; } = "";
    public ResultadoProductos Resultado { get; private set; } = new();
    public Paginacion Paginacion { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(
        string? texto,
        int pagina = 1)
    {
        Texto = texto?.Trim() ?? "";

        if (EsCodigoBarras(Texto))
        {
            return RedirectToPage(
                "/Productos/Detalles",
                new { codigo = Texto });
        }

        if (Texto.Length < 2)
        {
            if (Texto.Length > 0)
            {
                ViewData["Error"] =
                    "La búsqueda debe contener al menos dos caracteres.";
            }

            return Page();
        }

        try
        {
            Resultado = await _openFoodFacts.BuscarAsync(
                Texto,
                pagina,
                HttpContext.RequestAborted);

            await MarcarFavoritosAsync();
            PrepararPaginacion();
        }
        catch (OpenFoodFactsExcepcion excepcion)
        {
            ViewData["Error"] = excepcion.Message;
        }

        return Page();
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
            PaginaRazor = "/Productos/Buscar",
            Texto = Texto
        };
    }

    private static bool EsCodigoBarras(string texto) =>
        texto.Length is >= 4 and <= 24
        && texto.All(char.IsDigit);
}
