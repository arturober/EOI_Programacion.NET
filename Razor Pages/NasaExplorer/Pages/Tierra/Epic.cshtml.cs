using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NasaExplorer.DTOs;
using NasaExplorer.Modelos;
using NasaExplorer.Servicios;

namespace NasaExplorer.Pages.Tierra;

// Muestra las colecciones de la cámara EPIC a color natural o procesado.
public class EpicModel(
    INasaServicio nasaServicio,
    IFavoritosServicio favoritosServicio,
    UserManager<Usuario> userManager) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string Coleccion { get; set; } = "natural";

    [BindProperty(SupportsGet = true)]
    public DateOnly? Fecha { get; set; }

    public List<EpicImagenDto> Imagenes { get; private set; } = [];
    public HashSet<string> Favoritos { get; private set; } = [];
    public string? Error { get; private set; }

    public async Task OnGetAsync()
    {
        Coleccion = Coleccion is "natural" or "enhanced" or "aerosol" or "cloud"
            ? Coleccion
            : "natural";

        try
        {
            Imagenes = await nasaServicio.ObtenerEpicAsync(Coleccion, Fecha);

            if (User.Identity?.IsAuthenticated == true)
            {
                string usuarioId = userManager.GetUserId(User)!;
                Favoritos = await favoritosServicio.ObtenerReferenciasAsync(
                    usuarioId,
                    "EPIC");
            }
        }
        catch (ApiExternaExcepcion excepcion)
        {
            Error = excepcion.Message;
        }
    }
}
