using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using NasaExplorer.Configuracion;
using NasaExplorer.DTOs;
using NasaExplorer.Servicios;

namespace NasaExplorer.Pages;

// La portada intenta cargar APOD, pero sigue funcionando si esa API falla.
public class IndexModel(
    INasaServicio nasaServicio,
    IOptions<NasaOpciones> opciones) : PageModel
{
    public ApodDto? Apod { get; private set; }
    public string? ErrorApod { get; private set; }
    public bool ClaveConfigurada =>
        !string.IsNullOrWhiteSpace(opciones.Value.ApiKey);

    public async Task OnGetAsync()
    {
        if (!ClaveConfigurada)
        {
            return;
        }

        try
        {
            Apod = await nasaServicio.ObtenerApodAsync(
                DateOnly.FromDateTime(DateTime.Today));
        }
        catch (ApiExternaExcepcion excepcion)
        {
            ErrorApod = excepcion.Message;
        }
    }
}
