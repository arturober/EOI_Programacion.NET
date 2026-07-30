using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Videojuegos.Modelos;
using Videojuegos.Servicios;

namespace Videojuegos.Pages.Videojuegos;

// Atiende las búsquedas por nombre de videojuego.
public class BuscarModel : PageModel
{
    private readonly IRawgServicio _rawg;
    private readonly IBibliotecaServicio _biblioteca;
    private readonly UserManager<Usuario> _userManager;

    public BuscarModel(
        IRawgServicio rawg,
        IBibliotecaServicio biblioteca,
        UserManager<Usuario> userManager)
    {
        _rawg = rawg;
        _biblioteca = biblioteca;
        _userManager = userManager;
    }

    public string Texto { get; private set; } = "";
    public PaginaVideojuegos Resultado { get; private set; } = new();

    public async Task OnGetAsync(string? texto, int pagina = 1)
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
            Resultado = await _rawg.BuscarAsync(
                Texto,
                pagina,
                HttpContext.RequestAborted);

            string? usuarioId = _userManager.GetUserId(User);
            if (usuarioId is not null)
            {
                HashSet<int> ids = await _biblioteca.ObtenerIdsAsync(
                    usuarioId,
                    HttpContext.RequestAborted);

                foreach (VideojuegoResumen videojuego in Resultado.Resultados)
                {
                    videojuego.EstaEnBiblioteca = ids.Contains(videojuego.Id);
                }
            }
        }
        catch (RawgExcepcion excepcion)
        {
            ViewData["Error"] = excepcion.Message;
        }
    }
}
