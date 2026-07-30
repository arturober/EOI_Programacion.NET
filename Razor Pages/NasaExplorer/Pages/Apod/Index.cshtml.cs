using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NasaExplorer.DTOs;
using NasaExplorer.Modelos;
using NasaExplorer.Servicios;

namespace NasaExplorer.Pages.Apod;

// Permite viajar por el archivo APOD mediante una fecha.
public class IndexModel(
    INasaServicio nasaServicio,
    IFavoritosServicio favoritosServicio,
    UserManager<Usuario> userManager) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public DateOnly? Fecha { get; set; }

    public ApodDto? Apod { get; private set; }
    public string? Error { get; private set; }
    public bool EsFavorito { get; private set; }
    public DateOnly FechaMinima => new(1995, 6, 16);
    public DateOnly FechaMaxima => DateOnly.FromDateTime(DateTime.Today);

    public async Task OnGetAsync()
    {
        Fecha ??= FechaMaxima;

        if (Fecha < FechaMinima || Fecha > FechaMaxima)
        {
            Error = "Elige una fecha comprendida entre el 16 de junio de 1995 y hoy.";
            return;
        }

        try
        {
            Apod = await nasaServicio.ObtenerApodAsync(Fecha.Value);

            if (User.Identity?.IsAuthenticated == true)
            {
                string usuarioId = userManager.GetUserId(User)!;
                HashSet<string> referencias =
                    await favoritosServicio.ObtenerReferenciasAsync(usuarioId, "APOD");
                EsFavorito = referencias.Contains(Apod.Fecha);
            }
        }
        catch (ApiExternaExcepcion excepcion)
        {
            Error = excepcion.Message;
        }
    }
}
