using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Peliculas.Modelos;
using Peliculas.Servicios;

namespace Peliculas.Pages.Favoritos;

// Recibe los formularios de los corazones de toda la aplicación.
[Authorize]
public class AlternarModel : PageModel
{
    private readonly IFavoritosServicio _favoritos;
    private readonly ITmdbServicio _tmdb;
    private readonly UserManager<Usuario> _userManager;

    public AlternarModel(
        IFavoritosServicio favoritos,
        ITmdbServicio tmdb,
        UserManager<Usuario> userManager)
    {
        _favoritos = favoritos;
        _tmdb = tmdb;
        _userManager = userManager;
    }

    public async Task<IActionResult> OnPostAsync(
        int peliculaId,
        string accion,
        string? returnUrl,
        CancellationToken cancellationToken)
    {
        string? usuarioId = _userManager.GetUserId(User);
        if (usuarioId is null)
        {
            return Challenge();
        }

        if (peliculaId <= 0)
        {
            TempData["Error"] = "El identificador de la película no es válido.";
            return RedirigirLocal(returnUrl);
        }

        try
        {
            if (string.Equals(accion, "quitar", StringComparison.OrdinalIgnoreCase))
            {
                await _favoritos.QuitarAsync(
                    usuarioId, peliculaId, cancellationToken);
                TempData["Mensaje"] = "La película se ha quitado de tus favoritas.";
            }
            else
            {
                // Se consulta el detalle para guardar una copia local útil y actual.
                PeliculaDetalle pelicula = await _tmdb.ObtenerDetalleAsync(
                    peliculaId, cancellationToken);
                await _favoritos.AgregarAsync(
                    usuarioId, pelicula, cancellationToken);
                TempData["Mensaje"] = "La película se ha añadido a tus favoritas.";
            }
        }
        catch (TmdbExcepcion excepcion)
        {
            TempData["Error"] = excepcion.Message;
        }

        return RedirigirLocal(returnUrl);
    }

    private IActionResult RedirigirLocal(string? returnUrl)
    {
        // Solo se permiten destinos internos para evitar redirecciones maliciosas.
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }

        return RedirectToPage("/Favoritos/Index");
    }
}
