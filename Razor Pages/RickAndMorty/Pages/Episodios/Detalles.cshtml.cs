using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RickAndMorty.DTOs;
using RickAndMorty.Servicios;

namespace RickAndMorty.Pages.Episodios;

// Relaciona el episodio seleccionado con sus personajes.
public class DetallesModel : PageModel
{
    private readonly IRickAndMortyServicio _servicio;

    public DetallesModel(IRickAndMortyServicio servicio)
    {
        _servicio = servicio;
    }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    public EpisodioDto? Episodio { get; private set; }
    public IReadOnlyList<PersonajeDto> Personajes { get; private set; } = [];
    public string? ErrorApi { get; private set; }

    public async Task<IActionResult> OnGetAsync(
        CancellationToken cancellationToken)
    {
        if (Id <= 0)
        {
            return RedirectToPage("/Episodios/Index");
        }

        try
        {
            Episodio = await _servicio.ObtenerEpisodioAsync(
                Id,
                cancellationToken);

            Personajes = await _servicio.ObtenerPersonajesPorUrlsAsync(
                Episodio.Personajes,
                100,
                cancellationToken);
        }
        catch (RickAndMortyApiExcepcion excepcion)
        {
            ErrorApi = excepcion.Message;
        }

        return Page();
    }
}
