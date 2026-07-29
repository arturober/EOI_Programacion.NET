using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Peliculas.Modelos;
using Peliculas.Servicios;

namespace Peliculas.Pages.Favoritos;

// Muestra únicamente las favoritas del usuario que ha iniciado sesión.
[Authorize]
public class IndexModel : PageModel
{
    private readonly IFavoritosServicio _favoritos;
    private readonly UserManager<Usuario> _userManager;

    public IndexModel(
        IFavoritosServicio favoritos,
        UserManager<Usuario> userManager)
    {
        _favoritos = favoritos;
        _userManager = userManager;
    }

    public IReadOnlyList<PeliculaResumen> Peliculas { get; private set; } = [];

    public string Orden { get; private set; } = "recientes";

    public async Task OnGetAsync(
        string? orden,
        CancellationToken cancellationToken)
    {
        Orden = orden?.ToLowerInvariant() switch
        {
            "titulo" => "titulo",
            "estreno" => "estreno",
            "puntuacion" => "puntuacion",
            _ => "recientes"
        };

        string usuarioId = _userManager.GetUserId(User)!;
        IReadOnlyList<Favorito> favoritos = await _favoritos.ListarAsync(
            usuarioId, cancellationToken);

        IEnumerable<Favorito> ordenados = Orden switch
        {
            "titulo" => favoritos.OrderBy(
                favorito => favorito.Pelicula.Titulo),
            "estreno" => favoritos.OrderByDescending(
                favorito => favorito.Pelicula.FechaEstreno),
            "puntuacion" => favoritos.OrderByDescending(
                favorito => favorito.Pelicula.Puntuacion),
            _ => favoritos.OrderByDescending(
                favorito => favorito.FechaAgregadaUtc)
        };

        Peliculas = ordenados.Select(favorito => new PeliculaResumen
        {
            Id = favorito.Pelicula.TmdbId,
            Titulo = favorito.Pelicula.Titulo,
            TituloOriginal = favorito.Pelicula.TituloOriginal,
            RutaPoster = favorito.Pelicula.RutaPoster,
            FechaEstreno = favorito.Pelicula.FechaEstreno,
            Puntuacion = favorito.Pelicula.Puntuacion,
            EsFavorita = true
        }).ToList();
    }
}
