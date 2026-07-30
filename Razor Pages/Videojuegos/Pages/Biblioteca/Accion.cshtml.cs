using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Videojuegos.Modelos;
using Videojuegos.Servicios;

namespace Videojuegos.Pages.Biblioteca;

// Añade, actualiza o elimina videojuegos de la colección privada.
[Authorize]
public class AccionModel : PageModel
{
    private readonly IRawgServicio _rawg;
    private readonly IBibliotecaServicio _biblioteca;
    private readonly UserManager<Usuario> _userManager;

    public AccionModel(
        IRawgServicio rawg,
        IBibliotecaServicio biblioteca,
        UserManager<Usuario> userManager)
    {
        _rawg = rawg;
        _biblioteca = biblioteca;
        _userManager = userManager;
    }

    public IActionResult OnGet()
    {
        // Esta página solo admite POST para evitar cambios mediante enlaces.
        return RedirectToPage("/Index");
    }

    public async Task<IActionResult> OnPostAsync(
        int videojuegoId,
        string accion,
        EstadoVideojuego estado = EstadoVideojuego.Pendiente,
        int? puntuacion = null,
        string? comentario = null,
        string? returnUrl = null)
    {
        string? usuarioId = _userManager.GetUserId(User);
        if (usuarioId is null)
        {
            return Challenge();
        }

        if (videojuegoId <= 0)
        {
            TempData["Error"] = "No se ha recibido el videojuego.";
            return Volver(returnUrl);
        }

        try
        {
            switch (accion.Trim().ToLowerInvariant())
            {
                case "quitar":
                    await _biblioteca.QuitarAsync(
                        usuarioId,
                        videojuegoId,
                        HttpContext.RequestAborted);

                    TempData["Mensaje"] =
                        "El videojuego se ha quitado de tu biblioteca.";
                    break;

                case "actualizar":
                    if (!Enum.IsDefined(estado))
                    {
                        TempData["Error"] = "El estado no es válido.";
                        break;
                    }

                    if (comentario?.Length > 500)
                    {
                        TempData["Error"] =
                            "El comentario no puede superar 500 caracteres.";
                        break;
                    }

                    await _biblioteca.ActualizarAsync(
                        usuarioId,
                        videojuegoId,
                        estado,
                        puntuacion,
                        comentario,
                        HttpContext.RequestAborted);

                    TempData["Mensaje"] =
                        "Los datos personales se han actualizado.";
                    break;

                default:
                    // La ficha permite guardar una copia útil en SQLite.
                    VideojuegoDetalle videojuego =
                        await _rawg.ObtenerDetalleAsync(
                            videojuegoId,
                            HttpContext.RequestAborted);

                    await _biblioteca.AgregarAsync(
                        usuarioId,
                        videojuego,
                        estado,
                        HttpContext.RequestAborted);

                    TempData["Mensaje"] =
                        "El videojuego se ha añadido a tu biblioteca.";
                    break;
            }
        }
        catch (RawgExcepcion excepcion)
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
            : RedirectToPage("/Biblioteca/Index");
    }
}
