using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Peliculas.Modelos;
using Peliculas.Servicios;

namespace Peliculas.Pages.Peliculas;

// Prepara la ficha completa de una película.
public class DetallesModel : PageModel
{
    private readonly ITmdbServicio _tmdb;
    private readonly IFavoritosServicio _favoritos;
    private readonly UserManager<Usuario> _userManager;

    public DetallesModel(
        ITmdbServicio tmdb,
        IFavoritosServicio favoritos,
        UserManager<Usuario> userManager)
    {
        _tmdb = tmdb;
        _favoritos = favoritos;
        _userManager = userManager;
    }

    public PeliculaDetalle Pelicula { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(
        int id,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return NotFound();
        }

        try
        {
            Pelicula = await _tmdb.ObtenerDetalleAsync(id, cancellationToken);

            // El corazón aparece relleno cuando la película ya está guardada.
            string? usuarioId = _userManager.GetUserId(User);
            if (usuarioId is not null)
            {
                HashSet<int> idsFavoritos =
                    await _favoritos.ObtenerIdsAsync(
                        usuarioId, cancellationToken);

                Pelicula.EsFavorita = idsFavoritos.Contains(id);

                foreach (PeliculaResumen recomendacion
                    in Pelicula.Recomendaciones)
                {
                    recomendacion.EsFavorita =
                        idsFavoritos.Contains(recomendacion.Id);
                }
            }

            return Page();
        }
        catch (TmdbExcepcion excepcion)
            when (excepcion.CodigoEstado == System.Net.HttpStatusCode.NotFound)
        {
            return NotFound();
        }
        catch (TmdbExcepcion excepcion)
        {
            ViewData["Error"] = excepcion.Message;
            Pelicula.Id = id;
            return Page();
        }
    }
}
