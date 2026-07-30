using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Recetas.Modelos;
using Recetas.Servicios;

namespace Recetas.Pages.Favoritos;

// Lee los favoritos de SQLite sin depender de TheMealDB.
[Authorize]
public class IndexModel : PageModel
{
    private readonly IColeccionServicio _coleccion;
    private readonly UserManager<Usuario> _userManager;

    public IndexModel(
        IColeccionServicio coleccion,
        UserManager<Usuario> userManager)
    {
        _coleccion = coleccion;
        _userManager = userManager;
    }

    public IReadOnlyList<RecetaResumen> Recetas { get; private set; } = [];
    public string Orden { get; private set; } = "recientes";

    public async Task OnGetAsync(string? orden)
    {
        Orden = orden?.ToLowerInvariant() switch
        {
            "titulo" => "titulo",
            "categoria" => "categoria",
            "area" => "area",
            _ => "recientes"
        };

        string usuarioId = _userManager.GetUserId(User)!;
        IReadOnlyList<Favorito> favoritos =
            await _coleccion.ListarFavoritosAsync(
                usuarioId,
                HttpContext.RequestAborted);

        IEnumerable<Favorito> ordenados = Orden switch
        {
            "titulo" => favoritos.OrderBy(
                favorito => favorito.Receta.Nombre),
            "categoria" => favoritos.OrderBy(
                favorito => favorito.Receta.Categoria),
            "area" => favoritos.OrderBy(
                favorito => favorito.Receta.Area),
            _ => favoritos
        };

        Recetas = ordenados.Select(favorito => new RecetaResumen
            {
                Id = favorito.Receta.TheMealDbId,
                Nombre = favorito.Receta.Nombre,
                ImagenUrl = favorito.Receta.ImagenUrl,
                Categoria = favorito.Receta.Categoria,
                Area = favorito.Receta.Area,
                EsFavorita = true
            })
            .ToList()
            .AsReadOnly();
    }
}
