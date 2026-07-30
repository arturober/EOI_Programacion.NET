using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NasaExplorer.DTOs;
using NasaExplorer.Modelos;
using NasaExplorer.Servicios;

namespace NasaExplorer.Pages.ClimaEspacial;

// DONKI ofrece diferentes catálogos de sucesos solares y geomagnéticos.
public class IndexModel(
    INasaServicio nasaServicio,
    IFavoritosServicio favoritosServicio,
    UserManager<Usuario> userManager) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string Tipo { get; set; } = "CME";

    [BindProperty(SupportsGet = true)]
    public DateOnly? Desde { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateOnly? Hasta { get; set; }

    public List<DonkiEventoVista> Eventos { get; private set; } = [];
    public HashSet<string> Favoritos { get; private set; } = [];
    public string? Error { get; private set; }

    public async Task OnGetAsync()
    {
        Tipo = Tipo is "CME" or "GST" or "FLR" or "IPS" ? Tipo : "CME";
        Hasta ??= DateOnly.FromDateTime(DateTime.Today);
        Desde ??= Hasta.Value.AddDays(-30);

        if (Desde > Hasta || Hasta.Value.DayNumber - Desde.Value.DayNumber > 180)
        {
            Error = "Selecciona un intervalo válido de hasta 180 días.";
            return;
        }

        try
        {
            Eventos = await nasaServicio.ObtenerClimaEspacialAsync(
                Tipo,
                Desde.Value,
                Hasta.Value);

            if (User.Identity?.IsAuthenticated == true)
            {
                string usuarioId = userManager.GetUserId(User)!;
                Favoritos = await favoritosServicio.ObtenerReferenciasAsync(
                    usuarioId,
                    "DONKI");
            }
        }
        catch (ApiExternaExcepcion excepcion)
        {
            Error = excepcion.Message;
        }
    }
}
