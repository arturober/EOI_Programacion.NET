using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Peliculas.Modelos;
using Peliculas.Servicios;

namespace Peliculas.Pages.Peliculas;

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

    public TipoListado Tipo { get; private set; }
    public PaginaPeliculas Resultado { get; private set; } = new();

    public async Task OnGetAsync(
        string? tipo,
        int pagina = 1,
        CancellationToken cancellationToken = default)
    {
        Tipo = TipoListadoExtensiones.DesdeTexto(tipo);

        try
        {
            Resultado = await _tmdb.ObtenerListadoAsync(
                Tipo, pagina, cancellationToken);

            string? usuarioId = _userManager.GetUserId(User);

            if (usuarioId is not null)
            {
                HashSet<int> ids = await _favoritos.ObtenerIdsAsync(
                    usuarioId, cancellationToken);

                foreach (PeliculaResumen pelicula in Resultado.Resultados)
                {
                    pelicula.EsFavorita = ids.Contains(pelicula.Id);
                }
            }
        }
        catch (TmdbExcepcion excepcion)
        {
            ViewData["Error"] = excepcion.Message;
        }
    }
}
