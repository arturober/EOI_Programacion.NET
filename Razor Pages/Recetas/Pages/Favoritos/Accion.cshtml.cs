using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Recetas.Modelos;
using Recetas.Servicios;

namespace Recetas.Pages.Favoritos;

// Añade o quita una receta de la lista privada de favoritos.
[Authorize]
public class AccionModel : PageModel
{
    private readonly ITheMealDbServicio _theMealDb;
    private readonly IColeccionServicio _coleccion;
    private readonly UserManager<Usuario> _userManager;

    public AccionModel(
        ITheMealDbServicio theMealDb,
        IColeccionServicio coleccion,
        UserManager<Usuario> userManager)
    {
        _theMealDb = theMealDb;
        _coleccion = coleccion;
        _userManager = userManager;
    }

    public IActionResult OnGet() => RedirectToPage("/Index");

    public async Task<IActionResult> OnPostAsync(
        int recetaId,
        string accion,
        string? returnUrl)
    {
        string? usuarioId = _userManager.GetUserId(User);
        if (usuarioId is null)
        {
            return Challenge();
        }

        if (recetaId <= 0)
        {
            TempData["Error"] = "No se ha recibido la receta.";
            return Volver(returnUrl);
        }

        try
        {
            if (accion.Equals("quitar", StringComparison.OrdinalIgnoreCase))
            {
                await _coleccion.QuitarFavoritoAsync(
                    usuarioId,
                    recetaId,
                    HttpContext.RequestAborted);

                TempData["Mensaje"] = "La receta se ha quitado de favoritos.";
            }
            else
            {
                RecetaDetalle receta =
                    await _theMealDb.ObtenerDetalleAsync(
                        recetaId,
                        HttpContext.RequestAborted);

                await _coleccion.AgregarFavoritoAsync(
                    usuarioId,
                    receta,
                    HttpContext.RequestAborted);

                TempData["Mensaje"] = "La receta se ha añadido a favoritos.";
            }
        }
        catch (TheMealDbExcepcion excepcion)
        {
            TempData["Error"] = excepcion.Message;
        }

        return Volver(returnUrl);
    }

    private IActionResult Volver(string? returnUrl)
    {
        return Url.IsLocalUrl(returnUrl)
            ? LocalRedirect(returnUrl!)
            : RedirectToPage("/Favoritos/Index");
    }
}
