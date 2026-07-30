using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Videojuegos.Configuracion;
using Videojuegos.Modelos;
using Videojuegos.Servicios;

namespace Videojuegos.Pages;

// Prepara los escaparates de la página de inicio.
public class IndexModel : PageModel
{
    private readonly IRawgServicio _rawg;
    private readonly IBibliotecaServicio _biblioteca;
    private readonly UserManager<Usuario> _userManager;
    private readonly RawgOpciones _opciones;

    public IndexModel(
        IRawgServicio rawg,
        IBibliotecaServicio biblioteca,
        UserManager<Usuario> userManager,
        IOptions<RawgOpciones> opciones)
    {
        _rawg = rawg;
        _biblioteca = biblioteca;
        _userManager = userManager;
        _opciones = opciones.Value;
    }

    public IReadOnlyList<VideojuegoResumen> Populares { get; private set; } = [];
    public IReadOnlyList<VideojuegoResumen> MejorValorados { get; private set; } = [];
    public IReadOnlyList<VideojuegoResumen> Proximamente { get; private set; } = [];
    public bool ApiKeyConfigurada => _opciones.TieneApiKey;

    public async Task OnGetAsync()
    {
        // Sin clave se muestra la ayuda y no se realizan peticiones fallidas.
        if (!ApiKeyConfigurada)
        {
            return;
        }

        try
        {
            PaginaVideojuegos populares =
                await _rawg.ObtenerListadoAsync(
                    TipoListado.Populares,
                    cancellationToken: HttpContext.RequestAborted);

            PaginaVideojuegos mejorValorados =
                await _rawg.ObtenerListadoAsync(
                    TipoListado.MejorValorados,
                    cancellationToken: HttpContext.RequestAborted);

            PaginaVideojuegos proximamente =
                await _rawg.ObtenerListadoAsync(
                    TipoListado.Proximamente,
                    cancellationToken: HttpContext.RequestAborted);

            Populares = populares.Resultados.Take(8).ToList().AsReadOnly();
            MejorValorados =
                mejorValorados.Resultados.Take(8).ToList().AsReadOnly();
            Proximamente =
                proximamente.Resultados.Take(8).ToList().AsReadOnly();

            await MarcarBibliotecaAsync(
                Populares.Concat(MejorValorados).Concat(Proximamente));
        }
        catch (RawgExcepcion excepcion)
        {
            ViewData["Error"] = excepcion.Message;
        }
    }

    private async Task MarcarBibliotecaAsync(
        IEnumerable<VideojuegoResumen> videojuegos)
    {
        string? usuarioId = _userManager.GetUserId(User);
        if (usuarioId is null)
        {
            return;
        }

        HashSet<int> ids = await _biblioteca.ObtenerIdsAsync(
            usuarioId,
            HttpContext.RequestAborted);

        foreach (VideojuegoResumen videojuego in videojuegos)
        {
            videojuego.EstaEnBiblioteca = ids.Contains(videojuego.Id);
        }
    }
}
