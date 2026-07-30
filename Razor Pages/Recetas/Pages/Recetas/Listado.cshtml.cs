using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Recetas.Modelos;
using Recetas.Servicios;

namespace Recetas.Pages.Recetas;

// Reutiliza la misma página para categorías y cocinas del mundo.
public class ListadoModel : PageModel
{
    private readonly ITheMealDbServicio _theMealDb;
    private readonly IColeccionServicio _coleccion;
    private readonly UserManager<Usuario> _userManager;

    public ListadoModel(
        ITheMealDbServicio theMealDb,
        IColeccionServicio coleccion,
        UserManager<Usuario> userManager)
    {
        _theMealDb = theMealDb;
        _coleccion = coleccion;
        _userManager = userManager;
    }

    public string Tipo { get; private set; } = "categoria";
    public string Valor { get; private set; } = "";
    public IReadOnlyList<RecetaResumen> Resultados { get; private set; } = [];

    public async Task OnGetAsync(string? tipo, string? valor)
    {
        Tipo = tipo?.Equals(
            "area",
            StringComparison.OrdinalIgnoreCase) == true
                ? "area"
                : "categoria";
        Valor = valor?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(Valor))
        {
            ViewData["Error"] = "No se ha indicado el filtro.";
            return;
        }

        try
        {
            Resultados = Tipo == "area"
                ? await _theMealDb.FiltrarAreaAsync(
                    Valor,
                    HttpContext.RequestAborted)
                : await _theMealDb.FiltrarCategoriaAsync(
                    Valor,
                    HttpContext.RequestAborted);

            string? usuarioId = _userManager.GetUserId(User);
            if (usuarioId is not null)
            {
                HashSet<int> ids =
                    await _coleccion.ObtenerIdsFavoritosAsync(
                        usuarioId,
                        HttpContext.RequestAborted);

                foreach (RecetaResumen receta in Resultados)
                {
                    receta.EsFavorita = ids.Contains(receta.Id);
                }
            }
        }
        catch (TheMealDbExcepcion excepcion)
        {
            ViewData["Error"] = excepcion.Message;
        }
    }
}
