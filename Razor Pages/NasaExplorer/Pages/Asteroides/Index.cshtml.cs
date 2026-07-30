using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NasaExplorer.DTOs;
using NasaExplorer.Modelos;
using NasaExplorer.Servicios;

namespace NasaExplorer.Pages.Asteroides;

// Consulta aproximaciones a la Tierra mediante NeoWs.
public class IndexModel(
    INasaServicio nasaServicio,
    IFavoritosServicio favoritosServicio,
    UserManager<Usuario> userManager) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public DateOnly? Desde { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateOnly? Hasta { get; set; }

    public AsteroidesResultado? Resultado { get; private set; }
    public HashSet<string> Favoritos { get; private set; } = [];
    public string? Error { get; private set; }

    public async Task OnGetAsync()
    {
        Desde ??= DateOnly.FromDateTime(DateTime.Today);
        Hasta ??= Desde.Value.AddDays(7);

        try
        {
            Resultado = await nasaServicio.ObtenerAsteroidesAsync(
                Desde.Value,
                Hasta.Value);

            if (User.Identity?.IsAuthenticated == true)
            {
                string usuarioId = userManager.GetUserId(User)!;
                Favoritos = await favoritosServicio.ObtenerReferenciasAsync(
                    usuarioId,
                    "Asteroide");
            }
        }
        catch (ApiExternaExcepcion excepcion)
        {
            Error = excepcion.Message;
        }
    }
}
