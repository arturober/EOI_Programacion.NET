using Futbol.Modelos;
using Futbol.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Futbol.Pages.Favoritos;

// Muestra únicamente los favoritos del usuario autenticado.
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

    public IReadOnlyList<EquipoFavorito> Equipos { get; private set; } = [];

    public async Task OnGetAsync(
        CancellationToken cancellationToken)
    {
        string usuarioId = _userManager.GetUserId(User)!;
        Equipos = await _favoritos.ObtenerAsync(
            usuarioId,
            cancellationToken);
    }
}
