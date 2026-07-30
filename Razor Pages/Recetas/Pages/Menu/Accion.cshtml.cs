using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Recetas.Modelos;
using Recetas.Servicios;

namespace Recetas.Pages.Menu;

// Asigna o elimina la receta de un día del menú semanal.
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

    public IActionResult OnGet() => RedirectToPage("/Menu/Index");

    public async Task<IActionResult> OnPostAsync(
        string accion,
        DiaMenu dia,
        int recetaId = 0,
        string? returnUrl = null)
    {
        string? usuarioId = _userManager.GetUserId(User);
        if (usuarioId is null)
        {
            return Challenge();
        }

        if (!Enum.IsDefined(dia))
        {
            TempData["Error"] = "El día de la semana no es válido.";
            return Volver(returnUrl);
        }

        try
        {
            if (accion.Equals("quitar", StringComparison.OrdinalIgnoreCase))
            {
                await _coleccion.QuitarDiaAsync(
                    usuarioId,
                    dia,
                    HttpContext.RequestAborted);

                TempData["Mensaje"] =
                    $"Se ha vaciado el {dia.Titulo().ToLowerInvariant()}.";
            }
            else
            {
                RecetaDetalle receta =
                    await _theMealDb.ObtenerDetalleAsync(
                        recetaId,
                        HttpContext.RequestAborted);

                await _coleccion.AsignarDiaAsync(
                    usuarioId,
                    dia,
                    receta,
                    HttpContext.RequestAborted);

                TempData["Mensaje"] =
                    $"La receta se ha añadido al {dia.Titulo().ToLowerInvariant()}.";
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
            : RedirectToPage("/Menu/Index");
    }
}
