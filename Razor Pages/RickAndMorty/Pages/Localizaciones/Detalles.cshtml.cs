using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RickAndMorty.DTOs;
using RickAndMorty.Servicios;

namespace RickAndMorty.Pages.Localizaciones;

// Muestra una localización y una selección de sus residentes.
public class DetallesModel : PageModel
{
    private readonly IRickAndMortyServicio _servicio;

    public DetallesModel(IRickAndMortyServicio servicio)
    {
        _servicio = servicio;
    }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    public LocalizacionDto? Localizacion { get; private set; }
    public IReadOnlyList<PersonajeDto> Residentes { get; private set; } = [];
    public bool ResidentesRecortados { get; private set; }
    public string? ErrorApi { get; private set; }

    public async Task<IActionResult> OnGetAsync(
        CancellationToken cancellationToken)
    {
        if (Id <= 0)
        {
            return RedirectToPage("/Localizaciones/Index");
        }

        try
        {
            Localizacion = await _servicio.ObtenerLocalizacionAsync(
                Id,
                cancellationToken);

            // Se limita la relación para evitar URL excesivamente largas.
            ResidentesRecortados = Localizacion.Residentes.Count > 40;
            Residentes = await _servicio.ObtenerPersonajesPorUrlsAsync(
                Localizacion.Residentes,
                40,
                cancellationToken);
        }
        catch (RickAndMortyApiExcepcion excepcion)
        {
            ErrorApi = excepcion.Message;
        }

        return Page();
    }
}
