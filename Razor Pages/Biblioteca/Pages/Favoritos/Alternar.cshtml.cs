using Biblioteca.Modelos;
using Biblioteca.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Biblioteca.Pages.Favoritos;

// Procesa el botón de favorito y devuelve al usuario a la página anterior.
[Authorize]
public class AlternarModel : PageModel
{
    private readonly IOpenLibraryServicio _openLibrary;
    private readonly IFavoritosServicio _favoritos;
    private readonly UserManager<Usuario> _userManager;

    public AlternarModel(
        IOpenLibraryServicio openLibrary,
        IFavoritosServicio favoritos,
        UserManager<Usuario> userManager)
    {
        _openLibrary = openLibrary;
        _favoritos = favoritos;
        _userManager = userManager;
    }

    public IActionResult OnGet()
    {
        // Esta página solo admite POST para evitar cambios mediante enlaces.
        return RedirectToPage("/Index");
    }

    public async Task<IActionResult> OnPostAsync(
        string libroId,
        string accion,
        string? returnUrl)
    {
        string? usuarioId = _userManager.GetUserId(User);
        if (usuarioId is null)
        {
            return Challenge();
        }

        if (string.IsNullOrWhiteSpace(libroId))
        {
            TempData["Error"] = "No se ha recibido el libro.";
            return Volver(returnUrl);
        }

        try
        {
            if (accion.Equals("quitar", StringComparison.OrdinalIgnoreCase))
            {
                await _favoritos.QuitarAsync(
                    usuarioId,
                    libroId,
                    HttpContext.RequestAborted);

                TempData["Mensaje"] = "El libro se ha quitado de favoritos.";
            }
            else
            {
                // Se obtiene la ficha para guardar una copia útil en SQLite.
                LibroDetalle libro = await _openLibrary.ObtenerDetalleAsync(
                    libroId,
                    HttpContext.RequestAborted);

                await _favoritos.AgregarAsync(
                    usuarioId,
                    libro,
                    HttpContext.RequestAborted);

                TempData["Mensaje"] = "El libro se ha añadido a favoritos.";
            }
        }
        catch (OpenLibraryExcepcion excepcion)
        {
            TempData["Error"] = excepcion.Message;
        }

        return Volver(returnUrl);
    }

    private IActionResult Volver(string? returnUrl)
    {
        // Solo se aceptan rutas internas para impedir redirecciones externas.
        return Url.IsLocalUrl(returnUrl)
            ? LocalRedirect(returnUrl!)
            : RedirectToPage("/Favoritos/Index");
    }
}
