using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RickAndMorty.DTOs;
using RickAndMorty.Modelos;
using RickAndMorty.Servicios;

namespace RickAndMorty.Pages.Personajes;

// Carga la ficha, los episodios relacionados y el estado de favorito.
public class DetallesModel : PageModel
{
    private readonly IRickAndMortyServicio _servicio;
    private readonly IFavoritosServicio _favoritos;
    private readonly UserManager<Usuario> _userManager;

    public DetallesModel(
        IRickAndMortyServicio servicio,
        IFavoritosServicio favoritos,
        UserManager<Usuario> userManager)
    {
        _servicio = servicio;
        _favoritos = favoritos;
        _userManager = userManager;
    }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    public PersonajeDto? Personaje { get; private set; }
    public IReadOnlyList<EpisodioDto> Episodios { get; private set; } = [];
    public bool EsFavorito { get; private set; }
    public string? ErrorApi { get; private set; }

    public async Task<IActionResult> OnGetAsync(
        CancellationToken cancellationToken)
    {
        if (Id <= 0)
        {
            return RedirectToPage("/Personajes/Index");
        }

        try
        {
            Personaje = await _servicio.ObtenerPersonajeAsync(
                Id,
                cancellationToken);

            Episodios = await _servicio.ObtenerEpisodiosPorUrlsAsync(
                Personaje.Episodios,
                60,
                cancellationToken);

            string? usuarioId = _userManager.GetUserId(User);
            if (usuarioId is not null)
            {
                EsFavorito = await _favoritos.ContieneAsync(
                    usuarioId,
                    Id,
                    cancellationToken);
            }
        }
        catch (RickAndMortyApiExcepcion excepcion)
        {
            ErrorApi = excepcion.Message;
        }

        return Page();
    }
}
