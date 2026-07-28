using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Pokemon.Models;
using Pokemon.Services;

namespace Pokemon.Pages;

public class DetalleModel : PageModel
{
    private readonly PokeApiService _pokeApi;

    public PokemonDetalleApi? Pokemon { get; private set; }

    public DetalleModel(PokeApiService pokeApi)
    {
        _pokeApi = pokeApi;
    }

    public async Task<IActionResult> OnGetAsync(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return NotFound();
        }

        Pokemon = await _pokeApi.ObtenerDetalleAsync(nombre);

        if (Pokemon is null)
        {
            return NotFound();
        }

        return Page();
    }
}
