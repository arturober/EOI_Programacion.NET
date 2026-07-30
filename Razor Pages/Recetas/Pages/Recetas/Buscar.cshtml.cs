using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Recetas.Modelos;
using Recetas.Servicios;

namespace Recetas.Pages.Recetas;

// Atiende las búsquedas por nombre de receta.
public class BuscarModel : PageModel
{
    private readonly ITheMealDbServicio _theMealDb;
    private readonly IColeccionServicio _coleccion;
    private readonly UserManager<Usuario> _userManager;

    public BuscarModel(
        ITheMealDbServicio theMealDb,
        IColeccionServicio coleccion,
        UserManager<Usuario> userManager)
    {
        _theMealDb = theMealDb;
        _coleccion = coleccion;
        _userManager = userManager;
    }

    public string Texto { get; private set; } = "";
    public IReadOnlyList<RecetaResumen> Resultados { get; private set; } = [];

    public async Task OnGetAsync(string? texto)
    {
        Texto = texto?.Trim() ?? "";

        if (Texto.Length < 2)
        {
            if (Texto.Length > 0)
            {
                ViewData["Error"] =
                    "La búsqueda debe contener al menos dos caracteres.";
            }

            return;
        }

        try
        {
            Resultados = await _theMealDb.BuscarAsync(
                Texto,
                HttpContext.RequestAborted);

            await MarcarFavoritasAsync();
        }
        catch (TheMealDbExcepcion excepcion)
        {
            ViewData["Error"] = excepcion.Message;
        }
    }

    private async Task MarcarFavoritasAsync()
    {
        string? usuarioId = _userManager.GetUserId(User);
        if (usuarioId is null)
        {
            return;
        }

        HashSet<int> ids = await _coleccion.ObtenerIdsFavoritosAsync(
            usuarioId,
            HttpContext.RequestAborted);

        foreach (RecetaResumen receta in Resultados)
        {
            receta.EsFavorita = ids.Contains(receta.Id);
        }
    }
}
