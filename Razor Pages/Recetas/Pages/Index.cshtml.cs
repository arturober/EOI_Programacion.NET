using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Recetas.Modelos;
using Recetas.Servicios;

namespace Recetas.Pages;

// Prepara la receta aleatoria y varias selecciones de inicio.
public class IndexModel : PageModel
{
    private readonly ITheMealDbServicio _theMealDb;
    private readonly IColeccionServicio _coleccion;
    private readonly UserManager<Usuario> _userManager;

    public IndexModel(
        ITheMealDbServicio theMealDb,
        IColeccionServicio coleccion,
        UserManager<Usuario> userManager)
    {
        _theMealDb = theMealDb;
        _coleccion = coleccion;
        _userManager = userManager;
    }

    public RecetaDetalle? Aleatoria { get; private set; }
    public IReadOnlyList<RecetaResumen> Vegetarianas { get; private set; } = [];
    public IReadOnlyList<RecetaResumen> Pescado { get; private set; } = [];
    public IReadOnlyList<RecetaResumen> Postres { get; private set; } = [];

    public async Task OnGetAsync()
    {
        try
        {
            Aleatoria = await _theMealDb.ObtenerAleatoriaAsync(
                HttpContext.RequestAborted);

            Vegetarianas = (await _theMealDb.FiltrarCategoriaAsync(
                    "Vegetarian",
                    HttpContext.RequestAborted))
                .Take(4)
                .ToList()
                .AsReadOnly();

            Pescado = (await _theMealDb.FiltrarCategoriaAsync(
                    "Seafood",
                    HttpContext.RequestAborted))
                .Take(4)
                .ToList()
                .AsReadOnly();

            Postres = (await _theMealDb.FiltrarCategoriaAsync(
                    "Dessert",
                    HttpContext.RequestAborted))
                .Take(4)
                .ToList()
                .AsReadOnly();

            IEnumerable<RecetaResumen> recetas =
                Vegetarianas.Concat(Pescado).Concat(Postres);

            if (Aleatoria is not null)
            {
                recetas = recetas.Append(Aleatoria);
            }

            await MarcarFavoritasAsync(recetas);
        }
        catch (TheMealDbExcepcion excepcion)
        {
            ViewData["Error"] = excepcion.Message;
        }
    }

    private async Task MarcarFavoritasAsync(
        IEnumerable<RecetaResumen> recetas)
    {
        string? usuarioId = _userManager.GetUserId(User);
        if (usuarioId is null)
        {
            return;
        }

        HashSet<int> ids = await _coleccion.ObtenerIdsFavoritosAsync(
            usuarioId,
            HttpContext.RequestAborted);

        foreach (RecetaResumen receta in recetas)
        {
            receta.EsFavorita = ids.Contains(receta.Id);
        }
    }
}
