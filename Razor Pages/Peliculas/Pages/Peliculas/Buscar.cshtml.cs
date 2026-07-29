using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Peliculas.Modelos;
using Peliculas.Servicios;

namespace Peliculas.Pages.Peliculas;

public class BuscarModel : PageModel
{
    private readonly ITmdbServicio _tmdb;
    private readonly IFavoritosServicio _favoritos;
    private readonly UserManager<Usuario> _userManager;

    public BuscarModel(
        ITmdbServicio tmdb,
        IFavoritosServicio favoritos,
        UserManager<Usuario> userManager)
    {
        _tmdb = tmdb;
        _favoritos = favoritos;
        _userManager = userManager;
    }

    public string Texto { get; private set; } = "";
    public PaginaPeliculas Resultado { get; private set; } = new();

    public async Task OnGetAsync(
        string? texto,
        int pagina = 1,
        CancellationToken cancellationToken = default)
    {
        Texto = texto?.Trim() ?? "";

        if (Texto.Length < 2)
        {
            return;
        }

        try
        {
            Resultado = await _tmdb.BuscarAsync(
                Texto, pagina, cancellationToken);

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
