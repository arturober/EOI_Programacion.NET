using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenFoodFacts.Modelos;
using OpenFoodFacts.Servicios;

namespace OpenFoodFacts.Pages.Comparar;

// Compara los datos guardados de entre dos y cuatro favoritos.
[Authorize]
public class IndexModel : PageModel
{
    private readonly IColeccionServicio _coleccion;
    private readonly UserManager<Usuario> _userManager;

    public IndexModel(
        IColeccionServicio coleccion,
        UserManager<Usuario> userManager)
    {
        _coleccion = coleccion;
        _userManager = userManager;
    }

    public IReadOnlyList<ProductoGuardado> Favoritos { get; private set; } = [];
    public IReadOnlyList<ProductoGuardado> Comparados { get; private set; } = [];
    public HashSet<string> Seleccionados { get; private set; } = [];

    public async Task OnGetAsync(string[]? codigos)
    {
        string usuarioId = _userManager.GetUserId(User)!;

        IReadOnlyList<Favorito> favoritos =
            await _coleccion.ListarFavoritosAsync(
                usuarioId,
                HttpContext.RequestAborted);

        Favoritos = favoritos
            .Select(elemento => elemento.Producto)
            .OrderBy(producto => producto.Nombre)
            .ToList()
            .AsReadOnly();

        Seleccionados = (codigos ?? [])
            .Where(codigo => !string.IsNullOrWhiteSpace(codigo))
            .Distinct(StringComparer.Ordinal)
            .Take(4)
            .ToHashSet(StringComparer.Ordinal);

        if (Seleccionados.Count > 0)
        {
            Comparados = await _coleccion.ObtenerParaCompararAsync(
                usuarioId,
                Seleccionados,
                HttpContext.RequestAborted);
        }

        if ((codigos?.Length ?? 0) > 4)
        {
            ViewData["Error"] =
                "Solo se pueden comparar cuatro productos a la vez.";
        }
    }
}
