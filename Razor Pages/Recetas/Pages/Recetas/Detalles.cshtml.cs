using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Recetas.Modelos;
using Recetas.Servicios;

namespace Recetas.Pages.Recetas;

// Obtiene instrucciones, ingredientes y enlaces de la receta.
public class DetallesModel : PageModel
{
    private readonly ITheMealDbServicio _theMealDb;
    private readonly IColeccionServicio _coleccion;
    private readonly UserManager<Usuario> _userManager;

    public DetallesModel(
        ITheMealDbServicio theMealDb,
        IColeccionServicio coleccion,
        UserManager<Usuario> userManager)
    {
        _theMealDb = theMealDb;
        _coleccion = coleccion;
        _userManager = userManager;
    }

    public RecetaDetalle Receta { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            Receta = await _theMealDb.ObtenerDetalleAsync(
                id,
                HttpContext.RequestAborted);

            string? usuarioId = _userManager.GetUserId(User);
            if (usuarioId is not null)
            {
                HashSet<int> ids =
                    await _coleccion.ObtenerIdsFavoritosAsync(
                        usuarioId,
                        HttpContext.RequestAborted);

                Receta.EsFavorita = ids.Contains(Receta.Id);
            }

            return Page();
        }
        catch (TheMealDbExcepcion excepcion)
            when (excepcion.CodigoEstado == HttpStatusCode.NotFound)
        {
            return NotFound();
        }
        catch (TheMealDbExcepcion excepcion)
        {
            ViewData["Error"] = excepcion.Message;
            return Page();
        }
    }
}
