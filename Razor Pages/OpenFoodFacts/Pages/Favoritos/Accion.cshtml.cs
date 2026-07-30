using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenFoodFacts.Modelos;
using OpenFoodFacts.Servicios;

namespace OpenFoodFacts.Pages.Favoritos;

// Añade o quita un producto de la lista privada del usuario.
[Authorize]
public class AccionModel : PageModel
{
    private readonly IOpenFoodFactsServicio _openFoodFacts;
    private readonly IColeccionServicio _coleccion;
    private readonly UserManager<Usuario> _userManager;

    public AccionModel(
        IOpenFoodFactsServicio openFoodFacts,
        IColeccionServicio coleccion,
        UserManager<Usuario> userManager)
    {
        _openFoodFacts = openFoodFacts;
        _coleccion = coleccion;
        _userManager = userManager;
    }

    public IActionResult OnGet() => RedirectToPage("/Index");

    public async Task<IActionResult> OnPostAsync(
        string codigo,
        string accion,
        string? returnUrl)
    {
        string? usuarioId = _userManager.GetUserId(User);
        if (usuarioId is null)
        {
            return Challenge();
        }

        if (string.IsNullOrWhiteSpace(codigo))
        {
            TempData["Error"] = "No se ha recibido el producto.";
            return Volver(returnUrl);
        }

        try
        {
            if (accion.Equals(
                "quitar",
                StringComparison.OrdinalIgnoreCase))
            {
                await _coleccion.QuitarFavoritoAsync(
                    usuarioId,
                    codigo,
                    HttpContext.RequestAborted);

                TempData["Mensaje"] =
                    "El producto se ha quitado de favoritos.";
            }
            else
            {
                ProductoDetalle producto =
                    await _openFoodFacts.ObtenerProductoAsync(
                        codigo,
                        HttpContext.RequestAborted);

                await _coleccion.AgregarFavoritoAsync(
                    usuarioId,
                    producto,
                    HttpContext.RequestAborted);

                TempData["Mensaje"] =
                    "El producto se ha añadido a favoritos.";
            }
        }
        catch (OpenFoodFactsExcepcion excepcion)
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
