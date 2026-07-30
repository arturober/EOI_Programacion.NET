using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RickAndMorty.Modelos;
using RickAndMorty.Servicios;

namespace RickAndMorty.Pages.Favoritos;

// Muestra únicamente la colección del usuario autenticado.
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

    public IReadOnlyList<PersonajeFavorito> Personajes { get; private set; } =
        [];

    public async Task OnGetAsync(
        CancellationToken cancellationToken)
    {
        string usuarioId = _userManager.GetUserId(User)!;
        Personajes = await _favoritos.ObtenerAsync(
            usuarioId,
            cancellationToken);
    }
}
