using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Videojuegos.Modelos;
using Videojuegos.Servicios;

namespace Videojuegos.Pages.Videojuegos;

// Obtiene la ficha completa y marca si pertenece a la biblioteca.
public class DetallesModel : PageModel
{
    private readonly IRawgServicio _rawg;
    private readonly IBibliotecaServicio _biblioteca;
    private readonly UserManager<Usuario> _userManager;

    public DetallesModel(
        IRawgServicio rawg,
        IBibliotecaServicio biblioteca,
        UserManager<Usuario> userManager)
    {
        _rawg = rawg;
        _biblioteca = biblioteca;
        _userManager = userManager;
    }

    public VideojuegoDetalle Videojuego { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            Videojuego = await _rawg.ObtenerDetalleAsync(
                id,
                HttpContext.RequestAborted);

            string? usuarioId = _userManager.GetUserId(User);
            if (usuarioId is not null)
            {
                HashSet<int> ids = await _biblioteca.ObtenerIdsAsync(
                    usuarioId,
                    HttpContext.RequestAborted);

                Videojuego.EstaEnBiblioteca = ids.Contains(Videojuego.Id);
            }

            return Page();
        }
        catch (RawgExcepcion excepcion)
            when (excepcion.CodigoEstado == HttpStatusCode.NotFound)
        {
            return NotFound();
        }
        catch (RawgExcepcion excepcion)
        {
            ViewData["Error"] = excepcion.Message;
            return Page();
        }
    }
}
