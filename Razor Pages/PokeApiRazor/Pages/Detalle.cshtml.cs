using Microsoft.AspNetCore.Mvc.RazorPages;
using PokeApiRazor.Models;
using PokeApiRazor.Services;

namespace PokeApiRazor.Pages;

// Este PageModel obtiene y prepara el detalle de un único Pokémon.
public class DetalleModel : PageModel
{
    // El servicio centraliza todas las llamadas a PokeAPI.
    private readonly PokeApiService _pokeApi;

    public DetalleModel(PokeApiService pokeApi)
    {
        _pokeApi = pokeApi;
    }

    // La vista leerá esta propiedad cuando la carga termine correctamente.
    public PokemonDetalle? Pokemon { get; private set; }

    // La vista utilizará este mensaje si se produce un error de red.
    public string? MensajeError { get; private set; }

    // El parámetro procede de la ruta /Detalle/pikachu o /Detalle/25.
    public async Task OnGetAsync(string nombreOId)
    {
        try
        {
            Pokemon = await _pokeApi.ObtenerDetalleAsync(nombreOId);

            // Indicamos un 404 real al navegador cuando el recurso no existe.
            if (Pokemon is null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
            }
        }
        catch (HttpRequestException)
        {
            Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            MensajeError =
                "No se ha podido obtener la información de PokeAPI.";
        }
        catch (TaskCanceledException)
        {
            Response.StatusCode = StatusCodes.Status504GatewayTimeout;
            MensajeError =
                "PokeAPI ha tardado demasiado en responder.";
        }
    }
}
