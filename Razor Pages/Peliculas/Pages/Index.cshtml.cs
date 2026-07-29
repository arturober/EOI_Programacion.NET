using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Peliculas.Modelos;
using Peliculas.Servicios;

namespace Peliculas.Pages;

public class IndexModel : PageModel
{
    private readonly ITmdbServicio _tmdb;
    private readonly IFavoritosServicio _favoritos;
    private readonly UserManager<Usuario> _userManager;

    public IndexModel(
        ITmdbServicio tmdb,
        IFavoritosServicio favoritos,
        UserManager<Usuario> userManager)
    {
        _tmdb = tmdb;
        _favoritos = favoritos;
        _userManager = userManager;
    }

    public bool EstaConfigurado => _tmdb.EstaConfigurado;
    public PaginaPeliculas Tendencias { get; private set; } = new();
    public PaginaPeliculas Cartelera { get; private set; } = new();
    public PaginaPeliculas MejorValoradas { get; private set; } = new();

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        if (!EstaConfigurado)
        {
            return;
        }

        try
        {
            Task<PaginaPeliculas> tendenciasTask =
                _tmdb.ObtenerListadoAsync(
                    TipoListado.Tendencias, 1, cancellationToken);
            Task<PaginaPeliculas> carteleraTask =
                _tmdb.ObtenerListadoAsync(
                    TipoListado.EnCartelera, 1, cancellationToken);
            Task<PaginaPeliculas> valoradasTask =
                _tmdb.ObtenerListadoAsync(
                    TipoListado.MejorValoradas, 1, cancellationToken);

            await Task.WhenAll(
                tendenciasTask, carteleraTask, valoradasTask);

            Tendencias = await tendenciasTask;
            Cartelera = await carteleraTask;
            MejorValoradas = await valoradasTask;

            await MarcarFavoritasAsync(cancellationToken);
        }
        catch (TmdbExcepcion excepcion)
        {
            ViewData["Error"] = excepcion.Message;
        }
    }

    private async Task MarcarFavoritasAsync(
        CancellationToken cancellationToken)
    {
        string? usuarioId = _userManager.GetUserId(User);

        if (usuarioId is null)
        {
            return;
        }

        HashSet<int> ids = await _favoritos.ObtenerIdsAsync(
            usuarioId, cancellationToken);

        foreach (PeliculaResumen pelicula in Tendencias.Resultados
            .Concat(Cartelera.Resultados)
            .Concat(MejorValoradas.Resultados))
        {
            pelicula.EsFavorita = ids.Contains(pelicula.Id);
        }
    }
}
