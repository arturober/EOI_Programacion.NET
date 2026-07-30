using Biblioteca.Modelos;
using Biblioteca.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Biblioteca.Pages.Favoritos;

// Lee los favoritos de SQLite y permite ordenarlos sin llamar a la API.
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

    public IReadOnlyList<LibroResumen> Libros { get; private set; } = [];
    public string Orden { get; private set; } = "recientes";

    public async Task OnGetAsync(string? orden)
    {
        Orden = orden?.ToLowerInvariant() switch
        {
            "titulo" => "titulo",
            "anio" => "anio",
            "puntuacion" => "puntuacion",
            _ => "recientes"
        };

        string usuarioId = _userManager.GetUserId(User)!;
        IReadOnlyList<Favorito> favoritos = await _favoritos.ListarAsync(
            usuarioId,
            HttpContext.RequestAborted);

        IEnumerable<Favorito> ordenados = Orden switch
        {
            "titulo" => favoritos.OrderBy(
                favorito => favorito.Libro.Titulo),
            "anio" => favoritos.OrderByDescending(
                favorito => favorito.Libro.PrimeraPublicacion),
            "puntuacion" => favoritos.OrderByDescending(
                favorito => favorito.Libro.Puntuacion),
            _ => favoritos
        };

        Libros = ordenados
            .Select(favorito => new LibroResumen
            {
                Id = favorito.Libro.OpenLibraryId,
                Titulo = favorito.Libro.Titulo,
                Autores = string.IsNullOrWhiteSpace(favorito.Libro.Autores)
                    ? []
                    : [favorito.Libro.Autores],
                PortadaId = favorito.Libro.PortadaId,
                PrimeraPublicacion = favorito.Libro.PrimeraPublicacion,
                Puntuacion = favorito.Libro.Puntuacion,
                EsFavorito = true
            })
            .ToList()
            .AsReadOnly();
    }
}
