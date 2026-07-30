using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NasaExplorer.Modelos;
using NasaExplorer.Servicios;

namespace NasaExplorer.Pages.Favoritos;

// Solo el usuario autenticado puede leer o borrar su colección.
[Authorize]
public class IndexModel(
    UserManager<Usuario> userManager,
    IFavoritosServicio favoritosServicio) : PageModel
{
    public List<Favorito> Favoritos { get; private set; } = [];

    public async Task OnGetAsync()
    {
        string usuarioId = userManager.GetUserId(User)!;
        Favoritos = await favoritosServicio.ObtenerAsync(usuarioId);
    }

    public async Task<IActionResult> OnPostEliminarAsync(int id)
    {
        string? usuarioId = userManager.GetUserId(User);
        if (usuarioId is null)
        {
            return Challenge();
        }

        bool eliminado = await favoritosServicio.EliminarAsync(usuarioId, id);
        TempData["Mensaje"] = eliminado
            ? "Favorito eliminado."
            : "No se ha encontrado ese favorito.";
        TempData["TipoMensaje"] = eliminado ? "success" : "warning";
        return RedirectToPage();
    }
}
