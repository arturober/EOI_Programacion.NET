using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PokeApiRazor.Models;
using PokeApiRazor.Services;

namespace PokeApiRazor.Pages;

// PageModel contiene el código C# del listado de Pokémon.
public class ListadoModel : PageModel
{
    // El servicio se recibe mediante inyección de dependencias.
    private readonly PokeApiService _pokeApi;

    public ListadoModel(PokeApiService pokeApi)
    {
        _pokeApi = pokeApi;
    }

    // SupportsGet permite rellenar la propiedad desde ?busqueda=...
    [BindProperty(SupportsGet = true)]
    public string? Busqueda { get; set; }

    // La página también llega mediante la dirección, por ejemplo ?pagina=2.
    [BindProperty(SupportsGet = true)]
    public int Pagina { get; set; } = 1;

    // Estos son los datos que utilizará el archivo .cshtml.
    public ResultadoPokemon Resultado { get; private set; } = new();

    // Si la API falla, guardamos un mensaje comprensible para la vista.
    public string? MensajeError { get; private set; }

    // OnGetAsync se ejecuta al abrir la página mediante GET.
    public async Task OnGetAsync()
    {
        try
        {
            Resultado = await _pokeApi.ObtenerListadoAsync(
                Busqueda,
                Pagina);

            // El servicio puede corregir una página que ya no existe.
            Pagina = Resultado.Pagina;
        }
        catch (HttpRequestException)
        {
            MensajeError =
                "No se ha podido conectar con PokeAPI. Inténtalo de nuevo.";
        }
        catch (TaskCanceledException)
        {
            MensajeError =
                "PokeAPI ha tardado demasiado en responder. Inténtalo de nuevo.";
        }
    }
}
