using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenFoodFacts.Modelos;
using OpenFoodFacts.Servicios;

namespace OpenFoodFacts.Pages.Favoritos;

// Lee los favoritos desde SQLite sin realizar peticiones a la API.
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

    public IReadOnlyList<ProductoResumen> Productos { get; private set; } = [];
    public string Orden { get; private set; } = "recientes";

    public async Task OnGetAsync(string? orden)
    {
        Orden = orden?.ToLowerInvariant() switch
        {
            "titulo" => "titulo",
            "marca" => "marca",
            "nutriscore" => "nutriscore",
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
                favorito => favorito.Producto.Nombre),
            "marca" => favoritos.OrderBy(
                favorito => favorito.Producto.Marca),
            "nutriscore" => favoritos.OrderBy(
                favorito => favorito.Producto.NutriScore),
            _ => favoritos
        };

        Productos = ordenados.Select(favorito => new ProductoResumen
            {
                Codigo = favorito.Producto.Codigo,
                Nombre = favorito.Producto.Nombre,
                Marca = favorito.Producto.Marca,
                Cantidad = favorito.Producto.Cantidad,
                ImagenUrl = favorito.Producto.ImagenUrl,
                NutriScore = favorito.Producto.NutriScore,
                GrupoNova = favorito.Producto.GrupoNova,
                GreenScore = favorito.Producto.GreenScore,
                EsFavorito = true
            })
            .ToList()
            .AsReadOnly();
    }
}
