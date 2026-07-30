using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Videojuegos.Modelos;
using Videojuegos.Servicios;

namespace Videojuegos.Pages.Videojuegos;

// Muestra una de las colecciones disponibles en el menú Explorar.
public class IndexModel : PageModel
{
    private readonly IRawgServicio _rawg;
    private readonly IBibliotecaServicio _biblioteca;
    private readonly UserManager<Usuario> _userManager;

    public IndexModel(
        IRawgServicio rawg,
        IBibliotecaServicio biblioteca,
        UserManager<Usuario> userManager)
    {
        _rawg = rawg;
        _biblioteca = biblioteca;
        _userManager = userManager;
    }

    public TipoListado Tipo { get; private set; }
    public PaginaVideojuegos Resultado { get; private set; } = new();

    public async Task OnGetAsync(string? tipo, int pagina = 1)
    {
        Tipo = TipoListadoExtensiones.DesdeTexto(tipo);

        try
        {
            Resultado = await _rawg.ObtenerListadoAsync(
                Tipo,
                pagina,
                HttpContext.RequestAborted);

            await MarcarBibliotecaAsync();
        }
        catch (RawgExcepcion excepcion)
        {
            ViewData["Error"] = excepcion.Message;
        }
    }

    private async Task MarcarBibliotecaAsync()
    {
        string? usuarioId = _userManager.GetUserId(User);
        if (usuarioId is null)
        {
            return;
        }

        HashSet<int> ids = await _biblioteca.ObtenerIdsAsync(
            usuarioId,
            HttpContext.RequestAborted);

        foreach (VideojuegoResumen videojuego in Resultado.Resultados)
        {
            videojuego.EstaEnBiblioteca = ids.Contains(videojuego.Id);
        }
    }
}
